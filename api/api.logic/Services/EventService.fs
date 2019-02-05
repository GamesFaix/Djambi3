﻿module Djambi.Api.Logic.Services.EventService

open Djambi.Api.Model
open Djambi.Api.Common.Collections

let private applyGameStatusChangedEffect (effect : GameStatusChangedEffect) (game : Game) : Game =
    match (effect.oldValue, effect.newValue) with
    | (Pending, Started) ->
        //This case is a lot more complicated
        GameStartService.applyStartGame game
    | _ ->
        { game with status = effect.newValue }

let private applyTurnCycleChangedEffect (effect : TurnCycleChangedEffect) (game : Game) : Game =
    { game with turnCycle = effect.newValue }

let private applyParameterChangedEffect (effect : ParametersChangedEffect) (game : Game) : Game =
    { game with parameters = effect.newValue }

let private applyPlayerEliminatedEffect (effect : PlayerEliminatedEffect) (game : Game) : Game =
    { game with 
        players = game.players |> List.replaceIf 
            (fun p -> p.id = effect.playerId) 
            (fun p -> { p with status = PlayerStatus.Eliminated })
    }

let private applyPieceKilledEffect (effect : PieceKilledEffect) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> p.id = effect.oldPiece.id) 
            (fun p -> { p with kind = PieceKind.Corpse; playerId = None }) 
    }

let private applyPlayerRemovedEffect (effect : PlayerRemovedEffect) (game : Game) : Game =
    { game with 
        players = game.players |> List.filter (fun p -> p.id <> effect.playerId)
    }

let private applyPlayerOutOfMovesEffect (effect : PlayerOutOfMovesEffect) (game : Game) : Game =
    //This effect is just to communicate what happened,
    //the same event should also create a PlayerEliminated and PiecesOwnershipChanged effect
    game

let private applyPlayerAddedEffect (effect : PlayerAddedEffect) (game : Game) : Game =
    let player : Player = 
        {
            id = 0
            gameId = game.id
            userId = Some effect.userId
            kind = effect.kind
            name = match effect.name with Some x -> x | None -> ""
            status = PlayerStatus.Pending
            colorId = None
            startingRegion = None
            startingTurnNumber = None
        }

    { game with players = List.append game.players [player] }

let private applyNeutralPlayerAddedEffect (effect : NeutralPlayerAddedEffect) (game : Game) : Game =
    let player : Player = 
        {
            id = 0
            gameId = game.id
            userId = None
            kind = PlayerKind.Neutral
            name = effect.name
            status = PlayerStatus.Pending
            colorId = None
            startingRegion = None
            startingTurnNumber = None
        }

    { game with players = List.append game.players [player] }

let private applyPieceOwnershipChangedEffect (effect : PieceOwnershipChangedEffect) (game : Game) : Game = 
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> p.id = effect.oldPiece.id)
            (fun p -> { p with playerId = effect.newPlayerId })
    }

let private applyPieceMovedEffect (effect : PieceMovedEffect) (game : Game) : Game =
    { game with 
        pieces = game.pieces |> List.replaceIf
            (fun p -> effect.oldPiece.id = p.id)
            (fun p -> { p with cellId = effect.newCellId }) 
    }

let private applyCurrentTurnChangedEffect (effect : CurrentTurnChangedEffect) (game : Game) : Game =
    { game with currentTurn = effect.newValue }

let private applyEffect (effect : Effect) (game : Game) : Game =
    match effect with 
    | Effect.GameStatusChanged e -> applyGameStatusChangedEffect e game
    | Effect.TurnCycleChanged e -> applyTurnCycleChangedEffect e game
    | Effect.ParametersChanged e -> applyParameterChangedEffect e game
    | Effect.PlayerEliminated e -> applyPlayerEliminatedEffect e game
    | Effect.PieceKilled e -> applyPieceKilledEffect e game       
    | Effect.PlayerRemoved e -> applyPlayerRemovedEffect e game
    | Effect.PlayerOutOfMoves e -> applyPlayerOutOfMovesEffect e game
    | Effect.PlayerAdded e -> applyPlayerAddedEffect e game
    | Effect.NeutralPlayerAdded e -> applyNeutralPlayerAddedEffect e game
    | Effect.PieceOwnershipChanged e -> applyPieceOwnershipChangedEffect e game
    | Effect.PieceMoved e -> applyPieceMovedEffect e game
    | Effect.CurrentTurnChanged e -> applyCurrentTurnChangedEffect e game

let applyEvent (game : Game) (eventRequest : CreateEventRequest) : Game =
    let mutable game = game
    for ef in eventRequest.effects do
        game <- applyEffect ef game
    game
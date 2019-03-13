﻿module Djambi.Api.Logic.Services.PlayerService

open System
open System.Linq
open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.Model
open Djambi.Api.Logic
    
let getAddPlayerEvent (game : Game, request : CreatePlayerRequest) (session : Session) : CreateEventRequest HttpResult =
    let self = session.user
    if game.status <> GameStatus.Pending
    then Error <| HttpException(400, "Can only add players to pending games.")
    elif request.name.IsSome 
        && game.players |> List.exists (fun p -> String.Equals(p.name, request.name.Value, StringComparison.OrdinalIgnoreCase))
    then Error <| HttpException(409, "A player with that name already exists.")
    elif game.players.Length >= game.parameters.regionCount
    then Error <| HttpException(400, "Max player count reached.")
      else
        match request.kind with
        | PlayerKind.User ->
            if request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a user player.")
            elif request.name.IsSome
            then Error <| HttpException(400, "Cannot provide name when adding a user player.")
            elif game.players |> List.exists (fun p -> p.kind = PlayerKind.User && p.userId = request.userId)
            then Error <| HttpException(409, "User is already a player.")
            elif not (self.has EditPendingGames) && request.userId.Value <> self.id
            then Error <| HttpException(403, "Cannot add other users to a game.")
            else Ok ()

        | PlayerKind.Guest ->
            if not game.parameters.allowGuests
            then Error <| HttpException(400, "Game does not allow guest players.")
            elif request.userId.IsNone
            then Error <| HttpException(400, "UserID must be provided when adding a guest player.")
            elif request.name.IsNone
            then Error <| HttpException(400, "Must provide name when adding a guest player.")
            elif not (self.has EditPendingGames) && request.userId.Value <> self.id
            then Error <| HttpException(403, "Cannot add guests for other users to a game.")
            else Ok ()

        | PlayerKind.Neutral ->
            Error <| HttpException(400, "Cannot directly add neutral players to a game.")
    |> Result.map (fun _ -> 
        {
            kind = EventKind.PlayerJoined
            effects = [ PlayerAddedEffect.fromRequest request ]
            createdByUserId = self.id
            actingPlayerId = None
        }
    )

let getRemovePlayerEvent (game : Game, playerId : int) (session : Session) : CreateEventRequest HttpResult =
    let self = session.user
    if game.status <> Pending then
        Error <| HttpException(400, "Cannot remove players unless game is Pending.")
    else
        match game.players |> List.tryFind (fun p -> p.id = playerId) with
        | None -> Error <| HttpException(404, "Player not found.")
        | Some player ->
            match player.userId with
            | None -> Error <| HttpException(400, "Cannot remove neutral players from game.")
            | Some x ->
                if not <| (self.has EditPendingGames
                    || game.createdByUserId = self.id
                    || x = self.id)
                then Error <| HttpException(403, "Cannot remove other users from game.")        
                else 
                    let effects = new ArrayList<Effect>()

                    let playerIdsToRemove =
                        match player.kind with 
                        | User -> 
                            game.players 
                            |> List.filter (fun p -> p.userId = player.userId) 
                            |> List.map (fun p -> p.id)
                        | Guest -> [playerId]
                        | _ -> [] //Already eliminated this case in validation above

                    for pId in playerIdsToRemove do
                        effects.Add(Effect.PlayerRemoved { playerId = pId })

                    //Cancel game if creator quit
                    if game.createdByUserId = player.userId.Value
                        && player.kind = PlayerKind.User
                    then 
                        effects.Add(Effect.GameStatusChanged { oldValue = GameStatus.Pending; newValue = GameStatus.Aborted })
                    else ()

                    {
                        kind = EventKind.PlayerRemoved
                        effects = effects |> Seq.toList
                        createdByUserId = self.id
                        actingPlayerId = ContextService.getActingPlayerId session game
                    }
                    |> Ok

let fillEmptyPlayerSlots (game : Game) : Effect list AsyncHttpResult =
    let missingPlayerCount = game.parameters.regionCount - game.players.Length

    let getNeutralPlayerNamesToUse (possibleNames : string list) =
        Enumerable.Except(
            possibleNames,
            game.players |> Seq.map (fun p -> p.name),
            StringComparer.OrdinalIgnoreCase)
        |> List.ofSeq
        |> List.shuffle
        |> Seq.take missingPlayerCount

    if missingPlayerCount = 0
    then okTask []
    else
        GameRepository.getNeutralPlayerNames()
        |> thenMap getNeutralPlayerNamesToUse
        |> thenMap (Seq.map (fun name -> PlayerAddedEffect.fromRequest <| CreatePlayerRequest.neutral name ))    
        |> thenMap Seq.toList

let private getFinalAcceptDrawEffects(game : Game, request : PlayerStatusChangeRequest) : Effect list =
    let otherLivingPlayers = 
        game.players 
        |> List.filter (fun p -> 
            p.id <> request.playerId &&
            p.status = Alive
        )

    let nonNeutralPlayers = 
        otherLivingPlayers
        |> List.filter (fun p -> p.userId.IsSome)

    let neutralPlayers = 
        otherLivingPlayers
        |> List.filter (fun p -> p.userId.IsNone)

    if nonNeutralPlayers.IsEmpty
    then
        //If accepting draw, and everyone has accepted
        let neutralPlayerConcedeEffects = 
            neutralPlayers 
            |> List.map (fun p -> 
                Effect.PlayerStatusChanged{ 
                    playerId = p.id
                    oldStatus=p.status
                    newStatus=Conceded
                }
            )

        let finalEffect = Effect.GameStatusChanged { oldValue = Started; newValue = Finished }
        let fx = List.append neutralPlayerConcedeEffects [finalEffect]
        fx
    else [] //If not last player to accept, nothing special happens
   
let getUpdatePlayerStatusEvent (game : Game, request : PlayerStatusChangeRequest) (session : Session) : CreateEventRequest HttpResult = 
    if game.status <> Started then
        Error <| HttpException(400, "Cannot change player status unless game is Started.")
    else    
        SecurityService.ensurePlayerOrHas OpenParticipation session game
        |> Result.bind (fun _ ->
            let player = game.players |> List.find (fun p -> p.id = request.playerId)
            let oldStatus = player.status
            let newStatus = request.status

            if oldStatus = newStatus
                then Error <| HttpException(400, "Cannot change player status to current status.")
            else 
                let primaryEffect = Effect.PlayerStatusChanged { 
                    oldStatus = oldStatus
                    newStatus = newStatus
                    playerId = player.id 
                } 

                let event = {
                    kind = EventKind.PlayerStatusChanged
                    effects = [ primaryEffect ]
                    createdByUserId = session.user.id
                    actingPlayerId = Some request.playerId
                }

                match (oldStatus, newStatus) with
                | (Alive, AcceptsDraw) ->
                    let finalAcceptDrawEffects = getFinalAcceptDrawEffects (game, request)
                    Ok { event with effects = primaryEffect :: finalAcceptDrawEffects }
                | (AcceptsDraw, Alive) ->
                    //If revoking draw, just change status
                    Ok event
                | (Alive, Conceded)
                | (AcceptsDraw, Conceded)
                | (Alive, WillConcede)
                | (AcceptsDraw, WillConcede) ->
                    let isPlayersTurn = game.turnCycle.[0] = request.playerId
                    if isPlayersTurn
                    then
                        let primary = Effect.PlayerStatusChanged { 
                            oldStatus = oldStatus
                            newStatus = Conceded
                            playerId = player.id 
                        } 
                        //Remove from turn cycle
                        //Abandon pieces
                        //Check for victory and draw
                        //End turn
                        //Check for player out of moves

                        let fx = [primary]
                        Ok { event with effects = fx }
                    else
                        let primary = Effect.PlayerStatusChanged { 
                            oldStatus = oldStatus
                            newStatus = WillConcede
                            playerId = player.id 
                        } 
                        Ok { event with effects = [primary] }

                | _ -> 
                    Error <| HttpException(400, "Player status transition not allowed.")
        )
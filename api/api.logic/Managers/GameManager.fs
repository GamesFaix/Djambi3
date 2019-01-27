﻿module Djambi.Api.Logic.Managers.GameManager

open Djambi.Api.Logic.Services
open Djambi.Api.Common.Control
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Model
open Djambi.Api.Db.Repositories
open Djambi.ClientGenerator.Annotations

let private isGameViewableByActiveUser (session : Session) (game : Game) : bool =
    let self = session.user
    game.parameters.isPublic
    || game.createdByUserId = self.id
    || game.players |> List.exists(fun p -> p.userId = Some self.id)

[<ClientFunction(HttpMethod.Post, "/games/query", ClientSection.Game)>]
let getGames (query : GamesQuery) (session : Session) : Game list AsyncHttpResult =
    GameRepository.getGames query
    |> thenMap (fun games ->
        if session.user.isAdmin
        then games
        else games |> List.filter (isGameViewableByActiveUser session)
    )

//TODO: Requires integration tests
[<ClientFunction(HttpMethod.Get, "/games/%i", ClientSection.Game)>]
let getGame (gameId : int) (session : Session) : Game AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (fun game ->
        if isGameViewableByActiveUser session game
        then Ok <| game
        else Error <| HttpException(404, "Game not found.")        
    )
    
[<ClientFunction(HttpMethod.Post, "/games", ClientSection.Game)>]
let createGame (parameters : GameParameters) (session : Session) : Game AsyncHttpResult =
    GameCrudService.createGame parameters session

[<ClientFunction(HttpMethod.Post, "/events/query", ClientSection.Events)>]
let getEvents (gameId : int, query : EventsQuery) (session : Session) : Event list AsyncHttpResult =
    GameRepository.getGame gameId
    |> thenBind (SecurityService.ensureAdminOrPlayer session)
    |> thenBindAsync (fun _ -> EventRepository.getEvents (gameId, query))

let private processEvent (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest HttpResult) : StateAndEventResponse AsyncHttpResult = 
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        getCreateEventRequest game
        |> Result.bindAsync (fun eventRequest -> 
            let newGame = EventService.applyEvent game eventRequest
            EventRepository.persistEvent (eventRequest, game, newGame)
        )
    )
    
let private processEventAsync (gameId : int) (getCreateEventRequest : Game -> CreateEventRequest AsyncHttpResult): StateAndEventResponse AsyncHttpResult = 
    GameRepository.getGame gameId
    |> thenBindAsync (fun game -> 
        getCreateEventRequest game
        |> thenBindAsync (fun eventRequest -> 
            let newGame = EventService.applyEvent game eventRequest
            EventRepository.persistEvent (eventRequest, game, newGame)
        )
    )

//TODO: Requires integration tests
[<ClientFunction(HttpMethod.Put, "/games/%i/parameters", ClientSection.Game)>]
let updateGameParameters (gameId : int) (parameters : GameParameters) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> GameCrudService.getUpdateGameParametersEvent (game, parameters) session)
    
[<ClientFunction(HttpMethod.Post, "/games/%i/players", ClientSection.Player)>]
let addPlayer (gameId : int) (request : CreatePlayerRequest) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> PlayerService.getAddPlayerEvent (game, request) session)

[<ClientFunction(HttpMethod.Delete, "/games/%i/players/%i", ClientSection.Player)>]
let removePlayer (gameId : int, playerId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> PlayerService.getRemovePlayerEvent (game, playerId) session)

[<ClientFunction(HttpMethod.Post, "/games/%i/start-request", ClientSection.Game)>]
let startGame (gameId: int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEventAsync gameId (fun game -> GameStartService.getGameStartEvent game session)

[<ClientFunction(HttpMethod.Post, "/games/%i/current-turn/selection-request", ClientSection.Turn)>]
let selectCell (gameId : int, cellId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> TurnService.getCellSelectedEvent (game, cellId) session)

[<ClientFunction(HttpMethod.Post, "/games/%i/current-turn/reset-request", ClientSection.Turn)>]
let resetTurn (gameId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> TurnService.getResetTurnEvent game session)

[<ClientFunction(HttpMethod.Post, "/games/%i/current-turn/commit-request", ClientSection.Turn)>]
let commitTurn (gameId : int) (session : Session) : StateAndEventResponse AsyncHttpResult =
    processEvent gameId (fun game -> TurnService.getCommitTurnEvent game session)
﻿namespace Djambi.Api.Http

open Microsoft.AspNetCore.Http

open Giraffe

open Djambi.Api.Http.LobbyJsonModels
open Djambi.Api.Http.LobbyMappings
open Djambi.Api.Persistence

type LobbyController(repository : LobbyRepository) =
    //Users
    member this.createUser =
        fun (next : HttpFunc)(ctx : HttpContext) ->
            task {
                let! requestJson = ctx.BindJsonAsync<CreateUserJsonModel>()
                let request = requestJson |> mapCreateUserRequest
                let! response = repository.createUser(request)
                let responseJson = response |> mapUserResponse
                return! json responseJson next ctx
            }

    member this.deleteUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! _ = repository.deleteUser(userId)
            return! json () next ctx
        }

    member this.getUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = repository.getUser(userId)
                let responseJson = response |> mapUserResponse
                return! json responseJson next ctx
            }

    member this.updateUser(userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Update user %i not yet implemented" userId
                }
                return! json placeHolderResponse next ctx
            }

//Game lobby
    member this.getOpenGames =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! response = repository.getOpenGames()
                let responseJson = response |> List.map mapLobbyGameResponse
                return! json responseJson next ctx
            }

    member this.createGame =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! requestJson = ctx.BindModelAsync<CreateGameJsonModel>()
                let request = requestJson |> mapCreateGameRequest
                let! response = repository.createGame(request)
                let responseJson = response |> mapLobbyGameResponse
                return! json responseJson next ctx
            }

    member this.deleteGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let! _ = repository.deleteGame(gameId)
                return! json () next ctx
            }

    member this.addPlayerToGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Add player %i to game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.removePlayerFromGame(gameId : int, userId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Delete player %i from game %i not yet implemented" userId gameId
                }
                return! json placeHolderResponse next ctx
            }

    member this.startGame(gameId : int) =
        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                let placeHolderResponse = {
                    text = sprintf "Start game %i not yet implemented" gameId
                }
                return! json placeHolderResponse next ctx
            }
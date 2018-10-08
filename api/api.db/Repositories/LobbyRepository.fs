﻿namespace Djambi.Api.Db.Repositories

open System.Data
open System.Threading.Tasks
open Dapper
open FSharp.Control.Tasks
open Djambi.Api.Common
open Djambi.Api.Common.Enums
open Djambi.Api.Db.Mappings.LobbyDbMapping
open Djambi.Api.Db.Model.LobbyDbModel
open Djambi.Api.Db.SqlUtility
open Djambi.Api.Model.LobbyModel
    
module LobbyRepository =

//Games
    let createGame(request : CreateGameRequest) : LobbyGameMetadata Task =
        let param = new DynamicParameters()
        param.Add("BoardRegionCount", request.boardRegionCount)
        param.AddOption("Description", request.description)

        let cmd = proc("Lobby.CreateGame", param)

        task {
            use cn = getConnection()
            let! gameId = cn.QuerySingleAsync<int>(cmd)
            return {
                id = gameId 
                description = request.description
                status = GameStatus.Open
                boardRegionCount = request.boardRegionCount
                players = List.empty
            }
        }
        
    let deleteGame(gameId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        let cmd = proc("Lobby.DeleteGame", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let private getGamesInner(gameId : int option, userId : int option, status : GameStatus option) : LobbyGameMetadata list Task =
        let param = new DynamicParameters()
        param.AddOption("GameId", gameId)
        param.AddOption("UserId", userId)
        param.AddOption("StatusId", status |> Option.map mapGameStatusToId)
        let cmd = proc("Lobby.GetGamesWithPlayers", param)

        task {
            use cn = getConnection()
            let! sqlModels = cn.QueryAsync<LobbyGamePlayerSqlModel>(cmd)
            return sqlModels |> mapLobbyGamesResponse
        }
        
    let getGame(gameId : int) : LobbyGameMetadata Task =
        getGamesInner(Some gameId, None, None)
        |> Task.map List.head

    let getGames() : LobbyGameMetadata list Task =
        getGamesInner(None, None, None)

    let getOpenGames() : LobbyGameMetadata list Task =
        getGamesInner(None, None, Some Open)

    let getUserGames(userId : int) : LobbyGameMetadata list Task =
        getGamesInner(None, Some userId, None)

//Players
    let addPlayerToGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        
        let cmd = proc("Lobby.AddPlayerToGame", param)

        task {
            use cn = getConnection()
            let! playerId = cn.QuerySingleAsync<int>(cmd) //Id not currently used
            return ()
        }

    let addVirtualPlayerToGame(gameId : int, name : string) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("Name", name)

        let cmd = proc("Lobby.AddVirtualPlayerToGame", param)

        task {
            use cn = getConnection()
            let! playerId = cn.QuerySingleAsync<int>(cmd) //Id not currently used
            return ()
        }

    let removePlayerFromGame(gameId : int, userId : int) : Unit Task =
        let param = new DynamicParameters()
        param.Add("GameId", gameId)
        param.Add("UserId", userId)
        let cmd = proc("Lobby.RemovePlayerFromGame", param)

        task {
            use cn = getConnection()
            let! _ = cn.ExecuteAsync(cmd)
            return ()
        }

    let getVirtualPlayerNames() : string list Task =
        let param = new DynamicParameters()

        let cmd = proc("Lobby.GetVirtualPlayerNames", param)

        task {
            use cn = getConnection()
            return! cn.QueryAsync<string>(cmd)
                    |> Task.map Seq.toList
        } 

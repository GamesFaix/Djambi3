﻿namespace Djambi.Api.IntegrationTests.Db

open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common.Control.AsyncHttpResult
open Djambi.Api.Db.Repositories
open Djambi.Api.IntegrationTests
open Djambi.Api.Model

type GameRepositoryTests() =
    inherit TestsBase()

    [<Fact>]
    let ``Create game should work``() =
        //Arrange
        let userId = 1
        let request = getCreateGameRequest(userId)
        task {
            //Act
            let! gameId = GameRepository.createGame request |> thenValue

            //Assert
            Assert.NotEqual(0, gameId)
        }

    [<Fact>]
    let ``Get game should work`` () =
        //Arrange
        let userId = 1
        let request = getCreateGameRequest(userId)
        task {
            let! gameId = GameRepository.createGame request |> thenValue

            //Act
            let! game = GameRepository.getGame gameId |> thenValue

            //Assert
            Assert.Equal(gameId, game.id)
            Assert.Equal(request.parameters, game.parameters)
        }
        
    [<Fact>]
    let ``Get games should work``() =
        //Arrange
        let userId = 1
        let request = getCreateGameRequest(userId)
        task {
            let! gameId = GameRepository.createGame request |> thenValue
            let query = GamesQuery.empty

            //Act
            let! games = GameRepository.getGames query |> thenValue

            //Assert
            let exists = games |> List.exists (fun l -> l.id = gameId)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add user player should work``() =
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = GameRepository.createGame gameRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.user user.id

            //Act
            let! _ = GameRepository.addPlayer (gameId, request) |> thenValue

            //Assert
            let! players = GameRepository.getPlayersForGames [gameId] |> thenValue
            let exists = players
                         |> List.exists (fun p -> p.userId = Some user.id
                                                  && p.name = user.name
                                                  && p.kind = PlayerKind.User)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add neutral player should work``() =
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        task {
            let! gameId = GameRepository.createGame gameRequest |> thenValue
            let request = CreatePlayerRequest.neutral "test"

            //Act
            let! _ = GameRepository.addPlayer (gameId, request) |> thenValue

            //Assert
            let! players = GameRepository.getPlayersForGames [gameId] |> thenValue
            let exists = players |> List.exists (fun p ->
                p.userId = None
                && p.name = request.name.Value
                && p.kind = PlayerKind.Neutral)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Add guest player should work``() =
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = GameRepository.createGame gameRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let request = CreatePlayerRequest.guest (user.id, "test")

            //Act
            let! _ = GameRepository.addPlayer (gameId, request) |> thenValue

            //Assert
            let! players = GameRepository.getPlayersForGames [gameId] |> thenValue
            let exists = players |> List.exists (fun p ->
                p.userId = Some user.id
                && p.name = request.name.Value
                && p.kind = PlayerKind.Guest)
            Assert.True(exists)
        }

    [<Fact>]
    let ``Remove player should work``() =
        //Arrange
        let userId = 1
        let gameRequest = getCreateGameRequest(userId)
        let userRequest = getCreateUserRequest()
        task {
            let! gameId = GameRepository.createGame gameRequest |> thenValue
            let! user = UserRepository.createUser userRequest |> thenValue
            let playerRequest = CreatePlayerRequest.user user.id
            let! player = GameRepository.addPlayer (gameId, playerRequest) |> thenValue

            //Act
            let! _ = GameRepository.removePlayer player.id |> thenValue

            //Assert
            let! players = GameRepository.getPlayersForGames [gameId] |> thenValue
            let exists = players |> List.exists (fun p -> p.id = player.id)
            Assert.False(exists)
        }
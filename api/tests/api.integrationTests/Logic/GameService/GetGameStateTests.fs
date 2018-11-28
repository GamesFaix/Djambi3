﻿namespace Djambi.Api.IntegrationTests.Logic.GameService

open System
open FSharp.Control.Tasks
open Xunit
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.IntegrationTests
open Djambi.Api.Logic.Services
open Djambi.Api.Model.PlayerModel

type GetGameStateTests() =
    inherit TestsBase()

//TODO: Get game state should work

//TODO: Get game state should fail if not logged in

//TODO: Get game state should fail if not in game and not admin

//TODO: Get game state shoudl work if not in game, but admin
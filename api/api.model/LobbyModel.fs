﻿namespace Djambi.Api.Model

open System
open Djambi.Api.Common.Enums

module LobbyModel =

    type Role = 
        | Admin
        | Normal
        | Guest

    type User = 
        {
            id : int
            name : string
            role : Role
            password : string
            failedLoginAttempts : int
            lastFailedLoginAttemptOn : DateTime option
        }
        
    type LobbyPlayer =
        {
            id : int
            userId : int option
            name : string
        }

    type CreateUserRequest =
        {
            name : string
            role : Role
            password : string
        }

    type CreateGameRequest = 
        {
            description : string option
            boardRegionCount : int
        }
                
    type LobbyGameMetadata = 
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            description : string option
            players : LobbyPlayer list
        }

    type LoginRequest = 
        {
            userName : string
            password : string
        }

    type Session =
        {
            id : int
            userIds : int list
            token : string
            createdOn : DateTime
            expiresOn : DateTime
            isShared : bool
        }
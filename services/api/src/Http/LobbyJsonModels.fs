namespace Djambi.Api.Http

module LobbyJsonModels =

    open System
    open Djambi.Api.Common.Enums
    
    [<CLIMutable>]
    type PlaceHolderJsonModel =
        {
            text : string
        }

    //Use for POST and PATCH /users
    [<CLIMutable>]
    type CreateUserJsonModel =
        {
            name : string
        }

    //Use for GET /users
    [<CLIMutable>]
    type UserJsonModel =
        {
            id : int
            name : string
        }
        
    type PlayerJsonModel =
        {
            id : int
            userId : int Nullable
            name : string
        }

    //Use for POST /games
    [<CLIMutable>]
    type CreateGameJsonModel =
        {
            boardRegionCount : int    
            description : string
        }

    //Use for GET /games
    type LobbyGameJsonModel = 
        {
            id : int
            status : GameStatus
            boardRegionCount : int
            description : string
            players : PlayerJsonModel list
        }
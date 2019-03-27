﻿namespace Djambi.Api.Logic.Services

open System.Collections.Concurrent
open Djambi.Api.Common.Control
open Djambi.Api.Logic.Interfaces

type NotificationService() =
    let subscribers = new ConcurrentDictionary<int, ISubscriber>()
    
    interface INotificationService with
        member x.add subscriber =
            subscribers.[subscriber.userId] <- subscriber

        member x.remove userId =
            subscribers.TryRemove userId
            |> ignore

        member x.send response =
            let creatorId = response.event.createdByUserId

            let otherUserIds =
                response.game.players
                |> Seq.filter (fun p -> 
                    p.userId.IsSome && 
                    p.userId.Value <> creatorId
                )
                |> Seq.map (fun p -> p.userId.Value)
                |> Seq.toList            

            subscribers.Values
            |> Seq.filter (fun s -> otherUserIds |> List.contains s.userId)
            |> Seq.map (fun s -> s.send response)
            |> AsyncHttpResult.whenAll
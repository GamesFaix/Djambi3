﻿namespace Djambi.Api.Db

open Djambi.Api.Db.Repositories
open Djambi.Api.Db.Interfaces

type DbRoot(connectionString : string) =
    let ctxProvider = CommandContextProvider(connectionString)
    let _games = GameRepository(ctxProvider)
    let _users = UserRepository(ctxProvider)
    let _events = EventRepository(ctxProvider, _games)
    let _sessions = SessionRepository(ctxProvider, _users)
    let _snapshots = SnapshotRepository(ctxProvider)

    member x.events = _events
    member x.games = _games
    member x.sessions = _sessions
    member x.snapshots = _snapshots
    member x.users = _users

    interface IDbRoot with  
        member x.events = x.events :> IEventRepository
        member x.games = x.games :> IGameRepository
        member x.sessions = x.sessions :> ISessionRepository
        member x.snapshots = x.snapshots :> ISnapshotRepository
        member x.users = x.users :> IUserRepository
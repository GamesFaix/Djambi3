namespace Djambi.Api.Logic.Services

open Djambi.Api.Common.Collections
open Djambi.Api.Common.Control
open Djambi.Api.Model
open Djambi.Api.Logic

type PlayerStatusChangeService(eventServ : EventService,
                               indirectEffectsServ : IndirectEffectsService) =

    let getFinalAcceptDrawEffects(game : Game, request : PlayerStatusChangeRequest) : Effect list =
        let otherLivingPlayersNotAcceptingADraw =
            game.players
            |> List.filter (fun p ->
                p.id <> request.playerId &&
                p.status = Alive
            )

        if otherLivingPlayersNotAcceptingADraw.IsEmpty
        then
            [
                Effect.GameStatusChanged { oldValue = InProgress; newValue = Over }
            ]
        else [] //If not last player to accept, nothing special happens

    member x.getUpdatePlayerStatusEvent (game : Game, request : PlayerStatusChangeRequest) (session : Session) : CreateEventRequest HttpResult =
        if game.status <> InProgress then
            Error <| HttpException(400, "Cannot change player status unless game is InProgress.")
        else
            Security.ensurePlayerOrHas OpenParticipation session game
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
                            let effects = new ArrayList<Effect>()

                            let primary = Effect.PlayerStatusChanged {
                                oldStatus = oldStatus
                                newStatus = Conceded
                                playerId = player.id
                            }

                            effects.Add(primary)
                            let game = eventServ.applyEffect primary game
                            effects.AddRange (indirectEffectsServ.getIndirectEffectsForConcede (game, request))
                            Ok { event with effects = effects |> Seq.toList }
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
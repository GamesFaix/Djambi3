namespace Djambi.Api.Logic.Managers

open Djambi.Api.Logic.Interfaces
open Djambi.Api.Logic.ModelExtensions
open System.Threading.Tasks
open Djambi.Api.Common.Control
open Djambi.Api.Logic.ModelExtensions.BoardModelExtensions
open Djambi.Api.Model

type BoardManager() =
    interface IBoardManager with
        member __.getBoard regionCount session =
            BoardModelUtility.getBoard regionCount |> Task.FromResult

        member __.getCellPaths (regionCount, cellId) session =
            let board = BoardModelUtility.getBoardMetadata(regionCount)
            let cell = board.cells() |> Seq.find(fun c -> c.id = cellId)
            board.pathsFromCell(cell)
            |> List.map (List.map (fun c -> c.id))
            |> Task.FromResult
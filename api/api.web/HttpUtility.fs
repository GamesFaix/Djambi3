﻿namespace Djambi.Api.Web

open System
open System.Threading.Tasks
open Giraffe
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Primitives
open Djambi.Api.Common
open Djambi.Api.Common.AsyncHttpResult
open Djambi.Api.Logic.Services
open Djambi.Api.Model.SessionModel

type HttpHandler = HttpFunc -> HttpContext -> HttpContext option Task 

module HttpUtility = 

    let handle<'a> (func : HttpContext -> 'a AsyncHttpResult) : HttpHandler =

        fun (next : HttpFunc) (ctx : HttpContext) ->
            task {
                try 
                    let! result = func ctx
                    match result with
                    | Ok value -> 
                        ctx.Response.Headers.Add("Access-Control-Allow-Credentials", StringValues("true"))
                        return! json value next ctx
                    | Error ex -> 
                        ctx.SetStatusCode ex.statusCode
                        return! json ex.Message next ctx                    
                with
                | :? HttpException as ex -> 
                    ctx.SetStatusCode ex.statusCode
                    return! json ex.Message next ctx
                | _ as ex -> 
                    ctx.SetStatusCode 500
                    return! json ex.Message next ctx
            }
            
    let cookieName = "DjambiSession"

    let getSessionFromContext (ctx : HttpContext) : Session AsyncHttpResult =
        let token = ctx.Request.Cookies.Item(cookieName)

        if token |> String.IsNullOrEmpty
        then errorTask <| HttpException(401, "Not signed in.")
        else 
            SessionService.getSession token
            |> thenReplaceError 404 (HttpException(401, "Not signed in."))

    let getSessionAndModelFromContext<'a> (ctx : HttpContext) : ('a * Session) AsyncHttpResult =
        getSessionFromContext ctx 
        |> thenBindAsync (fun session -> 
            ctx.BindModelAsync<'a>()
            |> Task.map (fun model -> Ok (model, session))
        )

    let ensureNotSignedIn (ctx : HttpContext) : Result<Unit, HttpException> =
        let token = ctx.Request.Cookies.Item(cookieName)
        if token |> String.IsNullOrEmpty |> not
        then Error <| HttpException(401, "Operation not allowed if already signed in.")
        else Ok ()

    let ensureNotSignedInAndGetModel<'a> (ctx : HttpContext) : 'a AsyncHttpResult =
        match ensureNotSignedIn ctx with
        | Error ex -> errorTask ex
        | Ok _ -> ctx.BindModelAsync<'a>()
                  |> Task.map Ok
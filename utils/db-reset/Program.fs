﻿open System
open System.Data.SqlClient
open System.IO
open System.Reflection
open System.Text.RegularExpressions

open Dapper

open Microsoft.Extensions.Configuration

open Djambi.Utilities

let private env = Environment.load(5);
let private config = 
    ConfigurationBuilder()
        .AddJsonFile("appsettings.json", false)
        .Build()

let private getSqlDirectory =
    let asm = Assembly.GetExecutingAssembly()
    let uri = new Uri(asm.CodeBase)
    let asmDir = uri.LocalPath |> Path.GetDirectoryName
    let relativeDir = config.["sqlDirectoryRelativePath"]
    Path.Combine(asmDir, relativeDir)

let private getConnectionString name =
    config.GetConnectionString(name).Replace("{sqlAddress}", env.sqlAddress)
   
let private masterConnectionString = getConnectionString "master"        
let private djambiConnectionString = getConnectionString "djambi"

let private executeCommand (cnStr : string)(command : string) : unit =
    use cn = new SqlConnection(cnStr)
    
    cn.Execute(command) |> ignore
    
let private dropAndCreateDb() : unit =
    printfn "Dropping and creating database"
    let sql = "IF EXISTS(SELECT * FROM sys.databases WHERE name='Djambi')
               DROP DATABASE Djambi;
               CREATE DATABASE Djambi;
               ALTER DATABASE Djambi SET COMPATIBILITY_LEVEL = 120" //120 = SQL Server 2014
    executeCommand masterConnectionString sql

let private loadFile (relativePath : string) : unit =
    printfn "Loading %s" relativePath
    let absolutePath = Path.Combine(getSqlDirectory, relativePath)
    let text = File.ReadAllText(absolutePath)
    let commands = Regex.Split(text, "\s+GO")
                   |> Seq.filter (String.IsNullOrEmpty >> not)
    for c in commands do    
        executeCommand djambiConnectionString c

let getFilesInOrder : string seq =
    seq {
        //Order is very important for foreign keys
        let tables = [

            //Static data first
            "EventKinds"
            "GameStatuses"
            "NeutralPlayerNames"
            "PlayerKinds"
            "PlayerStatuses"
            "Privileges"

            //Then entities
            "Users"
            "Sessions"
            "Games"
            "Players"
            "Events"

            //Then relations
            "UserPrivileges"
        ]

        yield! tables |> Seq.map (fun name -> sprintf "Tables\\dbo.%s.sql" name)

        let getFiles folder =
            Path.Combine(getSqlDirectory, folder)
            |> Directory.EnumerateFiles
            |> Seq.map (fun path -> Path.Combine(folder, Path.GetFileName(path)))

        let folders = [
            "Types\\User-defined Data Types"
            "Stored Procedures"
            "Data"
        ]
 
        yield! folders |> Seq.collect getFiles
    }

let createAdminUser() : unit =
    printfn "Creating admin user"

    let cmd = sprintf 
                "INSERT INTO dbo.Users ([Name], [Password], [CreatedOn], [FailedLoginAttempts], [LastFailedLoginAttemptOn]) 
                 VALUES (N'%s', N'%s', GETUTCDATE(), 0, NULL)"
                env.adminUsername
                env.adminPassword

    executeCommand djambiConnectionString cmd

[<EntryPoint>]
let main argv =
    dropAndCreateDb()

    for f in getFilesInOrder do
        loadFile f

    createAdminUser()
 
    printfn "Done"
    0

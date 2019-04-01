﻿open System
open Aardvark.Base
open Aardvark.Application
open Aardvark.Application.Slim
open Aardvark.UI
open Aardium
open Inc

open Suave
open Suave.WebPart

open System.Threading

[<EntryPoint; STAThread>]
let main argv = 
    let port,processId = 
        match argv with
            | [|port;processId|] -> 
                match System.Int32.TryParse port, System.Int32.TryParse processId with
                    | (true,v), (true, pId) -> v, pId
                    | _ -> 
                        failwith "usage: port parentProcessId"
            | _ -> failwith "usage: port parentProcessId"

    let killThread = 
        Thread(ThreadStart(fun _ -> 
            let p = System.Diagnostics.Process.GetProcessById processId
            p.WaitForExit()
            System.Environment.Exit(2)
        ))
    killThread.Start()


    let uri = sprintf "http://localhost:%d/" port
    Log.line "Service will run here: %s" uri


    Ag.initialize()
    Aardvark.Init()
    Aardium.init()

    use app = new OpenGlApplication()
    let instance = App.app |> App.start

    WebPart.runServer port [ 
        MutableApp.toWebPart' app.Runtime false instance
        Suave.Files.browseHome
    ] |> ignore

    0 

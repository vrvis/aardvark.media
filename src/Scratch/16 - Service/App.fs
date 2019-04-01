﻿module Inc.App
open Aardvark.UI
open Aardvark.UI.Primitives

open Aardvark.Base
open Aardvark.Base.Incremental
open Aardvark.Base.Rendering
open Inc.Model

module Support =
    open System.Net
    open System.Net.Sockets

    let getFreePort() =  
        try
            let ep = new IPEndPoint(IPAddress.Loopback, 0)
            use s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            s.Bind(ep)
            let ep = unbox<IPEndPoint> s.LocalEndPoint
            ep.Port
        with _ ->
            4321

let update (send : InstanceMessage -> unit) (model : Model) (msg : Message) =
    match msg with
        | Start e -> 
            let id = System.Guid.NewGuid() |> string
            let port = Support.getFreePort()
            let stdout s = send (InstanceMessage.Stdout(id,s))
            let stderr s = send (InstanceMessage.Stdout(id,s))
            let dead () = send (InstanceMessage.Exit id)
            let p = InterProcess.runService e.assembly e.workingDirectory port stdout stderr dead id
            let instance = 
                {                   
                    p = p
                    id = id
                    port = port
                    endpoint = e
                }
            { model with running = HMap.add id instance model.running }
        | InstanceStatus (InstanceMessage.Exit e) -> 
            printfn "removing instance: %A" e
            { model with running = HMap.remove e model.running }
        | InstanceStatus (InstanceMessage.Stderr(id,e)) -> 
            printfn "instance %A errors: %s" id e
            model
        | InstanceStatus (InstanceMessage.Stdout(id,e)) -> 
            printfn "instance %A says: %s" id e 
            model
        | Kill i -> 
            match HMap.tryFind i model.running with
                | Some instance -> 
                    Log.warn "killing: %A" instance
                    instance.p.Kill()
                    model
                | _ -> 
                    Log.warn "cannot kill non-existing instance"
                    model

let view (model : MModel) =
    let runningInstances = model.running |> AMap.toASet |> ASet.sortBy fst
    require Html.semui (
        page (fun request -> 
            match Map.tryFind "page" request.queryParams with
                | Some "client" -> 
                    match Map.tryFind "id" request.queryParams with
                        | Some id -> 
                            let link = sprintf "./%s/stdout.txt" id
                            div [] [
                                a [attribute "href" link] [text "Output"]
                                a [attribute "href" "javascript:history.back()"] [text "Back"]  
                            ]
                        | None -> text "id not found"
                | _ -> 
                    div [] [
                        Incremental.div AttributeMap.empty <|
                            alist {
                                for endpoint in model.executables do
                                    yield 
                                        button [onClick (fun _ -> Start endpoint)] [
                                            text endpoint.prettyName
                                        ]

                            }
                        Incremental.div AttributeMap.empty <| 
                            alist {
                                for (id,i) in runningInstances do
                                    yield div [] [
                                        let link = sprintf ".?page=client&id=%s" id
                                        yield a [attribute "href" link] [text "super"]
                                        yield button [onClick (fun _ -> Kill id)] [text "kill"]
                                    ]
                            }
                    ]
        )
    )


let threads (model : Model) = 
    ThreadPool.empty


module Discovery = 
    open System
    open System.IO

    let discoverApps () =  
        [ 
            { assembly = "17 - Serviced.dll"         
              workingDirectory = "."
              prettyName = "Demo"       
              url = (fun port -> sprintf "http://localhost:%d" port)
            }
        ]


let app send =                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
    {
        unpersist = Unpersist.instance     
        threads = threads 
        initial = 
            { 
               executables = 
                    Discovery.discoverApps() |> PList.ofList
               running = HMap.empty
            }
        update = update send
        view = view
    }

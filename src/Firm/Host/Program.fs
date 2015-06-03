open System
open System.IO
open System.Diagnostics
open Microsoft.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.FileSystems
open Microsoft.Owin.StaticFiles
open Owin

let parseArgs = function
    | [|e|] -> e
    | _ -> Directory.GetCurrentDirectory()

let configure root (app: IAppBuilder) =
    let options = FileServerOptions(FileSystem = PhysicalFileSystem(root), RequestPath = PathString.Empty)
    app.UseFileServer(options) |> ignore

[<EntryPoint>]
let main args = 
    let root = parseArgs args
    printfn "Starting http server on path: %s" root
    use s = WebApp.Start("http://localhost:8080", configure root)
    let url = "http://localhost:8080"
    printfn "Listening at %s" url
    printfn "Press enter to exit..."
    Process.Start(url) |> ignore
    Console.ReadLine() |> ignore
    0

open System
open System.IO
open System.Diagnostics
open Microsoft.Owin
open Microsoft.Owin.Hosting
open Microsoft.Owin.FileSystems
open Microsoft.Owin.StaticFiles
open Owin

let parseArgs = function
    | [|e; b|] -> (e, Some b)
    | [|e|] -> (e, None)
    | _ -> (Directory.GetCurrentDirectory(), None)

let configure root (baseUri : string option) (app : IAppBuilder) =
    let options =
        match baseUri with
        | Some b -> FileServerOptions(EnableDirectoryBrowsing = true, FileSystem = PhysicalFileSystem(root), RequestPath = PathString.FromUriComponent(b))
        | None -> FileServerOptions(FileSystem = PhysicalFileSystem(root), RequestPath = PathString.Empty)
    app.UseFileServer(options) |> ignore

[<EntryPoint>]
let main args = 
    let (root, baseUri) = parseArgs args
    printfn "Starting http server on path: %s" root
    use s = WebApp.Start("http://localhost:8080", configure root baseUri)
    let url = "http://localhost:8080" + (baseUri |> (function | Some b -> b | None -> ""))
    printfn "Listening at %s" url
    printfn "Press enter to exit..."
    Process.Start(url) |> ignore
    Console.ReadLine() |> ignore
    0

open System
open System.IO
open System.Diagnostics
open Microsoft.Owin.Hosting
open Microsoft.Owin.FileSystems
open Microsoft.Owin.StaticFiles
open Owin

[<EntryPoint>]
let main args = 
    let root = args |> function | [|e|] -> e | _ -> Directory.GetCurrentDirectory()
    Console.WriteLine("Starting http server on path: {0}", root)
    let url = "http://localhost:8080"
    let options = FileServerOptions(EnableDirectoryBrowsing = true, FileSystem = PhysicalFileSystem(root))
    WebApp.Start(url, fun builder -> builder.UseFileServer(options) |> ignore) |> ignore
    Console.WriteLine("Listening at " + url);
    Console.WriteLine("Press enter to exit...")
    Process.Start(url) |> ignore
    Console.ReadLine() |> ignore
    0

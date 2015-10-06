open System
open System.IO
open System.Diagnostics

let run prog arguments =
    let processStartInfo = ProcessStartInfo(prog, arguments)
    processStartInfo.UseShellExecute <- false
    processStartInfo.CreateNoWindow <- true
    let processor = Process.Start(processStartInfo)
    processor.WaitForExit()

let Init() =
 System.Console.WriteLine "Initializing packages..."
 run "mono" ".paket/paket.exe restore"
 System.Console.WriteLine "Done!"

let CallFake target =
  if Directory.Exists "packages" <> true then
    Init()
  System.Console.WriteLine (sprintf "Calling FAKE with target %s..." target)
  run "mono" (sprintf "packages/FAKE/tools/FAKE.exe fake.fsx %s" target)
  System.Console.WriteLine "Done!"

let Generate() =
  if Directory.Exists "bin" <> true then
    CallFake "Build"
  System.Console.WriteLine "Generating site"
  run "mono" "bin/Generator.exe"

let Preview() =
  if Directory.Exists "output" <> true then
    Generate()
  System.Console.WriteLine "Starting HTTP server to preview site..."
  run "mono" "bin/Host.exe '../output'"
  System.Console.WriteLine "Done!"

let Reset() =
  if Directory.Exists "packages" then
    System.Console.WriteLine "Removing packages..."
    run "rm" "-r packages"
  if Directory.Exists "bin" then
    System.Console.WriteLine "Removing bin..."
    run "rm" "-r bin"
  if Directory.Exists "output" then
    System.Console.WriteLine "Removing output..."
    run "rm" "-r output"
  System.Console.WriteLine "Done!"

let Help() =
  System.Console.WriteLine "Command format: fsharpi firm.fsx <command>"
  System.Console.WriteLine "Available commands:"
  System.Console.WriteLine "help        lists the available arguments"
  System.Console.WriteLine "reset       Removes all downloaded and generated files/folders."
  System.Console.WriteLine "init        Installs the required paket dependencies."
  System.Console.WriteLine "clean       Cleans the build directory."
  System.Console.WriteLine "build       Builds the firm application."
  System.Console.WriteLine "generate    Generates the web site."
  System.Console.WriteLine "preview     Starts a local http server for previewing the generated blog."

let Router (args:string[]) =
  match (args.[1].ToString()) with
  | "build" -> CallFake "Build"
  | "clean" -> CallFake "Clean"
  | "generate" -> Generate()
  | "help" -> Help()
  | "init" -> Init()
  | "preview" -> Preview()
  | "reset" -> Reset()
  | _ -> ()

let start() =
  let args = fsi.CommandLineArgs
  if args.Length > 1 then
    Router args
  else
    Help()

start()

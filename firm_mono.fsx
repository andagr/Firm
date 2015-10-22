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
    Directory.Delete("packages", true)
  if Directory.Exists "bin" then
    System.Console.WriteLine "Removing bin..."
    Directory.Delete("bin", true)
  if Directory.Exists "output" then
    System.Console.WriteLine "Removing output..."
    Directory.Delete("output", true)
  System.Console.WriteLine "Done!"

let IsDuplicateBlogPost (args:string[]) =
  System.Console.WriteLine "Validating the blog directory is unique..."
  let title = args.[2].ToString()
  if Directory.Exists (sprintf "data/input/blog/post/%s" title) then
    System.Console.WriteLine "A blog post with this title already exists."
    false
  else
    true

let IsDirectoryArgExist (args:string[]) =
  System.Console.WriteLine "Validating the blog parameter was provided..."
  if args.Length = 2 then
    System.Console.WriteLine "The required blog parameter is missing."
    false
  else
    true

let IsPostDirectoryExist (args:string[]) =
  System.Console.WriteLine "Validating the post directory exists..."
  if Directory.Exists "data/input/blog/post" <> true then
    System.Console.WriteLine "The required data/input/blog/posts directory is missing."
    false
  else
    true

let IsValidateBlogCommand (args:string[]) =
  System.Console.WriteLine "Validating the blog command..."
  IsDirectoryArgExist args && IsPostDirectoryExist args && IsDuplicateBlogPost args

let CreateBlogMarkdownFile path title =
  System.Console.WriteLine "Creating the blog post markdown file..."
  let markdownFile = File.Create (path + "/index.md")
  markdownFile.Close()
  markdownFile.Dispose()
  System.Console.WriteLine "Writing title to blog post markdown file..."
  File.WriteAllText((path + "/index.md"),
    (sprintf @"%s" ("> " + title)))

let CreateJsonMetaFile path title =
  System.Console.WriteLine "Creating the blog post json meta file..."
  let jsonMetaFile = File.Create (path + "/meta.json")
  jsonMetaFile.Close()
  jsonMetaFile.Dispose()
  System.Console.WriteLine "Writing meta data to blog post json meta file..."
  File.WriteAllText((path + "/meta.json"),
    (sprintf
    @"{
    ""title"": ""%s"",
    ""date"": ""%s"",
    ""tags"": [ """", """" ]
    }" title (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))))

let Blog (args:string[]) =
  System.Console.WriteLine "Creating Blog Post..."
  if (IsValidateBlogCommand args) then
    let title = args.[2].ToString()
    let blogPostDirectoryPath = (sprintf "data/input/blog/post/%s" title)
    System.Console.WriteLine "Creating the blog post directory..."
    let blogPostDirectory = Directory.CreateDirectory(blogPostDirectoryPath)
    CreateBlogMarkdownFile blogPostDirectoryPath title
    CreateJsonMetaFile blogPostDirectoryPath title
    System.Console.WriteLine "Done!"
  else
    System.Console.WriteLine "Please correct the command and try again!"
    
let Help() =
  System.Console.WriteLine "Command format: fsharpi firm_mono.fsx <command> <argument>"
  System.Console.WriteLine "Available commands:"
  System.Console.WriteLine "help                      Lists the available arguments."
  System.Console.WriteLine "reset                     Removes all downloaded and generated files/folders."
  System.Console.WriteLine "init                      Installs the required paket dependencies."
  System.Console.WriteLine "clean                     Cleans the build directory."
  System.Console.WriteLine "build                     Builds the firm application."
  System.Console.WriteLine "generate                  Generates the web site."
  System.Console.WriteLine "preview                   Starts a local http server for previewing the generated blog."
  System.Console.WriteLine "blog <title-of-blog-post> Creates the required blog post directory and files."

let Router (args:string[]) =
  match (args.[1].ToString()) with
  | "blog" -> Blog args
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

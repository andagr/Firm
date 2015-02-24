namespace Generator

open System.IO
open Files
open FirmRazor
open ViewModels
open FSharp.Data
open FSharp.Literate

module Transformation =
    type Meta = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    let splitInputs fromDir =
        let split allInputs input =
            let posts, pages, resources = allInputs
            match input with
            | Post po -> (po::posts, pages, resources)
            | Page pa -> (posts, pa::pages, resources)
            | Resource r -> (posts, pages, r::resources)
        fromDir
        |> inputFiles
        |> Seq.fold split ([], [], [])

    let postModels posts =
        let toModel (post:Post) =
            let name = contentName post.File
            let meta = Meta.Load(post.Meta)
            let doc =
                match post.InputType with
                | Md -> Literate.WriteHtml(Literate.ParseMarkdownFile(post.File))
                | Html -> File.ReadAllText(post.File)
            (PostModel(name, meta.Title, meta.Date, meta.Tags, doc), post.File)
        posts |> List.map toModel

    let processResources fd td (resources:Resource list) =
        resources
        |> List.iter (fun r -> copy fd td r.File)

    let processPosts fd td (posts:Post list) =
        let all = postModels posts
        let latest, file = 
            all
            |> List.sortBy (fun (pm, _) -> pm.Date)
            |> List.rev
            |> List.head
        writePost latest (targetFile fd td file)
        // Todo: Better handling of file / targetFile

    let processInputs fd td (posts, pages, resources) =
        processResources fd td resources
        processPosts fd td posts
        //Todo: Process pages

    let generate root =
        let fromDir = root @+ "input"
        let toDir = root @+ "output"
        fromDir
        |> splitInputs
        |> processInputs fromDir toDir

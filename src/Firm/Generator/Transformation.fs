namespace Generator

open System.IO
open Files
open Output
open Models
open FSharp.Data
open FSharp.Literate

module Transformation =
    type Meta = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    let dirEnumerator id =
        Directory.EnumerateFiles(id, "*", SearchOption.AllDirectories)

    let postModels posts =
        let toModel (post:PostFile) =
            let name = contentName post.File
            let meta = Meta.Load(post.Meta)
            let doc =
                match post.InputType with
                | Md -> Literate.WriteHtml(Literate.ParseMarkdownFile(post.File))
                | Html -> File.ReadAllText(post.File)
            (PostModel(name, meta.Title, meta.Date, meta.Tags, doc), post.File)
        posts |> List.map toModel

    let processPosts fd td (posts:PostFile list) =
        let all = postModels posts
        let latest, file = 
            all
            |> List.sortBy (fun (pm, _) -> pm.Date)
            |> List.rev
            |> List.head
        writePost latest (targetFile fd td file)
        // Todo: Better handling of file / targetFile

    let processPosts (posts: PostFile list) =
        ()

    let processPages (pages: PageFile list) =
        ()

    let processResources (resources: ResourceFile list) =
        resources
        |> List.iter Output.copyResource

    let private processInputs (posts, pages, resources) =
        processPosts posts
        processPages pages
        processResources resources

    let generate root =
        let id = root @+ "input"
        let od = root @+ "output"
        Files.inputFiles dirEnumerator (id, od)
        |> processInputs

namespace Generator

open System.IO
open Files
open Output
open Models
open FSharp.Data
open FSharp.Literate

module Transformation =
    type ConfigReader = JsonProvider<"""{ "disqusShortname": "a-name" }""">
    type MetaReader = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    type Config =
        { DisqusShortname: string }

    let private dirEnumerator id =
        Directory.EnumerateFiles(id, "*", SearchOption.AllDirectories)

    let private fileExists file =
        File.Exists(file)

    let processPosts config index archive (posts: PostFile list) =
        let postModels =
            posts
            |> List.map (fun p ->
                let meta = MetaReader.Load(p.Meta)
                let doc = Literate.WriteHtml(Literate.ParseMarkdownFile(p.File.Input))
                p, PostModel(meta.Title, meta.Date, meta.Tags, doc))
        postModels
        |> List.map (fun (pf, p) -> (pf, SinglePostModel(config.DisqusShortname, p)))
        |> List.iter Output.Razor.writePost
        let mPostModels = postModels |> List.map snd
        Output.Razor.writeArchive mPostModels archive 
        index |> List.iter (Output.Razor.writeIndex mPostModels)

    let processPages (pages: PageFile list) =
        pages |> List.iter Output.Razor.writePage

    let processResources (resources: ResourceFile list) =
        resources |> List.iter Output.copyResource

    let private processInputs config index archive (posts, pages, resources) =
        processPosts config index archive posts
        processPages pages
        processResources resources

    let generate root =
        let id = root @+ "input"
        let od = root @+ "output"
        Files.inputFiles dirEnumerator fileExists (id, od)
        |> processInputs
            {DisqusShortname = ConfigReader.Load(root @+ "config.json").DisqusShortname}
            (Files.index od)
            (Files.archive od)

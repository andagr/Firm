namespace Generator

open System.IO
open Files
open Output
open Models
open FSharp.Data
open FSharp.Literate

module Transformation =
    type Config = JsonProvider<"""{ "disqusShortname": "a-name" }""">
    type Meta = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    let private dirEnumerator id =
        Directory.EnumerateFiles(id, "*", SearchOption.AllDirectories)

    let private fileExists file =
        File.Exists(file)

    let private config root =
        Config.Load(root @+ "config.json")

    let postModel (post:PostFile) =
        let meta = Meta.Load(post.Meta.Input)
        let doc = Literate.WriteHtml(Literate.ParseMarkdownFile(post.File.Input))
        PostModel(meta.Title, meta.Date, meta.Tags, doc)

    let processPosts config index archive (posts: PostFile list) =
        let allPosts = posts |> List.map (fun p -> (p, postModel p))
        allPosts |> List.iter Output.Razor.writePost
        let allPostModels = allPosts |> List.map snd
        Output.Razor.writeArchive allPostModels archive 
        index |> List.iter (Output.Razor.writeIndex allPostModels)

    let processPages (pages: PageFile list) =
        pages
        |> List.iter Output.Razor.writePage

    let processResources (resources: ResourceFile list) =
        resources
        |> List.iter Output.copyResource

    let private processInputs index archive config (posts, pages, resources) =
        processPosts config index archive posts
        processPages pages
        processResources resources

    let generate root =
        let id = root @+ "input"
        let od = root @+ "output"
        Files.inputFiles dirEnumerator fileExists (id, od)
        |> processInputs (config root) (Files.index od) (Files.archive od)

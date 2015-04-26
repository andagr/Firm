namespace Generator

open System.IO
open Files
open Models
open FSharp.Data
open FSharp.Literate

module Transformation =
    type ConfigReader = JsonProvider<"""{ "disqusShortname": "a-name" }""">
    type MetaReader = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    type Config =
        { DisqusShortname: string }

    let dirEnumerator id =
        Directory.EnumerateFiles(id, "*", SearchOption.AllDirectories)

    let fileExists file =
        File.Exists(file)

    let private getTagCloud (allPosts: PostModel list) =
        allPosts
        |> Seq.collect (fun pm -> pm.Tags)
        |> Seq.groupBy (fun t -> t)
        |> Seq.map (fun (t, ts) -> TagCloudModel(t, Seq.length ts))

    let private processPosts config index archive (posts: PostFile list) =
        let postModels =
            posts
            |> List.map (fun pf ->
                let meta = MetaReader.Load(pf.Meta)
                let doc = Literate.WriteHtml(Literate.ParseMarkdownFile(pf.File.Input))
                pf, PostModel(pf.Name, meta.Title, meta.Date, meta.Tags, doc))
            |> List.sortBy (fun (pf, pm) -> pm.Date)
            |> List.rev
        let allPosts = postModels |> List.map snd
        let tagCloud = getTagCloud allPosts
        postModels
        |> List.map (fun (pf, p) -> (pf, SinglePostModel(config.DisqusShortname, p, allPosts, tagCloud)))
        |> List.iter Output.Razor.writePost
        let allPostsModel = AllPostsModel(config.DisqusShortname, allPosts, tagCloud)
        Output.Razor.writeArchive allPostsModel archive 
        index |> List.iter (Output.Razor.writeIndex allPostsModel)
        (allPosts, tagCloud)

    let private processPages config (pages: PageFile list) (allPosts, tagCloud) =
        pages
        |> List.map (fun p ->
            (p, PageModel(
                    config.DisqusShortname,
                    Literate.WriteHtml(Literate.ParseMarkdownFile(p.File.Input)),
                    allPosts,
                    tagCloud)))
        |> List.iter Output.Razor.writePage

    let private processResources (resources: ResourceFile list) =
        resources |> List.iter Output.copyResource

    let private processInputs config index archive (posts, pages, resources) =
        let allPostsData = processPosts config index archive posts
        processPages config pages allPostsData
        processResources resources

    let generate root =
        let id = root @+ "input"
        let od = root @+ "output"
        Output.Razor.compileTemplates root
        Files.inputFiles dirEnumerator fileExists (id, od)
        |> processInputs
            {DisqusShortname = ConfigReader.Load(root @+ "config.json").DisqusShortname}
            (Files.index od)
            (Files.archive od)

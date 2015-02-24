namespace Generator

open System.IO
open System.Collections.Generic
open Files
open ViewModels
open RazorEngine
open RazorEngine.Templating

module FirmRazor =
    type TplKeys =
        { Post: string
          Page: string
          Index: string
          Archive: string }

    let tplKeys =
        { Post = "post.cshtml"
          Page = "page.cshtml"
          Index = "index.csthml"
          Archive = "archive.cshtml" }

    let compileTemplates root =
        let tplInfo tplDir tpl =
            (tplDir @+ tpl, tpl)
        let compileTemplate tplInfo =
            let tpl, name = tplInfo
            printfn "Compiling template %s..." name
            Engine.Razor.AddTemplate(name, File.ReadAllText(tpl))
        let tplDir = root @+ "templates" @+ "razor"
        [ tplInfo tplDir tplKeys.Post
          tplInfo tplDir tplKeys.Page
          tplInfo tplDir tplKeys.Index
          tplInfo tplDir tplKeys.Archive ]
        |> List.iter compileTemplate

    let writePost (source:PostModel) targetFile =
        let result = Engine.Razor.RunCompile(tplKeys.Post, typeof<PostModel>, source)
        File.WriteAllText(targetFile, result)

    let writeArchive (source:IEnumerable<PostModel>) targetFile =
        let result = Engine.Razor.RunCompile(tplKeys.Archive, typeof<IEnumerable<PostModel>>, source)
        File.WriteAllText(targetFile, result)

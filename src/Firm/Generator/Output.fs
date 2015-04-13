namespace Generator

open System.IO
open System.Collections.Generic
open Files
open Models
open RazorEngine
open RazorEngine.Templating

module Output =
    let copyResource resource =
        if not (File.Exists(resource.File.Output)) then
            printfn "Copying resource %s to %s" resource.File.Input resource.File.Output
            File.Copy(resource.File.Input, resource.File.Output)

    module Razor =
        type TplKeys =
            { Post: string
              Page: string
              Index: string
              Archive: string }

        let tplKeys =
            { Post = "post.cshtml"
              Page = "page.cshtml"
              Index = "index.cshtml"
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

        let writePost (file: PostFile, model: SinglePostModel) =
            if not (File.Exists(file.File.Output)) then
                printfn "Compiling and writing post %s" file.File.Output
                let result = Engine.Razor.RunCompile(tplKeys.Post, typeof<SinglePostModel>, model)
                File.WriteAllText(file.File.Output, result)

        let writeIndex (model: IEnumerable<PostModel>) file =
            printfn "Compiling and writing index %s" file
            let result = Engine.Razor.RunCompile(tplKeys.Index, typeof<IEnumerable<PostModel>>, model)
            File.WriteAllText(file, result)

        let writeArchive (model: IEnumerable<PostModel>) file =
            printfn "Compiling and writing archive %s" file
            let result = Engine.Razor.RunCompile(tplKeys.Archive, typeof<IEnumerable<PostModel>>, model)
            File.WriteAllText(file, result)

        let writePage (file: PageFile) =
            if not (File.Exists(file.File.Output)) then
                printfn "Compiling and writing page %s" file.File.Output
                let result = Engine.Razor.RunCompile(tplKeys.Page)
                File.WriteAllText(file.File.Output, result)

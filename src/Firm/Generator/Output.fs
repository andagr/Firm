namespace Firm

open System.IO
open Files
open Firm.Models
open RazorEngine
open RazorEngine.Templating

module Output =
    let copyResource resource =
        if not (File.Exists(resource.File.Output)) then
            printfn "Copying resource %s to %s" resource.File.Input resource.File.Output
            Directory.CreateDirectory(Path.GetDirectoryName(resource.File.Output)) |> ignore
            File.Copy(resource.File.Input, resource.File.Output)

    module Razor =
        type TplKeys =
            { Layout: string
              Post: string
              Page: string
              Index: string
              Archive: string }

        let tplKeys =
            { Layout = "_layout.cshtml"
              Post = "post.cshtml"
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
            [ tplInfo tplDir tplKeys.Layout
              tplInfo tplDir tplKeys.Post
              tplInfo tplDir tplKeys.Page
              tplInfo tplDir tplKeys.Index
              tplInfo tplDir tplKeys.Archive ]
            |> List.iter compileTemplate

        let writePost (file: PostFile, model: SinglePostModel) =
            if not (File.Exists(file.File.Output)) then
                printfn "Compiling and writing post %s" file.File.Output
                Directory.CreateDirectory(Path.GetDirectoryName(file.File.Output)) |> ignore
                let result = Engine.Razor.RunCompile(tplKeys.Post, typeof<SinglePostModel>, model)
                File.WriteAllText(file.File.Output, result)

        let writeIndex (model: AllPostsModel) file =
            printfn "Compiling and writing index %s" file
            Directory.CreateDirectory(Path.GetDirectoryName(file)) |> ignore
            let result = Engine.Razor.RunCompile(tplKeys.Index, typeof<AllPostsModel>, model)
            File.WriteAllText(file, result)

        let writeArchive (model: AllPostsModel) file =
            printfn "Compiling and writing archive %s" file
            Directory.CreateDirectory(Path.GetDirectoryName(file)) |> ignore
            let result = Engine.Razor.RunCompile(tplKeys.Archive, typeof<AllPostsModel>, model)
            File.WriteAllText(file, result)

        let writePage (file: PageFile, model: PageModel) =
            if not (File.Exists(file.File.Output)) then
                printfn "Compiling and writing page %s" file.File.Output
                Directory.CreateDirectory(Path.GetDirectoryName(file.File.Output)) |> ignore
                let result = Engine.Razor.RunCompile(tplKeys.Page, typeof<PageModel>, model)
                File.WriteAllText(file.File.Output, result)

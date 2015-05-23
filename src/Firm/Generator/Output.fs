namespace Firm

open System.IO
open System.Xml.Linq
open Files
open Firm.Models
open RazorEngine
open RazorEngine.Templating
open FSharp.Markdown

module Output =
    let copyResource (resource: ResourceFile) =
        if not (File.Exists(resource.File.Output)) then
            printfn "Copying resource %s to %s" resource.File.Input resource.File.Output
            Directory.CreateDirectory(Path.GetDirectoryName(resource.File.Output)) |> ignore
            File.Copy(resource.File.Input, resource.File.Output)

    module Xml =
        let element name (children: XObject list) =
            XElement(XName.Get name, children) :> XObject

        let attribute name value =
            XAttribute(XName.Get name, value)

        let document (children: XObject list) =
            XDocument(children)

        let text (value: string) =
            XText(value)

        // Todo: Configure below values
        // Todo: Fix image urls for posts
        let generateRss (rssFileName: string) (postFiles: PostFile list) =
            let channelMeta = [
                element "title" [text "Include Brain"]
                element "link" [text "http://includebrain.com"]
                element "description" [text "Ramblings of a developer."]
                element "language" [text "en-us"]
                element "category" [text "Software Development"]
                element "managingEditor" [text "andreas@includebrain.com"]
                element "webMaster" [text "andreas@includebrain.com"] ]
            let itemElems = 
                postFiles
                    |> List.sortBy (fun pf -> pf.Meta.Date)
                    |> List.rev
                    |> List.map (fun pf ->
                    let html = Markdown.TransformHtml(File.ReadAllText(pf.File.Input))
                    element "item" [
                        element "title" [text pf.Meta.Title]
                        element "link" [text ("http://includebrain.com/blog/post/" + pf.Name)]
                        element "description" [text html]])
            let rss =
                document [
                    element "rss" [
                        attribute "version" "2.0"
                        element "channel" (channelMeta@itemElems) ] ]
            Directory.CreateDirectory(Path.GetDirectoryName(rssFileName)) |> ignore
            rss.Save(rssFileName)

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

namespace Firm

open System.IO
open System.Xml.Linq
open Files
open Firm.Models
open RazorEngine
open RazorEngine.Templating
open FSharp.Literate

module Output =
    let copyResource (resource: ResourceFile) =
        if not (File.Exists(resource.File.Output)) then
            printfn "Copying resource %s to %s" resource.File.Input resource.File.Output
            Directory.CreateDirectory(Path.GetDirectoryName(resource.File.Output)) |> ignore
            File.Copy(resource.File.Input, resource.File.Output)

    module Xml =
        // Idea for helper functions copied from http://luketopia.net/2013/10/06/xml-transformations-with-fsharp/
        let element name (children: XObject seq) =
            XElement(XName.Get name, children) :> XObject

        let attribute name value =
            XAttribute(XName.Get name, value) :> XObject

        let document (children: XObject seq) =
            XDocument(children)

        let text (value: string) =
            XText(value) :> XObject

        let generateRss (rssFileName: string) (items: (LiterateDocument * string) list) =
            let channelMeta = [
                element "title" [text "Include Brain"]
                element "link" [text "http://includebrain.com"]
                element "description" [text "Ramblings of a developer."]
                element "language" [text "en-us"]
                element "category" [text "Software Development"]
                element "managingEditor" [text "andreas@includebrain.com"]
                element "webMaster" [text "andreas@includebrain.com"] ]
            let itemElems = items |> List.map (fun (ld, title) ->
                element "item" [
                    element "title" [text title]
                    element "description" [text (Literate.WriteHtml(ld))]])
            let rss =
                document [
                    element "rss" [
                        attribute "version" "2.0"
                        element "channel" (channelMeta@itemElems) ] ]
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

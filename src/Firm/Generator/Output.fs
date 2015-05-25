namespace Firm

open System.IO
open System.Xml.Linq
open Files
open Firm.Models
open Data
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

        let generateRss outputDir (config: ConfigData) (postFiles: PostFile list) =
            let channelMeta = [
                element "title" [text config.Rss.Title]
                element "link" [text config.BaseUrl]
                element "description" [text config.Rss.Description]
                element "language" [text config.Rss.Language]
                element "category" [text config.Rss.Category]
                element "managingEditor" [text config.Rss.ManagingEditor]
                element "webMaster" [text config.Rss.WebMaster] ]
            let itemElems = 
                postFiles
                    |> List.sortBy (fun pf -> pf.Meta.Date)
                    |> List.rev
                    |> List.map (fun pf ->
                    let md = Markdown.Parse(File.ReadAllText(pf.File.Input)) |> Urls.Markdown.withAbsUrls config.BaseUrl
                    let html = Markdown.WriteHtml(md)
                    element "item" [
                        element "title" [text pf.Meta.Title]
                        element "link" [text (Urls.toAbsUrl config.BaseUrl (sprintf "/blog/post/%s" pf.Name))]
                        element "description" [text html]])
            let rss =
                document [
                    element "rss" [
                        attribute "version" "2.0"
                        element "channel" (channelMeta@itemElems) ] ]
            let blogDir = outputDir @+ "blog"
            Directory.CreateDirectory(blogDir) |> ignore
            rss.Save(blogDir @+ "rss.xml")

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

namespace Generator

open System.IO
open Files
open RazorEngine
open RazorEngine.Templating

module FirmRazor =
    let tplInfo tplDir tpl =
        (tplDir @+ tpl, tpl)

    let compileTemplate tplInfo =
        let tpl, name = tplInfo
        printfn "Compiling template %s..." name
        Engine.Razor.AddTemplate(name, File.ReadAllText(tpl))

    let compileTemplates root =
        let tplDir = root @+ "templates" @+ "razor"
        [ tplInfo tplDir "post.cshtml"
          tplInfo tplDir "page.cshtml"
          tplInfo tplDir "index.cshtml"
          tplInfo tplDir "archive.cshtml" ]
        |> List.iter compileTemplate

#r "bin/RazorEngine.dll"
#r "bin/FSharp.CodeFormat.dll"
#r "bin/FSharp.Markdown.dll"
#r "bin/FSharp.Literate.dll"

open System.Collections.Generic
open FSharp.Literate
open FSharp.Markdown


//https://tpetricek.github.io/FSharp.Formatting/markdown.html
let document = """
# F# Hello world
Hello world in [F#](http://fsharp.net) looks like this:

    printfn "Hello world!"

Hello world 2 in [F#](/fsharp/) looks like this:

Test: [test][2]

For more see [fsharp.org][1].

  [1]: http://fsharp.org
  [2]: /test """



let parsed = Literate.ParseMarkdownString(document)

/// Returns all links in a specified span node
let rec collectSpanLinks span = seq {
  match span with
  | DirectLink(_, (url, _)) -> yield url
  | IndirectLink(_, _, key) -> yield fst (parsed.DefinedLinks.[key])
  | Matching.SpanLeaf _ -> ()
  | Matching.SpanNode(_, spans) ->
      for s in spans do yield! collectSpanLinks s }

/// Returns all links in the specified paragraph node
let rec collectParLinks par = seq {
  match par with
  | Matching.ParagraphLeaf _ -> ()
  | Matching.ParagraphNested(_, pars) -> 
      for ps in pars do 
        for p in ps do yield! collectParLinks p 
  | Matching.ParagraphSpans(_, spans) ->
      for s in spans do yield! collectSpanLinks s }

/// Collect links in the entire document
Seq.collect collectParLinks parsed.Paragraphs
|> List.ofSeq


let toAbsUrl (prefix : string) url =
    match url with
    | u when String.length u >= 2 && u.StartsWith("//") -> u
    | u when u.StartsWith("/") -> prefix.TrimEnd('/') + u
    | u -> u

let withAbsUrls (doc : LiterateDocument) =
    let rec fromSpan = function
        | DirectLink(spans, (url, title)) -> DirectLink(spans, (toAbsUrl "http://localhost/" url, title))
        | Matching.SpanNode(sni, ss) ->
            let ss = List.map fromSpan ss
            Matching.SpanNode(sni, ss)
        | other -> other
    let rec fromPar = function
        | Matching.ParagraphNested(pni, nested) ->
            let ps = List.map (List.map fromPar) nested
            Matching.ParagraphNested(pni, ps)
        | Matching.ParagraphSpans(si, ss) ->
            let ss = List.map fromSpan ss
            Matching.ParagraphSpans(si, ss)
        | other -> other
    let fromDefLinks (defLinks:IDictionary<string, string * string option>) =
        defLinks
        |> Seq.map (fun kvp -> kvp.Key, (toAbsUrl "http://localhost/" (fst kvp.Value), snd kvp.Value))
        |> dict
    doc.With(paragraphs = List.map fromPar doc.Paragraphs, definedLinks = fromDefLinks doc.DefinedLinks)

Literate.WriteHtml(withAbsUrls parsed)
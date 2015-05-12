#r "bin/RazorEngine.dll"
#r "bin/FSharp.CodeFormat.dll"
#r "bin/FSharp.Markdown.dll"
#r "bin/FSharp.Literate.dll"

open FSharp.Literate
open FSharp.Markdown


//https://tpetricek.github.io/FSharp.Formatting/markdown.html
let document = """
# F# Hello world
Hello world in [F#](http://fsharp.net) looks like this:

    printfn "Hello world!"

For more see [fsharp.org][1].

  [1]: http://fsharp.org """



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

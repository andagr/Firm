namespace Firm

open System.Collections.Generic
open FSharp.Literate
open FSharp.Markdown

module Urls =
    let toAbsUrl (prefix:string) (url:string) =
        match url with
        | u when u.StartsWith("//") -> u
        | u when u.StartsWith("/") -> prefix.TrimEnd('/') + u
        | u -> u

    let withAbsUrls baseUrl (doc:LiterateDocument) =
        let rec fromSpan = function
            | DirectLink(spans, (url, title)) -> DirectLink(spans, (toAbsUrl baseUrl url, title))
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
            |> Seq.map (fun kvp -> kvp.Key, (toAbsUrl baseUrl (fst kvp.Value), snd kvp.Value))
            |> dict
        doc.With(paragraphs = List.map fromPar doc.Paragraphs, definedLinks = fromDefLinks doc.DefinedLinks)

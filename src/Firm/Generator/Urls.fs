namespace Firm

open System.Collections.Generic
open FSharp.Markdown
open FSharp.Literate

module Urls =
    let toAbsUrl (prefix:string) (url:string) =
        match url with
        | u when u.StartsWith("//") -> u
        | u when u.StartsWith("/") -> prefix.TrimEnd('/') + u
        | u -> u

    let withAbsUrls baseUrl (paragraphs:MarkdownParagraphs, definedLinks: IDictionary<string, (string * string option)>) =
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
        List.map fromPar paragraphs, fromDefLinks definedLinks

    module Markdown =
        let withAbsUrls baseUrl (doc:MarkdownDocument) =
            let paragraphs, definedLinks = withAbsUrls baseUrl (doc.Paragraphs, doc.DefinedLinks)
            MarkdownDocument(paragraphs, definedLinks)

    module Literate =
        let withAbsUrls baseUrl (doc: LiterateDocument) =
            let paragraphs, definedLinks = withAbsUrls baseUrl (doc.Paragraphs, doc.DefinedLinks)
            doc.With(paragraphs = paragraphs, definedLinks = definedLinks)
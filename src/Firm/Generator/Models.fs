namespace Models

open System
open System.Collections.Generic

type PostModel(title: string, date: DateTime, tags: IEnumerable<string>, document: string) =
    member t.Title = title
    member t.Date = date
    member t.Tags = tags
    member t.Document = document
    override t.ToString() =
        let docLength = min (String.length document) 10
        let docBeginning = document.Substring(0, docLength) + "..."
        sprintf "Name: %s, Title: %s, Date: %A, Tags: %A, Document: %s" name title date tags docBeginning
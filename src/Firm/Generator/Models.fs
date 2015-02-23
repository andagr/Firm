namespace Models

open System
open System.Collections.Generic
open FSharp.Literate

type PostModel(name: string, title: string, date: DateTime, tags: IEnumerable<string>, document: LiterateDocument) = 
    member t.Name = name
    member t.Title = title
    member t.Date = date
    member t.Tags = tags
    member t.Document = document
    override t.ToString() =
        sprintf "Name: %s, Title: %s, Date: %A, Tags: %A, Document: %A" name title date tags document
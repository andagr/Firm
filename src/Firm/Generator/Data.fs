namespace Firm

open System
open FSharp.Data

module Data =
    type ConfigReader = JsonProvider<"""{ "baseUrl": "http://localhost:8080/", "disqusShortname": "a-name" }""">
    type MetaReader = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    type ConfigData =
        { BaseUrl: string
          DisqusShortname: string }
        with
            static member fromFile (file: string) =
                let reader = ConfigReader.Load(file)
                { BaseUrl = reader.BaseUrl
                  DisqusShortname = reader.DisqusShortname }

    type MetaData =
        { Title: string
          Date: DateTime
          Tags: string list }
        with
            static member fromFile (file: string) =
                let reader = MetaReader.Load(file)
                { Title = reader.Title
                  Date = reader.Date
                  Tags = List.ofArray reader.Tags }

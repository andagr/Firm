namespace Firm

open System
open FSharp.Data

module Data =
    type ConfigReader = JsonProvider<"""{
        "baseUrl": "http://localhost:8080/",
        "disqusShortname": "a-name",
        "rss": {
            "title": "Title",
            "description": "A description",
            "language": "en-us",
            "category": "Software Development",
            "managingEditor": "someemail@example.com",
            "webMaster": "someemail@example.com"
        }}""">
    type MetaReader = JsonProvider<"""{ "title": "Hello", "date": "2013-07-27 21:22:35", "tags": ["blog", "hello"] }""">

    type Rss =
        { Title: string
          Description: string
          Language: string
          Category: string
          ManagingEditor: string
          WebMaster: string }

    type ConfigData =
        { BaseUrl: string
          DisqusShortname: string
          Rss: Rss }
        with
            static member fromFile (file: string) =
                let reader = ConfigReader.Load(file)
                { BaseUrl = reader.BaseUrl
                  DisqusShortname = reader.DisqusShortname
                  Rss = 
                    { Title = reader.Rss.Title
                      Description = reader.Rss.Description
                      Language = reader.Rss.Language
                      Category = reader.Rss.Category
                      ManagingEditor = reader.Rss.ManagingEditor
                      WebMaster = reader.Rss.WebMaster } }

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

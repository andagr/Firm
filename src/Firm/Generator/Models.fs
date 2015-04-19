namespace Models

open System
open System.Collections.Generic

type PostModel(name: string, title: string, date: DateTime, tags: IEnumerable<string>, document: string) =
    member t.Name = name
    member t.Title = title
    member t.Date = date
    member t.Tags = tags
    member t.Document = document
    override t.ToString() =
        let docLength = min ((String.length document) - 3) 10
        let docBeginning = document.Substring(0, docLength) + "..."
        sprintf "Title: %s, Date: %A, Tags: %A, Document: %s" title date tags docBeginning

type SinglePostModel(disqusShortname: string, post: PostModel, allPosts: IEnumerable<PostModel>) =
    member t.DisqusShortname = disqusShortname
    member t.Post = post
    member t.AllPosts = allPosts

type AllPostsModel(disqusShortname: string, allPosts: IEnumerable<PostModel>) =
    member t.DisqusShortname = disqusShortname
    member t.AllPosts = allPosts

type PageModel(disqusShortName: string, document: string, allPosts: IEnumerable<PostModel>) =
    member t.DisqusShortname = disqusShortName
    member t.Document = document
    member t.AllPosts = allPosts

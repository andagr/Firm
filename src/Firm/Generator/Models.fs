namespace Firm.Models

open System
open Firm

type PostModel(name: string, title: string, date: DateTime, tags: string seq, document) = 
    member t.Name = name
    member t.Title = title
    member t.Date = date
    member t.Tags = tags
    member t.Document = document
    override t.ToString() = 
        let docLength = min ((String.length document) - 3) 10
        let docBeginning = document.Substring(0, docLength) + "..."
        sprintf "Title: %s, Date: %A, Tags: %A, Document: %s" title date tags docBeginning

type TagModel(name: string, count: int) = 
    member t.Name = name
    member t.Count = count

type SinglePostModel(baseUrl, disqusShortname: string, post: PostModel, allPosts: PostModel seq, tags: TagModel seq) = 
    member t.DisqusShortname = disqusShortname
    member t.Post = post
    member t.AllPosts = allPosts
    member t.Tags = tags
    member t.ToAbsUrl(path) = Urls.toAbsUrl baseUrl path

type AllPostsModel(baseUrl, disqusShortname: string, allPosts: PostModel seq, tags: TagModel seq) = 
    member t.DisqusShortname = disqusShortname
    member t.AllPosts = allPosts
    member t.Tags = tags
    member t.ToAbsUrl(path) = Urls.toAbsUrl baseUrl path

type PageModel(baseUrl, disqusShortName: string, document: string, allPosts: PostModel seq, tags: TagModel seq) = 
    member t.DisqusShortname = disqusShortName
    member t.Document = document
    member t.AllPosts = allPosts
    member t.Tags = tags
    member t.ToAbsUrl(path) = Urls.toAbsUrl baseUrl path

namespace Generator

open System
open System.IO

module FileHelper =
    let inline (@+) path1 path2 =
        Path.Combine(path1, path2)

    type Input =
        | Post of FileInfo * FileInfo
        | Page of FileInfo
        | Resource of FileInfo

    let (|Content|Resource|) (ext:string) =
        match (ext.ToLowerInvariant()) with
        | ".md" | ".html" -> Content
        | _ -> Resource

    let (|Post|Page|Resource|) f =
        let dirName = Path.GetDirectoryName(f)
        let fileExt = Path.GetExtension(f)
        match fileExt with
        | Content when File.Exists(dirName @+ "meta.json") -> Post(FileInfo(f), FileInfo(dirName @+ "meta.json"))
        | Content -> Page(FileInfo(f))
        | Resource -> Resource(FileInfo(f))

    let listInputs root =
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.where (fun f -> f <> "meta.json")
        |> Seq.map (function
                    | Post (f, m) -> Post(f, m)
                    | Page f -> Page(f)
                    | Resource f -> Resource(f))
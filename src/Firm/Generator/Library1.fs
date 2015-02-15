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

    let dirName f =
        Path.GetDirectoryName(f)

    let fileExt f =
        Path.GetExtension(f)

    let (|Post|Page|Resource|) f =
        match fileExt f with
        | ".md" | ".html" when File.Exists((dirName f) @+ "meta.json") -> Post(FileInfo(f), FileInfo((dirName f) @+ "meta.json"))
        | ".md" | ".html" -> Page(FileInfo(f))
        | _ -> Resource(FileInfo(f))

    let listInputs root =
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.map (function
                    | Post (f, m) -> Post(f, m)
                    | Page f -> Page(f)
                    | Resource f -> Resource(f))
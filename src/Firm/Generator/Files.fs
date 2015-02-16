namespace Generator

open System
open System.IO

module Files =
    let inline (@+) path1 path2 =
        Path.Combine(path1, path2)

    let (|Content|Resource|) (ext:string) =
        match (ext.ToLowerInvariant()) with
        | ".md" | ".html" -> Content
        | _ -> Resource

    let (|Post|Page|Resource|) f =
        let dirName = Path.GetDirectoryName(f)
        match Path.GetExtension(f) with
        | Content when File.Exists(dirName @+ "meta.json") -> Post(FileInfo(f), FileInfo(dirName @+ "meta.json"))
        | Content -> Page(FileInfo(f))
        | Resource -> Resource(FileInfo(f))

    let inputFiles root =
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.where (fun f -> f <> "meta.json")

    let dirSepChars = [|Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar|]

    let trimEndDirSep (path:string) =
        path.TrimEnd(dirSepChars)

//    let relativePath (baseDir:string) (file:string) =
//        let filePath = Path.GetDirectoryName(file)
//        let fileDirs = filePath.Split(dirSepChars)
//        let baseDirDirs = baseDir.Split(dirSepChars)
//        for i in 0..(Array.length fileDirs) do
//            
//
//    let copy (fromBaseDir:string) (toBaseDir:string) (file:string) =
//        let fromBaseDir = fromBaseDir |> trimTrailingSlash
//        let toBaseDir = toBaseDir |> trimTrailingSlash
//        
//        File.Copy(fromFile, toDir @+ Path.GetFileName(fromFile))

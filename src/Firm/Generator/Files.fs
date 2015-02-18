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

    let relativePath (baseDir:string) (file:string) =
        let fileDirs = file.Split(dirSepChars, StringSplitOptions.RemoveEmptyEntries)
        let baseDirDirs = baseDir.Split(dirSepChars, StringSplitOptions.RemoveEmptyEntries)
        let baseDirDirsLen = Array.length baseDirDirs
        if Array.length fileDirs <= baseDirDirsLen || fileDirs.[..baseDirDirsLen - 1] <> baseDirDirs then
            failwith "Base dir must be a prefix to file dir."
        String.concat (string Path.DirectorySeparatorChar) fileDirs.[baseDirDirsLen..]
        
    let copy (fromBaseDir:string) (toBaseDir:string) (file:string) =
        let relFilePath = relativePath fromBaseDir file
        File.Copy(file, toBaseDir @+ relFilePath)

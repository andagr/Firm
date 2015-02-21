namespace Generator

open System
open System.IO

module Files =
    let inline (@+) path1 path2 =
        Path.Combine(path1, path2)

    type InputType =
        | Md
        | Html

    let (|Content|Resource|) (ext:string) =
        match (ext.ToLowerInvariant()) with
        | ".md" -> Content(Md)
        | ".html" -> Content(Html)
        | _ -> Resource

    let (|Post|Page|Resource|) f =
        let dirName = Path.GetDirectoryName(f)
        match Path.GetExtension(f) with
        | Content it when File.Exists(dirName @+ "meta.json") -> Post(f, dirName @+ "meta.json", it)
        | Content it -> Page(f, it)
        | Resource -> Resource(f)

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

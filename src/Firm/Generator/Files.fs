namespace Generator

open System
open System.IO

module Files =
    let (@+) path1 path2 =
        Path.Combine(path1, path2)

    type InputType =
        | Md
        | Html

    type Post =
        { File: string
          Meta: string
          InputType: InputType }

    type Page =
        { File: string
          InputType: InputType}

    type Resource =
        { File: string }

    let (|Content|Resource|) (ext:string) =
        match (ext.ToLowerInvariant()) with
        | ".md" -> Content(Md)
        | ".html" -> Content(Html)
        | _ -> Resource

    let (|Post|Page|Resource|) f =
        let meta = Path.GetDirectoryName(f) @+ "meta.json"
        match Path.GetExtension(f) with
        | Content it when File.Exists(meta) -> Post {File = f; Meta = meta; InputType = it}
        | Content it -> Page {Page.File = f; InputType = it}
        | Resource -> Resource {Resource.File = f}

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

    let targetFile fromBaseDir toBaseDir file =
        toBaseDir @+ (relativePath fromBaseDir file)
        
    let copy (fromBaseDir:string) (toBaseDir:string) (file:string) =
        let target = targetFile fromBaseDir toBaseDir file
        if not (File.Exists(target)) then
            printfn "Copying %s to %s" file target
            File.Copy(file, target)

    let contentName (file:string) =
        let paths = file.Split(dirSepChars)
        paths.[Array.length paths - 2]
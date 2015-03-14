namespace Generator

open System
open System.IO

module Files =
    let (@+) path1 path2 =
        Path.Combine(path1, path2)

    type FileData =
        { Input: string
          Output: string }

    type PostFile =
        { File: FileData
          Meta: FileData }

    type PageFile =
        { File: FileData }

    type ResourceFile = 
        { File: FileData }

    type InputFile =
        | PostFile of PostFile
        | PageFile of PageFile
        | ResourceFile of ResourceFile

    let private dirSepChars = [|Path.DirectorySeparatorChar; Path.AltDirectorySeparatorChar|]

    let private relativePath (baseDir:string) (file:string) =
        let fileDirs = file.Split(dirSepChars, StringSplitOptions.RemoveEmptyEntries)
        let baseDirDirs = baseDir.Split(dirSepChars, StringSplitOptions.RemoveEmptyEntries)
        let baseDirDirsLen = Array.length baseDirDirs
        if Array.length fileDirs <= baseDirDirsLen || fileDirs.[..baseDirDirsLen - 1] <> baseDirDirs
            then failwith "Base dir must be a prefix to file dir."
        String.concat (string Path.DirectorySeparatorChar) fileDirs.[baseDirDirsLen..]

    let private fileNames (inputDir, outputDir) file =
        let outf = outputDir @+ (relativePath inputDir file)
        let outfwoe = Path.GetFileNameWithoutExtension(outf)
        let outfwe = outfwoe + ".html"
        {Input = file; Output = outfwe}

    let private (|Content|Resource|) (f:string) =
        match (Path.GetExtension(f.ToLowerInvariant())) with
        | ".md" -> Content
        | _ -> Resource

    let private input (id, od) f =
        let meta = Path.GetDirectoryName(f) @+ "meta.json"
        match f with
        | Content when File.Exists(meta) -> PostFile({File = (fileNames (id, od) f); Meta = (fileNames (id, od) meta)})
        | Content -> PageFile({PageFile.File = fileNames (id, od) f})
        | Resource -> ResourceFile({ResourceFile.File = fileNames (id, od) f})

    let unzip (posts, pages, resources) input =
        match input with
        | PostFile p -> (p::posts, pages, resources)
        | PageFile p -> (posts, p::pages, resources)
        | InputFile.ResourceFile r -> (posts, pages, r::resources)

    let inputFiles (inputDir, outputDir) =
        Directory.EnumerateFiles(inputDir, "*", SearchOption.AllDirectories)
        |> Seq.where (fun f -> f <> "meta.json")
        |> Seq.map (fun f -> input (inputDir, outputDir) f)
        |> Seq.fold unzip ([], [], [])

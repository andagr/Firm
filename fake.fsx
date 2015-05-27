#I "packages/FAKE/tools"
#r "FakeLib.dll"

open System
open System.IO
open Fake

let buildDir = "bin/"

Target "Clean" (fun () -> 
    CleanDir buildDir
)

Target "Build" (fun () -> 
    !! "src/**/*.sln"
    |> MSBuildRelease buildDir "Build"
    |> Log "Build-Output: "
)

Target "RegenerateClean" (fun () ->
    Directory.EnumerateDirectories("output/")
    |> Seq.filter (function
         |"output/.git" -> false
         |_ -> true)
    |> DeleteDirs
    Directory.EnumerateFiles("output/")
    |> Seq.filter (function
        |"output/.gitattributes" | "output/.gitignore" | "output/CNAME" -> false
        | _ -> true)
    |> DeleteFiles
)

"Clean"
    ==> "Build"

RunTargetOrDefault "Build"
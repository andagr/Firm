#I @"packages\FAKE\tools"
#r "FakeLib.dll"

open Fake

let buildDir = @"bin\"

Target "Clean" (fun () -> 
    CleanDir buildDir
)

Target "Build" (fun () -> 
    !! "src/**/*.sln"
    |> MSBuildRelease buildDir "Build"
    |> Log "Build-Output: "
)

"Clean"
    ==> "Build"

RunTargetOrDefault "Build"
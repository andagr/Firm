namespace Generator


open Files
open FirmRazor
open Models

module Transformation =
    type Models =
        { Latest: PostModel
          All: PostModel list}

    let processPost d post =
        ()

    let processPage d page =
        ()

    let processResource d resource =
        let fd, td = d
        copy fd td resource.File

    let processInput d input =
        match input with
        | Post po -> processPost d po
        | Page pa -> processPage d pa
        | Resource r -> processResource d r

    let processInputs d inputs =
        inputs
        |> Seq.iter (fun i -> processInput d i)

    let generate root =
        let fromDir = root @+ "input"
        let toDir = root @+ "output"
        fromDir
        |> inputFiles
        |> processInputs (fromDir, toDir)

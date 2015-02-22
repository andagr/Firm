namespace Generator


open Files
open FirmRazor

module Transformation =
    let processPost d f m it =
        ()

    let processPage d f it =
        ()

    let processResource d f =
        let fd, td = d
        copy fd td f

    let processInput d input =
        match input with
        | Post (f, m, it) -> processPost d f m it
        | Page (f, it) -> processPage d f it
        | Resource f -> processResource d f

    let processInputs d inputs =
        inputs
        |> Seq.iter (fun i -> processInput d i)

    let generate root =
        let fromDir = root @+ "input"
        let toDir = root @+ "output"
        fromDir
        |> inputFiles
        |> processInputs (fromDir, toDir)

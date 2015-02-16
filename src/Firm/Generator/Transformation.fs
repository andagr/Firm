namespace Generator

open Files

module Transformation =
    let processPost f m =
        ()

    let processPage f =
        ()

    let processResource f =
        ()

    let processInput input =
        match input with
        | Post (f, m) -> processPost f m
        | Page f -> processPage f
        | Resource f -> processResource f

    let processInputs inputs =
        inputs
        |> Seq.iter processInput

    let generate fromDir toDir =
        inputFiles >> processInputs
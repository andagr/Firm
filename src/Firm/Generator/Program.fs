namespace Generator

open System.IO

module Program =
    [<EntryPoint>]
    let main args =
        Transformation.generate (Directory.GetCurrentDirectory())
        0
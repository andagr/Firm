namespace Firm

open System.IO

module Program =
    [<EntryPoint>]
    let main _ =
        Transformation.generate (Directory.GetCurrentDirectory())
        0
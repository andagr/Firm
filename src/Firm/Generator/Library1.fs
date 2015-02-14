namespace Generator

open System.IO

module FileHelpers =
    let findAllFiles dir =
        Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)
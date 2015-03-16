#load "src/Firm/Generator/Files.fs"

open System.IO
open Generator
open Files

let dirEnumerator id=
        Directory.EnumerateFiles(id, "*", SearchOption.AllDirectories)

let fileExists file =
        File.Exists(file)

Generator.Files.inputFiles dirEnumerator fileExists (__SOURCE_DIRECTORY__ @+ "input", __SOURCE_DIRECTORY__ @+ "output")
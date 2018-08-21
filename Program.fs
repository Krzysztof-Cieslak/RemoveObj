open System.IO
open Argu

type CLIArguments =
    | Bin
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Bin _ -> "remove also bin directories."

let removeobjs andBin =

    let dir = Directory.GetCurrentDirectory()

    Directory.GetFiles(dir, "*.*proj", SearchOption.AllDirectories)
    |> Seq.map Path.GetDirectoryName
    |> Seq.distinct
    |> Seq.collect (fun p -> seq {
        yield Path.Combine (p, "obj")
        if andBin then
            yield Path.Combine (p, "bin")
        })
    |> Seq.filter Directory.Exists
    |> Seq.iter (fun n ->
        Directory.Delete(n, true)
        printfn "Deleted: %s" n)

[<EntryPoint>]
let main argv =
    try
        let parser = ArgumentParser.Create<CLIArguments>(programName = "removeobj")

        let results = parser.Parse argv

        let andBin = results.Contains Bin

        removeobjs andBin

        0
    with
    | :? ArguParseException as ex ->
        printfn "%s" ex.Message
        match ex.ErrorCode with
        | ErrorCode.HelpText -> 0
        | _ -> -2
    | ex ->
        printfn "%s" ex.Message
        -1

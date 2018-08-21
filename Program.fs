open System.IO
open Argu

type CLIArguments =
    | Bin
with
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Bin _ -> "remove also bin directories."

let removeobjs () =

    try
        let dir = Directory.GetCurrentDirectory()

        Directory.GetFiles(dir, "*.*proj", SearchOption.AllDirectories)
        |> Seq.map (fun p ->
            let p = Path.GetDirectoryName p
            Path.Combine (p, "obj")
        )
        |> Seq.iter (fun n ->
            if Directory.Exists n then
                Directory.Delete(n, true)
                printfn "Deleted: %s" n)
        0
    with
    | ex ->
        printfn "%s" ex.Message
        -1

[<EntryPoint>]
let main argv =
    try
        let parser = ArgumentParser.Create<CLIArguments>(programName = "removeobj")

        let results = parser.Parse argv

        let andBin = results.Contains Bin

        removeobjs ()
    with
    | :? ArguParseException as ex ->
        printfn "%s" ex.Message
        match ex.ErrorCode with
        | ErrorCode.HelpText -> 0
        | _ -> -2
    | ex ->
        printfn "%s" ex.Message
        -3

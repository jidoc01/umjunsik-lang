open UmFSharp.Core
open Argu

type UmArguments =
    | Path of path: string
    | [<CustomCommandLine ("--")>] StandardInput

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Path _ -> "Path to the Um program"
            | StandardInput -> "Read the Um program from standard input"

let runInterpreter scriptText =
    scriptText
    |> UmHelper.loadFromText
    |> Interpreter
    |> fun inter -> inter.Run (); inter

let handleReturnValue (inter: Interpreter) =
    match inter.ReturnValue with
    | None -> failwith "No return value"
    | Some v -> v

[<EntryPoint>]
let main args =
    let progName = System.AppDomain.CurrentDomain.FriendlyName
    let argParser = ArgumentParser.Create<UmArguments> (programName = progName)
    let results = argParser.Parse args
    results.GetAllResults ()
    |> Seq.tryPick (
        function
        | Path path ->
            System.IO.File.ReadAllText path
            |> runInterpreter
            |> handleReturnValue
            |> Some
        | StandardInput ->
            System.Console.In.ReadToEnd ()
            |> runInterpreter
            |> handleReturnValue
            |> Some
        )
    |> Option.defaultValue 0

namespace UmFSharp.Core

open AST

[<AutoOpen>]
module private InterpretHelper =
    let MinPC = 1
    let Success = 0

    let rec evalExpr ctx = function
        | Const x -> x
        | ExternalCall (extCallKind, args) -> evalExternalCall ctx args extCallKind
        | Load varId ->
            match ctx.Vars.TryGetValue varId with
            | false, _ -> failwith "Out-of-bound variable access."
            | true, value -> value
        | BinOp (operKind, lhs, rhs) ->
            let lhsVal = evalExpr ctx lhs
            let rhsVal = evalExpr ctx rhs
            let oper = operKind.OperationFunctor
            oper lhsVal rhsVal

    and evalExternalCall ctx args = function
        | ReadInt ->
            ctx.Streams.InputStream
            |> Stream.readLineFromStream
            |> Int.Parse
        | WriteInt ->
            args
            |> List.exactlyOne
            |> evalExpr ctx
            |> string
            |> Stream.writeStringToStream ctx.Streams.OutputStream
            Success
        | WriteChar ->
            if List.isEmpty args then "\n"
            else
                args
                |> List.exactlyOne
                |> evalExpr ctx
                |> char
                |> string
            |> Stream.writeStringToStream ctx.Streams.OutputStream
            Success

    let rec runStmt ctx = function
        | IfNotThen (cond, stmt) ->
            if evalExpr ctx cond = 0 then runStmt ctx stmt
            else { ctx with PC = ctx.PC + 1 }
        | Goto expr -> { ctx with PC = evalExpr ctx expr }
        | Ret expr -> { ctx with ReturnValue = Some <| evalExpr ctx expr }
        | Def (dstVarId, srcExpr) ->
            let srcVal = evalExpr ctx srcExpr
            { ctx with Vars = Map.add dstVarId srcVal ctx.Vars; PC = ctx.PC + 1 }
        | SideEffect expr ->
            evalExpr ctx expr |> ignore
            { ctx with PC = ctx.PC + 1 }

    let rec runByStep ctx prog =
        if ctx.PC = ctx.Length then
            { ctx with ReturnValue = Some 0 }
        elif ctx.PC > ctx.Length || ctx.PC < MinPC then
            failwith "Invalid PC."
        else
            match prog.PerLineStmtInfos.TryGetValue ctx.PC with
            | false, _ -> runByStep { ctx with PC = ctx.PC + 1 } prog
            | true, (_annot, stmt) -> runStmt ctx stmt

type Interpreter (prog: Prog, ?streams: Streams) =
    let mutable ctx = {
        Length = prog.Length
        PC = MinPC
        Vars = Map.empty
        ReturnValue = None
        Streams = defaultArg streams Streams.DefaultStandardStreams
    }

    /// The return value of the program.
    /// If the program has not terminated yet, it returns None.
    member __.ReturnValue with get () = ctx.ReturnValue

    /// Run the program until it terminates.
    member __.Run () =
        while Option.isNone ctx.ReturnValue do
            __.RunByStep ()

    /// Run the program by one step.
    member __.RunByStep () =
        ctx <- runByStep ctx prog


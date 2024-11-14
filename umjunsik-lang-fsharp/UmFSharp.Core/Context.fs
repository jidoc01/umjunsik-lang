namespace UmFSharp.Core

open System.IO
open System.Collections.Generic

type Context = {
    /// The current value of each variable.
    Vars: Map<VarId, Int>
    /// The current program counter.
    PC: Int
    /// Return value of the current program.
    ReturnValue: Int option
    /// The IO functions.
    Streams: Streams
    /// The length (its largest line number) of the program.
    Length: Int
}

and Streams = {
    InputStream: Stream
    OutputStream: Stream
}

with
    static member DefaultStandardStreams = {
        InputStream = System.Console.OpenStandardInput ()
        OutputStream = System.Console.OpenStandardOutput ()
    }


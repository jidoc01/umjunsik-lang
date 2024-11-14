module Stream

open System.IO
open System.Collections.Generic

/// TODO: clear the pool when the stream is closed.
let private readerPool = Dictionary<Stream, StreamReader> ()
let private writerPool = Dictionary<Stream, StreamWriter> ()

let private getOrAddReader (stream: Stream) =
    match readerPool.TryGetValue stream with
    | true, reader -> reader
    | false, _ ->
        let reader = new StreamReader (stream)
        readerPool.Add (stream, reader)
        reader

let private getOrAddWriter (stream: Stream) =
    match writerPool.TryGetValue stream with
    | true, writer -> writer
    | false, _ ->
        let writer = new StreamWriter (stream)
        writer.AutoFlush <- true
        writerPool.Add (stream, writer)
        writer

let readLineFromStream (stream: Stream) =
    let reader = getOrAddReader stream
    reader.ReadLine ()

let writeStringToStream (stream: Stream) (s: string) =
    let writer = getOrAddWriter stream
    writer.Write s

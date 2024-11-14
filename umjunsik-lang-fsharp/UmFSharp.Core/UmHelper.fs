module UmFSharp.Core.UmHelper

open FSharp.Text

let loadFromText (text: string) =
    Lexing.LexBuffer<_>.FromString text
    |> UmParser.prog UmLexer.token

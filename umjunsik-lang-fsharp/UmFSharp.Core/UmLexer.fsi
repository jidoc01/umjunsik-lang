module UmFSharp.Core.UmLexer

open System
open AST
open UmParser
open FSharp.Text.Lexing/// Rule token
val token: lexbuf: LexBuffer<char> -> token

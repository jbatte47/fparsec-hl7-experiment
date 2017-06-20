open System
open System.IO

open FParsec

let headerBegin = pstring "MSH"
let fieldDivSpec = headerBegin >>. anyChar
let componentDivSpec = fieldDivSpec >>. anyChar
let repeaterDivSpec = componentDivSpec >>. anyChar
let escapeSpec = repeaterDivSpec >>. anyChar
let subDivSpec = escapeSpec >>. anyChar

let getMatch p str =
  match run p str with
  | Success(result, _, _) -> result
  | Failure(error, _, _) -> failwith error

let message = File.ReadAllText("sample.hl7")
let fieldDiv = pchar (getMatch fieldDivSpec message)
let componentDiv = pchar (getMatch componentDivSpec message)
let repeaterDiv = pchar (getMatch repeaterDivSpec message)
let escape = pchar (getMatch escapeSpec message)
let subDiv = pchar (getMatch subDivSpec message)

[<EntryPoint>]
let main argv =
  try
    let lines = getMatch (sepBy (restOfLine false) newline) message
    let splitLines = lines |> List.map (getMatch (sepBy (restOfLine false) fieldDiv))

    printfn "result: %A" splitLines
  with
    | _ as error -> printfn "Error! %A" error

  0

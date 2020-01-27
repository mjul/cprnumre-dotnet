open System
open CprNumre


[<EntryPoint>]
let main argv =
    let txt = "290220-1234"
    printfn "Parsing CPR-nummer %s ..." txt
    let result = CprNummer.tryParseCprNummer txt
    match result with 
    | Some cpr -> 
        printfn "Birthday is: %s" (match (CprNummer.birthday cpr) with 
                                  | Some bd -> bd.ToString("dd-MM-yyyy") 
                                  | None-> "Not a valid date")
        printfn "Gender is: %A" (CprNummer.gender cpr) 
        printfn "Modulus 11 checksum correct: %b" (CprNummer.isChecksumValid cpr)
        Console.WriteLine("ToString does not leak private information: {0}", cpr) 
    | None ->
        printfn "Error. Could not parse CPR-nummer."
    0 // return an integer exit code

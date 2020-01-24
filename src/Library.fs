namespace CprNumre

open System
open System.Text.RegularExpressions


/// <Summary>
/// A CPR-nummer is a Danish goverment-issued person ID number.
/// This type has value semantics.
/// </Summary>
/// See <a href="http://cpr.dk">CPR.dk</a> for a description of the domain.
[<Struct>]
type CprNummer =
    // we use an integer representation to get automatic value (equality) semantics
    // if we stored an array of digits we would get reference equality on the arrays
    {
      /// Digits 1-2
      Fødselsdag: uint8
      /// Digits 3-4
      Fødselsmåned: uint8
      /// Digits 5-6<
      Fødselsår: uint8
      /// Digits 7-10
      Løbenummer: uint16 }

/// <Summary>
/// Functional style API for working with <see cref="CprNumre.CprNummer">CprNummer</see>.
///</Summary>
module CprNummer =

    // Compile the pattern for better performance in bulk processing
    let private cprNummerRegex = Regex(@"^(\d{2})(\d{2})(\d{2})-?(\d{4})$", RegexOptions.Compiled)

    /// Try to parse CPR-nummer from a string, returning a
    /// value if it is syntactically valid, otherwise None.
    let tryParseCprNummer str: Option<CprNummer> =
        let m = cprNummerRegex.Match(str)
        if m.Success then
            let digits12 = (uint8) (UInt16.Parse m.Groups.[1].Value)
            let digits34 = (uint8) (UInt16.Parse m.Groups.[2].Value)
            let digits56 = (uint8) (UInt16.Parse m.Groups.[3].Value)
            let digits710 = (UInt16.Parse m.Groups.[4].Value)
            Some
                { Fødselsdag = digits12
                  Fødselsmåned = digits34
                  Fødselsår = digits56
                  Løbenummer = digits710 }
        else
            None

    /// <summary>
    /// Predicate to check if the CprNummer instance is syntactically valid.
    /// </summary>
    /// <remarks>
    /// Note that this only verifies the syntactic validity, namely that is represents ten digits.
    /// For testing semantic validity
    /// </remarks>
    let isSyntacticallyValid (cprNummer: CprNummer) =
        cprNummer.Fødselsdag <= 99uy && cprNummer.Fødselsmåned <= 99uy && cprNummer.Fødselsår <= 99uy
        && cprNummer.Løbenummer <= 9999us

    let birthday cpr =
        let yearOffset =
            match (cpr.Fødselsdag, cpr.Fødselsmåned) with
            | (dd, mm) when dd >= 0uy && dd <= 31uy && mm >= 0uy && mm <= 12uy ->
                // This is the table on page 7 in the PDF documentation from CPR.dk
                // See https://cpr.dk/media/17534/personnummeret-i-cpr.pdf
                // We could simplify it, but it is kept in this verbose style
                // to match the specification document.
                match ((int32) cpr.Fødselsår, (int32) cpr.Løbenummer) with
                | (yy, ln) when ln >= 0000 && ln <= 0999 -> Some 1900
                | (yy, ln) when ln >= 1000 && ln <= 1999 -> Some 1900
                | (yy, ln) when ln >= 2000 && ln <= 2999 -> Some 1900
                | (yy, ln) when ln >= 3000 && ln <= 3999 -> Some 1900
                | (yy, ln) when ln >= 4000 && ln <= 4999 && yy >= 00 && yy <= 36 -> Some 2000
                | (yy, ln) when ln >= 4000 && ln <= 4999 && yy >= 37 && yy <= 99 -> Some 1900
                | (yy, ln) when ln >= 5000 && ln <= 5999 && yy >= 00 && yy <= 57 -> Some 2000
                | (yy, ln) when ln >= 5000 && ln <= 5999 && yy >= 58 && yy <= 99 -> Some 1800
                | (yy, ln) when ln >= 6000 && ln <= 6999 && yy >= 00 && yy <= 57 -> Some 2000
                | (yy, ln) when ln >= 6000 && ln <= 6999 && yy >= 58 && yy <= 99 -> Some 1800
                | (yy, ln) when ln >= 7000 && ln <= 7999 && yy >= 00 && yy <= 57 -> Some 2000
                | (yy, ln) when ln >= 7000 && ln <= 7999 && yy >= 58 && yy <= 99 -> Some 1800
                | (yy, ln) when ln >= 8000 && ln <= 8999 && yy >= 00 && yy <= 57 -> Some 2000
                | (yy, ln) when ln >= 8000 && ln <= 8999 && yy >= 58 && yy <= 99 -> Some 1800
                | (yy, ln) when ln >= 9000 && ln <= 9999 && yy >= 00 && yy <= 36 -> Some 2000
                | (yy, ln) when ln >= 9000 && ln <= 9999 && yy >= 37 && yy <= 99 -> Some 1900
                | _ -> None
        yearOffset
        |> Option.map (fun x -> DateTime(x + (int) cpr.Fødselsår, (int) cpr.Fødselsmåned, (int) cpr.Fødselsdag))

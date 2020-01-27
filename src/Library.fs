namespace CprNumre

open System
open System.Text.RegularExpressions

/// Fødselsdag (Danish for day of birth) is digits 1-2 of the CPR-nummer.
type Fødselsdag = uint8
/// Fødselsmåned (Danish for month of birth) is digits 3-4 of the CPR-nummer.
type Fødselsmåned = uint8
/// Fødselsår (Danish for year of birth) is digits 5-6 of the CPR-nummer.
type Fødselsår = uint8
/// Løbenummer (Danish for serial number) is digits 7-10 of the CPR-nummer.
type Løbenummer = uint16

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
      Fødselsdag: Fødselsdag
      /// Digits 3-4
      Fødselsmåned: Fødselsmåned
      /// Digits 5-6<
      Fødselsår: Fødselsår
      /// Digits 7-10
      Løbenummer: Løbenummer }
    // Don't let the CPR-nummer leak into logs etc. through ToString
    override x.ToString() = "CPR-nummer xxxxxx-xxxx"

/// The gender of the person associated with the CPR-nummer.
type Gender = | Male | Female


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

    /// Check that a value is a valid Fødselsår (digits 5-6 of the CPR-nummer) 
    let isValidFødselsårValue (fødselsår:Fødselsår) = 0uy <= fødselsår && fødselsår <= 99uy

    /// Check that a value is a valid Løbenummer (digits 7-10 of the CPR-nummer) 
    let isValidLøbenummerValue (løbenummer:Løbenummer) = 0us <= løbenummer && løbenummer <= 9999us

    /// <summary>
    /// Predicate to check if the CprNummer instance is syntactically valid.
    /// </summary>
    /// <remarks>
    /// Note that this only verifies the syntactic validity, namely that is represents ten digits.
    /// </remarks>
    let isSyntacticallyValid (cprNummer: CprNummer) =
        cprNummer.Fødselsdag <= 99uy && cprNummer.Fødselsmåned <= 99uy 
            && isValidFødselsårValue cprNummer.Fødselsår
            && isValidLøbenummerValue cprNummer.Løbenummer

    /// <summary>
    /// Predicate to check if the CprNummer instance has a valid checksum.
    /// </summary>
    let isChecksumValid (cprNummer: CprNummer) = 
        let checksum = 4*((int cprNummer.Fødselsdag / 10) % 10)
                        + 3*(int cprNummer.Fødselsdag % 10)
                        + 2*((int cprNummer.Fødselsmåned / 10) % 10)
                        + 7*(int cprNummer.Fødselsmåned % 10)
                        + 6*((int cprNummer.Fødselsår / 10) % 10)
                        + 5*(int cprNummer.Fødselsår % 10)
                        + 4*((int cprNummer.Løbenummer / 1000) % 10)
                        + 3*((int cprNummer.Løbenummer / 100) % 10)
                        + 2*((int cprNummer.Løbenummer / 10) % 10)
        let expectedControlDigit = 11 - (checksum % 11)              
        let actualControlDigit = int cprNummer.Løbenummer % 10
        expectedControlDigit = actualControlDigit

    /// <summary>
    /// Calculate the year of birth for a given two-digit year and the associated løbenummer.
    /// </summary>
    /// <param name="fødselsår">the last two digits of the year of birth (digits 5-6 in the CPR-nummer)</param>
    /// <remarks>
    /// Example: <c>birthyearForFødselsårLøbenummer 99 1234</c> returns <c>1999</c>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Raised if one of the parameters are out of range.</exception>
    let birthyearForFødselsårLøbenummer fødselsår løbenummer = 
        if not (isValidFødselsårValue fødselsår) then raise (ArgumentOutOfRangeException("fødselsår",fødselsår, "Fødselsår is not valid"))
        if not (isValidLøbenummerValue løbenummer) then raise (ArgumentOutOfRangeException("løbenummer", løbenummer, "Løbenummer is not valid"))
        // This is the table on page 7 in the PDF documentation from CPR.dk
        // See https://cpr.dk/media/17534/personnummeret-i-cpr.pdf
        // Not that the age offset just uses the 7th digit 
        // (the thousands of the Løbenummer), so we can simplify a bit
        let thousandsOfLøbenummer = (løbenummer / 1000us) % 10us
        let splitRange minOfHighRange offsetBelow offsetAbove yy = if yy < minOfHighRange then offsetBelow else offsetAbove
        let yearOffset = 
            match (int fødselsår, int thousandsOfLøbenummer) with
            | (_, 0) | (_, 1) | (_, 2) | (_, 3) -> 1900
            | (yy, 4) -> if yy >= 00 && yy <= 36 then 2000 else 1900
            | (yy, 5) -> if yy >= 00 && yy <= 57 then 2000 else 1800
            | (yy, 6) -> if yy >= 00 && yy <= 57 then 2000 else 1800
            | (yy, 7) -> if yy >= 00 && yy <= 57 then 2000 else 1800
            | (yy, 8) -> if yy >= 00 && yy <= 57 then 2000 else 1800
            | (yy, 9) -> if yy >= 00 && yy <= 36 then 2000 else 1900
            | (_,_) -> failwithf "fødselsår/løbenummer out of range (%i, %i)" fødselsår løbenummer
        yearOffset + (int fødselsår)

    /// <summary>
    /// Get the birthday as an Option if it is valid.
    /// Returns None if the CPR-nummer is not valid (e.g. for Erstatningspersonnummer)
    /// </summary>
    let birthday cpr =
        try 
            Some (DateTime(birthyearForFødselsårLøbenummer cpr.Fødselsår cpr.Løbenummer, (int) cpr.Fødselsmåned, (int) cpr.Fødselsdag))
        with
            | :? System.ArgumentOutOfRangeException as ex -> None


    /// <summary>
    /// Get the Gender from the CPR-nummer.
    /// </summary>
    let gender (cprNummer: CprNummer) = 
        match ((int cprNummer.Løbenummer) % 2) with
        | 0 -> Female
        | _ -> Male
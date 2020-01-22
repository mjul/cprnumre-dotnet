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
        Løbenummer: uint16
      }

/// <Summary>
/// Functional style API for working with <see cref="CprNumre.CprNummer">CprNummer</see>.
///</Summary>
module CprNummer =

    // Compile the pattern for better performance in bulk processing
    let private cprNummerRegex = Regex(@"^(\d{2})(\d{2})(\d{2})-?(\d{4})$", RegexOptions.Compiled)

    /// Try to parse CPR-nummer from a string, returning a 
    /// value if it is syntactically valid, otherwise None.
    let tryParseCprNummer str : Option<CprNummer> = 
        let m = cprNummerRegex.Match(str)
        if m.Success then 
            let digits12 = (uint8)(UInt16.Parse m.Groups.[1].Value)
            let digits34 = (uint8)(UInt16.Parse m.Groups.[2].Value)
            let digits56 = (uint8)(UInt16.Parse m.Groups.[3].Value)
            let digits710 = (UInt16.Parse m.Groups.[4].Value)
            Some { Fødselsdag=digits12; 
                    Fødselsmåned = digits34; 
                    Fødselsår=digits56; 
                    Løbenummer=digits710 }
        else
            None  

    /// <summary>
    /// Predicate to check if the CprNummer instance is syntactically valid.
    /// </summary>
    /// <remarks>
    /// Note that this only verifies the syntactic validity, namely that is represents ten digits.
    /// For testing semantic validity 
    /// </remarks>
    let isSyntacticallyValid (cprNummer:CprNummer) =
        cprNummer.Fødselsdag <= 99uy
            && cprNummer.Fødselsmåned <= 99uy 
            && cprNummer.Fødselsår <= 99uy
            && cprNummer.Løbenummer <= 9999us
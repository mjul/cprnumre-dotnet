namespace Tests.CprNumre

open System
open Xunit

open CprNumre

module ValueSemantics = 

    module ``CprNummer has value semantics`` =
        let a = CprNummer.tryParseCprNummer "010203-1234" |> Option.get
        let b = CprNummer.tryParseCprNummer "010203-1234" |> Option.get
        let c = CprNummer.tryParseCprNummer "010203-1234" |> Option.get

        [<Fact>]
        let ``its Equals is reflexive`` () =
            Assert.Equal(a,a)

        [<Fact>]
        let ``its Equals is transitive`` () =
            Assert.Equal(a,b)
            Assert.Equal(b,c)
            Assert.Equal(a,c)

        [<Fact>]
        let ``its Equals is symmetric`` () =
            Assert.Equal(a,b)
            Assert.Equal(b,a)


module Parsing = 

    [<Theory>]
    [<InlineData("010203-1234", 1,2,3,1234)>]
    [<InlineData("0102031234", 1,2,3,1234)>]
    [<InlineData("311299-1111", 31,12,99,1111)>]
    let ``tryParseCprNummer can parse valid digits with and without dash`` (str, dag, måned, år, løbenummer) =
        let actual = CprNummer.tryParseCprNummer str
        Assert.True(Option.isSome actual)
        let value = Option.get actual
        Assert.Equal(dag, value.Fødselsdag)
        Assert.Equal(måned, value.Fødselsmåned)
        Assert.Equal(år, value.Fødselsår)
        Assert.Equal(løbenummer, value.Løbenummer)

    [<Theory>]
    [<InlineData("010203-123")>]
    [<InlineData("010203-12")>]
    [<InlineData("010203")>]
    let ``tryParseCprNummer cannot parse too short string`` (str) =
        let actual = CprNummer.tryParseCprNummer str
        Assert.True(Option.isNone actual)

module Validation =

    type ``isSyntacticallyValid`` () =

        static member AllDatesInLeapYearData
            with get() =
                [for dayOffset in 0..365 do
                    let date : obj[] =  [| (System.DateTime(2020,1,1).AddDays((float)dayOffset)) |]
                    yield date]

        [<Theory>]
        [<MemberData("AllDatesInLeapYearData")>]
        member this.``isSyntacticallyValid must accept all birthday dates in the year`` (date:DateTime) = 
            let cpr = {Fødselsdag=(uint8)date.Day;Fødselsmåned=(uint8)date.Month; Fødselsår=(uint8)(date.Year % 99); Løbenummer=1234us }        
            let actual = CprNummer.isSyntacticallyValid cpr
            Assert.True(actual)

            
        [<Theory>]
        [<MemberData("AllDatesInLeapYearData")>]
        member this.``isSyntacticallyValid must accept erstatningspersonnummer dates`` (date:DateTime) = 
            let erstatningspersonnummer = {
                Fødselsdag=(uint8)(date.Day+60); Fødselsmåned=(uint8)date.Month; 
                Fødselsår=(uint8)(date.Year % 99); Løbenummer=1234us }        
            let actual = CprNummer.isSyntacticallyValid erstatningspersonnummer
            Assert.True(actual)

        [<Theory>]
        [<InlineData(31,2,3,1234)>]
        [<InlineData(99,2,3,1234)>]
        [<InlineData(1,13,3,1234)>]
        [<InlineData(1,99,3,1234)>]
        member this.``isSyntacticallyValid must accept non calendar dates`` (dag, måned, år, løbenummer) = 
            let erstatningspersonnummer = {
                Fødselsdag=dag; Fødselsmåned=måned; 
                Fødselsår=år; Løbenummer=løbenummer }        
            let actual = CprNummer.isSyntacticallyValid erstatningspersonnummer
            Assert.True(actual)

        [<Theory>]
        [<InlineData(100,2,3,1234)>]
        [<InlineData(1,100,3,1234)>]
        [<InlineData(1,2,100,1234)>]
        [<InlineData(1,2,3,12345)>]
        member this.``isSyntacticallyValid must reject numbers with too many digits`` (dag, måned, år, løbenummer) = 
            let erstatningspersonnummer = {
                Fødselsdag=dag; Fødselsmåned=måned; 
                Fødselsår=år; Løbenummer=løbenummer }        
            let actual = CprNummer.isSyntacticallyValid erstatningspersonnummer
            Assert.False(actual)

module Decoding = 
    type ``Birthday`` () =

        // See https://cpr.dk/media/17534/personnummeret-i-cpr.pdf
        [<Theory>]
        [<InlineData("010200-0001", 01,02,1900)>]
        [<InlineData("010299-0001", 01,02,1999)>]
        [<InlineData("010200-0999", 01,02,1900)>]
        [<InlineData("010299-0999", 01,02,1999)>]

        [<InlineData("010200-4000", 01,02,2000)>]
        [<InlineData("010200-4999", 01,02,2000)>]
        [<InlineData("010236-4000", 01,02,2036)>]
        [<InlineData("010236-4999", 01,02,2036)>]

        [<InlineData("010258-5000", 01,02,1858)>]
        [<InlineData("010258-5999", 01,02,1858)>]
        [<InlineData("010299-5000", 01,02,1899)>]
        [<InlineData("010299-5999", 01,02,1899)>]

        let ``birthday examples``  (str, day, month, year) = 
            let cpr = CprNummer.tryParseCprNummer(str) |> Option.get
            let expected = Some (DateTime.SpecifyKind(DateTime(year, month, day), DateTimeKind.Unspecified))
            let actual = CprNummer.birthday cpr
            Assert.Equal(expected, actual)

module Privacy = 
    [<Fact>]
    let ``No sensitive data in ToString`` () =
        let cpr = {Fødselsdag = 1uy; Fødselsmåned = 2uy; Fødselsår = 99uy; Løbenummer = 1234us}
        let actual = cpr.ToString()
        Assert.DoesNotContain("01", actual)
        Assert.DoesNotContain("1", actual)
        Assert.DoesNotContain("02", actual)
        Assert.DoesNotContain("2", actual)
        Assert.DoesNotContain("99", actual)
        Assert.DoesNotContain("1999", actual)
        Assert.DoesNotContain("1234", actual)
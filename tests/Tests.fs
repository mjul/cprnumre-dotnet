namespace Tests.CprNumre.CprNumre

open System
open Xunit

open CprNumre

module ValueSemantics = 
    let ``CprNummer has value semantics`` () =
        Assert.True(false)

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

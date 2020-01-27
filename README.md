# CPR-nummer library for .NET Standard 2.0

A _CPR-nummer_ (plural: _CPR-numre_) is a Danish government citizen ID number assigned by ["Det Centrale Personregister" (CPR)](https://cpr.dk/). It is also known as _personnummer_ ("person number").

The _CPR-nummer_ is used for a number of purposes such as _e.g._ tax reporting, identifying people in legal documents and in
_[NemID](https://www.nemid.nu/)_, the government-provided Danish two-factor authentication system.
It is also used for _[NemKonto](https://www.nemkonto.dk)_ where you can transfer money to an account by the owners' _CPR-nummer_ without knowing the account details.

This library makes using it simple and supports the often-missed edge cases such as modulus checksums and valid variants such as _erstatningspersonnumer_ ("substitute _CPR-nummer_").

## Handles Date of Birth, Gender 
These numbers encode the date of birth and gender. This library implement their semantics. It correctly determines the year of birth based on the two (least significant) year digits in the CPR-nummer and extra information encoded in the control digits.

## Handles Validation
The _CPR-nummer_ is encoded to enable validation of correct entry (modulus control). This is implemented by the library.

## GDPR Friendly: Protects Personal Information
The _CPR-nummer_ is considered personal information. Therefore the library defaults to not printing the number in `.ToString()` to reduce the risk of accidentally leaking sensitive personal information into _e.g._ log files.

# Notes for Implementers
## Citizens May Change CPR-numbers 
Note that citizens might obtain a new _CPR-number_, for example when they try to change their gender surgically. Hence it is a good idea to model it as a temporal attribute on a person. Do not use it as a key.


# Legal Information

The _CPR-number_ is classified as _sensitive_ in GDPR (_da._: "personfølsom oplysning").

Private companies may use of the _CPR-nummer_ as regulated by 
Danish law:

> Private virksomheders anvendelse af personnummer er reguleret i § 11, stk. 2 i lov om behandling af personoplysninger (persondataloven).

Source: [CPR.dk FAQ](https://cpr.dk/spoergsmaal-og-svar/personnummer/)

You can read the legislation in [Retsinformation: Lov om behandling af personoplysninger](https://www.retsinformation.dk/Forms/R0710.aspx?id=828) _LOV nr 429 af 31/05/2000_.



# License
Copyright (c) 2020 Martin Jul.
This library is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.


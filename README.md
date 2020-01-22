# CPR-nummer library for .NET

CPR-nummer (plural: CPR-numre) are Danish government citizen ID numbers assigned by "Det Centrale Personregister" [CPR](https://cpr.dk/). 

## Handles Date of Birth, Gender 
These numbers encode the date of birth and gender. This library implement their semantics. It correctly determines the year of birth based on the two (least significant) year digits in the CPR-nummer and extra information encoded in the control digits.

## Handles Validation
The CPR-nummer is encoded to enable validation of correct entry (modulus control). This is implemented by the library.

## Protects Personal Information
The CPR-nummer is considered personal information. Therefor the library defaults to not printing the number in `.ToString()` to reduce the risk of accidentally leaking sensitive personal information into _e.g._ log files.

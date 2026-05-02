---
title: "Validation" 
---
The `ViewModelBase` derives from `ModelBase`, thus all information here also applies to the `ViewModelBase`

Validation is very important for data objects. Therefore, the ModelBase supports all kinds of different validation:

-   Internal validation via the ValidateFields and ValidateBusinessRules methods
-   Validation via data annotations (attributes)
-   External validators using the IValidatorProvider and IValidator interfaces

The validation results are cached and only executed when a property changes (the object becomes dirty) or when the validation is forced.

## Different types of validation

There are two different types of validation in Catel, namely warnings and errors. There are also two flavors of validations, namely field validation and business rule validation.

## Order of execution of events

The order of execution is very important if you want to perform very advanced validation (such as translating validations at the end of each validation sequence).

1.  IValidator.BeforeValidation
2.  OnValidation (raises Validating event)
    **if not already validated**
3.  IValidator.BeforeValidateFields
4.  OnValidatingFields (raises the ValidatingFields event)
5.  IValidator.ValidateFields
6.  ValidateFields
7.  OnValidatedFields (raises the ValidatedFields event)
8.  IValidator.AfterValidateFields
9.  IValidator.BeforeValidateBusinessRules
10. OnValidatingBusinessRules (raises the ValidatingBusinessRules event)
11. IValidator.ValidateBusinessRules
12. ValidateBusinessRules
13. OnValidatedBusinessRules (raises the ValidatedBusinessRules event)
14. IValidator.AfterValidateBusinessRules
    **end if not already validated**
15. OnValidated (raises the Validated event)
16. IValidator.AfterValidation

There are lots of events, and it may seem complex and confusing at first sight. However, all these events give developers the opportunity to hook into the validation sequence at any time.


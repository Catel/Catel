---
title: "Deferring validation" 
---
The opinions about validation differ from person to person. Some people think it is best practice to immediately show the errors to the users. Others want to defer it to the moment where the user clicks the Save or OK button. Catel supports both "best practices".

## Suspending validation for a batch of changes

Sometimes you need to change a batch of properties and don't want validation to be called after every property change. This can be accomplished by using the following code inside a *ModelBase* class:

```
SuspendValidation = true;
Â 
// change several properties here
Â 
SuspendValidation = false;
Â 
// Now force validation
Validate(true);
```

## Deferring validation in view models

Below is a table of properties that can be used to affect the validation deferring.

Property|Validate immediately|Defer to a later moment
---|---|---
DeferValidationUntilFirstSaveCall|false|true
ValidateModelsOnInitialization|true|false

If the DeferValidationUntilFirstSaveCall property, is used, it must be set as first property in the view model because the validation kicks in immediately when properties change.



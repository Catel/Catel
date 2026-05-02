---
title: "Getting a summary of validation results" 
---
Sometimes you just need to get a summary of all warnings and errors of an object. All validation is gathered in the IValidationContext and available on that class. However, there are some convenience classes that allow a developer to create a summary based on a specific tag. This convenience class is IValidationSummary, which gathers the right information from an instance of IValidationContext.

## Creating a summary of all validations

Â To retrieve a summary of all validations from a IValidationContext, use the following code:

```
var validationSummary = validationContext.GetValidationSummary();
```

## Creating a summary of all validations with a specific tag

To retrieve a summary of all validations with a specific tag from a IValidationContext, use the following code:

```
var validationSummary = validationContext.GetValidationSummary("tag");
```


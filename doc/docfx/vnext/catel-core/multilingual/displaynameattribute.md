---
title: "DisplayNameAttribute" 
---
Catel implements a custom implementation of the `DisplayNameAttribute` known in the `DataAnnotations` namespace of .NET. This attribute uses the *ILanguageService* so you can easily have all translations in a single location when using Catel.

## Using the attribute

Using the attribute is really easy. Just put the attribute on top of any method, property, field or parameter like this:

```
public enum Condition
{
    [DisplayName("Contains")]
    Contains,

    [Display("StartsWith")]
    StartsWith,

    [Display("EndsWith")]
    EndsWith,

    [Display("EqualTo")]
    EqualTo,

    [Display("NotEqualTo")]
    NotEqualTo,

    [Display("GreaterThan")]
    GreaterThan,

    [Display("LessThan")]
    LessThan,

    [Display("GreaterThanOrEqualTo")]
    GreaterThanOrEqualTo,

    [Display("LessThanOrEqualTo")]
    LessThanOrEqualTo,

    [Display("IsEmpty")]
    IsEmpty,

    [Display("NotIsEmpty")]
    NotIsEmpty,

    [Display("IsNull")]
    IsNull,

    [Display("NotIsNull")]
    NotIsNull
}
```

## Using the ILanguageService

By default, the attribute uses the `ILanguageService` to resolve the values to show. The resource name that is passed into the constructor of the attribute will be used to resolve the localized value.


---
title: "DisplayAttribute" 
---
Catel implements a custom implementation of the `DisplayAttribute` known in the `DataAnnotations` namespace of .NET. This attribute uses the *ILanguageService* so you can easily have all translations in a single location when using Catel.

Note that this is still work in progress, we would love to support this, but MS sealed the class so we cannot derive from it (at the moment)

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

By default, the attribute uses the `ILanguageService` to resolve the values to show. The `DisplayAttribute` uses conventions to resolve the different values from the resources.

Property|ResourceName used when resolving|Fallback value
---|---
ShortName|[ResourceName]\_ShortName|Name
Name|[ResourceName]\_Name|No fallback value, will return empty value
Description|[ResourceName]\_Description|No fallback value, will return empty value
Prompt|[ResourceName]\_Prompt|No fallback value, will return empty value
GroupName|[ResourceName]\_GroupName|No fallback value, will return empty value


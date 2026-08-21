---
title: "Exposing model properties on view models"
---

When using Catel's MVVM pattern, a view model often needs to forward properties from an underlying model. Without Catel.Fody, each forwarded property requires a full `ViewModelToModel` mapping plus a registered `PropertyData` field — a lot of repetitive code. The `[Expose]` attribute, weaved by Catel.Fody, eliminates this boilerplate.

## The problem: manual property forwarding

Using `[ViewModelToModel]` directly requires registering both the model property and the forwarded property:

```csharp
[Model]
public Person Person
{
    get { return GetValue<Person>(PersonProperty); }
    private set { SetValue(PersonProperty, value); }
}

public static readonly PropertyData PersonProperty = RegisterProperty("Person", typeof(Person));

[ViewModelToModel("Person")]
public string FirstName
{
    get { return GetValue<string>(FirstNameProperty); }
    set { SetValue(FirstNameProperty, value); }
}

public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string));
```

For a model with many properties, this becomes very verbose.

## The solution: [Expose]

The `[Expose]` attribute, combined with property weaving, lets you declare which model properties should be forwarded without writing the individual forwarded properties at all:

```csharp
[Model]
[Expose("FirstName")]
[Expose("MiddleName")]
[Expose("LastName")]
private Person Person { get; set; }
```

Catel.Fody will:

1. Register a dynamic property on the view model for each exposed name.
2. Set up a `ViewModelToModel` mapping between that dynamic property and the corresponding model property.

The result is functionally identical to defining each `[ViewModelToModel]` property by hand, including automatic validation synchronization between the model and the view model.

## How validation synchronization works

When you expose properties from a model decorated with `[Model]`, Catel automatically copies validation results from the model to the view model. This means that validation errors on `Person.FirstName` are reflected directly on the `FirstName` property of the view model — no extra code required.

## Disabling exposed property weaving

To disable this feature for the entire project, set [`WeaveExposedProperties`](configuration.md#weaveexposedproperties) to `false` in `FodyWeavers.xml`:

```xml
<Catel WeaveExposedProperties="false" />
```

---
title: "Catel.Fody"
---

Catel.Fody is an add-in for [Fody](https://github.com/Fody/Fody), an extensible tool for weaving .NET assemblies at compile time. It automatically rewrites simple auto-properties into full Catel dependency-property style properties, weaves argument checks, improves logging calls, and more — all without you having to write boilerplate code.

## Features

| Feature | Description |
|---------|-------------|
| [Property weaving](weaving-properties.md) | Rewrites auto-properties on `ObservableObject`, `ModelBase`, and `ViewModelBase` |
| [Computed property notifications](weaving-properties.md#computed-properties) | Automatically raises `PropertyChanged` for calculated properties |
| [Argument check weaving](weaving-arguments.md) | Converts argument-check attributes into actual guard code |
| [Logging weaving](#weaving-logging) | Replaces `LogManager.GetCurrentClassLogger()` with the faster `LogManager.GetLogger(typeof(...))` form |
| [Exposed properties](exposing-properties.md) | Exposes model properties on a view model using `[Expose]` |
| [Configuration](configuration.md) | Fine-grained control over which features are enabled |

## Quick start

1. Install the NuGet package:

```
dotnet add package Catel.Fody
```

2. Ensure `FodyWeavers.xml` exists in your project and contains the `<Catel />` element:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Weavers>
  <Catel />
</Weavers>
```

> The `FodyWeavers.xml` file is created automatically when you first install the Fody NuGet package. If it already exists, just add `<Catel />` to it.

## Disabling weaving for specific types, properties or methods

To prevent Catel.Fody from weaving a type, a specific property, or a method, decorate it with the `[NoWeaving]` attribute:

```csharp
// Disable weaving for the entire class
[NoWeaving]
public class MyClass : ModelBase
{
    // ...
}

// Disable weaving for a single computed property
public class Person : ModelBase
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [NoWeaving]
    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

`[NoWeaving]` can also be applied to a parameterized `On<PropertyName>Changed` method to suppress the build warning that Catel.Fody emits when it finds such a method but cannot weave it. See [Parameterized OnXChanged methods](weaving-properties.md#parameterized-onxchanged-methods) for details.

## Weaving logging

Catel.Fody automatically converts calls to `LogManager.GetCurrentClassLogger()` into the slightly faster `LogManager.GetLogger(typeof(ClassName))` form at compile time:

```csharp
// Before weaving
private static readonly ILog Log = LogManager.GetCurrentClassLogger();

// After weaving (equivalent compiled output)
private static readonly ILog Log = LogManager.GetLogger(typeof(MyClass));
```

This is a transparent optimization; no source-code changes are required.


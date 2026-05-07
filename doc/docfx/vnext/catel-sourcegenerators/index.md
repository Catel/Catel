---
title: "Catel.SourceGenerators"
---
# Catel.SourceGenerators

Catel 7 introduces source generators to bridge the gap between .NET dependency injection (DI) and XAML. Because XAML requires parameterless constructors for types it instantiates, traditional constructor-based DI cannot be used directly. `Catel.SourceGenerators` solves this by automatically generating the required boilerplate constructors at compile time, letting developers write clean DI-friendly code while still satisfying XAML's requirements.

The source code for `Catel.SourceGenerators` is available at [https://github.com/Catel/Catel.SourceGenerators](https://github.com/Catel/Catel.SourceGenerators).

## Installation

Add the following package reference to your `.csproj` file:

```xml
<PackageReference Include="Catel.SourceGenerators" Version="1.0.0" PrivateAssets="all" />
```

> The `PrivateAssets="all"` attribute ensures that the source generator package is not treated as a runtime dependency.

## Features

| Feature | Description |
|---------|-------------|
| [XAML Constructors](xaml-constructors.md) | Automatically generates parameterless constructors for XAML types (UserControls, Windows, etc.) that resolve services from the DI container. |
| [Injected services and models](injected-services-and-models.md) | Use `[InjectedService]` and `[InjectedModel]` to generate constructor overloads without writing complex constructors by hand. |

## How it works

At compile time, the source generator:

1. Scans partial classes that derive from Catel view types (e.g., `UserControl`, `DataWindow`, `Window`).
2. Inspects any existing constructors that accept DI services.
3. Generates an additional parameterless constructor that resolves each required service from `IoCContainer.ServiceProvider`.

The generated code is placed in a separate partial class file and never overwrites developer-written code.

## Next steps

- [XAML Constructors](xaml-constructors.md) — learn how the source generator handles dependency injection for XAML types
- [Injected services and models](injected-services-and-models.md) — use attributes to replace complex constructor overloads in views, behaviors, markup extensions, and view models
- [Debugging](debugging.md) — diagnose issues with the source generator output

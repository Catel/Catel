---
title: "Weaving argument checks"
---

Catel.Fody can automatically insert argument guard calls at the beginning of a method body based on attributes applied to the method parameters. It also optimises any existing expression-based `Argument` calls to use the faster name-and-value overload.

## Argument checking via attributes

Decorating a method parameter with one of the Catel.Fody argument attributes causes the corresponding `Argument` check to be injected at the top of the method body at compile time.

**Before weaving:**

```csharp
public void DoSomething([NotNullOrEmpty] string myString, [NotNull] object myObject)
{
    // method body
}
```

**After weaving:**

```csharp
public void DoSomething(string myString, object myObject)
{
    Argument.IsNotNullOrEmpty("myString", myString);
    Argument.IsNotNull("myObject", myObject);

    // method body
}
```

### How it works

1. Finds all types in the assembly.
2. For each type, finds all methods.
3. For each method, finds all parameters decorated with a Catel.Fody argument attribute.
4. Inserts the corresponding `Argument` check as the first instruction(s) in the method body.

## Available argument-check attributes

| Attribute | Generates |
|-----------|-----------|
| `[NotNull]` | `Argument.IsNotNull` |
| `[NotNullOrEmptyArray]` | `Argument.IsNotNullOrEmptyArray` |
| `[NotNullOrEmpty]` | `Argument.IsNotNullOrEmpty` |
| `[NotNullOrWhitespace]` | `Argument.IsNotNullOrWhitespace` |
| `[Match]` | `Argument.IsMatch` |
| `[NotMatch]` | `Argument.IsNotMatch` |
| `[NotOutOfRange]` | `Argument.IsNotOutOfRange` |
| `[Maximum]` | `Argument.IsMaximum` |
| `[Minimal]` | `Argument.IsMinimal` |
| `[OfType]` | `Argument.IsOfType` |
| `[ImplementsInterface]` | `Argument.ImplementsInterface` |
| `[InheritsFrom]` | `Argument.InheritsFrom` |

## Automatic expression-check optimisation

Catel.Core provides an expression-based overload for argument checks that is convenient to write but slightly slower at runtime because it must parse the expression tree to extract the parameter name:

```csharp
Argument.IsNotNull(() => myString);
```

Catel.Fody automatically rewrites these to the faster name-and-value overload at compile time:

```csharp
Argument.IsNotNull("myString", myString);
```

This provides a noticeable performance improvement when argument checks are called at high frequency (e.g., more than 50 times per second), without requiring any source-code changes.

> Because Catel.Fody eliminates the runtime overhead of expression-based checks, you can freely use the expression overload for cleaner, refactoring-friendly code without paying a performance penalty.

## Disabling argument check weaving

To disable argument check weaving for the entire project, set [`WeaveArguments`](configuration.md#weavearguments) to `false` in `FodyWeavers.xml`:

```xml
<Catel WeaveArguments="false" />
```

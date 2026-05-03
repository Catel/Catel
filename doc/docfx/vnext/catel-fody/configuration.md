---
title: "Catel.Fody configuration"
---

All Catel.Fody options are configured in `FodyWeavers.xml` by adding attributes to the `<Catel />` element.

```xml
<Weavers>
  <Catel WeaveArguments="false" WeaveLogging="false" />
</Weavers>
```

The sections below describe every available option together with its default value.

## WeaveProperties

Weave all regular auto-properties on classes that inherit (directly or indirectly) from `Catel.Data.ModelBase` into full Catel properties.

> **Default value:** `true`

```xml
<Catel WeaveProperties="false" />
```

See [Weaving properties](weaving-properties.md) for details and examples.

## WeaveExposedProperties

Weave all Catel properties decorated with both `[Model]` and `[Expose]` as automatic `ViewModelToModel` mappings.

> **Default value:** `true`

```xml
<Catel WeaveExposedProperties="false" />
```

See [Exposing properties on view models](exposing-properties.md) for details.

## WeaveCalculatedProperties

Automatically raise `PropertyChanged` notifications for computed (calculated) properties when one of the source properties they reference changes.

For example, given:

```csharp
public string FirstName { get; set; }
public string LastName { get; set; }
public string FullName => $"{FirstName} {LastName}".Trim();
```

Catel.Fody will automatically hook up change notifications so that `FullName` raises `PropertyChanged` whenever `FirstName` or `LastName` changes.

> **Default value:** `true`

```xml
<Catel WeaveCalculatedProperties="false" />
```

See [Computed properties](weaving-properties.md#computed-properties) for more information.

## WeaveArguments

Weave argument-check attributes on method parameters into actual guard calls at the beginning of the method body.

> **Default value:** `true`

```xml
<Catel WeaveArguments="false" />
```

See [Weaving argument checks](weaving-arguments.md) for details.

## WeaveLogging

Replace calls to `LogManager.GetCurrentClassLogger()` with the slightly faster `LogManager.GetLogger(typeof(ClassName))` form.

> **Default value:** `true`

```xml
<Catel WeaveLogging="false" />
```

## GeneratedPropertyDataAccessibility

Controls the accessibility modifier of the `PropertyData` fields that Catel.Fody generates. Accepted values are `Public`, `Internal`, and `Private`.

> **Default value:** `Public`

```xml
<!-- Make all generated PropertyData fields internal -->
<Catel GeneratedPropertyDataAccessibility="Internal" />
```

Example — with the default (`Public`) accessibility a weaved property looks like:

```csharp
public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));
```

With `Internal`:

```csharp
internal static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));
```

## DisableWarningsForAutoPropertyInitializers

When a property uses a C# auto-property initializer (introduced in C# 6), Catel.Fody emits a warning because the initializer value is moved into the constructor rather than used as the registered default value. Set this to `true` to suppress those warnings if you are aware of and comfortable with this behavior.

> **Default value:** `false`

```xml
<Catel DisableWarningsForAutoPropertyInitializers="true" />
```

See [Auto-property initializers](weaving-properties.md#auto-property-initializers) for an explanation of how this feature works.

## GenerateXmlSchemas

> **Note:** XML schema generation has been removed in Catel 7 along with the serialization engine. This option has no effect in projects targeting Catel 7 or later.

Generates `XmlSchemaProvider` attributes and the required static schema methods on all classes that inherit from `ModelBase`. Disabled by default because it is only needed for WCF-based serialization scenarios.

> **Default value:** `false`

```xml
<Catel GenerateXmlSchemas="true" />
```

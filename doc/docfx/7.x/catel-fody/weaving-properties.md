---
title: "Weaving properties"
---

Catel.Fody rewrites simple auto-properties on classes that inherit from `ObservableObject`, `ModelBase`, or `ViewModelBase` into full Catel properties at compile time. This eliminates the boilerplate `PropertyData` registration and `GetValue`/`SetValue` calls while keeping your source code clean and readable.

## Basic property weaving

The following auto-property:

```csharp
public string Name { get; set; }
```

is woven into the following code at compile time.

**For `ModelBase` and `ViewModelBase`:**

```csharp
public string Name
{
    get { return GetValue<string>(NameProperty); }
    set { SetValue(NameProperty, value); }
}

public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string));
```

**For `ObservableObject`:**

```csharp
private string _name;

public string Name
{
    get { return _name; }
    set
    {
        _name = value;
        RaisePropertyChanged(nameof(Name));
    }
}
```

### How it works

In the background, Catel.Fody performs the following steps for each eligible auto-property:

1. Finds all types in the assembly that derive from `ObservableObject`, `ModelBase`, or `ViewModelBase`.
2. Checks that the property has an automatically generated backing field (only auto-properties are processed).
3. Adds the `PropertyData` field for `ModelBase`/`ViewModelBase` types.
4. Initialises the `PropertyData` field in the static constructor of the type.
5. Replaces the getter and setter body with the appropriate `GetValue`/`SetValue` calls.

### Automatically excluded properties

The following properties are never weaved:

- Properties of type `ICommand` (commands must not be registered as `PropertyData`).
- Properties that already have a custom getter or setter implementation.

## Computed properties

If a computed (read-only) property references other weaved properties, Catel.Fody can automatically raise `PropertyChanged` for it whenever one of its source properties changes.

Given:

```csharp
public string FirstName { get; set; }
public string LastName { get; set; }

public string FullName => string.Format("{0} {1}", FirstName, LastName).Trim();
```

Catel.Fody will weave the `OnPropertyChanged` override so that `FullName` raises its notification whenever `FirstName` or `LastName` changes:

```csharp
protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
{
    base.OnPropertyChanged(e);

    if (e.PropertyName.Equals("FirstName"))
    {
        base.RaisePropertyChanged("FullName");
    }

    if (e.PropertyName.Equals("LastName"))
    {
        base.RaisePropertyChanged("FullName");
    }
}
```

> **Note:** This feature is automatically disabled for types that already have a hand-written `OnPropertyChanged` override, because Catel.Fody cannot safely determine where to inject the additional notifications.

To opt a specific computed property out of this behaviour, use `[NoWeaving]`:

```csharp
[NoWeaving]
public string FullName => string.Format("{0} {1}", FirstName, LastName).Trim();
```

This feature can also be disabled globally — see [`WeaveCalculatedProperties`](configuration.md#weavecalculatedproperties) in the configuration reference.

## Automatic change notifications

Catel.Fody automatically searches for methods named `On<PropertyName>Changed`. If such a method exists, it is called whenever the corresponding property changes:

```csharp
public string Name { get; set; }

private void OnNameChanged()
{
    // automatically called whenever Name changes
}
```

### Parameterized OnXChanged methods

Catel.Fody can only weave the parameterless form of `On<PropertyName>Changed`. If the method has parameters, Catel.Fody cannot inject the call and previously emitted a build warning.

Since Catel.Fody [v6.x](https://github.com/Catel/Catel.Fody/pull/682), decorating the parameterized method with `[NoWeaving]` silences that warning. This lets you keep a parameterized overload (called manually or via a subscription) alongside the auto-woven parameterless one:

```csharp
public string Name { get; set; }

// Woven automatically — called by Catel whenever Name changes
private void OnNameChanged()
{
    // react to the change without knowing old / new value
}

// Not woven — call this manually when you need the old and new values
[NoWeaving]
private void OnNameChanged(string oldValue, string newValue)
{
    // react to the change with access to old and new value
}
```

> **Note:** The parameterized overload is never called automatically by Catel.Fody. You are responsible for invoking it yourself (for example, from the parameterless overload using `GetValue` or by subscribing to `PropertyChanged`).


## Specifying default values

By default, Catel uses `null` for reference types and `default(T)` for value types as the registered default value. To specify a custom default, use the `[DefaultValue]` attribute:

```csharp
public class Person : ModelBase
{
    [DefaultValue("Geert")]
    public string FirstName { get; set; }

    public string LastName { get; set; }
}
```

This is weaved into:

```csharp
public class Person : ModelBase
{
    public string FirstName
    {
        get { return GetValue<string>(FirstNameProperty); }
        set { SetValue(FirstNameProperty, value); }
    }

    public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), "Geert");

    public string LastName
    {
        get { return GetValue<string>(LastNameProperty); }
        set { SetValue(LastNameProperty, value); }
    }

    public static readonly PropertyData LastNameProperty = RegisterProperty("LastName", typeof(string), null);
}
```

## Auto-property initializers

C# 6 introduced auto-property initializers:

```csharp
public class MyModel : ModelBase
{
    public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();
}
```

Catel.Fody handles this by moving the initialization expression into the constructor(s) of the class. The property is still fully weaved into a Catel property; however, the default value is set at construction time rather than in the `RegisterProperty` call.

Catel.Fody emits a build warning for each auto-property initializer it encounters, because the initialization is moved into constructors rather than being registered as a Catel default value. To suppress these warnings when you are aware of this behaviour, set [`DisableWarningsForAutoPropertyInitializers`](configuration.md#disablewarningsforautopropertyinitializers) to `true` in `FodyWeavers.xml`.

## Excluding properties from backup

By default, properties generated by Catel.Fody participate in the backup/restore mechanism of `ModelBase`. To opt a property out, decorate it with `[ExcludeFromBackup]`:

```csharp
[ExcludeFromBackup]
public string Name { get; set; }
```

This is weaved into:

```csharp
public string Name
{
    get { return GetValue<string>(NameProperty); }
    set { SetValue(NameProperty, value); }
}

public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), includeInBackup: false);
```

## Controlling PropertyData accessibility

By default, the generated `PropertyData` fields are `public`. You can change this to `internal` or `private` for the entire project by setting [`GeneratedPropertyDataAccessibility`](configuration.md#generatedpropertydataaccessibility) in `FodyWeavers.xml`.

---
title: "Injected services and models"
---
# Injected services and models

`Catel.SourceGenerators` can generate constructor overloads based on attributes. This helps avoid manually maintaining complex constructor combinations.

Use these attributes:

- `[InjectedService]` to inject services into generated constructors.
- `[InjectedModel]` to inject a model into view models.

## Views, markup extensions, and behaviors

For XAML-related types, decorate fields with `[InjectedService]` and let the source generator include these services in generated constructors.

```csharp
public partial class MyView : UserControl
{
    [InjectedService]
    private readonly IMyService1 _myService1;

    [InjectedService]
    private readonly IMyService2 _myService2;

    partial void OnInitializedComponent()
    {
        _myService1.DoWork();
    }
}
```

The generator will produce constructor overloads that include:

1. The default Catel dependencies for the view type.
2. All members marked with `[InjectedService]`.
3. The parameterless constructor required by XAML.

This is the recommended replacement for manually writing large constructor overload chains just to pass additional services.

## View models

View models deriving from `ViewModelBase` can use `[InjectedService]` in the same way.

```csharp
public partial class MyViewModel : ViewModelBase
{
    [InjectedService]
    private readonly IMyService1 _myService1;

    [InjectedService]
    private readonly IMyService2 _myService2;

    partial void OnConstructed()
    {
        _myService1.DoWork();
    }
}
```

The generated constructor contains `IServiceProvider` plus the services marked with `[InjectedService]`.

### Injecting a model

Use `[InjectedModel]` on exactly one field or property in a view model:

```csharp
public partial class MyViewModel : ViewModelBase
{
    [InjectedModel]
    public MyModel Model { get; private set; }
}
```

Behavior:

- `[InjectedModel]` can only be used once per view model.
- The injected model is generated as the first constructor parameter.
- You can combine `[InjectedModel]` and `[InjectedService]`.

If the model type is nullable (`MyModel?`), the generator creates both:

- A constructor that accepts the model.
- A constructor without a model parameter.

## Important rules

- Do not combine manually declared constructors with `[InjectedService]` on the same type.
- If both are present, the source generator reports a compile-time error.
- Keep classes `partial` so generated constructor code can be emitted.

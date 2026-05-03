---
title: "XAML Constructors"
---
# XAML Constructors

XAML requires that types it instantiates provide a parameterless (default) constructor. This conflicts with constructor-based dependency injection, which is the recommended approach in .NET.

`Catel.SourceGenerators` resolves this conflict by automatically generating a parameterless constructor that retrieves each dependency from `IoCContainer.ServiceProvider`. Developers write only the DI-friendly constructors they need; the generator handles the rest.

## How it works

The source generator looks for `partial` classes that:

- Inherit from a Catel XAML view base class (e.g., `UserControl`, `DataWindow`, `Window`).
- Have at least one constructor with parameters.

For each such class, the generator creates a companion partial class file that contains:

- A private static helper method `GetService<T>()` that returns `null` when the application is in design mode (so the XAML designer does not attempt to resolve services).
- A generated parameterless constructor that delegates to the existing DI constructor.

## Using an explicit constructor

Write the constructors you need with full dependency injection. The source generator will add the parameterless constructor automatically.

**Developer-written code:**

```csharp
public partial class MyUserControl : UserControl
{
    public MyUserControl(ILogger<MyUserControl> logger, IViewModelWrapperService viewModelWrapperService)
        : base(logger, viewModelWrapperService)
    {
        InitializeComponent();
    }
}
```

**Generated code (added automatically by the source generator):**

```csharp
public partial class MyUserControl
{
    private static T GetService<T>()
        where T : class
    {
        if (Catel.CatelEnvironment.IsInDesignMode)
        {
            return null!;
        }

        return Catel.IoC.IoCContainer.ServiceProvider.GetRequiredService<T>();
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
    public MyUserControl()
        : this(GetService<ILogger<MyUserControl>>(), GetService<IViewModelWrapperService>())
    {
    }
}
```

## Without any developer-written constructors

If the class has no constructors at all, the source generator creates the full set of constructors automatically, including the DI-based constructor and the parameterless fallback.

**Developer-written code (no constructors):**

```csharp
public partial class MyUserControl : UserControl
{
}
```

**Generated code:**

```csharp
public partial class MyUserControl
{
    private static T GetService<T>()
        where T : class
    {
        if (Catel.CatelEnvironment.IsInDesignMode)
        {
            return null!;
        }

        return Catel.IoC.IoCContainer.ServiceProvider.GetRequiredService<T>();
    }

    partial void OnInitializingComponent();
    partial void OnInitializedComponent();

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
    [ActivatorUtilitiesConstructor]
    public MyUserControl(
        System.IServiceProvider serviceProvider,
        Catel.Services.IViewModelWrapperService viewModelWrapperService,
        Catel.MVVM.IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        OnInitializingComponent();
        InitializeComponent();
        OnInitializedComponent();
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
    public MyUserControl()
        : this(
            GetService<System.IServiceProvider>(),
            GetService<Catel.Services.IViewModelWrapperService>(),
            GetService<Catel.MVVM.IDataContextSubscriptionService>())
    {
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
    public MyUserControl(
        Catel.MVVM.IViewModel? viewModel,
        System.IServiceProvider serviceProvider,
        Catel.Services.IViewModelWrapperService viewModelWrapperService,
        Catel.MVVM.IDataContextSubscriptionService dataContextSubscriptionService)
        : base(viewModel, serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        OnInitializingComponent();
        InitializeComponent();
        OnInitializedComponent();
    }
}
```

The two `partial` methods `OnInitializingComponent` and `OnInitializedComponent` are hook points. Implement them in your own partial class file to execute custom logic before or after `InitializeComponent()` is called.

## Windows and DataWindows

The same pattern applies to `Window` and `DataWindow` types.

**Developer-written code:**

```csharp
public partial class MyWindow : DataWindow
{
    public MyWindow(
        ILogger<MyWindow> logger,
        IServiceProvider serviceProvider,
        IWrapControlService wrapControlService,
        ILanguageService languageService)
        : base(serviceProvider, wrapControlService, languageService)
    {
        InitializeComponent();
    }
}
```

**Generated parameterless constructor:**

```csharp
public partial class MyWindow
{
    private static T GetService<T>()
        where T : class
    {
        if (Catel.CatelEnvironment.IsInDesignMode)
        {
            return null!;
        }

        return Catel.IoC.IoCContainer.ServiceProvider.GetRequiredService<T>();
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
    public MyWindow()
        : this(
            GetService<ILogger<MyWindow>>(),
            GetService<IServiceProvider>(),
            GetService<IWrapControlService>(),
            GetService<ILanguageService>())
    {
    }
}
```

## Design-time support

The generated `GetService<T>()` helper always returns `null` when `Catel.CatelEnvironment.IsInDesignMode` is `true`. This prevents the XAML designer from attempting to resolve services from the DI container, which would fail at design time.

## Requirements

- The view class must be declared as `partial`.
- The project must reference `Catel.SourceGenerators` (with `PrivateAssets="all"`).
- The DI container (`IoCContainer.ServiceProvider`) must be configured before any XAML view is instantiated at runtime.

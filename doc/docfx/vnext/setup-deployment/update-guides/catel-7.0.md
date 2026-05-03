---
title: 'Catel 7.0'
---
This guide describes how to update your code to be fully compatible with Catel 7.0.

Catel 7.x is a major breaking-change release aimed at using as much functionality from .NET (Core) as possible. This release drops the following Catel-specific features in favor of standardized .NET replacements:

- Logging (replaced by `Microsoft.Extensions.Logging`)
- IoC / Dependency injection (replaced by `Microsoft.Extensions.DependencyInjection`)
- Serialization (removed; use `Orc.Serialization.Json` or `Orc.Serialization.Yaml`)

This guide assumes that you are coming from Catel 6.x. If not, please read that guide first.

Encountered issues while upgrading to this version? Add them here to help out others!

# Generic

## NuGet package updates

Update all Catel NuGet package references to version 7.x:

```xml
<PackageReference Include="Catel.Core" Version="7.0.0" />
<PackageReference Include="Catel.MVVM" Version="7.0.0" />
```

If you use Catel.Fody, update it to version 7.x as well:

```xml
<PackageReference Include="Catel.Fody" Version="7.0.0" PrivateAssets="all" />
```

## Source code generator

A new source code generator (`Catel.SourceGenerators`) has been developed to generate boilerplate code. It is still possible to develop projects without it, but because dependency injection in XAML requires a parameterless constructor, using the source generator is strongly recommended.

Add this to the `.csproj` file:

```xml
<PackageReference Include="Catel.SourceGenerators" Version="7.0.0" PrivateAssets="all" />
```

## Dependency injection (DI) / Inversion of Control (IoC)

All Catel-specific IoC components (`ServiceLocator`, `TypeFactory`, `IDependencyResolver`, etc.) have been removed.

Use the native dependency injection from .NET (`Microsoft.Extensions.DependencyInjection`) instead.

Catel's `ServiceLocator` allowed late-bound registration, but .NET DI requires all services to be registered up front. To keep initialization flexible, two new interfaces have been introduced:

- `IoCContainer` — static wrapper around the app-wide `IServiceProvider`
- `IConstructAtStartup` — a singleton type implementing this interface is automatically constructed at startup when `CreateTypesThatMustBeConstructedAtStartup()` is called
- `IInitializeAtStartup` — extends `IConstructAtStartup`; the `Initialize()` method is also called automatically at startup

Call the following at a point in startup where the service provider is ready:

```csharp
serviceProvider.CreateTypesThatMustBeConstructedAtStartup();
```

### Before (Catel 6)

```csharp
// Registration
var serviceLocator = ServiceLocator.Default;
serviceLocator.RegisterType<IMyService, MyService>();

// Resolving
var myService = serviceLocator.ResolveType<IMyService>();

// Constructor injection via TypeFactory
var obj = TypeFactory.Default.CreateInstance<MyViewModel>();
```

### After (Catel 7)

```csharp
// Registration
var services = new ServiceCollection();
services.AddSingleton<IMyService, MyService>();

// Resolving
var myService = serviceProvider.GetRequiredService<IMyService>();

// Constructor injection is automatic — just declare constructor parameters
```

## Modular service registration

Each Catel library now provides an extension method to register its services. No features are registered automatically; registration is opt-in.

To add Catel to an application:

```csharp
var services = new ServiceCollection();

services.AddCatelCore();
services.AddCatelMvvm();
```

## Hosting model

Catel now supports the .NET generic hosting model, similar to ASP.NET Core.

```csharp
public partial class App : Application
{
#pragma warning disable IDISP006 // Implement IDisposable
    private readonly IHost _host;
#pragma warning restore IDISP006 // Implement IDisposable

    public App()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddCatelCore();
                services.AddCatelMvvm();

                services.AddLogging(x =>
                {
                    x.AddConsole();
                    x.AddDebug();
                });
            });

        _host = hostBuilder.Build();

        IoCContainer.ServiceProvider = _host.Services;
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceProvider = IoCContainer.ServiceProvider;

        var configurationService = serviceProvider.GetRequiredService<IConfigurationService>();
        await configurationService.LoadAsync();

        serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

        // TODO: Show main window here
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        using (_host)
        {
            await _host.StopAsync();
        }

        base.OnExit(e);
    }
}
```

## Logging

The Catel-specific logging infrastructure has been removed. Use the .NET standard logging abstractions instead.

.NET logging uses DI, but static classes should not be forced to use DI just for logging. Catel provides a thin `LogManager` bridge:

- **Dependency injection:** Inject `ILogger<T>` (works in views, view models, services, etc.)
- **Static logger:** Use `LogManager.GetLogger(typeof(MyClass))`

`LogManager` resolves the logger from the hosting `IServiceProvider`, or falls back to `LogManager.FallbackLoggerFactory` (useful in unit tests), or returns a `NullLogger` when neither is available.

### Before (Catel 6)

```csharp
private static readonly ILog Log = LogManager.GetCurrentClassLogger();

Log.Info("Starting...");
Log.Warning("Something went wrong: {0}", message);
Log.Error(exception, "An error occurred");
```

### After (Catel 7)

```csharp
private static readonly ILogger Logger = LogManager.GetLogger(typeof(MyClass));

Logger.LogInformation("Starting...");
Logger.LogWarning("Something went wrong: {Message}", message);
Logger.LogError(exception, "An error occurred");
```

Migration summary:

| Catel 6 | Catel 7 |
|---------|---------|
| `LogManager.GetCurrentClassLogger()` | `LogManager.GetLogger(typeof(MyClass))` |
| `ILog` | `ILogger` (from `Microsoft.Extensions.Logging`) |
| `Log.Info(` | `Logger.LogInformation(` |
| `Log.Warning(` | `Logger.LogWarning(` |
| `Log.Error(` | `Logger.LogError(` |
| `Log.Debug(` | `Logger.LogDebug(` |
| `LogListenerBase` / custom log listeners | `ILoggerProvider` (standard .NET) |
| `DebugLogListener` / `ConsoleLogListener` | `x.AddDebug()` / `x.AddConsole()` in `AddLogging(...)` |

# Catel.Core

## Serialization

The built-in Catel serialization engine has been fully removed. This includes `SavableModelBase<T>`, `ISavableModel`, `IModelSerialization`, and all XML/JSON serialization infrastructure.

### Migration options

Use one of the following libraries instead:

- `Orc.Serialization.Json` — JSON serialization
- `Orc.Serialization.Yaml` — YAML serialization

### Before (Catel 6)

```csharp
public class MyModel : SavableModelBase<MyModel>
{
    public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

    public string Name
    {
        get => GetValue<string>(NameProperty);
        set => SetValue(NameProperty, value);
    }
}

// Save / load
var model = new MyModel { Name = "test" };
model.Save("path/to/file.xml");

var loaded = MyModel.Load("path/to/file.xml");
```

### After (Catel 7)

```csharp
// Model no longer needs SavableModelBase — use plain ModelBase or a POCO
public class MyModel : ModelBase
{
    public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

    public string Name
    {
        get => GetValue<string>(NameProperty);
        set => SetValue(NameProperty, value);
    }
}

// Serialize using Orc.Serialization.Json
var serializer = serviceProvider.GetRequiredService<IJsonSerializer>();
await serializer.SerializeToFileAsync(model, "path/to/file.json");

var loaded = await serializer.DeserializeFromFileAsync<MyModel>("path/to/file.json");
```

## IEditableObject and IAdvancedEditableObject removed from ModelBase

`ModelBase` no longer implements `IEditableObject` or `IAdvancedEditableObject`. The related event args (`BeginEditEventArgs`, `CancelEditEventArgs`, `EndEditEventArgs`, etc.) and the `IAdvancedEditableObject` interface have also been removed.

If your code relies on `BeginEdit()` / `EndEdit()` / `CancelEdit()`, implement `IEditableObject` explicitly in your own model class.

# Catel.MVVM

## Auditing removed

The entire Catel auditing infrastructure has been removed. This includes `AuditingManager`, `AuditorBase`, `IAuditor`, `AuditingHelper`, and the built-in auditors (`InvalidateCommandManagerOnViewModelInitializationAuditor`, `SubscribeKeyboardEventsOnViewModelCreationAuditor`).

If you have custom auditors, replace them with a different mechanism. For example:

- Handle the `ViewModelManager.ViewModelCreated` event to react when view models are created.
- Use middleware or decorators in the DI container to intercept service calls.

## View model hierarchy

The view model hierarchy has been reorganized. In Catel 7 there are three base classes:

| Class | Purpose |
|-------|---------|
| `ViewModelBase` | Lightweight base. Requires `IServiceProvider` in the constructor. Handles basic VM lifecycle (initialize, save, cancel, close). |
| `NavigationViewModelBase` | Extends `ViewModelBase` with navigation context support. |
| `FeaturedViewModelBase` | Extends `NavigationViewModelBase` with `[Model]`/`[ViewModelToModel]` attribute processing, data validation, and throttling. Use this when migrating from Catel 6's `ViewModelBase`. |

In Catel 6, `ViewModelBase` contained all features. In Catel 7, most existing view models that use `[Model]`, `[ViewModelToModel]`, or validation should derive from `FeaturedViewModelBase`.

### Before (Catel 6)

```csharp
public class PersonViewModel : ViewModelBase
{
    public PersonViewModel(IPerson person)
    {
        Person = person;
    }

    [Model]
    public IPerson Person { get; private set; }

    [ViewModelToModel(nameof(Person))]
    public string FirstName { get; set; }
}
```

### After (Catel 7)

```csharp
public class PersonViewModel : FeaturedViewModelBase
{
    public PersonViewModel(IServiceProvider serviceProvider, IPerson person)
        : base(serviceProvider)
    {
        Person = person;
    }

    [Model]
    public IPerson Person { get; private set; }

    [ViewModelToModel(nameof(Person))]
    public string FirstName { get; set; }
}
```

## Commands

`Command` and `TaskCommand` now require `IServiceProvider` as the first constructor parameter. This makes the authentication provider and dispatcher service injectable rather than resolved from the old `ServiceLocator`.

### Before (Catel 6)

```csharp
public ICommand SaveCommand { get; } = new Command(ExecuteSave, CanExecuteSave);

public ICommand LoadCommand { get; } = new TaskCommand(ExecuteLoadAsync);
```

### After (Catel 7)

```csharp
// Constructed inside a view model where IServiceProvider is already available
public ICommand SaveCommand { get; }
public ICommand LoadCommand { get; }

public MyViewModel(IServiceProvider serviceProvider)
    : base(serviceProvider)
{
    SaveCommand = new Command(serviceProvider, ExecuteSave, CanExecuteSave);
    LoadCommand = new TaskCommand(serviceProvider, ExecuteLoadAsync);
}
```

## Views

Dependency injection and XAML conflict because XAML requires parameterless constructors and the service provider is not available to all XAML types.

Catel solves this with the `Catel.SourceGenerators` package, which automatically generates a parameterless constructor that resolves all dependencies from `IoCContainer.ServiceProvider`.

### UserControl

#### Before (Catel 6)

```csharp
public partial class MyUserControl : UserControl
{
    public MyUserControl() { }

    public MyUserControl(IMyViewModel viewModel)
        : base(viewModel) { }
}
```

#### After (Catel 7 — with source generator)

Declare only the DI constructor. The source generator creates the parameterless constructor automatically:

```csharp
public partial class MyUserControl : UserControl
{
    public MyUserControl(IServiceProvider serviceProvider,
        IViewModelWrapperService viewModelWrapperService,
        IDataContextSubscriptionService dataContextSubscriptionService)
        : base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
    {
        InitializeComponent();
    }
}
```

Or omit all constructors entirely — the source generator generates everything:

```csharp
// No constructors needed; the source generator handles it
public partial class MyUserControl : UserControl { }
```

The generated code uses `OnInitializingComponent` and `OnInitializedComponent` partial methods for custom initialization hooks.

### DataWindow / Window

#### Before (Catel 6)

```csharp
public partial class MyWindow : DataWindow
{
    public MyWindow() : base(DataWindowMode.OkCancel) { }
}
```

#### After (Catel 7)

```csharp
public partial class MyWindow : DataWindow
{
    public MyWindow(IServiceProvider serviceProvider,
        IWrapControlService wrapControlService,
        ILanguageService languageService)
        : base(serviceProvider, wrapControlService, languageService, DataWindowMode.OkCancel)
    {
        InitializeComponent();
    }
}
```

With the source generator, the parameterless constructor is generated automatically.

# Unit testing

With the new dependency injection approach, isolating unit and integration tests is easier.

## Service collection initialization

Create a shared helper in the test project to set up the service collection:

```csharp
internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        // Add additional services here

        return serviceCollection;
    }
}
```

Inside a unit test:

```csharp
[Test]
public void Execute_Throws_Exception()
{
    var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

    using var serviceProvider = serviceCollection.BuildServiceProvider();

    var command = new Command(serviceProvider, () => { throw new Exception(); }, () => true);
    Assert.Throws<Exception>(() => command.Execute());
}
```

## Global initialization

Add these packages to the test project:

```xml
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="9.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="9.0.0" />
```

```csharp
[SetUpFixture]
public class GlobalInitialization
{
    [OneTimeSetUp]
    public static void SetUp()
    {
        LogManager.FallbackLoggerFactory = LoggerFactory.Create(x =>
        {
            if (Debugger.IsAttached)
            {
                x.SetMinimumLevel(LogLevel.Debug);
                x.AddDebug();
            }

            x.AddConsole();
        });

        var culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        // Required for deterministic type cache initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);

        // Set a global service provider for helpers such as LanguageHelper
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        IoCContainer.ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
```

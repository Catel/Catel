---
title: 'Catel 7.0'
description: ""
weight: 700
---
This guide describes how to update your code to be fully compatible with Catel 7.0.

{{% notice warning %}}
Catel 7.x is a massive breaking changes released, aimed to use as much features from .NET (core) as possible. This release drops the following features from Catel and uses the standardized .NET replacements:

- Logging
- IoC
- Serialization
{{% /notice %}}

{{% notice warning %}}
This guide assumes that you are coming from Catel 6.x. If not, please read that guide first.
{{% /notice %}}

Encountered issues while upgrading to this version? Add them here to help out others!

# Generic

## Source code generator

A new source code generator (Catel.SourceGenerators) has been developed to assist in generating boiler-plate code. It's still possible to develop projects without it, but with the lack of dependency injection in XAML, it's strongly recommended to use the source generator.

Add this to the csproj file:

```
<PackageReference Include="Catel.SourceGenerators" Version="1.0.0" PrivateAssets="all" />
```

## Dependency Injection (DI) / Inversion of Control (IoC) 

All Catel specific IoC components (ServiceLocator, TypeFactory, etc) have been removed.

Use the native dependency injection from .NET.

Catelâ€™s ServiceLocator allowed late-bound registration, but .NET requires all services to be registered up front. To keep initialization flexible, we introduced `IConstructAtStartup` and `IInitializeAtStartup`.

* IoCContainer => contains the app-wide IoC container (IServiceProvider)
* IConstructAtStartup => a type implementing this interface, registered in the service collection, will be automatically constructed at startup
* IInitializeAtStartup => a type implementing this interface, registered in the serviec collection, will be automatically constructed at startup and the `Initialize` method will be called to allow custom initialization.

It is important to call this code at a point where it makes sense:

```
serviceProvider.CreateTypesThatMustBeConstructedAtStartup();
```

## Modular service registration

Each Catel library (and also all the Orc libraries) have module definitions. This means that no features are automatically registered and are now opt-in.

To add Catel to an application, use the following code:

```
var services = new ServiceCollection();

services.AddCatelCore();
services.AddCatelMvvm();
```

## Hosting model

Catel now supports the .NET hosting model. It's possible to create an app host, similar to ASP.NET.

```
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

The logging features have been removed from Catel. It's recommended to use the .NET standard logging features.

.NET logging uses DI, but static classes shouldnâ€™t be forced to use DI just for logging. Our solution:

* Dependency injection: Inject `ILogger<T>` (even for views)
* Static logger: Use `LogManager.GetLogger(typeof(X))`

`LogManager` detects the hosting model and provides the right logger instance, or a `NullLogger` for unit tests. Itâ€™s also possible to register a custom logger factory as fallback.

To upgrade, replace:

```
private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
```

with:

```
private static readonly ILogger Logger = LogManager.GetLogger(typeof());
```

Then replace 

* `Log.` with `Logger.Log`
* `Logger.LogInfo(` with `Logger.LogInformation(`

# Catel.Core

## Serialization

The serialization engine has been fully removed from Catel.

Alternatives:

* Orc.Serialization.Json
* Orc.Serialization.Yaml

# Catel.MVVM

## View models

The view models have been refactored to remove any hidden dependencies. To minimize the risk of added dependencies in the future (that will cause breaking changes), a view model now requires the `IServiceProvider` as injected dependency.

Catelâ€™s `ViewModelBase` is powerful, but most view models donâ€™t need all its features. With true DI, weâ€™re splitting it:

* FeaturedViewModelBase: For advanced features (validation, throttling, etc.)
* ViewModelBase: Lightweight, for most use cases

Only use `FeaturedViewModelBase` if you intend to or are already using advanced features such as:

* `Model` and `ModelToViewModel` attributes
* Data validation
* Throttling
* Etc

## Views

Dependency injection and XAML is hard. Although the main window can be constructed using the service provider, XAML requires a few things:

* Objects defined in XAML must have an empty constructor
* The service provider is not available to (all) xaml types

After trying different approaches, Catel introduces source code generators. This allows any XAML type used with Catel to use dependency injection.

**Example: View Constructor with DI**

```
public partial class MyWindow
{
    public MyWindow(ILogger<MyWindow>, IServiceProvider serviceProvider, 
        IWrapControlService wrapControlService, ILanguageService languageService)
        : base(serviceProvider, wrapControlService, languageService)
    {
        InitializeComponent();
    }
}
```

The source generator creates a default constructor for runtime:

```
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
		: this(GetService<ILogger<MyWindow>>(), GetService<IServiceProvider>(), 
		       GetService<IWrapControlService>(), GetService<ILanguageService>())
	{
	}
}
```

You can even skip defining constructors, the source generators handle everything!

**User controls without any constructors:**

```
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
	public MyUserControl(System.IServiceProvider serviceProvider, Catel.Services.IViewModelWrapperService viewModelWrapperService, Catel.MVVM.IDataContextSubscriptionService dataContextSubscriptionService)
		: base(serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
	{
		OnInitializingComponent();
		InitializeComponent();
		OnInitializedComponent();
	}

	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
	public MyUserControl()
		: this(GetService<System.IServiceProvider>(), GetService<Catel.Services.IViewModelWrapperService>(), GetService<Catel.MVVM.IDataContextSubscriptionService>())
	{
	}

	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Catel.UserControlConstructors", "1.0.0.0")]
	public MyUserControl(Catel.MVVM.IViewModel? viewModel, System.IServiceProvider serviceProvider, Catel.Services.IViewModelWrapperService viewModelWrapperService, Catel.MVVM.IDataContextSubscriptionService dataContextSubscriptionService)
		: base(viewModel, serviceProvider, viewModelWrapperService, dataContextSubscriptionService)
	{
		OnInitializingComponent();
		InitializeComponent();
		OnInitializedComponent();
	}
}
```

Add custom logic in the partial `OnInitializingComponent` or `OnInitializedComponent`.

# Unit testing

With the new dependency injection approach, it's much easier to isolate unit / integration tests.

## Service collection initialization

It's recommended to create a single helper class in the test project to set up the basics:

```
namespace Orc.FeatureToggles.Tests;

using Catel;
using Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionHelper
{
    public static IServiceCollection CreateServiceCollection()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddLogging();
        serviceCollection.AddCatelCore();
        serviceCollection.AddOrcFeatureToggles();
        serviceCollection.AddOrcFeatureTogglesXaml();

        return serviceCollection;
    }
}
```

Inside a unit test, it can be used like this:

```
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

Add these dependencies to the test project:

```
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.1" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.1" />
```

```
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
                x.AddFilter(x => x == LogLevel.Debug);

                x.AddDebug();
            }

            x.AddConsole();
        });

        var culture = new CultureInfo("en-US");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;

        // Required since we do multithreaded initialization
        TypeCache.InitializeTypes(allowMultithreadedInitialization: false);

        // Set a global service provider for helpers such as LanguageHelper
        var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

        Catel.IoC.IoCContainer.ServiceProvider = serviceCollection.BuildServiceProvider();
    }
}
```


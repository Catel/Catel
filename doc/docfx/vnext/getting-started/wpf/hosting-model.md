---
title: "Hosting model"
---

Catel supports the .NET (core) hosted application model, similar to ASP.NET Core. This model provides a standardized way to configure services, logging, and application lifetime management using `Microsoft.Extensions.Hosting`.

## Why use the hosted model?

The hosted model gives you a familiar, structured startup experience:

- **Dependency injection** is configured in one place using `IServiceCollection`.
- **Logging** integrates naturally with `Microsoft.Extensions.Logging`.
- **Application lifetime** is managed through `IHost`, including clean startup and shutdown.

## Required NuGet packages

Add the following packages to your WPF project:

```xml
<PackageReference Include="Catel.Core" Version="7.0.0" />
<PackageReference Include="Catel.MVVM" Version="7.0.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="10.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.0" />
```

## Setting up the application

### Removing the default startup URI

By default, a WPF project uses `StartupUri` in `App.xaml` to open the main window. When using the hosted model, remove this attribute so that the application lifecycle is managed manually:

```xml
<Application x:Class="MyApp.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
    </Application.Resources>
</Application>
```

### Configuring App.xaml.cs

Replace the code-behind of `App.xaml.cs` with the following:

```csharp
namespace MyApp;

using System.Windows;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public partial class App
{
    // The host is disposed in OnExit via a using block, which covers its lifetime.
    // The IDISP006 warning is suppressed because WPF's Application class does not
    // implement IDisposable, so IDisposable cannot be forwarded through the class hierarchy.
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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var serviceProvider = _host.Services;

        serviceProvider.CreateTypesThatMustBeConstructedAtStartup();

        var mainWindow = ActivatorUtilities.CreateInstance<MainWindow>(serviceProvider);
        mainWindow.Show();
    }

    // OnExit is a WPF lifecycle override that must be async void because the base
    // method signature is void. Exceptions thrown inside the async continuation will
    // be unhandled, so ensure that _host.StopAsync() does not throw, or wrap it in
    // a try/catch if graceful-shutdown errors need to be handled.
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

### What each part does

| Section | Description |
|---------|-------------|
| `HostBuilder` | Creates the application host and configures services. |
| `AddCatelCore()` | Registers Catel core services in the DI container. |
| `AddCatelMvvm()` | Registers Catel MVVM services in the DI container. |
| `AddLogging(...)` | Configures logging providers (console, debug). |
| `IoCContainer.ServiceProvider` | Makes the built `IServiceProvider` available to Catel's IoC container and source-generated constructors. |
| `ActivatorUtilities.CreateInstance` | Creates the main window by resolving its dependencies from the service provider. |
| `OnExit` | Gracefully stops the host and disposes resources on application exit. |

## Registering additional services

Register your own services inside `ConfigureServices` before calling `hostBuilder.Build()`:

```csharp
.ConfigureServices((hostContext, services) =>
{
    services.AddCatelCore();
    services.AddCatelMvvm();

    // Register application services
    services.AddSingleton<IMyDataService, MyDataService>();
    services.AddTransient<IMyOtherService, MyOtherService>();

    services.AddLogging(x =>
    {
        x.AddConsole();
        x.AddDebug();
    });
})
```

## Full example

A complete, working example of this setup is available in the [Catel Examples repository](https://github.com/Catel/Catel.Examples/blob/master/src/Catel.Examples.WPF.PersonApplication/App.xaml.cs).

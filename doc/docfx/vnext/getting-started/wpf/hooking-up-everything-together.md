---
title: "Hooking up everything together" 
---
In this step we will hook everything together. The view models are already wired to their windows by Catel's naming convention, so the main task is to configure the application host and verify the complete flow.

## Setting up the application host

The application uses the .NET generic host for dependency injection. See the [Hosting model]({{< relref "getting-started/wpf/hosting-model.md" >}}) guide for a full explanation.

The `App.xaml.cs` bootstraps the host, registers Catel services, and creates the `MainWindow`:

```csharp
namespace Catel.Examples.PersonApplication;

using System.Windows;
using Catel.Examples.PersonApplication.Views;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public partial class App
{
#pragma warning disable IDISP006
    private readonly IHost _host;
#pragma warning restore IDISP006

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

Remove the `StartupUri` attribute from `App.xaml` so that the host controls application startup:

```xml
<Application x:Class="Catel.Examples.PersonApplication.App"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Application.Resources>
    </Application.Resources>
</Application>
```

## How view model resolution works

Catel uses naming conventions to link views to view models automatically:

| View | Resolved view model |
|------|---------------------|
| `MainWindow` | `MainWindowViewModel` |
| `PersonWindow` | `PersonViewModel` |

When `IUIVisualizerService.ShowDialogAsync<PersonViewModel>(person)` is called, Catel:

1. Creates a `PersonViewModel` with the supplied `person` injected into its constructor.
2. Looks up the registered view for `PersonViewModel` — by default the first match found by naming convention (`PersonWindow`).
3. Creates the window, sets the view model as its data context, and shows it as a dialog.

## Running the application

Build and run the application. You should see the `MainWindow` with two pre-populated persons. Use the Add, Edit, and Remove buttons (or double-click a person) to open the `PersonWindow` dialog.

## Up next

[Finalizing the application]({{< relref "getting-started/wpf/finalizing-the-application.md" >}})



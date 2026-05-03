---
title: "WPF implementation for exception handling" 
---
> **Note:** `IExceptionService` has been removed in Catel 7. Use standard .NET exception handling patterns for WPF application-wide exception handling.

For WPF applications, subscribe directly to the `AppDomain.CurrentDomain.UnhandledException` and `Dispatcher.UnhandledException` events in `App.xaml.cs`:

```csharp
public partial class App : Application
{
    private readonly ILogger<App> _logger;

    public App(ILogger<App> logger)
    {
        _logger = logger;

        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        DispatcherUnhandledException += OnDispatcherUnhandledException;
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            _logger.LogError(exception, "AppDomain.UnhandledException occurred");
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        _logger.LogError(e.Exception, "Dispatcher.UnhandledException occurred");
        e.Handled = true;
    }
}
```



---
title: "WPF implementation for exception handling" 
---
## Creating a watcher

The watcher below is a self-managed watcher that takes care of exception handling in case of unhandled exceptions in both the *AppDomain* and *Dispatcher*.

```
public class ExceptionWatcher
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    private readonly IExceptionService _exceptionService;
    private readonly IMessageService _messageService;

    public ExceptionWatcher(IExceptionService exceptionService, IMessageService messageService)
    {
        Argument.IsNotNull(() => exceptionService);
        Argument.IsNotNull(() => messageService);

        _exceptionService = exceptionService;
        _messageService = messageService;

        exceptionService.Register<Exception>(async exception =>
        {
            await _messageService.ShowAsync("An unknown exception occurred, please contact the developers");
        });

        var appDomain = AppDomain.CurrentDomain;
        appDomain.FirstChanceException += OnAppDomainFirstChanceException;
        appDomain.UnhandledException += OnAppDomainUnhandledException;

        var dispatcher = DispatcherHelper.CurrentDispatcher;
        if (dispatcher != null)
        {
            dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }
    }

    private void OnAppDomainFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
    {
        //var exception = e.Exception;
        //if (exception != null)
        //{
        //    _exceptionService.HandleException(exception);
        //}
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        if (exception != null)
        {
            Log.Error(exception, "AppDomain.UnhandledException occurred");

            _exceptionService.HandleException(exception);
        }
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var exception = e.Exception;
        if (exception != null)
        {
            Log.Error(exception, "Dispatcher.UnhandledException occurred");

            if (_exceptionService.HandleException(exception))
            {
                e.Handled = true;
            }
        }
    }
}
```

## Registering the exception watcher

In order for this exception handler to work, you need to register it so it stays alive in the application. Use the code below:

```
var serviceLocator = ServiceLocator.Default;
serviceLocator.RegisterTypeAndInstantiate<ExceptionWatcher>();
```


namespace Catel.Services;

using System.Windows.Threading;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class DispatcherProviderService : IDispatcherProviderService
{
    private readonly ILogger<DispatcherProviderService> _logger;

    private object? _appDispatcher;

    public DispatcherProviderService(ILogger<DispatcherProviderService> logger)
    {
        _logger = logger;
    }

    public virtual object GetApplicationDispatcher()
    {
        var dispatcher = _appDispatcher;
        if (dispatcher is null)
        {
            var app = System.Windows.Application.Current;
            dispatcher = _appDispatcher = app?.Dispatcher;

            if (dispatcher is null)
            {
                _logger.LogWarning("No application dispatcher found, creating temporary dispatcher");

                // Dispatcher.CurrentDispatcher is not useful, but we use it as fallback value, 
                // see https://github.com/Catel/Catel/issues/1762, but never store it in the field
                dispatcher = GetCurrentDispatcher();
            }
        }

        return dispatcher;
    }

    public virtual object GetCurrentDispatcher()
    {
        return Dispatcher.CurrentDispatcher;
    }
}

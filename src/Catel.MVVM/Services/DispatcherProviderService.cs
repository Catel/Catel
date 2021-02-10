namespace Catel.Services
{
#if !XAMARIN
    using System.Windows.Threading;
#endif

    using Catel.Logging;

    public class DispatcherProviderService : IDispatcherProviderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if !XAMARIN
        private object _appDispatcher;
#endif

        public virtual object GetApplicationDispatcher()
        {
#if XAMARIN
            throw new NotSupportedInPlatformException();
#else
            var dispatcher = _appDispatcher;
            if (dispatcher is null)
            {
                var app = System.Windows.Application.Current;
                _appDispatcher = app?.Dispatcher;

                if (_appDispatcher is null)
                {
                    Log.Warning($"No application dispatcher found, creating temporary dispatcher");

                    // Dispatcher.CurrentDispatcher is not useful, but we use it as fallback value, see https://github.com/Catel/Catel/issues/1762
                    return Dispatcher.CurrentDispatcher;
                }
            }

            return dispatcher;
#endif
        }

        public virtual object GetCurrentDispatcher()
        {
#if XAMARIN
            throw new NotSupportedInPlatformException();
#else
            return Dispatcher.CurrentDispatcher;
#endif
        }
    }
}

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Serves as a container for dependency injection for components that do not support
    /// dependency injection natively.
    /// </summary>
    public static class IoCContainer
    {
        private static IServiceProvider? _serviceProvider;

        public static IServiceProvider ServiceProvider
        {
            get
            {
                var serviceProvider = _serviceProvider;
                if (serviceProvider is null)
                {
                    throw new CatelException("The ServiceProvider is not set by the app host. Make sure to set it at app startup.");
                }

                return serviceProvider;
            }
            set 
            {
                _serviceProvider = value;
            }
        }
    }
}

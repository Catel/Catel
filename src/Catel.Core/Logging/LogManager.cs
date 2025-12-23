namespace Catel.Logging
{
    using System;
    using Catel.IoC;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class LogManager
    {
        public static ILoggerFactory? FallbackLoggerFactory { get; set; }

        public static ILogger<T> GetLogger<T>()
        {
            // Try global config first
            var serviceProvider = IoCContainer.ServiceProvider;
            if (serviceProvider is not null)
            {
                return serviceProvider.GetRequiredService<ILogger<T>>();
            }

            // In unit tests, etc, we return a dummy logger so code will still run
            var fallbackLoggerFactory = FallbackLoggerFactory;
            if (fallbackLoggerFactory is not null)
            {
                return fallbackLoggerFactory.CreateLogger<T>();
            }

            // No fallback set, use null logger that is always available
            return new NullLogger<T>();
        }

        public static ILogger GetLogger(Type type)
        {
            // Try global config first
            var serviceProvider = IoCContainer.ServiceProvider;
            if (serviceProvider is not null)
            {
                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger(type);
            }

            // In unit tests, etc, we return a dummy logger so code will still run
            var fallbackLoggerFactory = FallbackLoggerFactory;
            if (fallbackLoggerFactory is not null)
            {
                return fallbackLoggerFactory.CreateLogger(type);
            }

            // No fallback set, use null logger that is always available
            return new NullLogger();
        }

        private class NullLogger : NullLogger<object>
        {

        }

        private class NullLogger<T> : ILogger<T>
        {
            public IDisposable? BeginScope<TState>(TState state) 
                where TState : notnull
            {
                return new DisposableToken(this, static (x) => { }, static (x) => { });
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
            {
            }
        }
    }
}

namespace Catel
{
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.ThirdPartyNotices;
    using Configuration;
    using Data;
    using Messaging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Services;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class CatelCoreModule
    {
        public static IServiceCollection AddCatelCore(this IServiceCollection serviceCollection)
        {
            // Store the collection itself
            serviceCollection.TryAddKeyedSingleton<IServiceCollection>("CatelServiceCollection", serviceCollection);

            serviceCollection.TryAddSingleton<ITimeProvider, TimeProvider>();

            // Configuration
            serviceCollection.TryAddKeyedSingleton<IConfigurationBuilder, ConfigurationBuilder>("CatelConfiguration");
            serviceCollection.TryAddSingleton<IConfigurationService, ConfigurationService>();

            // Logging
            serviceCollection.TryAddSingleton<ILogger, InMemoryLogger>();
            serviceCollection.TryAddSingleton<ILoggerProvider, InMemoryLoggerProvider>();
            serviceCollection.TryAddSingleton<IInMemoryLoggingContainer, InMemoryLoggingContainer>();

            // IoC
            serviceCollection.TryAddSingleton<IServiceScopeManager, ServiceScopeManager>();

            // Services
            serviceCollection.TryAddSingleton<ILanguageService, LanguageService>();
            serviceCollection.TryAddSingleton<IAppDataService, AppDataService>();
            serviceCollection.TryAddSingleton<IMessageMediator, MessageMediator>();
            serviceCollection.TryAddSingleton<IDispatcherService, ShimDispatcherService>();

            serviceCollection.TryAddSingleton<IValidatorProvider, AttributeValidatorProvider>();

            serviceCollection.TryAddSingleton<Catel.Data.IObjectAdapter, Catel.Data.ExpressionTreeObjectAdapter>();

            serviceCollection.TryAddSingleton<IEntryAssemblyResolver, EntryAssemblyResolver>();

            serviceCollection.TryAddSingleton<IModelEqualityComparer, ModelEqualityComparer>();
            serviceCollection.TryAddSingleton<IObjectConverterService, ObjectConverterService>();

            serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator<>), typeof(IntegerObjectIdGenerator<>));
            serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator<,>), typeof(IntegerObjectIdGenerator<,>));
            //serviceCollection.TryAddSingleton < IObjectIdGenerator<T, long>, LongObjectIdGenerator<T>();
            //serviceCollection.TryAddSingleton < IObjectIdGenerator<T, ulong>, ULongObjectIdGenerator<T>();

            // Note: we don't have resources in Catel.Core at the moment
            //serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Resources"));
            //serviceCollection.AddSingleton<ILanguageSource>(new LanguageResourceSource("Catel.Core", "Catel.Properties", "Exceptions"));
            
            serviceCollection.AddSingleton<IThirdPartyNotice>((x) => new LibraryThirdPartyNotice("Catel", "https://www.catelproject.com", "Catel.Core", "Catel"));

            return serviceCollection;
        }

        public static ILoggingBuilder AddInMemory(this ILoggingBuilder loggingBuilder)
        {
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, InMemoryLoggerProvider>());

            return loggingBuilder;
        }
    }
}

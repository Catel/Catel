namespace Catel
{
    using System;
    using Catel.Reflection;
    using Configuration;
    using Data;
    using Messaging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Services;

    /// <summary>
    /// Core module which allows the registration of default services in the service collection.
    /// </summary>
    public static class CoreModule
    {
        public static IServiceCollection AddCatelCoreServices(this IServiceCollection serviceCollection)
        {
            // No need to clean the small boxing caches
            BoxingCache<bool>.Default.CleanUpInterval = TimeSpan.Zero;

            serviceCollection.TryAddSingleton<ILanguageService, LanguageService>();
            serviceCollection.TryAddSingleton<IAppDataService, AppDataService>();
            serviceCollection.TryAddSingleton<IMessageMediator, MessageMediator>();
            serviceCollection.TryAddSingleton<IDispatcherService, ShimDispatcherService>();

            serviceCollection.TryAddSingleton<IValidatorProvider, AttributeValidatorProvider>();

            serviceCollection.TryAddSingleton<Catel.Data.IObjectAdapter, Catel.Data.ExpressionTreeObjectAdapter>();

            serviceCollection.TryAddSingleton<IEntryAssemblyResolver, EntryAssemblyResolver>();

            serviceCollection.TryAddSingleton<IModelEqualityComparer, ModelEqualityComparer>();
            serviceCollection.TryAddSingleton<IConfigurationService, ConfigurationService>();
            serviceCollection.TryAddSingleton<IObjectConverterService, ObjectConverterService>();
            serviceCollection.TryAddSingleton<IRollingInMemoryLogService, RollingInMemoryLogService>();

            //serviceCollection.TryAddSingleton(typeof(IObjectIdGenerator<TObject,int>)  <IObjectIdGenerator<T, int>, IntegerObjectIdGenerator<T>();
            //serviceCollection.TryAddSingleton<IObjectIdGenerator<T, long>, LongObjectIdGenerator<T>();
            //serviceCollection.TryAddSingleton<IObjectIdGenerator<T, ulong>, ULongObjectIdGenerator<T>();

            return serviceCollection;
        }
    }
}

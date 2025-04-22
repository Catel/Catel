namespace Catel
{
    using System.Linq;
    using Catel.MVVM.Auditing;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using MVVM;
    using MVVM.Views;
    using Services;

    /// <summary>
    /// MVVM module which allows the registration of default services in the service locator.
    /// </summary>
    public static class MVVMModule
    {
        public static IServiceCollection AddCatelMvvmServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCatelCoreServices();

            serviceCollection.TryAddSingleton<IDataContextSubscriptionService, DataContextSubscriptionService>();
            serviceCollection.TryAddSingleton<ICommandManager, CommandManager>();
            serviceCollection.TryAddSingleton<IViewLoadManager, ViewLoadManager>();
            serviceCollection.TryAddSingleton<IViewModelWrapperService, ViewModelWrapperService>();
            serviceCollection.TryAddSingleton<IViewManager, ViewManager>();
            serviceCollection.TryAddSingleton<IViewModelManager, ViewModelManager>();
            serviceCollection.TryAddSingleton<IAutoCompletionService, AutoCompletionService>();
            serviceCollection.TryAddSingleton<IWrapControlService, WrapControlService>();
            serviceCollection.TryAddSingleton<IAuthenticationProvider, AuthenticationProvider>();

            // VM services
            serviceCollection.TryAddSingleton<IViewModelLocator, ViewModelLocator>();

            // Always overwrite the dispatcher service since we know Catel.Core registers a shim
            var registrationInfo = serviceCollection.FirstOrDefault(x => x.ServiceType == typeof(IDispatcherService));
            if (registrationInfo is null || registrationInfo.ImplementationType == typeof(ShimDispatcherService))
            {
                serviceCollection.TryAddSingleton<IDispatcherService, DispatcherService>();
            }

            // Only register if not yet registered
            serviceCollection.TryAddSingleton<IViewPropertySelector, FastViewPropertySelector>();
            serviceCollection.TryAddSingleton<IStateService, StateService>();
            serviceCollection.TryAddSingleton<IDispatcherProviderService, DispatcherProviderService>();
            serviceCollection.TryAddSingleton<IMessageService, MessageService>();
            serviceCollection.TryAddSingleton<IUrlLocator, UrlLocator>();
            serviceCollection.TryAddSingleton<IViewLocator, ViewLocator>();
            serviceCollection.TryAddSingleton<IViewModelLocator, ViewModelLocator>();
            serviceCollection.TryAddSingleton<IViewModelFactory, ViewModelFactory>();
            serviceCollection.TryAddSingleton<INavigationService, NavigationService>();
            serviceCollection.TryAddSingleton<INavigationRootService, NavigationRootService>();
            serviceCollection.TryAddSingleton<IViewContextService, ViewContextService>();
            serviceCollection.TryAddSingleton<IUIVisualizerService, UIVisualizerService>();
            serviceCollection.TryAddSingleton<IOpenFileService, OpenFileService>();
            serviceCollection.TryAddSingleton<ISaveFileService, SaveFileService>();
            serviceCollection.TryAddSingleton<ISelectDirectoryService, SelectDirectoryService>();
            serviceCollection.TryAddSingleton<IBusyIndicatorService, BusyIndicatorService>();
            serviceCollection.TryAddSingleton<IProcessService, ProcessService>();

            // Auditing
            serviceCollection.TryAddSingleton<IAuditingManager, AuditingManager>();

            // TODO: How to instantiate these and register them?
            //var invalidateCommandManagerOnViewModelInitializationAuditor = typeFactory.CreateRequiredInstance<InvalidateCommandManagerOnViewModelInitializationAuditor>();
            //AuditingManager.RegisterAuditor(invalidateCommandManagerOnViewModelInitializationAuditor);

            //var subscribeKeyboardEventsOnViewModelCreationAuditor = typeFactory.CreateRequiredInstance<SubscribeKeyboardEventsOnViewModelCreationAuditor>();
            //AuditingManager.RegisterAuditor(subscribeKeyboardEventsOnViewModelCreationAuditor);

            DesignTimeHelper.InitializeDesignTime();

            return serviceCollection;
        }
    }
}

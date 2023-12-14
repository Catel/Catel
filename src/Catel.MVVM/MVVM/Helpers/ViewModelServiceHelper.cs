namespace Catel.MVVM
{
    using System;
    using Catel.MVVM.Views;
    using Catel.Services;
    using IoC;
    using Logging;
    using Messaging;

    /// <summary>
    /// Helper class that registers all default services to a <see cref="IServiceLocator"/>.
    /// </summary>
    public static class ViewModelServiceHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Registers the default view model services.
        /// </summary>
        /// <param name="serviceLocator">The service locator to add the services to.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public static void RegisterDefaultViewModelServices(IServiceLocator serviceLocator)
        {
            ArgumentNullException.ThrowIfNull(serviceLocator);

            try
            {
                Log.Debug("Registering default service implementations for IoC container");

                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                // Always overwrite the dispatcher service since we know Catel.Core registers a shim
                var registrationInfo = serviceLocator.GetRegistrationInfo(typeof(IDispatcherService));
                if (registrationInfo is null || registrationInfo.ImplementingType == typeof(ShimDispatcherService))
                {
                    serviceLocator.RegisterType<IDispatcherService, DispatcherService>();
                }

                // Only register if not yet registered
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewPropertySelector, FastViewPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IStateService, StateService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherProviderService, DispatcherProviderService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationRootService, NavigationRootService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewContextService, ViewContextService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUIVisualizerService, UIVisualizerService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IOpenFileService, OpenFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISaveFileService, SaveFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISelectDirectoryService, SelectDirectoryService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IBusyIndicatorService, BusyIndicatorService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IProcessService, ProcessService>();

                Log.Debug("Registered default service implementations for IoC container");
            }
            catch (Exception ex)
            {
                throw Log.ErrorAndCreateException<Exception>(s => new Exception(s, ex), LanguageHelper.GetRequiredString("FailedToConfigureIoCContainer"));
            }            
        }
    }
}

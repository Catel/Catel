// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using Catel.MVVM.Views;
    using Catel.Services;

#if !XAMARIN && !XAMARIN_FORMS
    using Catel.Windows;
#endif

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
            Argument.IsNotNull("serviceLocator", serviceLocator);

            try
            {
                Log.Debug("Registering default service implementations for IoC container");

                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                if (!serviceLocator.IsTypeRegistered<ExceptionHandling.IExceptionService>())
                {
                    serviceLocator.RegisterInstance(ExceptionHandling.ExceptionService.Default);
                }

                serviceLocator.RegisterTypeIfNotYetRegistered<IViewPropertySelector, FastViewPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IStateService, StateService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherProviderService, DispatcherProviderService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationRootService, NavigationRootService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewContextService, ViewContextService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUIVisualizerService, UIVisualizerService>();

#if !XAMARIN && !XAMARIN_FORMS
                serviceLocator.RegisterTypeIfNotYetRegistered<IUIVisualizerService, UIVisualizerService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IOpenFileService, OpenFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISaveFileService, SaveFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISelectDirectoryService, SelectDirectoryService>();
#endif

                serviceLocator.RegisterTypeIfNotYetRegistered<IBusyIndicatorService, BusyIndicatorService>();

#if NET || NETCORE || UWP
                serviceLocator.RegisterTypeIfNotYetRegistered<IProcessService, ProcessService>();
#endif

                Log.Debug("Registered default service implementations for IoC container");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to configure IoC container");

                throw new Exception(Catel.ResourceHelper.GetString("FailedToConfigureIoCContainer"), ex);
            }            
        }
    }
}

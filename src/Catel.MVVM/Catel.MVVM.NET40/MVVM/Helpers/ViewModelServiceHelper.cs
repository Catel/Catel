// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelServiceHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    using Catel.Windows;

    using IoC;
    using Logging;
    using Memento;
    using Messaging;
    using Services;

    /// <summary>
    /// Helper class that registers all default services to a <see cref="IServiceLocator"/>.
    /// </summary>
    public static class ViewModelServiceHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static ILog Log = LogManager.GetCurrentClassLogger();

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

#if NETFX_CORE
                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                if (!serviceLocator.IsTypeRegistered<IMementoService>())
                {
                    serviceLocator.RegisterInstance(MementoService.Default);
                }

                if (!serviceLocator.IsTypeRegistered<ExceptionHandling.IExceptionService>())
                {
                    serviceLocator.RegisterInstance(ExceptionHandling.ExceptionService.Default);
                }

                // NOTE: Must be in this specific order
                serviceLocator.RegisterTypeIfNotYetRegistered<IDependencyPropertySelector, DependencyPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IPleaseWaitService, PleaseWaitService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IProcessService, ProcessService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
#elif WINDOWS_PHONE
                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                if (!serviceLocator.IsTypeRegistered<IMementoService>())
                {
                    serviceLocator.RegisterInstance(MementoService.Default);
                }

                if (!serviceLocator.IsTypeRegistered<ExceptionHandling.IExceptionService>())
                {
                    serviceLocator.RegisterInstance(ExceptionHandling.ExceptionService.Default);
                }

                // NOTE: Must be in this specific order
                serviceLocator.RegisterTypeIfNotYetRegistered<IDependencyPropertySelector, DependencyPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ILocationService, LocationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IPleaseWaitService, PleaseWaitService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IVibrateService, VibrateService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();

                serviceLocator.RegisterTypeIfNotYetRegistered<IAccelerometerService, AccelerometerService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ICameraService, CameraService>();

#elif SILVERLIGHT
                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                if (!serviceLocator.IsTypeRegistered<IMementoService>())
                {
                    serviceLocator.RegisterInstance(MementoService.Default);
                }

                if (!serviceLocator.IsTypeRegistered<ExceptionHandling.IExceptionService>())
                {
                    serviceLocator.RegisterInstance(ExceptionHandling.ExceptionService.Default);
                }

                // NOTE: Must be in this specific order
                serviceLocator.RegisterTypeIfNotYetRegistered<IDependencyPropertySelector, DependencyPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IOpenFileService, OpenFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IPleaseWaitService, PleaseWaitService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISaveFileService, SaveFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUIVisualizerService, UIVisualizerService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewExportService, ViewExportService>();
#else // WPF
                if (!serviceLocator.IsTypeRegistered<IMessageMediator>())
                {
                    serviceLocator.RegisterInstance(MessageMediator.Default);
                }

                if (!serviceLocator.IsTypeRegistered<IMementoService>())
                {
                    serviceLocator.RegisterInstance(MementoService.Default);
                }

                if (!serviceLocator.IsTypeRegistered<ExceptionHandling.IExceptionService>())
                {
                    serviceLocator.RegisterInstance(ExceptionHandling.ExceptionService.Default);
                }

                // NOTE: Must be in this specific order
                serviceLocator.RegisterTypeIfNotYetRegistered<IDependencyPropertySelector, DependencyPropertySelector>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IDispatcherService, DispatcherService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IMessageService, MessageService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<INavigationService, NavigationService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IOpenFileService, OpenFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IPleaseWaitService, PleaseWaitService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IProcessService, ProcessService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISaveFileService, SaveFileService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISelectDirectoryService, SelectDirectoryService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUIVisualizerService, UIVisualizerService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IUrlLocator, UrlLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewLocator, ViewLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelLocator, ViewModelLocator>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelFactory, ViewModelFactory>();
                serviceLocator.RegisterTypeIfNotYetRegistered<ISplashScreenService, SplashScreenService>();
                serviceLocator.RegisterTypeIfNotYetRegistered<IViewExportService, ViewExportService>();
#endif

                Log.Debug("Registered default service implementations for IoC container");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to configure IoC container");

                throw new Exception(Catel.ResourceHelper.GetString(typeof(ViewModelServiceHelper), "Exceptions", "FailedToConfigureIoCContainer"), ex);
            }            
        }
    }
}
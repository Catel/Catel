// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsControlsModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Windows.Interactivity;
    using MVVM;
    using MVVM.Views;
    using Reflection;
    using Services;
    using IoC;


    /// <summary>
    /// MVVM module which allows the registration of default services in the service locator.
    /// </summary>
    public class MVVMModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<ICommandManager, CommandManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewLoadManager, ViewLoadManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelWrapperService, ViewModelWrapperService>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IAutoCompletionService, AutoCompletionService>();

#if !XAMARIN && !WIN80
            serviceLocator.RegisterTypeIfNotYetRegistered<IInteractivityManager, InteractivityManager>();
#endif

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);

            // Don't use property, we cannot trust the cached property here yet in Visual Studio
            if (CatelEnvironment.GetIsInDesignMode())
            {
                foreach (var assembly in AssemblyHelper.GetLoadedAssemblies())
                {
                    var attributes = assembly.GetCustomAttributesEx(typeof (DesignTimeCodeAttribute));
                    foreach (var attribute in attributes)
                    {
                        // No need to do anything
                    }
                }
            }
        }
    }
}
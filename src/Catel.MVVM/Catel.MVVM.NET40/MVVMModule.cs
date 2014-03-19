// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsControlsModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.MVVM;
    using Catel.Reflection;
    using Catel.Services;
    using IoC;

#if !XAMARIN
    using Catel.MVVM.Views;
#endif

#if !NET && !XAMARIN
    using Catel.Windows;
#endif

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

#if !NET && !PCL  && !XAMARIN
            serviceLocator.RegisterTypeIfNotYetRegistered<IFrameworkElementLoadedManager, FrameworkElementLoadedManager>();
#endif

            serviceLocator.RegisterTypeIfNotYetRegistered<ICommandManager, CommandManager>();

#if !XAMARIN
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
#endif

            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IAutoCompletionService, AutoCompletionService>();

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
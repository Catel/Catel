// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsControlsModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.MVVM;
    using Catel.MVVM.Views;

    using IoC;
    using MVVM.Commands;
    using MVVM.Views.Interfaces;

#if !NET
    using Catel.Windows;
#endif

    /// <summary>
    /// MVVM module which allows the registration of default services in the service locator.
    /// </summary>
    public static class MVVMModule
    {
        #region Methods
        /// <summary>
        /// Registers the services in the specified <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public static void RegisterServices(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

#if !NET
            serviceLocator.RegisterTypeIfNotYetRegistered<IFrameworkElementLoadedManager, FrameworkElementLoadedManager>();
#endif

            serviceLocator.RegisterTypeIfNotYetRegistered<ICommandManager, CommandManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewRoutedCommandManager, ViewRoutedCommandManager>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);
        }
        #endregion
    }
}
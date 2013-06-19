// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using Catel.MVVM.Views;

    using IoC;
    using MVVM;

#if !NET
    using Catel.Windows;
#endif

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module
        /// </summary>
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;

#if !NET
            serviceLocator.RegisterTypeIfNotYetRegistered<IFrameworkElementLoadedManager, FrameworkElementLoadedManager>();
#endif

            serviceLocator.RegisterTypeIfNotYetRegistered<IViewManager, ViewManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IViewModelManager, ViewModelManager>();

            ViewModelServiceHelper.RegisterDefaultViewModelServices(serviceLocator);
        }
    }
}
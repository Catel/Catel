// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsPrismModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.IoC;
    using Microsoft.Practices.Prism.Regions;
    using Tasks;

#if NET
    using Modules.ModuleManager;
#endif

    /// <summary>
    /// Extensions.Prism module which allows the registration of default services in the service locator.
    /// </summary>
    public static class ExtensionsPrismModule
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

            serviceLocator.RegisterTypeIfNotYetRegistered<IBootstrapperTaskFactory, BootstrapperTaskFactory>();

            serviceLocator.RegisterType<RegionAdapterMappings, RegionAdapterMappings>();

#if NET
            serviceLocator.RegisterTypeIfNotYetRegistered<IModuleInfoManager, ModuleInfoManager>();
#endif
        }
        #endregion
    }
}
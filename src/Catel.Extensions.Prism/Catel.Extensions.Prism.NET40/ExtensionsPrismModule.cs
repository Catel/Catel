// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsPrismModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.IoC;
    using Catel.Services;

    using Microsoft.Practices.Prism.Regions;
    using Tasks;

#if NET
    using Modules.ModuleManager;
#endif

    /// <summary>
    /// Extensions.Prism module which allows the registration of default services in the service locator.
    /// </summary>
    public class ExtensionsPrismModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<IBootstrapperTaskFactory, BootstrapperTaskFactory>();

            serviceLocator.RegisterType<RegionAdapterMappings, RegionAdapterMappings>();
            serviceLocator.RegisterType<IUICompositionService, UICompositionService>();
        }
    }
}
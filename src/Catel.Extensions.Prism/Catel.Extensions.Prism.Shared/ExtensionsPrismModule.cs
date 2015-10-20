// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsPrismModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using IoC;
    using Services;

    using Microsoft.Practices.Prism.Regions;
    using Tasks;

    /// <summary>
    /// Extensions.Prism module which allows the registration of default services in the service locator.
    /// </summary>
    public partial class ExtensionsPrismModule : IServiceLocatorInitializer
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

        partial void InitializePlatform();
    }
}
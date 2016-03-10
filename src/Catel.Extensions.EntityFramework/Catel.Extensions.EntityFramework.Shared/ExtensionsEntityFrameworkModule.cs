// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsEntityFrameworkModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.Data;
    using Catel.IoC;

    /// <summary>
    /// Extensions.EntityFramework module which allows the registration of default services in the service locator.
    /// </summary>
    public partial class ExtensionsEntityFrameworkModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            serviceLocator.RegisterTypeIfNotYetRegistered<IConnectionStringManager, ConnectionStringManager>();
            serviceLocator.RegisterTypeIfNotYetRegistered<IContextFactory, ContextFactory>();
        }

        partial void InitializePlatform();
    }
}
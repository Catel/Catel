// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    /// <summary>
    /// Contains configurations for the IoC implementation in Catel.
    /// </summary>
    public static class IoCConfiguration
    {
        /// <summary>
        /// Initializes static members of the <see cref="IoCConfiguration"/> class.
        /// </summary>
        static IoCConfiguration()
        {
            var serviceLocator = new ServiceLocator();

            DefaultServiceLocator = serviceLocator;
            DefaultDependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
            DefaultTypeFactory = serviceLocator.ResolveType<ITypeFactory>();
        }

        /// <summary>
        /// Gets or sets the default service locator.
        /// </summary>
        /// <value>The default service locator.</value>
        public static IServiceLocator DefaultServiceLocator { get; set; }

        /// <summary>
        /// Gets or sets the default dependency resolver.
        /// </summary>
        /// <value>The default dependency resolver.</value>
        public static IDependencyResolver DefaultDependencyResolver { get; set; }

        /// <summary>
        /// Gets or sets the default type factory.
        /// </summary>
        /// <value>The default type factory.</value>
        public static ITypeFactory DefaultTypeFactory { get; set; }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using Catel.Logging;

    /// <summary>
    /// Contains configurations for the IoC implementation in Catel.
    /// </summary>
    public static class IoCConfiguration
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly object _lockObject = new object();

        private static IServiceLocator _defaultServiceLocator;
        private static IDependencyResolver _defaultDependencyResolver;
        private static ITypeFactory _defaultTypeFactory;

        /// <summary>
        /// Gets or sets the default service locator.
        /// </summary>
        /// <value>The default service locator.</value>
        public static IServiceLocator DefaultServiceLocator
        {
            get
            {
                lock (_lockObject)
                {
                    if (_defaultServiceLocator is null)
                    {
                        UpdateDefaultComponents();
                    }

                    return _defaultServiceLocator;
                }
            }
            private set
            {
                _defaultServiceLocator = value;
            }
        }

        /// <summary>
        /// Gets or sets the default dependency resolver.
        /// </summary>
        /// <value>The default dependency resolver.</value>
        public static IDependencyResolver DefaultDependencyResolver
        {
            get
            {
                lock (_lockObject)
                {
                    if (_defaultDependencyResolver is null)
                    {
                        UpdateDefaultComponents();
                    }

                    return _defaultDependencyResolver;
                }
            }
            private set
            {
                _defaultDependencyResolver = value;
            }
        }

        /// <summary>
        /// Gets or sets the default type factory.
        /// </summary>
        /// <value>The default type factory.</value>
        public static ITypeFactory DefaultTypeFactory
        {
            get
            {
                lock (_lockObject)
                {
                    if (_defaultTypeFactory is null)
                    {
                        UpdateDefaultComponents();
                    }

                    return _defaultTypeFactory;
                }
            }
            private set
            {
                _defaultTypeFactory = value;
            }
        }

        /// <summary>
        /// Updates the default components.
        /// <para />
        /// This method should be called when any of the factory methods has been changed.
        /// </summary>
        /// <exception cref="System.Exception">The method fails to create the <see cref="IServiceLocator"/> using the factory.</exception>
        public static void UpdateDefaultComponents()
        {
            Log.Info("Updating default components");

            // Don't initialize the first service locator (we are still loading assemblies at that time)
            bool initializeServiceLocator = (_defaultServiceLocator is not null);
            var serviceLocator = IoCFactory.CreateServiceLocator(initializeServiceLocator);

            lock (_lockObject)
            {
                DefaultServiceLocator = serviceLocator;
                DefaultDependencyResolver = serviceLocator.ResolveType<IDependencyResolver>();
                DefaultTypeFactory = serviceLocator.ResolveType<ITypeFactory>();
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using Logging;
    using Reflection;

    /// <summary>
    /// Factory responsible for creating IoC components.
    /// </summary>
    public static class IoCFactory
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly object _lockObject = new object();

        #region Constants
        private static Func<IServiceLocator> _createServiceLocatorFunc;
        private static Func<IServiceLocator, IDependencyResolver> _createDependencyResolverFunc;
        private static Func<IServiceLocator, ITypeFactory> _createTypeFactoryFunc;

        private static List<Type> _serviceLocatorInitializers = new List<Type>(); 
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="IoCFactory"/> class.
        /// </summary>
        static IoCFactory()
        {
            CreateServiceLocatorFunc = () => new ServiceLocator();
            CreateDependencyResolverFunc = serviceLocator => new CatelDependencyResolver(serviceLocator);
            CreateTypeFactoryFunc = serviceLocator => new TypeFactory(serviceLocator);

            TypeCache.AssemblyLoaded += OnAssemblyLoaded;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the create service locator function.
        /// </summary>
        /// <value>The create service locator function.</value>
        public static Func<IServiceLocator> CreateServiceLocatorFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createServiceLocatorFunc;
                }
            }
            set
            {
                Argument.IsNotNull("CreateServiceLocatorFunc", value);

                lock (_lockObject)
                {
                    _createServiceLocatorFunc = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the create dependency resolverfunction.
        /// </summary>
        /// <value>The create dependency resolver function.</value>
        public static Func<IServiceLocator, IDependencyResolver> CreateDependencyResolverFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createDependencyResolverFunc;
                }
            }
            set
            {
                Argument.IsNotNull("CreateDependencyResolverFunc", value);

                lock (_lockObject)
                {
                    _createDependencyResolverFunc = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the create default service locator function.
        /// </summary>
        /// <value>The create default service locator function.</value>
        public static Func<IServiceLocator, ITypeFactory> CreateTypeFactoryFunc
        {
            get
            {
                lock (_lockObject)
                {
                    return _createTypeFactoryFunc;
                }
            }
            set
            {
                Argument.IsNotNull("CreateTypeFactoryFunc", value);

                lock (_lockObject)
                {
                    _createTypeFactoryFunc = value;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when an assembly gets loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AssemblyLoadedEventArgs"/> instance containing the event data.</param>
        private static void OnAssemblyLoaded(object sender, AssemblyLoadedEventArgs e)
        {
            lock (_lockObject)
            {
                _serviceLocatorInitializers = null;
            }
        }

        /// <summary>
        /// Creates a service locator with all the customized components.
        /// </summary>
        /// <param name="initializeServiceLocator">if set to <c>true</c>, the <see cref="IServiceLocator"/> will be initialized using the <see cref="IServiceLocatorInitializer"/> interface.</param>
        /// <returns>The newly created <see cref="IServiceLocator" />.</returns>
        public static IServiceLocator CreateServiceLocator(bool initializeServiceLocator = true)
        {
            lock (_lockObject)
            {
                var serviceLocator = CreateServiceLocatorFunc();
                if (serviceLocator == null)
                {
                    string error = string.Format("Failed to create the IServiceLocator instance using the factory method");
                    Log.Error(error);
                    throw new Exception(error);
                }

                if (!serviceLocator.IsTypeRegistered<IDependencyResolver>())
                {
                    var dependencyResolver = CreateDependencyResolverFunc(serviceLocator);
                    if (dependencyResolver == null)
                    {
                        string error = string.Format("Failed to create the IDependencyResolver instance using the factory method");
                        Log.Error(error);
                        throw new Exception(error);
                    }

                    serviceLocator.RegisterInstance(typeof(IDependencyResolver), dependencyResolver);
                }

                if (!serviceLocator.IsTypeRegistered<ITypeFactory>())
                {
                    var typeFactory = CreateTypeFactoryFunc(serviceLocator);
                    if (typeFactory == null)
                    {
                        string error = string.Format("Failed to create the ITypeFactory instance using the factory method");
                        Log.Error(error);
                        throw new Exception(error);
                    }

                    serviceLocator.RegisterInstance(typeof(ITypeFactory), typeFactory);
                }

                if (initializeServiceLocator)
                {
                    if (_serviceLocatorInitializers == null)
                    {
                        _serviceLocatorInitializers = new List<Type>(TypeCache.GetTypes(x => !x.IsInterfaceEx() & x.ImplementsInterfaceEx<IServiceLocatorInitializer>()));
                    }

                    foreach (var serviceLocatorInitializer in _serviceLocatorInitializers)
                    {
                        try
                        {
                            var initializer = (IServiceLocatorInitializer)Activator.CreateInstance(serviceLocatorInitializer);
                            initializer.Initialize(serviceLocator);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Failed to initialize service locator using initializer '{0}'", serviceLocatorInitializer.GetSafeFullName());
                            throw;
                        }
                    }
                }

                return serviceLocator;
            }
        }
        #endregion
    }
}
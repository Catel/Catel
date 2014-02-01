// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using Catel.Logging;

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
        private static Func<IDependencyResolver, ITypeFactory> _createTypeFactoryFunc;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="IoCFactory"/> class.
        /// </summary>
        static IoCFactory()
        {
            CreateServiceLocatorFunc = () => new ServiceLocator();
            CreateDependencyResolverFunc = serviceLocator => new CatelDependencyResolver(serviceLocator);
            CreateTypeFactoryFunc = dependencyResolver => new TypeFactory(dependencyResolver);
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
        public static Func<IDependencyResolver, ITypeFactory> CreateTypeFactoryFunc
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
        /// Creates a service locator with all the customized components.
        /// </summary>
        /// <returns>The newly created <see cref="IServiceLocator"/>.</returns>
        public static IServiceLocator CreateServiceLocator()
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

                var dependencyResolver = CreateDependencyResolverFunc(serviceLocator);
                if (dependencyResolver == null)
                {
                    string error = string.Format("Failed to create the IDependencyResolver instance using the factory method");
                    Log.Error(error);
                    throw new Exception(error);
                }

                var typeFactory = CreateTypeFactoryFunc(dependencyResolver);
                if (typeFactory == null)
                {
                    string error = string.Format("Failed to create the ITypeFactory instance using the factory method");
                    Log.Error(error);
                    throw new Exception(error);
                }

                serviceLocator.RegisterInstance(typeof (IDependencyResolver), dependencyResolver);
                serviceLocator.RegisterInstance(typeof (ITypeFactory), typeFactory);

                return serviceLocator;
            }
        }
        #endregion
    }
}
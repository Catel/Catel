// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// Dependency resolver.
    /// </summary>
    public class DependencyResolver : IDependencyResolver
    {
        private readonly IServiceLocator _serviceLocator;
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyResolver"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, the <see cref="ServiceLocator.Default"/> will be used.</param>
        public DependencyResolver(IServiceLocator serviceLocator = null)
        {
            if (serviceLocator == null)
            {
                serviceLocator = ServiceLocator.Default;
            }

            _serviceLocator = serviceLocator;
            _typeFactory = serviceLocator.ResolveType<ITypeFactory>();
        }

        /// <summary>
        /// Resolves singly registered services that support arbitrary object creation.
        /// </summary>
        /// <param name="serviceType">The type of the requested service or object.</param>
        /// <returns>The requested service or object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        public object GetService(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            if (_serviceLocator.IsTypeRegistered(serviceType))
            {
                return _serviceLocator.ResolveType(serviceType);
            }

            if (!serviceType.IsInterface && !serviceType.IsAbstract)
            {
                return _typeFactory.CreateInstance(serviceType);
            }

            return null;
        }

        /// <summary>
        /// Resolves multiply registered services.
        /// </summary>
        /// <param name="serviceType">The type of the requested services.</param>
        /// <returns>The requested services.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType" /> is <c>null</c>.</exception>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            Argument.IsNotNull("serviceType", serviceType);

            if (_serviceLocator.IsTypeRegistered(serviceType))
            {
                return _serviceLocator.ResolveTypes(serviceType);
            }

            return new object[] { };
        }
    }
}
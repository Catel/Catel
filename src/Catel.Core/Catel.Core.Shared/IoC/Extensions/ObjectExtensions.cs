// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// IoC extensions for all objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Gets the <see cref="IServiceLocator"/> that was used to create the specified object.
        /// <para />
        /// This is a convenience call that internally does this:
        /// <para />
        /// <code>
        /// <![CDATA[
        /// var serviceLocator = DependencyResolverManager.Default;
        /// var dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);
        /// var serviceLocator = dependencyResolver.Resolve<IServiceLocator>();
        /// ]]>
        /// </code>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The <see cref="IServiceLocator"/> used to create this object.</returns>
        public static IServiceLocator GetServiceLocator(this object obj)
        {
            var dependencyResolver = GetDependencyResolver(obj);
            var serviceLocator = dependencyResolver.Resolve<IServiceLocator>();

            return serviceLocator;
        }

        /// <summary>
        /// Gets the <see cref="ITypeFactory"/> that was used to create the specified object.
        /// <para />
        /// This is a convenience call that internally does this:
        /// <para />
        /// <code>
        /// <![CDATA[
        /// var dependencyResolverManager = DependencyResolverManager.Default;
        /// var dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);
        /// var typeFactory = dependencyResolver.Resolve<ITypeFactory>();
        /// ]]>
        /// </code>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The <see cref="ITypeFactory"/> used to create this object.</returns>
        public static ITypeFactory GetTypeFactory(this object obj)
        {
            var dependencyResolver = GetDependencyResolver(obj);
            var typeFactory = dependencyResolver.Resolve<ITypeFactory>();

            return typeFactory;
        }

        /// <summary>
        /// Gets the <see cref="IDependencyResolver"/> that was used to create the specified object.
        /// <para />
        /// This is a convenience call that internally does this:
        /// <para />
        /// <code>
        /// <![CDATA[
        /// var dependencyResolverManager = DependencyResolverManager.Default;
        /// var dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);
        /// ]]>
        /// </code>
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The <see cref="IDependencyResolver"/> for this object.</returns>
        public static IDependencyResolver GetDependencyResolver(this object obj)
        {
            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;

            if (obj != null)
            {
                var dependencyResolverManager = DependencyResolverManager.Default;
                dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);
            }

            return dependencyResolver;
        }
    }
}
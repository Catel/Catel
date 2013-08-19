// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        public static ITypeFactory GetTypeFactory(this object obj)
        {
            Argument.IsNotNull("obj", obj);

            var dependencyResolverManager = DependencyResolverManager.Default;
            var dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);
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
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        public static IDependencyResolver GetDependencyResolver(this object obj)
        {
            Argument.IsNotNull("obj", obj);

            var dependencyResolverManager = DependencyResolverManager.Default;
            var dependencyResolver = dependencyResolverManager.GetDependencyResolverForInstance(obj);

            return dependencyResolver;
        }
    }
}
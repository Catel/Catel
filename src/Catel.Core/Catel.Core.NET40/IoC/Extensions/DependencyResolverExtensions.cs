// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyResolverExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Extensions for the <see cref="IDependencyResolver"/>.
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Determines whether the specified type with the specified tag can be resolved.
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver"/> is <c>null</c>.</exception>
        public static bool CanResolve<T>(this IDependencyResolver dependencyResolver, object tag = null)
        {
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            return dependencyResolver.CanResolve(typeof (T), tag);
        }

        /// <summary>
        /// Resolves the specified type with the specified tag.
        /// </summary>
        /// <typeparam name="T">Tye type to resolve.</typeparam>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The resolved object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        public static T Resolve<T>(this IDependencyResolver dependencyResolver, object tag = null)
        {
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            return (T)dependencyResolver.Resolve(typeof(T), tag);
        }

        /// <summary>
        /// Try to resolve the specified type with the specified tag.
        /// </summary>
        /// <typeparam name="T">Tye type to resolve.</typeparam>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The resolved object or <c>null</c> if the type could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        public static T TryResolve<T>(this IDependencyResolver dependencyResolver, object tag = null)
        {
            Argument.IsNotNull("dependencyResolver", dependencyResolver);

            try
            {
                return (T)dependencyResolver.Resolve(typeof(T), tag);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
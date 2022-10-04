namespace Catel.IoC
{
    using System;
    using Catel.Reflection;

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
        public static bool CanResolve<T>(this IDependencyResolver dependencyResolver, object? tag = null)
        {
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
        public static T? Resolve<T>(this IDependencyResolver dependencyResolver, object? tag = null)
        {
            return (T?)dependencyResolver.Resolve(typeof(T), tag);
        }

        /// <summary>
        /// Resolves the specified type with the specified tag.
        /// </summary>
        /// <typeparam name="T">Tye type to resolve.</typeparam>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>The resolved object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        public static T ResolveRequired<T>(this IDependencyResolver dependencyResolver, object? tag = null)
        {
            var service = (T?)dependencyResolver.Resolve(typeof(T), tag);
            if (service is null)
            {
                throw new CatelException($"Cannot resolve type '{typeof(T).GetSafeFullName()}'");
            }

            return service;
        }
    }
}

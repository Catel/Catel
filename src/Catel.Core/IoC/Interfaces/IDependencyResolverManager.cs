namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Allows the registration and retrieval of dependency resolvers for specific types or instances.
    /// </summary>
    public interface IDependencyResolverManager
    {
        /// <summary>
        /// Gets or sets the default dependency resolver.
        /// </summary>
        /// <value>The default dependency resolver.</value>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <c>null</c>.</exception>
        IDependencyResolver DefaultDependencyResolver { get; set; }

        /// <summary>
        /// Registers the dependency resolver for a specific instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        void RegisterDependencyResolverForInstance(object instance, IDependencyResolver dependencyResolver);

        /// <summary>
        /// Gets the dependency resolver for a specific instance. If there is no dependency resolver registered for
        /// the specific instance, this method will use the <see cref="GetDependencyResolverForType"/>.
        /// </summary>
        /// <param name="instance">The instance to retrieve the dependency resolver for.</param>
        /// <returns>The <see cref="IDependencyResolver"/> for the object.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> is <c>null</c>.</exception>
        IDependencyResolver GetDependencyResolverForInstance(object instance);

        /// <summary>
        /// Registers the dependency resolver for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="dependencyResolver" /> is <c>null</c>.</exception>
        void RegisterDependencyResolverForType(Type type, IDependencyResolver dependencyResolver);

        /// <summary>
        /// Gets the dependency resolver for a specific type. If there is no dependency resolver registered for
        /// the specific type, this method will returns the <see cref="DefaultDependencyResolver"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="IDependencyResolver"/> for the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        IDependencyResolver GetDependencyResolverForType(Type type);
    }
}

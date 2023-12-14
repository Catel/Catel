namespace Catel.IoC
{
    using System;

    /// <summary>
    /// EventArgs for the <see cref="IServiceLocator.TypeInstantiated"/> event.
    /// </summary>
    public class TypeInstantiatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeInstantiatedEventArgs" /> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="instance">The instance of a service.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        public TypeInstantiatedEventArgs(Type serviceType, Type serviceImplementationType, object? tag, RegistrationType registrationType, object instance)
        {
            ServiceType = serviceType;
            ServiceImplementationType = serviceImplementationType;
            Tag = tag;
            RegistrationType = registrationType;
            Instance = instance;
        }

        /// <summary>
        /// Gets the type of the service.
        /// </summary>
        /// <value>The type of the service.</value>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Gets the type of the actual implementation.
        /// </summary>
        /// <value>The type of the actual implementation.</value>
        public Type ServiceImplementationType { get; private set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object? Tag { get; private set; }

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        /// <value>The type of the registration.</value>
        public RegistrationType RegistrationType { get; private set; }

        /// <summary>
        /// Gets the resolved instance.
        /// </summary>
        /// <value>The resolved instance or null.</value>
        public object Instance { get;  }
    }
}

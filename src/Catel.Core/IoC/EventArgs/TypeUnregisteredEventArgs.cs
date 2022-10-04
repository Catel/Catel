namespace Catel.IoC
{
    using System;

    /// <summary>
    /// EventArgs for the <see cref="IServiceLocator.TypeUnregistered"/> event.
    /// </summary>
    public class TypeUnregisteredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeUnregisteredEventArgs" /> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        public TypeUnregisteredEventArgs(Type serviceType, Type serviceImplementationType, object tag, RegistrationType registrationType)
            : this(serviceType, serviceImplementationType, tag, registrationType, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeUnregisteredEventArgs" /> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="instance">The instance of the service was instantiated.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        public TypeUnregisteredEventArgs(Type serviceType, Type serviceImplementationType, object tag, 
            RegistrationType registrationType, object instance)
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
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }

        /// <summary>
        /// Gets the type of the actual implementation.
        /// </summary>
        /// <value>The type of the actual implementation.</value>
        public Type ServiceImplementationType { get; private set; }

        /// <summary>
        /// Gets the instance of the service if it was instantiated.
        /// </summary>
        public object Instance { get; private set; }

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        /// <value>The type of the registration.</value>
        public RegistrationType RegistrationType { get; private set; }
    }
}

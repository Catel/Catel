// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeRegisteredEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// EventArgs for the <see cref="IServiceLocator.TypeRegistered"/> event.
    /// </summary>
    public class TypeRegisteredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeRegisteredEventArgs" /> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="serviceImplementationType">Type of the service implementation.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceImplementationType"/> is <c>null</c>.</exception>
        public TypeRegisteredEventArgs(Type serviceType, Type serviceImplementationType, object tag, RegistrationType registrationType)
        {
            Argument.IsNotNull("serviceType", serviceType);
            Argument.IsNotNull("serviceImplementationType", serviceImplementationType);

            ServiceType = serviceType;
            ServiceImplementationType = serviceImplementationType;
            Tag = tag;
            RegistrationType = registrationType;
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
        public object Tag { get; private set; }

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        /// <value>The type of the registration.</value>
        public RegistrationType RegistrationType { get; private set; }
    }
}
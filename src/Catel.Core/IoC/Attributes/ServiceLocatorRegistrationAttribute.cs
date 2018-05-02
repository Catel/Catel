// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorRegistrationAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Available modes for the <see cref="ServiceLocatorRegistrationAttribute" />.
    /// </summary>
    public enum ServiceLocatorRegistrationMode
    {
        /// <summary>
        /// The type will be registered as transient.
        /// </summary>
        Transient,

        /// <summary>
        /// The singleton instance will be created immediately and then registered.
        /// </summary>
        SingletonInstantiateImmediately,

        /// <summary>
        /// The singleton instance will be created when it is first queried.
        /// </summary>
        SingletonInstantiateWhenRequired
    }

    /// <summary>
    /// The register attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceLocatorRegistrationAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorRegistrationAttribute"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="registrationMode">The registration mode.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="interfaceType" /> is <c>null</c>.</exception>
        public ServiceLocatorRegistrationAttribute(Type interfaceType, ServiceLocatorRegistrationMode registrationMode = ServiceLocatorRegistrationMode.SingletonInstantiateWhenRequired, object tag = null)
        {
            Argument.IsNotNull("InterfaceType", interfaceType);

            InterfaceType = interfaceType;
            RegistrationMode = registrationMode;
            Tag = tag;

            RegistrationType = (registrationMode == ServiceLocatorRegistrationMode.Transient) ? RegistrationType.Transient : RegistrationType.Singleton;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type InterfaceType { get; private set; }

        /// <summary>
        /// Gets the registration type.
        /// </summary>
        public RegistrationType RegistrationType { get; private set; }

        /// <summary>
        /// Gets the registration model.
        /// </summary>
        /// <value>The registration model.</value>
        public ServiceLocatorRegistrationMode RegistrationMode { get; private set; }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }
        #endregion
    }
}
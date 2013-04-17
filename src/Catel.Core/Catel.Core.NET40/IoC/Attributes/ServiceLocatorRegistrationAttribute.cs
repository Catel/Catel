// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorRegistrationAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// The register attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceLocatorRegistrationAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorRegistrationAttribute" /> class.
        /// </summary>
        /// <param name="interfaceType">The type.</param>
        /// <param name="registrationType">The registration type.</param>
        /// <param name="tag">The tag.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="interfaceType" /> is <c>null</c>.</exception>
        public ServiceLocatorRegistrationAttribute(Type interfaceType, RegistrationType registrationType = RegistrationType.Singleton, object tag = null)
        {
            Argument.IsNotNull("InterfaceType", interfaceType);

            InterfaceType = interfaceType;
            RegistrationType = registrationType;
            Tag = tag;
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
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }
        #endregion
    }
}
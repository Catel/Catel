// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Registration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET


namespace Catel.IoC
{
    using System;
    using System.Configuration;
    using Catel.Reflection;

    /// <summary>
    /// The registration element.
    /// </summary>
    public class Registration : ConfigurationElement
    {
        #region Constants
        /// <summary>
        /// The interface type property name.
        /// </summary>
        private const string InterfaceTypePropertyName = "interfaceType";

        /// <summary>
        /// The implementation type property name.
        /// </summary>
        private const string ImplementationTypePropertyName = "implementationType";

        /// <summary>
        /// The registration type property name.
        /// </summary>
        private const string RegistrationTypePropertyName = "registrationType";

        /// <summary>
        /// The registration tag property name.
        /// </summary>
        private const string TagPropertyName = "tag";
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the interface type name.
        /// </summary>
        /// <value>The name of the interface type.</value>
        [ConfigurationProperty(InterfaceTypePropertyName, IsRequired = true)]
        public string InterfaceTypeName
        {
            get { return (string) this[InterfaceTypePropertyName]; }
            set { this[InterfaceTypePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the implementation type name.
        /// </summary>
        /// <value>The name of the implementation type.</value>
        [ConfigurationProperty(ImplementationTypePropertyName, IsRequired = true)]
        public string ImplementationTypeName
        {
            get { return (string) this[ImplementationTypePropertyName]; }
            set { this[ImplementationTypePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the registration type.
        /// </summary>
        /// <value>The type of the registration.</value>
        [ConfigurationProperty(RegistrationTypePropertyName, DefaultValue = RegistrationType.Singleton)]
        public RegistrationType RegistrationType
        {
            get { return (RegistrationType) this[RegistrationTypePropertyName]; }
            set { this[RegistrationTypePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        [ConfigurationProperty(TagPropertyName)]
        public string Tag
        {
            get
            {
                var tag = (string) this[TagPropertyName];

                if (string.IsNullOrEmpty(tag))
                {
                    tag = null;
                }

                return tag;
            }
            set { this[TagPropertyName] = value; }
        }

        /// <summary>
        /// Gets the interface type.
        /// </summary>
        public Type InterfaceType
        {
            get { return TypeCache.GetType(InterfaceTypeName, allowInitialization: false); }
        }

        /// <summary>
        /// Gets the implementation type.
        /// </summary>
        public Type ImplementationType
        {
            get { return TypeCache.GetType(ImplementationTypeName, allowInitialization: false); }
        }
        #endregion
    }
}

#endif
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET


namespace Catel.IoC
{
    using System.Configuration;

    using Catel.IoC;

    /// <summary>
    /// The IoC configuration element.
    /// </summary>
    public sealed class ServiceLocatorConfiguration : ConfigurationElementCollection
    {
        #region Constants

        /// <summary>
        /// The name property name.
        /// </summary>
        private const string NamePropertyName = "name";

        /// <summary>
        /// The support dependency injection property name
        /// </summary>
        private const string SupportDependencyInjectionPropertyName = "supportDependencyInjection";

        /// <summary>
        /// The item element name.
        /// </summary>
        private const string ItemElementName = "register";
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorConfiguration" /> class.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="supportDependencyInjection">Indicates whether the <see cref="IServiceLocator" /> configured with this section will support dependency injection or not.</param>
        public ServiceLocatorConfiguration(string name = "default", bool supportDependencyInjection = true)
        {
            Name = name;
            SupportDependencyInjection = supportDependencyInjection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [ConfigurationProperty(NamePropertyName, DefaultValue = "default", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[NamePropertyName]; }
            set { this[NamePropertyName] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the configuration support dependency injection.
        /// </summary>
        /// <value><c>true</c> if [support dependency injection]; otherwise, <c>false</c>.</value>
        [ConfigurationProperty(SupportDependencyInjectionPropertyName, DefaultValue = true, IsRequired = false)]
        public bool SupportDependencyInjection
        {
            get { return (bool)this[SupportDependencyInjectionPropertyName]; }
            set { this[SupportDependencyInjectionPropertyName] = value; }
        }

        /// <summary>
        /// Gets the collection type.
        /// </summary>
        /// <value>The type of the collection.</value>
        /// <returns>The <see cref="T:System.Configuration.ConfigurationElementCollectionType" /> of this collection.</returns>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMapAlternate;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Creates a new ConfigurationElement.
        /// </summary>
        /// <returns>A new <see cref="System.Configuration.ConfigurationElement" />.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Registration();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="System.Configuration.ConfigurationElement" /> to return the key for.</param>
        /// <returns>An <see cref="object" /> that acts as the key for the specified <see cref="System.Configuration.ConfigurationElement" />.</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Registration)element).InterfaceTypeName;
        }

        /// <summary>
        /// Determines whether the given <param ref="elementName" /> is the collection element name.
        /// </summary>
        /// <param name="elementName">The element name</param>
        /// <returns><c>true</c> if is the element name, otherwise <c>false</c>.</returns>
        protected override bool IsElementName(string elementName)
        {
            return elementName == ItemElementName;
        }

        /// <summary>
        /// Configures an instance of <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="serviceLocator">The instance of <see cref="IServiceLocator" /></param>
        public void Configure(IServiceLocator serviceLocator)
        {
            serviceLocator.SupportDependencyInjection = SupportDependencyInjection;
            foreach (Registration registrationElement in this)
            {
                serviceLocator.RegisterType(registrationElement.InterfaceType, registrationElement.ImplementationType, registrationElement.Tag, registrationElement.RegistrationType);
            }
        }

        #endregion
    }
}

#endif
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorConfigurationCollection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Configuration;

    /// <summary>
    /// The IoC configuration collection.
    /// </summary>
    public sealed class ServiceLocatorConfigurationCollection : ConfigurationElementCollection
    {
        #region Constants

        /// <summary>
        /// The item element name.
        /// </summary>
        private const string ItemElementName = "serviceLocatorConfiguration";

        #endregion

        #region Properties
        
        /// <summary>
        /// The collection type
        /// </summary>
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
        /// Creates a new ConfigurationElement
        /// </summary>
        /// <returns>
        /// A new <see cref="System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceLocatorConfiguration();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">
        /// The <see cref="System.Configuration.ConfigurationElement"/> to return the key for.
        /// </param>
        /// <returns>
        /// An <see cref="object"/> that acts as the key for the specified <see cref="System.Configuration.ConfigurationElement"/>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceLocatorConfiguration)element).Name;
        }

        /// <summary>
        /// Determines whether the given <param ref="elementName"/> is the collection element name.
        /// </summary>
        /// <param name="elementName">The element name</param>
        /// <returns>
        /// <c>true</c> if is the element name, otherwise <c>false</c>.  
        /// </returns>
        protected override bool IsElementName(string elementName)
        {
            return string.Equals(elementName, ItemElementName, StringComparison.Ordinal);
        }
        
        #endregion
    }
}
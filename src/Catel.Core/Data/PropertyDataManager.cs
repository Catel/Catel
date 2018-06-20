// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Logging;

    /// <summary>
    /// Property data manager.
    /// </summary>
    public class PropertyDataManager
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Dictionary containing all the properties per type.
        /// </summary>
        private readonly Dictionary<Type, CatelTypeInfo> _propertyData = new Dictionary<Type, CatelTypeInfo>();

        /// <summary>
        /// Lock object for the <see cref="_propertyData"/> field.
        /// </summary>
        private readonly object _propertyDataLock = new object();

        private readonly XmlNameMapper<XmlElementAttribute> _xmlElementMappings;
        private readonly XmlNameMapper<XmlAttributeAttribute> _xmlAttributeMappings;
        #endregion

        /// <summary>
        /// Initializes static members of the <see cref="PropertyDataManager" /> class.
        /// </summary>
        static PropertyDataManager()
        {
            Default = new PropertyDataManager();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDataManager"/> class.
        /// </summary>
        public PropertyDataManager()
        {
            _xmlElementMappings = new XmlNameMapper<XmlElementAttribute>(this);
            _xmlAttributeMappings = new XmlNameMapper<XmlAttributeAttribute>(this);
        }

        #region Properties
        /// <summary>
        /// Gets the default instance of the property data manager.
        /// </summary>
        /// <value>The default.</value>
        public static PropertyDataManager Default { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the property data type information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="CatelTypeInfo"/> representing the specified type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public CatelTypeInfo GetCatelTypeInfo(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = RegisterProperties(type);
                }

                return typeInfo;
            }
        }

        /// <summary>
        /// Registers all the properties for the specified type.
        /// <para />
        /// This method can only be called once per type. The <see cref="PropertyDataManager"/> caches
        /// whether it has already registered the properties once.
        /// </summary>
        /// <param name="type">The type to register the properties for.</param>
        /// <returns>The property data type info.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The properties are not declared correctly.</exception>
        public CatelTypeInfo RegisterProperties(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                return typeInfo;
            }
        }

        /// <summary>
        /// Registers a property for a specific type.
        /// </summary>
        /// <param name="type">The type for which to register the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyData">The property data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">A property with the same name is already registered.</exception>
        public void RegisterProperty(Type type, string name, PropertyData propertyData)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("propertyData", propertyData);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                typeInfo.RegisterProperty(name, propertyData);
            }
        }

        /// <summary>
        /// Unregisters a property for a specific type.
        /// </summary>
        /// <param name="type">The type for which to register the property.</param>
        /// <param name="name">The name of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public void UnregisterProperty(Type type, string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var typeInfo))
                {
                    typeInfo = new CatelTypeInfo(type);
                    _propertyData[type] = typeInfo;
                }

                typeInfo.UnregisterProperty(name);
            }
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="type">The type for which to check whether the property is registered.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyRegistered(Type type, string name)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var propertyDataOfType))
                {
                    return false;
                }

                return propertyDataOfType.IsPropertyRegistered(name);
            }
        }

        /// <summary>
        /// Gets the property data.
        /// </summary>
        /// <param name="type">The type for which to get the property data.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The <see cref="PropertyData"/> of the requested property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        public PropertyData GetPropertyData(Type type, string name)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_propertyDataLock)
            {
                if (!_propertyData.TryGetValue(type, out var propertyDataOfType))
                {
                    throw Log.ErrorAndCreateException(msg => new PropertyNotRegisteredException(name, type),
                        "Property '{0}' on type '{1}' is not registered", name, type.FullName);
                }

                return propertyDataOfType.GetPropertyData(name);
            }
        }

        /// <summary>
        /// Determines whether the specified XML attribute is mapped to a property name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xmlName">Name of the XML.</param>
        /// <returns>
        /// <c>true</c> if the XML attribute is mapped to a property name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public bool IsXmlAttributeNameMappedToProperty(Type type, string xmlName)
        {
            return _xmlAttributeMappings.IsXmlNameMappedToProperty(type, xmlName);
        }

        /// <summary>
        /// Determines whether the specified property is mapped to an XML attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the property name is mapped to an XML attribute; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyNameMappedToXmlAttribute(Type type, string propertyName)
        {
            return _xmlAttributeMappings.IsPropertyNameMappedToXmlName(type, propertyName);
        }

        /// <summary>
        /// Maps the name of the XML attribute to a property name.
        /// </summary>
        /// <param name="type">The type for which to make the xml name.</param>
        /// <param name="xmlName">Name of the XML attribute.</param>
        /// <returns>
        /// Name of the property that represents the xml value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public string MapXmlAttributeNameToPropertyName(Type type, string xmlName)
        {
            return _xmlAttributeMappings.MapXmlNameToPropertyName(type, xmlName);
        }

        /// <summary>
        /// Maps the name of the property name to an XML attribute name.
        /// </summary>
        /// <param name="type">The type for which to make the xml name.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// Name of the XML attribute that represents the property value.
        /// </returns>
        public string MapPropertyNameToXmlAttributeName(Type type, string propertyName)
        {
            return _xmlAttributeMappings.MapPropertyNameToXmlName(type, propertyName);
        }

        /// <summary>
        /// Determines whether the specified XML element is mapped to a property name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xmlName">Name of the XML.</param>
        /// <returns>
        /// <c>true</c> if the XML element is mapped to a property name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public bool IsXmlElementNameMappedToProperty(Type type, string xmlName)
        {
            return _xmlElementMappings.IsXmlNameMappedToProperty(type, xmlName);
        }

        /// <summary>
        /// Determines whether the specified property is mapped to an XML element.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// <c>true</c> if the property name is mapped to an XML element; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyNameMappedToXmlElement(Type type, string propertyName)
        {
            return _xmlElementMappings.IsPropertyNameMappedToXmlName(type, propertyName);
        }

        /// <summary>
        /// Maps the name of the XML element to a property name.
        /// </summary>
        /// <param name="type">The type for which to make the xml name.</param>
        /// <param name="xmlName">Name of the XML element.</param>
        /// <returns>
        /// Name of the property that represents the xml value.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public string MapXmlElementNameToPropertyName(Type type, string xmlName)
        {
            return _xmlElementMappings.MapXmlNameToPropertyName(type, xmlName);
        }

        /// <summary>
        /// Maps the name of the property name to an XML element name.
        /// </summary>
        /// <param name="type">The type for which to make the xml name.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// Name of the XML element that represents the property value.
        /// </returns>
        public string MapPropertyNameToXmlElementName(Type type, string propertyName)
        {
            return _xmlElementMappings.MapPropertyNameToXmlName(type, propertyName);
        }
        #endregion
    }
}

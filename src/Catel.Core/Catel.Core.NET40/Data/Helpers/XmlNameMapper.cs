// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlNameMapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Reflection;

    /// <summary>
    /// Xml name mappings from xml to properties and back.
    /// </summary>
    /// <typeparam name="T">Type of the xml type to map, for example <see cref="XmlElementAttribute"/> or <see cref="XmlAttributeAttribute"/>.</typeparam>
    public class XmlNameMapper<T>
    {
        /// <summary>
        /// Lock object for the xml mappings fields.
        /// </summary>
        private readonly object _xmlMappingsLock = new object();

        /// <summary>
        /// Dictionary to provide fast xml name to property name mappings.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, string>> _xmlNameToPropertyNameMappings = new Dictionary<Type, Dictionary<string, string>>();

        /// <summary>
        /// Dictionary to provide fast property name to xml name mappings.
        /// </summary>
        private readonly Dictionary<Type, Dictionary<string, string>> _xmlPropertyNameToXmlNameMappings = new Dictionary<Type, Dictionary<string, string>>();

        /// <summary>
        /// The property data manager used to retrieve the properties of a type.
        /// </summary>
        private readonly PropertyDataManager _propertyDataManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlNameMapper&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyDataManager">The property data manager.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyDataManager"/> is <c>null</c>.</exception>
        internal XmlNameMapper(PropertyDataManager propertyDataManager)
        {
            Argument.IsNotNull("propertyDataManager", propertyDataManager);

            _propertyDataManager = propertyDataManager;
        }

        /// <summary>
        /// Determines whether the specified XML element is mapped to a property name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="xmlName">Name of the XML.</param>
        /// <returns>
        /// 	<c>true</c> if the XML element is mapped to a property name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public bool IsXmlNameMappedToProperty(Type type, string xmlName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("xmlName", xmlName);

            InitializeXmlPropertyMappings(type);

            lock (_xmlMappingsLock)
            {
                return _xmlNameToPropertyNameMappings[type].ContainsKey(xmlName);
            }
        }

        /// <summary>
        /// Determines whether the property name is mapped to an XML name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        ///   <c>true</c> if the XML element is mapped to a property name; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyNameMappedToXmlName(Type type, string propertyName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            InitializeXmlPropertyMappings(type);

            lock (_xmlMappingsLock)
            {
                return _xmlPropertyNameToXmlNameMappings[type].ContainsKey(propertyName);
            }
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
        public string MapXmlNameToPropertyName(Type type, string xmlName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("xmlName", xmlName);

            InitializeXmlPropertyMappings(type);

            lock (_xmlMappingsLock)
            {
                var typeMappings = _xmlNameToPropertyNameMappings[type];
                return typeMappings[xmlName];
            }
        }

        /// <summary>
        /// Maps the name of the property name to an XML name.
        /// </summary>
        /// <param name="type">The type for which to make the xml name.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// Name of the XML element that represents the property value.
        /// </returns>
        public string MapPropertyNameToXmlName(Type type, string propertyName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            InitializeXmlPropertyMappings(type);

            lock (_xmlMappingsLock)
            {
                var typeMappings = _xmlPropertyNameToXmlNameMappings[type];
                return typeMappings[propertyName];
            }
        }

        /// <summary>
        /// Initializes the XML property mappings.
        /// </summary>
        /// <param name="type">The type for which to initialize the xml mappings.</param>
        private void InitializeXmlPropertyMappings(Type type)
        {
            if (_xmlNameToPropertyNameMappings.ContainsKey(type))
            {
                return;
            }

            lock (_xmlMappingsLock)
            {
                _xmlNameToPropertyNameMappings.Add(type, new Dictionary<string, string>());
                _xmlPropertyNameToXmlNameMappings.Add(type, new Dictionary<string, string>());

                var catelTypeInfo = _propertyDataManager.GetCatelTypeInfo(type);
                foreach (var propertyData in catelTypeInfo.GetCatelProperties())
                {
                    var cachedPropertyInfo = propertyData.Value.GetPropertyInfo(type);
                    var propertyInfo = cachedPropertyInfo.PropertyInfo;
                    if (propertyInfo == null)
                    {
                        // Dynamic property, not mapped (always fixed)
                        continue;
                    }

                    // 1st, check if XmlIgnore is used
                    if (AttributeHelper.IsDecoratedWithAttribute<XmlIgnoreAttribute>(propertyInfo))
                    {
                        continue;
                    }

                    // 2nd, check if XmlAttribute is used
                    XmlAttributeAttribute xmlAttributeAttribute = null;
                    AttributeHelper.TryGetAttribute(propertyInfo, out xmlAttributeAttribute);
                    if (InitializeXmlAttributeAttribute(type, xmlAttributeAttribute, propertyData.Key))
                    {
                        continue;
                    }

                    // 3rd, check if XmlElement is used
                    XmlElementAttribute xmlElementAttribute = null;
                    AttributeHelper.TryGetAttribute(propertyInfo, out xmlElementAttribute);
                    if (InitializeXmlElementAttribute(type, xmlElementAttribute, propertyData.Key))
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the XML attribute attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attribute">The attribute. Can be <c>null</c> if not decorated with an attribute.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the attribute is handled by this mapper; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        private bool InitializeXmlAttributeAttribute(Type type, XmlAttributeAttribute attribute, string propertyName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            if (typeof(T) != typeof(XmlAttributeAttribute))
            {
                // If attribute has a value, we simply do not support it,  but we return true because
                // it should be seen as handled
                return attribute != null;
            }

            if (attribute != null)
            {
                string mappedName = attribute.AttributeName;
                if (string.IsNullOrWhiteSpace(mappedName))
                {
                    mappedName = propertyName;
                }

                _xmlNameToPropertyNameMappings[type].Add(mappedName, propertyName);
                _xmlPropertyNameToXmlNameMappings[type].Add(propertyName, mappedName);
            }

            return true;
        }

        /// <summary>
        /// Initializes the XML element attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attribute">The attribute. Can be <c>null</c> if not decorated with an attribute.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the attribute is handled by this mapper; otherwise, <c>false</c>.</returns>/// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> is <c>null</c> or whitespace.</exception>
        private bool InitializeXmlElementAttribute(Type type, XmlElementAttribute attribute, string propertyName)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            if (typeof(T) != typeof(XmlElementAttribute))
            {
                // If attribute has a value, we simply do not support it,  but we return true because
                // it should be seen as handled
                return attribute != null;
            }

            string mappedName = propertyName;
            if (attribute != null)
            {
                mappedName = attribute.ElementName;
            }

            if (string.IsNullOrEmpty(mappedName))
            {
                _xmlNameToPropertyNameMappings[type].Add(mappedName, propertyName);
                _xmlPropertyNameToXmlNameMappings[type].Add(propertyName, mappedName);
            }

            return true;
        }
    }
}
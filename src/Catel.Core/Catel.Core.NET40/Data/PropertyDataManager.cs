// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Serialization;
    using Logging;
    using Reflection;

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
        private readonly Dictionary<Type, Dictionary<string, PropertyData>> _propertyData = new Dictionary<Type, Dictionary<string, PropertyData>>();

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
        /// Gets the properties of a specific type.
        /// </summary>
        /// <param name="type">The type for which the properties to return.</param>
        /// <returns>Dictionary with the properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public Dictionary<string, PropertyData> GetProperties(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_propertyDataLock)
            {
                if (!_propertyData.ContainsKey(type))
                {
                    RegisterProperties(type);
                }

                return _propertyData[type];
            }
        }

        /// <summary>
        /// Registers all the properties for the specified type.
        /// <para />
        /// This method can only be called once per type. The <see cref="PropertyDataManager"/> caches
        /// whether it has already registered the properties once.
        /// </summary>
        /// <param name="type">The type to register the properties for.</param>
        /// <returns>The list of properties found on the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">The properties are not declared correctly.</exception>
        public IEnumerable<PropertyData> RegisterProperties(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_propertyDataLock)
            {
                if (_propertyData.ContainsKey(type))
                {
                    return _propertyData[type].Values;
                }

                var registeredPropertyData = new List<PropertyData>();

                registeredPropertyData.AddRange(FindFields(type));
                registeredPropertyData.AddRange(FindProperties(type));

                _propertyData[type] = registeredPropertyData.ToDictionary(registeredProperty => registeredProperty.Name);

                return registeredPropertyData;
            }
        }

        /// <summary>
        /// Finds the properties that represent a <see cref="PropertyData"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of <see cref="PropertyData"/> elements found as properties.</returns>
        /// <exception cref="InvalidOperationException">One ore more properties are not declared correctly.</exception>
        private static IEnumerable<PropertyData> FindProperties(Type type)
        {
            // Properties - safety checks for non-static properties
            var nonStaticProperties = (from property in type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true))
                                       where property.PropertyType == typeof(PropertyData)
                                       select property).ToList();
            foreach (var nonStaticProperty in nonStaticProperties)
            {
                string error = string.Format("The property '{0}' of type 'PropertyData' declared as instance, but they can only be used as static", nonStaticProperty.Name);

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            // Properties - safety checks for non-public fields
            var nonPublicProperties = (from property in type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true))
                                       where property.PropertyType == typeof(PropertyData) && !property.CanRead
                                       select property).ToList();
            foreach (var nonPublicProperty in nonPublicProperties)
            {
                string error = string.Format("The property '{0}' of type 'PropertyData' declared as non-public, but they can only be used as public", nonPublicProperty.Name);

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            // Properties - actual addition
            var foundProperties = new List<PropertyData>();

            var properties = new List<PropertyInfo>();
            properties.AddRange(type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, false)));
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(PropertyData))
                {
                    var propertyValue = property.GetValue(null, null) as PropertyData;
                    if (propertyValue != null)
                    {
                        foundProperties.Add(propertyValue);
                    }
                }
            }

            return foundProperties;
        }

        /// <summary>
        /// Finds the fields that represent a <see cref="PropertyData"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of <see cref="PropertyData"/> elements found as fields.</returns>
        /// <exception cref="InvalidOperationException">One ore more fields are not declared correctly.</exception>
        private IEnumerable<PropertyData> FindFields(Type type)
        {
            // Fields - safety checks for non-static fields
            var nonStaticFields = (from field in type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true))
                                   where field.FieldType == typeof(PropertyData)
                                   select field).ToList();
            foreach (var nonStaticField in nonStaticFields)
            {
                string error = string.Format("The field '{0}' of type 'PropertyData' declared as instance, but they can only be used as static", nonStaticField.Name);

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            // Fields - safety checks for non-public fields
            var nonPublicFields = (from field in type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true))
                                   where field.FieldType == typeof(PropertyData) && !field.IsPublic
                                   select field).ToList();
            foreach (var nonPublicField in nonPublicFields)
            {
                string error = string.Format("The field '{0}' of type 'PropertyData' declared as non-public, but they can only be used as public", nonPublicField.Name);

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            // Fields - actual addition
            var foundFields = new List<PropertyData>();

            var fields = new List<FieldInfo>();
            fields.AddRange(type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, false)));
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(PropertyData))
                {
                    var propertyValue = (field.IsStatic ? field.GetValue(null) : field.GetValue(this)) as PropertyData;
                    if (propertyValue != null)
                    {
                        foundFields.Add(propertyValue);
                    }
                }
            }

            return foundFields;
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
                if (!_propertyData.ContainsKey(type))
                {
                    _propertyData.Add(type, new Dictionary<string, PropertyData>());
                }

                var propertyDataItem = _propertyData[type];
                if (propertyDataItem.ContainsKey(name))
                {
                    throw new PropertyAlreadyRegisteredException(name, type);
                }

                propertyDataItem.Add(name, propertyData);
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
                if (_propertyData.ContainsKey(type))
                {
                    return _propertyData[type].ContainsKey(name);
                }

                return false;
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

            if (!IsPropertyRegistered(type, name))
            {
                throw new PropertyNotRegisteredException(name, type);
            }

            lock (_propertyDataLock)
            {
                return _propertyData[type][name];
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
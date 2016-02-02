// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyDataTypeInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Caching;
    using Catel.Logging;
    using Catel.Reflection;
    using Collections;

    /// <summary>
    /// Class containing all information about a Catel type (such as properties).
    /// </summary>
    public class CatelTypeInfo
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields
        private readonly object _lockObject = new object();

        private readonly IDictionary<string, PropertyData> _catelProperties = new ListDictionary<string, PropertyData>();
        private readonly IDictionary<string, CachedPropertyInfo> _nonCatelProperties = new ListDictionary<string, CachedPropertyInfo>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CatelTypeInfo" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public CatelTypeInfo(Type type)
        {
            Argument.IsNotNull("type", type);

            Type = type;

            RegisterProperties();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RegisterProperties"/> method has been called at least once.
        /// </summary>
        /// <value><c>true</c> if the <see cref="RegisterProperties"/> method has been called at least once; otherwise, <c>false</c>.</value>
        public bool IsRegisterPropertiesCalled { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the Catel properties.
        /// </summary>
        /// <returns>Dictionary containing the Catel properties.</returns>
        public IDictionary<string, PropertyData> GetCatelProperties()
        {
            // Clone or not to clone? For performance reasons decided not to
            return _catelProperties;
        }

        /// <summary>
        /// Gets the non-Catel properties.
        /// </summary>
        /// <returns>Dictionary containing the non-Catel properties.</returns>
        public IDictionary<string, CachedPropertyInfo> GetNonCatelProperties()
        {
            // Clone or not to clone? For performance reasons decided not to
            return _nonCatelProperties;
        }

        /// <summary>
        /// Gets the property data.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The <see cref="PropertyData"/> of the requested property.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyNotRegisteredException">Thrown when the property is not registered.</exception>
        public PropertyData GetPropertyData(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_lockObject)
            {
                PropertyData catelProperty;
                if (!_catelProperties.TryGetValue(name, out catelProperty))
                {
                    throw Log.ErrorAndCreateException(msg => new PropertyNotRegisteredException(name, Type),
                    "Property '{0}' on type '{1}' is not registered", name, Type.FullName);
                }

                return catelProperty;
            }
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public bool IsPropertyRegistered(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_lockObject)
            {
                return _catelProperties.ContainsKey(name);
            }
        }

        /// <summary>
        /// Registers all the properties for the specified type.
        /// <para/>
        /// This method can only be called once per type. The <see cref="PropertyDataManager"/> caches
        /// whether it has already registered the properties once.
        /// </summary>
        /// <exception cref="InvalidOperationException">The properties are not declared correctly.</exception>
        public void RegisterProperties()
        {
            lock (_lockObject)
            {
                if (IsRegisterPropertiesCalled)
                {
                    return;
                }

                var catelProperties = new List<PropertyData>();
                catelProperties.AddRange(FindCatelFields(Type));
                catelProperties.AddRange(FindCatelProperties(Type));
                foreach (var propertyData in catelProperties)
                {
                    _catelProperties[propertyData.Name] = propertyData;
                }

                var nonCatelProperties = FindNonCatelProperties(Type);
                foreach (var property in nonCatelProperties)
                {
                    _nonCatelProperties[property.Name] = new CachedPropertyInfo(property);
                }

                IsRegisterPropertiesCalled = true;
            }
        }

        /// <summary>
        /// Registers a property for a specific type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="propertyData">The property data.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyData"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">A property with the same name is already registered.</exception>
        public void RegisterProperty(string name, PropertyData propertyData)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("propertyData", propertyData);

            lock (_lockObject)
            {
                if (_catelProperties.ContainsKey(name))
                {
                    throw Log.ErrorAndCreateException(msg => new PropertyAlreadyRegisteredException(name, Type),
                        "Property '{0}' on type '{1}' is already registered", name, Type.FullName);
                }

                _catelProperties[name] = propertyData;
            }
        }

        /// <summary>
        /// Unregisters a property for a specific type.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        public void UnregisterProperty(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_lockObject)
            {
                if (_catelProperties.ContainsKey(name))
                {
                    _catelProperties.Remove(name);
                }
            }
        }

        /// <summary>
        /// Finds the non catel properties.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of <see cref="PropertyInfo"/> elements found as properties.</returns>
        private static IEnumerable<PropertyInfo> FindNonCatelProperties(Type type)
        {
            return (from property in type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true))
                    where property.PropertyType != typeof(PropertyData)
                    select property).ToList();
        }

        /// <summary>
        /// Finds the properties that represent a <see cref="PropertyData"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of <see cref="PropertyData"/> elements found as properties.</returns>
        /// <exception cref="InvalidOperationException">One ore more properties are not declared correctly.</exception>
        private static IEnumerable<PropertyData> FindCatelProperties(Type type)
        {
            // Properties - safety checks for non-static properties
            var nonStaticProperties = (from property in type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true))
                                       where property.PropertyType == typeof(PropertyData)
                                       select property).ToList();
            foreach (var nonStaticProperty in nonStaticProperties)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The property '{0}' of type 'PropertyData' declared as instance, but they can only be used as static", nonStaticProperty.Name);
            }

            // Properties - safety checks for non-public fields
            var nonPublicProperties = (from property in type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true))
                                       where property.PropertyType == typeof(PropertyData) && !property.CanRead
                                       select property).ToList();
            foreach (var nonPublicProperty in nonPublicProperties)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The property '{0}' of type 'PropertyData' declared as non-public, but they can only be used as public", nonPublicProperty.Name);
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
        private IEnumerable<PropertyData> FindCatelFields(Type type)
        {
            // CTL-212: Generic types are not supported for FieldInfo.GetValue
            if (type.ContainsGenericParametersEx())
            {
                return new PropertyData[] { };
            }

            PreventWrongDeclaredFields(type);

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

        private void PreventWrongDeclaredFields(Type type)
        {
            // Fields - safety checks for non-static fields
            var nonStaticFields = (from field in type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, false, true))
                                   where field.FieldType == typeof(PropertyData)
                                   select field).ToList();
            foreach (var nonStaticField in nonStaticFields)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The field '{0}' of type 'PropertyData' declared as instance, but they can only be used as static", nonStaticField.Name);
            }

            // Fields - safety checks for non-public fields
            var nonPublicFields = (from field in type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(true, true, true))
                                   where field.FieldType == typeof(PropertyData) && !field.IsPublic
                                   select field).ToList();
            foreach (var nonPublicField in nonPublicFields)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("The field '{0}' of type 'PropertyData' declared as non-public, but they can only be used as public", nonPublicField.Name);
            }
        }
        #endregion
    }
}
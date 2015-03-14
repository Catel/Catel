// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationModelCache.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Catel.Data;
    using Catel.Reflection;

    /// <summary>
    /// Class that contains info about serializable models.
    /// </summary>
    public class SerializationModelInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationModelInfo"/> class.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="catelPropertyNames">The catel property names.</param>
        /// <param name="fieldNames">The fields.</param>
        /// <param name="propertyNames">The properties.</param>
        public SerializationModelInfo(Type modelType, HashSet<string> catelPropertyNames, HashSet<string> fieldNames, HashSet<string> propertyNames)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("catelPropertyNames", catelPropertyNames);
            Argument.IsNotNull("fields", fieldNames);
            Argument.IsNotNull("properties", propertyNames);

            ModelType = modelType;

            var catelTypeInfo = PropertyDataManager.Default.GetCatelTypeInfo(modelType);

            CatelPropertyNames = catelPropertyNames;
            CatelProperties = new List<PropertyData>();
            CatelPropertiesByName = new Dictionary<string, PropertyData>();
            foreach (var catelPropertyName in catelPropertyNames)
            {
                var propertyData = catelTypeInfo.GetPropertyData(catelPropertyName);

                CatelProperties.Add(propertyData);
                CatelPropertiesByName[propertyData.Name] = propertyData;
            }

            FieldNames = fieldNames;
            Fields = new List<FieldInfo>();
            FieldsByName = new Dictionary<string, FieldInfo>();
            foreach (var fieldName in fieldNames)
            {
                var fieldInfo = modelType.GetFieldEx(fieldName);

                Fields.Add(fieldInfo);
                FieldsByName[fieldName] = fieldInfo;
            }

            PropertyNames = propertyNames;
            Properties = new List<PropertyInfo>();
            PropertiesByName = new Dictionary<string, PropertyInfo>();
            foreach (var propertyName in propertyNames)
            {
                var propertyInfo = modelType.GetPropertyEx(propertyName);

                Properties.Add(propertyInfo);
                PropertiesByName[propertyName] = propertyInfo;
            }
        }

        /// <summary>
        /// Gets the type of the model.
        /// </summary>
        /// <value>The type of the model.</value>
        public Type ModelType { get; private set; }

        /// <summary>
        /// Gets the catel property names.
        /// </summary>
        /// <value>The catel property names.</value>
        public HashSet<string> CatelPropertyNames { get; private set; } 

        /// <summary>
        /// Gets the catel properties.
        /// </summary>
        /// <value>The catel properties.</value>
        public List<PropertyData> CatelProperties { get; private set; }

        /// <summary>
        /// Gets the Catel properties by name.
        /// </summary>
        /// <value>The Catel properties by name.</value>
        public Dictionary<string, PropertyData> CatelPropertiesByName { get; private set; } 

        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <value>The field names.</value>
        public HashSet<string> FieldNames { get; private set; } 

        /// <summary>
        /// Gets the fields.
        /// </summary>
        /// <value>The fields.</value>
        public List<FieldInfo> Fields { get; private set; }

        /// <summary>
        /// Gets the fields by name.
        /// </summary>
        /// <value>The fields by name.</value>
        public Dictionary<string, FieldInfo> FieldsByName { get; private set; } 

        /// <summary>
        /// Gets the property names.
        /// </summary>
        /// <value>The property names.</value>
        public HashSet<string> PropertyNames { get; private set; } 

        /// <summary>
        /// Gets the properties.
        /// </summary>
        /// <value>The properties.</value>
        public List<PropertyInfo> Properties { get; private set; }

        /// <summary>
        /// Gets the properties by name.
        /// </summary>
        /// <value>The properties by name.</value>
        public Dictionary<string, PropertyInfo> PropertiesByName { get; private set; } 
    }
}
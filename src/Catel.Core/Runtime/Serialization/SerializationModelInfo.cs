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
        /// <param name="catelProperties">The catel properties.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="regularProperties">The properties.</param>
        public SerializationModelInfo(Type modelType, Dictionary<string, MemberMetadata> catelProperties, Dictionary<string, MemberMetadata> fields,
            Dictionary<string, MemberMetadata> regularProperties)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("catelProperties", catelProperties);
            Argument.IsNotNull("fields", fields);
            Argument.IsNotNull("properties", regularProperties);

            ModelType = modelType;

            CatelTypeInfo catelTypeInfo = null;

            CatelPropertyNames = new HashSet<string>(catelProperties.Keys);
            CatelProperties = new List<PropertyData>();
            CatelPropertiesByName = catelProperties;
            foreach (var catelProperty in catelProperties)
            {
                var propertyData = catelProperty.Value.Tag as PropertyData;
                if (propertyData is null)
                {
                    if (catelTypeInfo is null)
                    {
                        catelTypeInfo = PropertyDataManager.Default.GetCatelTypeInfo(modelType);
                    }

                    propertyData = catelTypeInfo.GetPropertyData(catelProperty.Key);
                }

                CatelProperties.Add(propertyData);
            }

            FieldNames = new HashSet<string>(fields.Keys);
            Fields = new List<FieldInfo>();
            FieldsByName = fields;
            foreach (var field in fields)
            {
                var fieldInfo = field.Value.Tag as FieldInfo;
                if (fieldInfo is null)
                {
                    fieldInfo = modelType.GetFieldEx(field.Key);
                }

                Fields.Add(fieldInfo);
            }

            PropertyNames = new HashSet<string>(regularProperties.Keys);
            Properties = new List<PropertyInfo>();
            PropertiesByName = regularProperties;
            foreach (var regularProperty in regularProperties)
            {
                var propertyInfo = regularProperty.Value.Tag as PropertyInfo;
                if (propertyInfo is null)
                {
                    propertyInfo = modelType.GetPropertyEx(regularProperty.Key);
                }

                Properties.Add(propertyInfo);
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
        public Dictionary<string, MemberMetadata> CatelPropertiesByName { get; private set; } 

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
        public Dictionary<string, MemberMetadata> FieldsByName { get; private set; } 

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
        public Dictionary<string, MemberMetadata> PropertiesByName { get; private set; } 
    }
}

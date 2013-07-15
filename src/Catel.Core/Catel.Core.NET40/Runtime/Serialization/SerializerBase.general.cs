// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.general.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using Data;
    using Logging;

    // A solution might be:
    //   - Serialize(stream) => serializes a complete object, virtual
    //   - SerializeProperties(stream) => serializes all properties and calls GetPropertiesToSerialize, virtual
    //   - SerializeProperties(TSerializationContext) => serializes the specified properties, created by SerializeProperties, abstract
    //   - SerializeProperty => serializes a single property with option to customize, virtual

    /// <summary>
    /// Base class for serializers that can serializer the <see cref="ModelBase" />.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the T serialization context.</typeparam>
    public abstract partial class SerializerBase<TSerializationContext> : IModelBaseSerializer<TSerializationContext>
        where TSerializationContext : class
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Determines whether the specified property on the specified model should be ignored by the serialization engine.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="property">The property.</param>
        /// <returns><c>true</c> if the property should be ignored, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldIgnoreProperty(ModelBase model, PropertyData property)
        {
            return false;
        }

        /// <summary>
        /// Gets the serializable properties for the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertiesToIgnore">The properties to ignore.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public virtual List<PropertyValue> GetSerializableProperties(ModelBase model, params string[] propertiesToIgnore)
        {
            Argument.IsNotNull("model", model);

            return ConvertDictionaryToListAndExcludeNonSerializableObjects(model, propertiesToIgnore);
        }

        /// <summary>
        /// Converts the list to a dictionary.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>Dictionary of property values.</returns>
        internal Dictionary<string, object> ConvertListToDictionary(IEnumerable<PropertyValue> list)
        {
            return ConvertListToDictionary(GetType(), list);
        }

        /// <summary>
        /// Converts the list to a dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="list">The list.</param>
        /// <returns>Dictionary of property values.</returns>
        private static Dictionary<string, object> ConvertListToDictionary(Type type, IEnumerable<PropertyValue> list)
        {
            var result = new Dictionary<string, object>();

            var propertyDataManager = PropertyDataManager.Default;

            foreach (var propertyValue in list)
            {
                if (propertyDataManager.IsPropertyRegistered(type, propertyValue.Name))
                {
#if NET
                    if (propertyValue.Value != null)
                    {
                        var propertyValueAsIDeserializationCallback = propertyValue.Value as IDeserializationCallback;
                        if ((propertyValueAsIDeserializationCallback != null) && (propertyValue.Value is ICollection))
                        {
                            // We are allowed to pass null, see remarks in OnDeserialization defined in this class
                            propertyValueAsIDeserializationCallback.OnDeserialization(null);
                        }
                    }
#endif

                    // Store the value (since deserialized values always override default values)
                    result[propertyValue.Name] = propertyValue.Value;
                }
            }

            return result;
        }

        /// <summary>
        /// Converts the dictionary to list and exclude non serializable objects.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertiesToIgnore">The properties to ignore.</param>
        /// <returns>List{PropertyValue}.</returns>
        private List<PropertyValue> ConvertDictionaryToListAndExcludeNonSerializableObjects(ModelBase model, params string[] propertiesToIgnore)
        {
            var propertiesToIgnoreHashSet = new HashSet<string>(propertiesToIgnore);

            var modelType = model.GetType();
            var listToSerialize = new List<PropertyValue>();

            var propertyDataManager = PropertyDataManager.Default;

            foreach (var dictionaryItem in model._propertyBag.GetAllProperties())
            {
                var propertyData = propertyDataManager.GetPropertyData(modelType, dictionaryItem.Key);
                if (!propertyData.IsSerializable)
                {
                    Log.Warning("Property '{0}' is not serializable, so will be excluded from the serialization", propertyData.Name);
                    continue;
                }

                if (!propertyData.IncludeInSerialization)
                {
                    Log.Debug("Property '{0}' is flagged to be excluded from serialization", propertyData.Name);
                    continue;
                }

                if (propertiesToIgnoreHashSet.Contains(propertyData.Name))
                {
                    Log.Info("Property '{0}' is being ignored for serialization", propertyData.Name);
                    continue;
                }

                if (ShouldIgnoreProperty(model, propertyData))
                {
                    Log.Info("Property '{0}' is being ignored for serialization", propertyData.Name);
                    continue;
                }

#if NET
                var collection = dictionaryItem.Value as ICollection;
                if (collection != null)
                {
                    bool validCollection = true;

                    foreach (var item in collection)
                    {
                        if ((item != null) && (item.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length == 0))
                        {
                            validCollection = false;
                            break;
                        }
                    }

                    if (!validCollection)
                    {
                        Log.Debug("Property '{0}' is a collection containing non-serializable objects, so will be excluded from serialization", propertyData.Name);
                        continue;
                    }
                }
#endif

                Log.Debug("Adding property '{0}' to list of objects to serialize", propertyData.Name);

                var actualValue = dictionaryItem.Value;
                var serializingValue = model.GetPropertyValueForSerialization(propertyData, actualValue);

                listToSerialize.Add(new PropertyValue(propertyData, propertyData.Name, serializingValue));
            }

            return listToSerialize;
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>ISerializationContext{`0}.</returns>
        protected ISerializationContext<TSerializationContext> GetContext(ModelBase model, TSerializationContext context)
        {
            return new SerializationContext<TSerializationContext>(model, context);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>ISerializationContext{`0}.</returns>
        protected abstract ISerializationContext<TSerializationContext> GetContext(ModelBase model, Stream stream);

        /// <summary>
        /// Populates the model with the specified properties.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="properties">The properties.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>properties</c>.</exception>
        protected virtual void PopulateModel(ModelBase model, params PropertyValue[] properties)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("properties", properties);

            foreach (var property in properties)
            {
                model.SetValue(property.Name, property.Value);
            }
        }
    }
}
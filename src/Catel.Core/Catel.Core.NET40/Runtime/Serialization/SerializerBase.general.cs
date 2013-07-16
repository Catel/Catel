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
    using Catel.IoC;
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

                    //foreach (var item in collection)
                    //{
                    //    if ((item != null) && (item.GetType().GetCustomAttributes(typeof(SerializableAttribute), true).Length == 0))
                    //    {
                    //        validCollection = false;
                    //        break;
                    //    }
                    //}

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
        /// Gets the context for the specified model type.
        /// <para />
        /// Use this method when no model instance is available. This method will create one.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContext> GetContext(Type modelType, TSerializationContext context)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", context);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);
            return GetContext(model, context);
        }

        /// <summary>
        /// Gets the context for the specified model type.
        /// <para />
        /// Use this method when no model instance is available. This method will create one.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContext> GetContext(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);
            return GetContext(model, stream);
        }

        /// <summary>
        /// Gets the context for the specified model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContext> GetContext(ModelBase model, TSerializationContext context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            return new SerializationContext<TSerializationContext>(model, context);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        protected abstract ISerializationContext<TSerializationContext> GetContext(ModelBase model, Stream stream);

        /// <summary>
        /// Appends the serialization context to the specified stream. This way each serializer can handle the serialization
        /// its own way and write the contents to the stream via this method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        protected abstract void AppendContextToStream(ISerializationContext<TSerializationContext> context, Stream stream);

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
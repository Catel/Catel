// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.deserialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Data;
    using IoC;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContext>
    {
        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void BeforeDeserialization(ISerializationContext<TSerializationContext> context)
        {
        }

        /// <summary>
        /// Called before the serializer starts deserializing a specific property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected virtual void BeforeDeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        /// <summary>
        /// Deserializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The deserialized property value.</returns>
        protected abstract object DeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue);

        /// <summary>
        /// Called after the serializer has deserialized a specific property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected virtual void AfterDeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        /// <summary>
        /// Called after the serializer has deserialized an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void AfterDeserialization(ISerializationContext<TSerializationContext> context)
        {
        }

        #region Methods
        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public virtual void Deserialize(ModelBase model, Stream stream)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(model, stream);

            Deserialize(model, context.Context);
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        public virtual void Deserialize(ModelBase model, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", serializationContext);

            var finalContext = GetContext(model, serializationContext);

            BeforeDeserialization(finalContext);

            DeserializeProperties(finalContext);

            AfterSerialization(finalContext);
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized <see cref="ModelBase"/>.</returns>
        public virtual ModelBase Deserialize(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(null, stream);

            return Deserialize(modelType, context.Context);
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <returns>The deserialized <see cref="ModelBase"/>.</returns>
        public virtual ModelBase Deserialize(Type modelType, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializationContext);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);

            Deserialize(model, serializationContext);

            return model;
        }

        /// <summary>
        /// Deserializes the properties.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized list of property values.</returns>
        public virtual List<PropertyValue> DeserializeProperties(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(null, stream);

            return DeserializeProperties(modelType, context.Context);
        }

        /// <summary>
        /// Deserializes the properties.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializedContext">The serialized context.</param>
        /// <returns>The deserialized list of property values.</returns>
        public virtual List<PropertyValue> DeserializeProperties(Type modelType, TSerializationContext serializedContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializedContext);

            var model = (ModelBase) TypeFactory.Default.CreateInstance(modelType);
            var finalContext = GetContext(model, serializedContext);

            return DeserializeProperties(finalContext);
        }

        /// <summary>
        /// Deserializes the properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of property values.</returns>
        protected virtual List<PropertyValue> DeserializeProperties(ISerializationContext<TSerializationContext> context)
        {
            var deserializedPropertyValues = new List<PropertyValue>();

            var propertiesToDeserialize = GetSerializableProperties(context.Model);
            foreach (var property in propertiesToDeserialize)
            {
                BeforeDeserializeProperty(context, property);

                var deserializedPropertyValue = DeserializeProperty(context, property);
                var propertyValue = new PropertyValue(property.PropertyData, property.Name, deserializedPropertyValue);
                deserializedPropertyValues.Add(propertyValue);

                PopulateModel(context.Model, propertyValue);

                AfterDeserializeProperty(context, property);
            }

            return deserializedPropertyValues;
        }
        #endregion
    }
}
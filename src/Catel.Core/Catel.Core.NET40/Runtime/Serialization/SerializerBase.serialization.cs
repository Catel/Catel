// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Runtime.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using Catel.Data;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContext>
    {
        #region IModelBaseSerializer<TSerializationContext> Members
        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public virtual void Serialize(ModelBase model, Stream stream)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(model, stream);

            Serialize(model, context.Context);
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        public virtual void Serialize(ModelBase model, TSerializationContext context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            var finalContext = GetContext(model, context);

            BeforeSerialization(finalContext);

            var properties = GetSerializableProperties(model);
            SerializeProperties(finalContext, properties);

            AfterSerialization(finalContext);
        }

        /// <summary>
        /// Serializes the properties.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="propertiesToIgnore">The properties to ignore.</param>
        public virtual void SerializeProperties(ModelBase model, Stream stream, params string[] propertiesToIgnore)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var properties = GetSerializableProperties(model, propertiesToIgnore);
            if (properties.Count == 0)
            {
                return;
            }

            var context = GetContext(model, stream);

            SerializeProperties(context, properties);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void BeforeSerialization(ISerializationContext<TSerializationContext> context)
        {
        }

        /// <summary>
        /// Called before the serializer starts serializing a specific property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected virtual void BeforeSerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The deserialized property value.</returns>
        protected abstract void SerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue);

        /// <summary>
        /// Called after the serializer has serialized a specific property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected virtual void AfterSerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        /// <summary>
        /// Called after the serializer has serialized an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void AfterSerialization(ISerializationContext<TSerializationContext> context)
        {
        }

        /// <summary>
        /// Serializes the properties.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertiesToSerialize">The properties to serialize.</param>
        protected virtual void SerializeProperties(ISerializationContext<TSerializationContext> context, List<PropertyValue> propertiesToSerialize)
        {
            foreach (var property in propertiesToSerialize)
            {
                BeforeSerializeProperty(context, property);

                SerializeProperty(context, property);

                AfterSerializeProperty(context, property);
            }
        }
        #endregion
    }
}
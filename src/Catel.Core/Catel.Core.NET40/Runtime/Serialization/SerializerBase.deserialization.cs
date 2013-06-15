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

    public partial class SerializerBase<TSerializationContext>
    {
        protected virtual void BeforeDeserialization(ISerializationContext<TSerializationContext> context)
        {
        }

        protected virtual void BeforeDeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        protected abstract object DeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue);

        protected virtual void AfterDeserializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        protected virtual void AfterDeserialization(ISerializationContext<TSerializationContext> context)
        {
        }

        #region Methods
        public virtual void Deserialize(ModelBase model, Stream stream)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(model, stream);

            Deserialize(model, context.Context);
        }

        public virtual void Deserialize(ModelBase model, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", serializationContext);

            var finalContext = GetContext(model, serializationContext);

            BeforeDeserialization(finalContext);

            DeserializeProperties(finalContext);

            AfterSerialization(finalContext);
        }

        public virtual ModelBase Deserialize(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(null, stream);

            return Deserialize(modelType, context.Context);
        }

        public virtual ModelBase Deserialize(Type modelType, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializationContext);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);

            Deserialize(model, serializationContext);

            return model;
        }

        public virtual List<PropertyValue> DeserializeProperties(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(null, stream);

            return DeserializeProperties(modelType, context.Context);
        }

        public virtual List<PropertyValue> DeserializeProperties(Type modelType, TSerializationContext serializedContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializedContext);

            var model = (ModelBase) TypeFactory.Default.CreateInstance(modelType);
            var finalContext = GetContext(model, serializedContext);

            return DeserializeProperties(finalContext);
        }

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
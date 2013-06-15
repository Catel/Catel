// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using Data;

    public partial class SerializerBase<TSerializationContext>
    {
        protected virtual void BeforeSerialization(ISerializationContext<TSerializationContext> context)
        {
        }

        protected virtual void BeforeSerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        protected abstract void SerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue);

        protected virtual void AfterSerializeProperty(ISerializationContext<TSerializationContext> context, PropertyValue propertyValue)
        {
        }

        protected virtual void AfterSerialization(ISerializationContext<TSerializationContext> context)
        {
        }

        public virtual void Serialize(ModelBase model, Stream stream)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(model, stream);

            Serialize(model, context.Context);
        }

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

        protected virtual void SerializeProperties(ISerializationContext<TSerializationContext> context, List<PropertyValue> propertiesToSerialize)
        {
            foreach (var property in propertiesToSerialize)
            {
                BeforeSerializeProperty(context, property);

                SerializeProperty(context, property);

                AfterSerializeProperty(context, property);
            }
        }
    }
}
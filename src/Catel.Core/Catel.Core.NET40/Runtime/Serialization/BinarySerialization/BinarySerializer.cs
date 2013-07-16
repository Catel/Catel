// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using Data;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class BinarySerializer : SerializerBase<SerializationInfo>, IBinarySerializer
    {
        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected override void SerializeProperty(ISerializationContext<SerializationInfo> context, PropertyValue propertyValue)
        {
            var serializationInfo = context.Context;

            serializationInfo.AddValue(propertyValue.Name, propertyValue);
        }

        /// <summary>
        /// Deserializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeProperty(ISerializationContext<SerializationInfo> context, PropertyValue propertyValue)
        {
            var serializationInfo = context.Context;

            var finalPropertyValue = (PropertyValue)serializationInfo.GetValue(propertyValue.Name, typeof(PropertyValue));
            return SerializationObject.SucceededToDeserialize(context.ModelType, propertyValue.Name, finalPropertyValue.Value);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>ISerializationContext{SerializationInfo}.</returns>
        protected override ISerializationContext<SerializationInfo> GetContext(ModelBase model, Stream stream)
        {
            var serializationInfo = new SerializationInfo(model.GetType(), new FormatterConverter());
            return new SerializationContext<SerializationInfo>(model, serializationInfo);
        }

        /// <summary>
        /// Appends the context to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void AppendContextToStream(ISerializationContext<SerializationInfo> context, Stream stream)
        {
            // Not required yet


            


            //binaryFormatter.Serialize(stream, context.Context);
        }
    }
}
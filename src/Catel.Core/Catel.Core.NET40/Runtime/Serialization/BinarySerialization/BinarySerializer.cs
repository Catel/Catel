// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System.IO;
    using System.Runtime.Serialization;
    using Data;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class BinarySerializer : SerializerBase<SerializationInfo>, IBinarySerializer
    {
        protected override void SerializeProperty(ISerializationContext<SerializationInfo> context, PropertyValue propertyValue)
        {
            var serializationInfo = context.Context;

            serializationInfo.AddValue(propertyValue.Name, propertyValue);
        }

        protected override object DeserializeProperty(ISerializationContext<SerializationInfo> context, PropertyValue propertyValue)
        {
            var serializationInfo = context.Context;

            var finalPropertyValue = (PropertyValue)serializationInfo.GetValue(propertyValue.Name, typeof(PropertyValue));
            return finalPropertyValue.Value;
        }

        protected override ISerializationContext<SerializationInfo> GetContext(ModelBase model, Stream stream)
        {
            var serializationInfo = new SerializationInfo(model.GetType(), new FormatterConverter());
            return new SerializationContext<SerializationInfo>(model, serializationInfo);
        }
    }
}
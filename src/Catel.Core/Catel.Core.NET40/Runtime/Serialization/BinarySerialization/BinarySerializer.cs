// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using Catel.IoC;
    using Catel.Logging;
    using Data;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class BinarySerializer : SerializerBase<BinarySerializationContextInfo>, IBinarySerializer
    {
        /// <summary>
        /// The property values key.
        /// </summary>
        private const string PropertyValuesKey = "PropertyValues";

        /// <summary>
        /// The deserialization binder with redirect support.
        /// </summary>
        private static RedirectDeserializationBinder _deserializationBinder;

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <remarks>
        /// When deserializing a stream, the binary serializer must use the <see cref="BinaryFormatter"/> because this will
        /// inject the right <see cref="SerializationInfo"/> into a new serializer.
        /// </remarks>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>ModelBase.</returns>
        public override ModelBase Deserialize(Type modelType, Stream stream)
        {
            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);

            Deserialize(model, stream);

            return model;
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <remarks>
        /// When deserializing a stream, the binary serializer must use the <see cref="BinaryFormatter"/> because this will
        /// inject the right <see cref="SerializationInfo"/> into a new serializer.
        /// </remarks>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        public override void Deserialize(ModelBase model, Stream stream)
        {
            var binaryFormatter = CreateBinaryFormatter(SerializationContextMode.Deserialization);

            var propertyValues = (List<PropertyValue>)binaryFormatter.Deserialize(stream);
            var context = GetContext(model, stream, SerializationContextMode.Deserialization, propertyValues);

            Deserialize(model, context.Context);
        }

        /// <summary>
        /// Serializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        protected override void SerializeProperty(ISerializationContext<BinarySerializationContextInfo> context, PropertyValue propertyValue)
        {
            var serializationContext = context.Context;
            var propertyValues = serializationContext.PropertyValues;

            propertyValues.Add(propertyValue);
        }

        /// <summary>
        /// Deserializes the property.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeProperty(ISerializationContext<BinarySerializationContextInfo> context, PropertyValue propertyValue)
        {
            var serializationContext = context.Context;
            var propertyValues = serializationContext.PropertyValues;

            var finalPropertyValue = (from x in propertyValues
                                      where string.Equals(x.Name, propertyValue.Name, StringComparison.Ordinal)
                                      select x).FirstOrDefault();

            if (finalPropertyValue != null)
            {
                return SerializationObject.SucceededToDeserialize(context.ModelType, propertyValue.Name, finalPropertyValue.Value);
            }

            return SerializationObject.FailedToDeserialize(context.ModelType, propertyValue.Name);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>ISerializationContext{SerializationInfo}.</returns>
        protected override ISerializationContext<BinarySerializationContextInfo> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode)
        {
            return GetContext(model, stream, contextMode, null);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="propertyValues">The property values.</param>
        /// <returns>The serialization context..</returns>
        private ISerializationContext<BinarySerializationContextInfo> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode, List<PropertyValue> propertyValues)
        {
            var serializationInfo = new SerializationInfo(model.GetType(), new FormatterConverter());
            var binaryFormatter = CreateBinaryFormatter(contextMode);

            if (propertyValues == null)
            {
                propertyValues = new List<PropertyValue>();
            }

            var contextInfo = new BinarySerializationContextInfo(serializationInfo, binaryFormatter, propertyValues);

            return new SerializationContext<BinarySerializationContextInfo>(model, contextInfo, contextMode);
        }

        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeDeserialization(ISerializationContext<BinarySerializationContextInfo> context)
        {
            // We need to deserialize the list of properties manually
            var serializationContext = context.Context;
            var serializationInfo = serializationContext.SerializationInfo;

            if (serializationContext.PropertyValues.Count > 0)
            {
                // Already done, this is probably a top-level object in the binary deserialization
                return;
            }

            try
            {
                var propertyValues = (List<PropertyValue>)serializationInfo.GetValue(PropertyValuesKey, typeof(List<PropertyValue>));
                serializationContext.PropertyValues.AddRange(propertyValues);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to deserialize list of property values for object '{0}'", context.ModelType.FullName);
            }
        }

        /// <summary>
        /// Called after the serializer has serialized an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void AfterSerialization(ISerializationContext<BinarySerializationContextInfo> context)
        {
            // We need to add the serialized property values to the serialization info manually here
            var serializationContext = context.Context;
            var serializationInfo = serializationContext.SerializationInfo;
            var propertyValues = serializationContext.PropertyValues;

            serializationInfo.AddValue(PropertyValuesKey, propertyValues);
        }

        /// <summary>
        /// Appends the context to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void AppendContextToStream(ISerializationContext<BinarySerializationContextInfo> context, Stream stream)
        {
            var serializationContext = context.Context;
            var propertyValues = serializationContext.PropertyValues;
            var binaryFormatter = serializationContext.BinaryFormatter;

            binaryFormatter.Serialize(stream, propertyValues);
        }

        /// <summary>
        /// Configures the binary formatter.
        /// </summary>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The binary formatter.</returns>
        private BinaryFormatter CreateBinaryFormatter(SerializationContextMode contextMode)
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            binaryFormatter.FilterLevel = TypeFilterLevel.Full;
            binaryFormatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;

            if (contextMode == SerializationContextMode.Deserialization)
            {
                if (_deserializationBinder == null)
                {
                    _deserializationBinder = new RedirectDeserializationBinder();
                }

                binaryFormatter.Binder = _deserializationBinder;
            }

            return binaryFormatter;
        }
    }
}
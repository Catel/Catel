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
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class BinarySerializer : SerializerBase<BinarySerializationContextInfo>, IBinarySerializer
    {
        #region Constants
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
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BinarySerializer" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        public BinarySerializer(ISerializationManager serializationManager)
            : base(serializationManager)
        {
        }
        #endregion

        #region IBinarySerializer Members
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
            Argument.IsNotNull("modelType", modelType);

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
            Argument.IsNotNull("model", model);

            var binaryFormatter = CreateBinaryFormatter(SerializationContextMode.Deserialization);

            var propertyValues = (List<PropertyValue>)binaryFormatter.Deserialize(stream);
            var memberValues = ConvertPropertyValuesToMemberValues(model.GetType(), propertyValues);
            using (var context = GetContext(model, stream, SerializationContextMode.Deserialization, memberValues))
            {
                Deserialize(model, context.Context);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected override void SerializeMember(ISerializationContext<BinarySerializationContextInfo> context, MemberValue memberValue)
        {
            var serializationContext = context.Context;
            var memberValues = serializationContext.MemberValues;

            memberValues.Add(memberValue);
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject"/> representing the deserialized value or result.</returns>
        protected override SerializationObject DeserializeMember(ISerializationContext<BinarySerializationContextInfo> context, MemberValue memberValue)
        {
            var serializationContext = context.Context;
            var memberValues = serializationContext.MemberValues;

            var finalMembervalue = (from x in memberValues
                                    where string.Equals(x.Name, memberValue.Name, StringComparison.Ordinal)
                                    select x).FirstOrDefault();

            if (finalMembervalue != null)
            {
                return SerializationObject.SucceededToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name, finalMembervalue.Value);
            }

            return SerializationObject.FailedToDeserialize(context.ModelType, memberValue.MemberGroup, memberValue.Name);
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
        /// <param name="memberValues">The member values.</param>
        /// <returns>The serialization context.</returns>
        private ISerializationContext<BinarySerializationContextInfo> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode, List<MemberValue> memberValues)
        {
            var serializationInfo = new SerializationInfo(model.GetType(), new FormatterConverter());
            var binaryFormatter = CreateBinaryFormatter(contextMode);

            if (memberValues == null)
            {
                memberValues = new List<MemberValue>();
            }

            var contextInfo = new BinarySerializationContextInfo(serializationInfo, binaryFormatter, memberValues);

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

            if (serializationContext.MemberValues.Count > 0)
            {
                // Already done, this is probably a top-level object in the binary deserialization
                return;
            }

            try
            {
                // NOTE: this will deserialize a list of PropertyValue objects to maintain backwards compatibility!
                var propertyValues = (List<PropertyValue>)serializationInfo.GetValue(PropertyValuesKey, typeof(List<PropertyValue>));
                var memberValues = ConvertPropertyValuesToMemberValues(context.ModelType, propertyValues);
                serializationContext.MemberValues.AddRange(memberValues);
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
            var memberValues = serializationContext.MemberValues;
            var propertyValues = ConvertMemberValuesToPropertyValues(memberValues);

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
            var memberValues = serializationContext.MemberValues;
            var binaryFormatter = serializationContext.BinaryFormatter;

            // NOTE: We have to keep backwards compatibility and serialize as PropertyValues list
            var propertyValues = ConvertMemberValuesToPropertyValues(memberValues);

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

        private List<PropertyValue> ConvertMemberValuesToPropertyValues(List<MemberValue> memberValues)
        {
            var propertyValues = new List<PropertyValue>();

            foreach (var memberValue in memberValues)
            {
                var propertyValue = new PropertyValue
                {
                    Name = memberValue.Name,
                    Value = memberValue.Value
                };

                propertyValues.Add(propertyValue);
            }

            return propertyValues;
        }

        private List<MemberValue> ConvertPropertyValuesToMemberValues(Type modelType, List<PropertyValue> propertyValues)
        {
            var memberValues = new List<MemberValue>();

            foreach (var propertyValue in propertyValues)
            {
                var memberGroup = GetMemberGroup(modelType, propertyValue.Name);
                var memberType = GetMemberType(modelType, propertyValue.Name);

                var memberValue = new MemberValue(memberGroup, modelType, memberType, propertyValue.Name, propertyValue.Value);
                memberValues.Add(memberValue);
            }

            return memberValues;
        }
        #endregion
    }
}
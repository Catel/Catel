// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Runtime.Serialization.Binary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using Data;
    using IoC;
    using Logging;
    using Reflection;

    /// <summary>
    /// The binary serializer.
    /// </summary>
    public class BinarySerializer : SerializerBase<BinarySerializationContextInfo>, IBinarySerializer
    {
        #region Constants
        private const string GraphId = "GraphId";
        private const string GraphRefId = "GraphRefId";

        /// <summary>
        /// The property values key.
        /// </summary>
        private const string PropertyValuesKey = "PropertyValues";

        /// <summary>
        /// The deserialization binder with redirect support.
        /// </summary>
        protected static RedirectDeserializationBinder DeserializationBinder;

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
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="objectAdapter">The object adapter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        public BinarySerializer(ISerializationManager serializationManager, ITypeFactory typeFactory, IObjectAdapter objectAdapter)
            : base(serializationManager, typeFactory, objectAdapter)
        {
            DeserializationBinder = new RedirectDeserializationBinder();
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
        /// <returns>The serialized object.</returns>
        public override object Deserialize(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);

            // Note: although this looks like an unnecessary overload, it's required to prevent duplicate scopes

            var model = TypeFactory.CreateInstance(modelType);

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
        public override void Deserialize(object model, Stream stream)
        {
            Argument.IsNotNull("model", model);

            using (var context = (SerializationContext<BinarySerializationContextInfo>)GetContext(model, stream, SerializationContextMode.Deserialization))
            {
                var referenceManager = context.ReferenceManager;
                if (referenceManager.Count == 0)
                {
                    Log.Debug("Reference manager contains no objects yet, adding initial reference which is the first model in the graph");

                    referenceManager.GetInfo(context.Model);
                }

                var binaryFormatter = CreateBinaryFormatter(SerializationContextMode.Deserialization);
                var propertyValues = (List<PropertyValue>)binaryFormatter.Deserialize(stream);
                var memberValues = ConvertPropertyValuesToMemberValues(context, model.GetType(), propertyValues);
                context.Context.MemberValues.AddRange(memberValues);

                Deserialize(model, context.Context);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Warms up the specified type.
        /// </summary>
        /// <param name="type">The type to warmup.</param>
        protected override void Warmup(Type type)
        {
            // No additional warmup required by the binary serializer
        }

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
        protected override ISerializationContext<BinarySerializationContextInfo> GetContext(object model, Stream stream, SerializationContextMode contextMode)
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
        private ISerializationContext<BinarySerializationContextInfo> GetContext(object model, Stream stream, SerializationContextMode contextMode, List<MemberValue> memberValues)
        {
            var serializationInfo = new SerializationInfo(model.GetType(), new FormatterConverter());
            var binaryFormatter = CreateBinaryFormatter(contextMode);

            if (memberValues == null)
            {
                memberValues = new List<MemberValue>();
            }

            var contextInfo = new BinarySerializationContextInfo(serializationInfo, memberValues, binaryFormatter);

            return new SerializationContext<BinarySerializationContextInfo>(model, contextInfo, contextMode);
        }

        /// <summary>
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void BeforeSerialization(ISerializationContext<BinarySerializationContextInfo> context)
        {
            base.BeforeSerialization(context);

            var referenceManager = context.ReferenceManager;
            if (referenceManager.Count == 0)
            {
                Log.Debug("Reference manager contains no objects yet, adding initial reference which is the first model in the graph");

                referenceManager.GetInfo(context.Model);
            }
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

            // Note: the ModelBase.serialization.binary.cs also contains logic to add models to the reference manager
            // because the binary serialization is used bottom => top which is not good if we need the references.
            try
            {
                var graphId = (int)serializationInfo.GetValue(GraphId, typeof(int));
                if (graphId != 0)
                {
                    var referenceManager = context.ReferenceManager;
                    if (referenceManager.GetInfoById(graphId) == null)
                    {
                        referenceManager.RegisterManually(graphId, context.Model);
                    }
                }
            }
            catch (Exception)
            {
                // Swallow
            }

            try
            {
                if (ShouldSerializeAsCollection(context.ModelType, context.Model))
                {
                    var collection = serializationInfo.GetValue(CollectionName, context.ModelType);
                    var memberValue = new MemberValue(SerializationMemberGroup.Collection, context.ModelType, context.ModelType, CollectionName, collection);
                    serializationContext.MemberValues.Add(memberValue);
                }
                else
                {
                    // NOTE: this will deserialize a list of PropertyValue objects to maintain backwards compatibility!
                    var propertyValues = (List<PropertyValue>)serializationInfo.GetValue(PropertyValuesKey, typeof(List<PropertyValue>));
                    var memberValues = ConvertPropertyValuesToMemberValues(context, context.ModelType, propertyValues);
                    serializationContext.MemberValues.AddRange(memberValues);
                }
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
            var propertyValues = ConvertMemberValuesToPropertyValues(context, memberValues);

            serializationContext.PropertyValues.AddRange(propertyValues);

            serializationInfo.AddValue(PropertyValuesKey, propertyValues);

            var referenceManager = context.ReferenceManager;
            var referenceInfo = referenceManager.GetInfo(context.Model);
            serializationInfo.AddValue(GraphId, referenceInfo.Id);
        }

        /// <summary>
        /// Appends the context to stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        protected override void AppendContextToStream(ISerializationContext<BinarySerializationContextInfo> context, Stream stream)
        {
            var serializationContext = context.Context;
            var binaryFormatter = serializationContext.BinaryFormatter;

            // NOTE: We have to keep backwards compatibility and serialize as PropertyValues list
            var propertyValues = serializationContext.PropertyValues;

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
                binaryFormatter.Binder = DeserializationBinder;
            }

            return binaryFormatter;
        }

        private List<PropertyValue> ConvertMemberValuesToPropertyValues(ISerializationContext context, List<MemberValue> memberValues)
        {
            var propertyValues = new List<PropertyValue>();

            var referenceManager = context.ReferenceManager;

            foreach (var memberValue in memberValues)
            {
                var propertyValue = new PropertyValue
                {
                    Name = memberValue.Name,
                    Value = memberValue.Value
                };

                if (memberValue.Value != null && memberValue.Value.GetType().IsClassType())
                {
                    var referenceInfo = referenceManager.GetInfo(memberValue.Value);
                    if (referenceInfo.IsFirstUsage || memberValue.MemberGroup == SerializationMemberGroup.Collection)
                    {
                        propertyValue.GraphId = referenceInfo.Id;
                    }
                    else
                    {
                        propertyValue.GraphRefId = referenceInfo.Id;
                        propertyValue.Value = null;
                    }
                }

                propertyValues.Add(propertyValue);
            }

            return propertyValues;
        }

        private List<MemberValue> ConvertPropertyValuesToMemberValues(ISerializationContext context, Type modelType, List<PropertyValue> propertyValues)
        {
            var memberValues = new List<MemberValue>();

            var referenceManager = context.ReferenceManager;

            foreach (var propertyValue in propertyValues)
            {
                var memberGroup = GetMemberGroup(modelType, propertyValue.Name);
                var memberType = GetMemberType(modelType, propertyValue.Name);

                if (propertyValue.GraphId != 0)
                {
                    if (referenceManager.GetInfoById(propertyValue.GraphId) == null)
                    {
                        referenceManager.RegisterManually(propertyValue.GraphId, propertyValue.Value);
                    }
                }

                if (propertyValue.GraphRefId != 0)
                {
                    var graphId = propertyValue.GraphRefId;
                    var referenceInfo = referenceManager.GetInfoById(graphId);
                    if (referenceInfo == null)
                    {
                        Log.Error("Expected to find graph object with id '{0}' in ReferenceManager, but it was not found. Defaulting value for member '{1}' to null", graphId, propertyValue.Name);
                        propertyValue.Value = null;
                    }
                    else
                    {
                        propertyValue.Value = referenceInfo.Instance;
                    }
                }

                var memberValue = new MemberValue(memberGroup, modelType, memberType, propertyValue.Name, propertyValue.Value);
                memberValues.Add(memberValue);
            }

            return memberValues;
        }
        #endregion
    }
}

#endif
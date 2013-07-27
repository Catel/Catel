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
        /// Called before the serializer starts deserializing a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void BeforeDeserializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject" /> representing the deserialized value or result.</returns>
        protected abstract SerializationObject DeserializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue);

        /// <summary>
        /// Called after the serializer has deserialized a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void AfterDeserializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
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

            var context = GetContext(model, stream, SerializationContextMode.Deserialization);

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

            var finalContext = GetContext(model, serializationContext, SerializationContextMode.Deserialization);

            var previousLeanAndMeanValue = model.LeanAndMeanModel;
            model.LeanAndMeanModel = true;

            BeforeDeserialization(finalContext);

            DeserializeMembers(finalContext);

            AfterDeserialization(finalContext);

            model.FinishDeserialization();

            model.LeanAndMeanModel = previousLeanAndMeanValue;
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

            var context = GetContext(modelType, stream, SerializationContextMode.Deserialization);

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
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized list of member values.</returns>
        public virtual List<MemberValue> DeserializeMembers(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var context = GetContext(modelType, stream, SerializationContextMode.Deserialization);

            return DeserializeMembers(modelType, context.Context);
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializedContext">The serialized context.</param>
        /// <returns>The deserialized list of member values.</returns>
        public virtual List<MemberValue> DeserializeMembers(Type modelType, TSerializationContext serializedContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializedContext);

            var finalContext = GetContext(modelType, serializedContext, SerializationContextMode.Deserialization);

            return DeserializeMembers(finalContext);
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of member values.</returns>
        protected virtual List<MemberValue> DeserializeMembers(ISerializationContext<TSerializationContext> context)
        {
            var deserializedMemberValues = new List<MemberValue>();

            var membersToDeserialize = GetSerializableMembers(context.Model);
            foreach (var member in membersToDeserialize)
            {
                BeforeDeserializeMember(context, member);

                var serializationObject = DeserializeMember(context, member);
                if (serializationObject.IsSuccessful)
                {
                    var memberValue = new MemberValue(member.MemberGroup, member.ModelType, member.Type, member.Name, serializationObject.MemberValue);

                    deserializedMemberValues.Add(memberValue);

                    AfterDeserializeMember(context, member);

                    PopulateModel(context.Model, memberValue);
                }
            }

            return deserializedMemberValues;
        }
        #endregion
    }
}
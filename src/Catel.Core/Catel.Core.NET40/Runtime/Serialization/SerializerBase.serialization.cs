// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel.Data;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContext>
    {
        #region Events
        /// <summary>
        /// Occurs when an object is about to be deserialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs> Deserializing;

        /// <summary>
        /// Occurs when an object is about to deserialize a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs> DeserializingMember;

        /// <summary>
        /// Occurs when an object has just deserialized a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs> DeserializedMember;

        /// <summary>
        /// Occurs when an object has just been deserialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs> Deserialized;
        #endregion

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

            using (var context = GetContext(model, stream, SerializationContextMode.Serialization))
            {
                Serialize(model, context.Context);

                AppendContextToStream(context, stream);
            }
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

            using (var finalContext = GetContext(model, context, SerializationContextMode.Serialization))
            {
                var serializingEventArgs = new SerializationEventArgs(finalContext);

                Serializing.SafeInvoke(this, serializingEventArgs);

                BeforeSerialization(finalContext);

                var members = GetSerializableMembers(model);
                SerializeMembers(finalContext, members);

                AfterSerialization(finalContext);

                Serialized.SafeInvoke(this, serializingEventArgs);
            }
        }

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        public virtual void SerializeMembers(ModelBase model, Stream stream, params string[] membersToIgnore)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            var members = GetSerializableMembers(model, membersToIgnore);
            if (members.Count == 0)
            {
                return;
            }

            using (var context = GetContext(model, stream, SerializationContextMode.Serialization))
            {
                SerializeMembers(context, members);

                AppendContextToStream(context, stream);
            }
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
        /// Called before the serializer starts serializing a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void BeforeSerializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The deserialized member value.</returns>
        protected abstract void SerializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue);

        /// <summary>
        /// Called after the serializer has serialized a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void AfterSerializeMember(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
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
        /// Serializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="membersToSerialize">The members to serialize.</param>
        protected virtual void SerializeMembers(ISerializationContext<TSerializationContext> context, List<MemberValue> membersToSerialize)
        {
            foreach (var member in membersToSerialize)
            {
                var memberSerializationEventArgs = new MemberSerializationEventArgs(context, member);

                SerializingMember.SafeInvoke(this, memberSerializationEventArgs);

                BeforeSerializeMember(context, member);

                SerializeMember(context, member);

                AfterSerializeMember(context, member);

                SerializedMember.SafeInvoke(this, memberSerializationEventArgs);
            }
        }
        #endregion
    }
}
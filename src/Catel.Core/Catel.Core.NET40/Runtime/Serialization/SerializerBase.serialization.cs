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

            var context = GetContext(model, stream, SerializationContextMode.Serialization);

            Serialize(model, context.Context);

            AppendContextToStream(context, stream);
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

            var finalContext = GetContext(model, context, SerializationContextMode.Serialization);

            BeforeSerialization(finalContext);

            var members = GetSerializableMembers(model);
            SerializeMembers(finalContext, members);

            AfterSerialization(finalContext);
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

            var context = GetContext(model, stream, SerializationContextMode.Serialization);

            SerializeMembers(context, members);

            AppendContextToStream(context, stream);
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
                BeforeSerializeMember(context, member);

                SerializeMember(context, member);

                AfterSerializeMember(context, member);
            }
        }
        #endregion
    }
}
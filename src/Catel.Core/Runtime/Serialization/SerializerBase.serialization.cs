// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.serialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Scoping;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContextInfo>
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

        #region ISerializer<TSerializationContextInfo> Members
        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        public virtual void Serialize(object model, Stream stream, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetSerializationContextInfo(model, model.GetType(), stream, SerializationContextMode.Serialization, configuration))
                {
                    Serialize(model, context);

                    AppendContextToStream(context, stream);
                }
            }
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="configuration">The configuration.</param>
        public void Serialize(object model, ISerializationContextInfo context, ISerializationConfiguration configuration = null)
        {
            Serialize(model, (TSerializationContextInfo)context, configuration);
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="configuration">The configuration.</param>
        public virtual void Serialize(object model, TSerializationContextInfo context, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var finalContext = GetContext(model, model.GetType(), context, SerializationContextMode.Serialization, configuration))
                {
                    Serialize(model, finalContext);
                }
            }
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected virtual void Serialize(object model, ISerializationContext<TSerializationContextInfo> context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

            Log.Debug("Using '{0}' serializer modifiers to deserialize type '{1}'", serializerModifiers.Length,
                context.ModelTypeName);

            var serializingEventArgs = new SerializationEventArgs(context);

            Serializing?.Invoke(this, serializingEventArgs);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnSerializing(context, model);
            }

            BeforeSerialization(context);

            var members = GetSerializableMembers(context, model);
            SerializeMembers(context, members);

            AfterSerialization(context);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnSerialized(context, model);
            }

            Serialized?.Invoke(this, serializingEventArgs);
        }

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        public virtual void SerializeMembers(object model, Stream stream, ISerializationConfiguration configuration, params string[] membersToIgnore)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetSerializationContextInfo(model, model.GetType(), stream, SerializationContextMode.Serialization, configuration))
                {
                    var members = GetSerializableMembers(context, model, membersToIgnore);
                    if (members.Count == 0)
                    {
                        return;
                    }

                    SerializeMembersOnly(context, stream, members);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called before the serializer starts serializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void BeforeSerialization(ISerializationContext<TSerializationContextInfo> context)
        {
        }

        /// <summary>
        /// Called before the serializer starts serializing a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void BeforeSerializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Serializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The deserialized member value.</returns>
        protected abstract void SerializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue);

        /// <summary>
        /// Called after the serializer has serialized a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void AfterSerializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Called after the serializer has serialized an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void AfterSerialization(ISerializationContext<TSerializationContextInfo> context)
        {
        }

        private void SerializeMembersOnly(ISerializationContext<TSerializationContextInfo> context, Stream stream, List<MemberValue> membersToSerialize)
        {
            BeforeSerialization(context);

            SerializeMembers(context, membersToSerialize);

            AfterSerialization(context);

            AppendContextToStream(context, stream);
        }

        /// <summary>
        /// Serializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="membersToSerialize">The members to serialize.</param>
        protected virtual void SerializeMembers(ISerializationContext<TSerializationContextInfo> context, List<MemberValue> membersToSerialize)
        {
            if (membersToSerialize.Count == 0)
            {
                return;
            }

            using (GetCurrentSerializationScopeManager(context.Configuration))
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

                foreach (var member in membersToSerialize)
                {
                    if (StartMemberSerialization(context, member, serializerModifiers))
                    {
                        EndMemberSerialization(context, member);
                    }
                }
            }
        }

        /// <summary>
        /// Starts member serialization by invoking all the right events.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="member">The member that is about to be serialized.</param>
        /// <param name="serializerModifiers">The serializer modifiers.</param>
        protected bool StartMemberSerialization(ISerializationContext<TSerializationContextInfo> context, 
            MemberValue member, ISerializerModifier[] serializerModifiers)
        {
            var skipByModifiers = false;
            foreach (var serializerModifier in serializerModifiers)
            {
                if (serializerModifier.ShouldIgnoreMember(context, context.Model, member))
                {
                    skipByModifiers = true;
                    break;
                }
            }

            if (skipByModifiers)
            {
                return false;
            }

            SerializingMember?.Invoke(this, new MemberSerializationEventArgs(context, member));

            BeforeSerializeMember(context, member);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.SerializeMember(context, member);
            }

            if (ShouldSerializeUsingParseAndToString(member, true))
            {
                var objectToStringValue = SerializeUsingObjectToString(context, member);
                if (!string.IsNullOrWhiteSpace(objectToStringValue))
                {
                    member.Value = objectToStringValue;
                }
            }

            return true;
        }

        /// <summary>
        /// Ends member serialization by invoking all the right events and running the modifiers.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="member">The member that has been deserialized.</param>
        protected void EndMemberSerialization(ISerializationContext<TSerializationContextInfo> context, MemberValue member)
        {
            SerializeMember(context, member);

            AfterSerializeMember(context, member);

            SerializedMember?.Invoke(this, new MemberSerializationEventArgs(context, member));
        }

        /// <summary>
        /// Deserializes the object using the <c>Parse(string, IFormatProvider)</c> method.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        protected virtual string SerializeUsingObjectToString(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
            var toStringMethod = GetObjectToStringMethod(memberValue.GetBestMemberType());
            if (toStringMethod is null)
            {
                return null;
            }

            try
            {
                var stringValue = (string)toStringMethod.Invoke(memberValue.Value, new object[] { context.Configuration.Culture });
                return stringValue;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to serialize type '{memberValue.GetBestMemberType().GetSafeFullName(false)}' using ToString(IFormatProvider)");
                return null;
            }
        }
        #endregion
    }
}

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
    using Catel.ApiCop.Rules;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Scoping;

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

        #region ISerializer<TSerializationContext> Members
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

                using (var context = GetContext(model, model.GetType(), stream, SerializationContextMode.Serialization, configuration))
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
            Serialize(model, (TSerializationContext)context, configuration);
        }

        /// <summary>
        /// Serializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="configuration">The configuration.</param>
        public virtual void Serialize(object model, TSerializationContext context, ISerializationConfiguration configuration = null)
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
        protected virtual void Serialize(object model, ISerializationContext<TSerializationContext> context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

            Log.Debug("Using '{0}' serializer modifiers to deserialize type '{1}'", serializerModifiers.Length,
                context.ModelTypeName);

            var serializingEventArgs = new SerializationEventArgs(context);

            Serializing.SafeInvoke(this, serializingEventArgs);

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

            Serialized.SafeInvoke(this, serializingEventArgs);
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

                using (var context = GetContext(model, model.GetType(), stream, SerializationContextMode.Serialization, configuration))
                {
                    var members = GetSerializableMembers(context, model, membersToIgnore);
                    if (members.Count == 0)
                    {
                        return;
                    }

                    SerializeMembers(context, members);

                    AppendContextToStream(context, stream);
                }
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
            ApiCop.UpdateRule<InitializationApiCopRule>("SerializerBase.WarmupAtStartup",
                x => x.SetInitializationMode(InitializationMode.Lazy, GetType().GetSafeFullName(false)));

            if (membersToSerialize.Count == 0)
            {
                return;
            }

            using (GetCurrentSerializationScopeManager(context.Configuration))
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

                foreach (var member in membersToSerialize)
                {
                    bool skipByModifiers = false;
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
                        continue;
                    }

                    var memberSerializationEventArgs = new MemberSerializationEventArgs(context, member);

                    SerializingMember.SafeInvoke(this, memberSerializationEventArgs);

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

                    SerializeMember(context, member);

                    AfterSerializeMember(context, member);

                    SerializedMember.SafeInvoke(this, memberSerializationEventArgs);
                }
            }
        }

        /// <summary>
        /// Deserializes the object using the <c>Parse(string, IFormatProvider)</c> method.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        protected virtual string SerializeUsingObjectToString(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
        {
            var toStringMethod = GetObjectToStringMethod(memberValue.GetBestMemberType());
            if (toStringMethod == null)
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

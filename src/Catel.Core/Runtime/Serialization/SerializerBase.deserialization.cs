﻿namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Collections;
    using Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Reflection;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContextInfo>
    {
        /// <summary>
        /// Occurs when an object is about to be serialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs>? Serializing;

        /// <summary>
        /// Occurs when an object is about to serialize a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs>? SerializingMember;

        /// <summary>
        /// Occurs when an object has just serialized a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs>? SerializedMember;

        /// <summary>
        /// Occurs when an object has just been serialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs>? Serialized;

        /// <summary>
        /// Called before the serializer starts deserializing an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void BeforeDeserialization(ISerializationContext<TSerializationContextInfo> context)
        {
        }

        /// <summary>
        /// Called before the serializer starts deserializing a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void BeforeDeserializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Deserializes the member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The <see cref="SerializationObject" /> representing the deserialized value or result.</returns>
        protected abstract SerializationObject DeserializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue);

        /// <summary>
        /// Called after the serializer has deserialized a specific member.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        protected virtual void AfterDeserializeMember(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
        }

        /// <summary>
        /// Called after the serializer has deserialized an object.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void AfterDeserialization(ISerializationContext<TSerializationContextInfo> context)
        {
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized model.
        /// </returns>
        public virtual object? Deserialize(object model, Stream stream, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetSerializationContextInfo(model, model.GetType(), stream, SerializationContextMode.Deserialization, configuration))
                {
                    return Deserialize(model, context);
                }
            }
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized model.
        /// </returns>
        public object? Deserialize(object model, ISerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            return Deserialize(model, (TSerializationContextInfo)serializationContext, configuration);
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public virtual object? Deserialize(object model, TSerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var finalContext = GetContext(model, model.GetType(), serializationContext, SerializationContextMode.Deserialization, configuration))
                {
                    return Deserialize(model, finalContext);
                }
            }
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The serialization context.</param>
        protected virtual object? Deserialize(object model, ISerializationContext<TSerializationContextInfo> context)
        {
            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

            //Log.Debug("Using '{0}' serializer modifiers to deserialize type '{1}'", serializerModifiers.Length, context.ModelTypeName);

            var serializingEventArgs = new SerializationEventArgs(context);

            Deserializing?.Invoke(this, serializingEventArgs);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserializing(context, model);
            }

            BeforeDeserialization(context);

            var deserializedMemberValues = DeserializeMembers(context);
            PopulateModel(context, deserializedMemberValues);

            // Always use the deserialized model (might be a value type)
            model = context.Model;

            AfterDeserialization(context);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserialized(context, model);
            }

            Deserialized?.Invoke(this, serializingEventArgs);

            return model;
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized <see cref="object" />.
        /// </returns>
        public virtual object? Deserialize(Type modelType, Stream stream, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetContext(modelType, stream, SerializationContextMode.Deserialization, configuration))
                {
                    return Deserialize(context.Model, context.Context, configuration);
                }
            }
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized <see cref="object" />.
        /// </returns>
        public object? Deserialize(Type modelType, ISerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            return Deserialize(modelType, (TSerializationContextInfo)serializationContext, configuration);
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized <see cref="object" />.
        /// </returns>
        public virtual object? Deserialize(Type modelType, TSerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            var model = ServiceProvider.GetRequiredService(modelType);

            Deserialize(model, serializationContext, configuration);

            return model;
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public virtual List<MemberValue> DeserializeMembers(Type modelType, Stream stream, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetContext(modelType, stream, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembersOnly(context);
                }
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="model">The model instance.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public virtual List<MemberValue> DeserializeMembers(object model, Stream stream, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetSerializationContextInfo(model, model.GetType(), stream, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembersOnly(context);
                }
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContextInfo">The serialization context information.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public List<MemberValue> DeserializeMembers(Type modelType, ISerializationContextInfo serializationContextInfo, ISerializationConfiguration? configuration = null)
        {
            return DeserializeMembers(modelType, (TSerializationContextInfo)serializationContextInfo, configuration);
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialized context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public virtual List<MemberValue> DeserializeMembers(Type modelType, TSerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var finalContext = GetContext(modelType, serializationContext, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembersOnly(finalContext);
                }
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="model">The model instance.</param>
        /// <param name="serializationContextInfo">The serialization context information.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public List<MemberValue> DeserializeMembers(object model, ISerializationContextInfo serializationContextInfo, ISerializationConfiguration? configuration = null)
        {
            return DeserializeMembers(model, (TSerializationContextInfo)serializationContextInfo, configuration);
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="model">The model instance.</param>
        /// <param name="serializationContext">The serialized context.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized list of member values.
        /// </returns>
        public virtual List<MemberValue> DeserializeMembers(object model, TSerializationContextInfo serializationContext, ISerializationConfiguration? configuration = null)
        {
            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var finalContext = GetContext(model, model.GetType(), serializationContext, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembersOnly(finalContext);
                }
            }
        }

        private List<MemberValue> DeserializeMembersOnly(ISerializationContext<TSerializationContextInfo> context)
        {
            BeforeDeserialization(context);

            var members = DeserializeMembers(context);

            AfterDeserialization(context);

            return members;
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of member values.</returns>
        protected virtual List<MemberValue> DeserializeMembers(ISerializationContext<TSerializationContextInfo> context)
        {
            var deserializedMemberValues = new List<MemberValue>();

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType).Reverse();

            var membersToDeserialize = GetSerializableMembers(context, context.Model);

            foreach (var member in membersToDeserialize)
            {
                StartMemberDeserialization(context, member);

                var serializationObject = DeserializeMember(context, member);

                var finalMemberValue = EndMemberDeserialization(context, member, serializationObject, serializerModifiers);
                if (finalMemberValue is not null)
                {
                    deserializedMemberValues.Add(finalMemberValue);
                }
            }

            return deserializedMemberValues;
        }

        /// <summary>
        /// Starts member deserialization by invoking all the right events.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="member">The member that is about to be deserialized.</param>
        protected virtual void StartMemberDeserialization(ISerializationContext<TSerializationContextInfo> context, MemberValue member)
        {
            DeserializingMember?.Invoke(this, new MemberSerializationEventArgs(context, member));

            BeforeDeserializeMember(context, member);
        }

        /// <summary>
        /// Ends member deserialization by invoking all the right events and running the modifiers.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="member">The member that has been deserialized.</param>
        /// <param name="serializationObject">Result of the member deserialization.</param>
        /// <param name="serializerModifiers">The serializer modifiers.</param>
        protected virtual MemberValue? EndMemberDeserialization(ISerializationContext<TSerializationContextInfo> context, MemberValue member,
            SerializationObject serializationObject, IEnumerable<ISerializerModifier> serializerModifiers)
        {
            if (!serializationObject.IsSuccessful)
            {
                return null;
            }

            // Note that we need to sync the member values every time
            var memberValue = new MemberValue(member.MemberGroup, member.ModelType, member.MemberType, member.Name,
                member.NameForSerialization, serializationObject.MemberValue);

            if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary ||
                ShouldSerializeAsDictionary(member))
            {
                var targetDictionary = ServiceProvider.GetService(member.MemberType) as IDictionary;
                if (targetDictionary is null)
                {
                    throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                        context.ModelTypeName);
                }

                var enumerable = memberValue.Value as List<SerializableKeyValuePair>;
                if (enumerable is not null)
                {
                    foreach (var item in enumerable)
                    {
                        if (item.Key is null)
                        {
                            continue;
                        }

                        targetDictionary.Add(item.Key, item.Value);
                    }
                }
                else
                {
                    var sourceDictionary = memberValue.Value as IDictionary;
                    if (sourceDictionary is not null)
                    {
                        foreach (var key in sourceDictionary.Keys)
                        {
                            targetDictionary.Add(key, sourceDictionary[key]);
                        }
                    }
                }

                member.Value = targetDictionary;
            }
            else if (memberValue.MemberGroup == SerializationMemberGroup.Collection)
            {
                var sourceCollection = memberValue.Value as IEnumerable;
                if (sourceCollection is not null)
                {
                    if (member.MemberType.IsArrayEx())
                    {
                        var elementType = member.MemberType.GetElementTypeEx();
                        if (elementType is null)
                        {
                            elementType = typeof(object);
                        }

                        member.Value = sourceCollection.ToArray(elementType);
                    }
                    else
                    {
                        var targetCollection = ServiceProvider.GetService(member.MemberType) as IList;
                        if (targetCollection is null)
                        {
                            throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                                context.ModelTypeName);
                        }

                        if (sourceCollection is not null)
                        {
                            foreach (var item in sourceCollection)
                            {
                                targetCollection.Add(item);
                            }
                        }

                        member.Value = targetCollection;
                    }
                }
            }
            else
            {
                member.Value = memberValue.Value;
            }

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.DeserializeMember(context, member);
                memberValue.Value = member.Value;
            }

            AfterDeserializeMember(context, member);
            memberValue.Value = member.Value;

            DeserializedMember?.Invoke(this, new MemberSerializationEventArgs(context, member));
            memberValue.Value = member.Value;

            return memberValue;
        }

        /// <summary>
        /// Deserializes the object using the <c>Parse(string, IFormatProvider)</c> method.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        protected virtual object? DeserializeUsingObjectParse(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
            // Note: don't use GetBestMemberType, it could return string type
            var parseMethod = GetObjectParseMethod(memberValue.MemberType);
            if (parseMethod is null)
            {
                return null;
            }

            var memberValueAsString = memberValue.Value as string;
            if (memberValueAsString is null)
            {
                return null;
            }

            try
            {
                var obj = parseMethod.Invoke(null, new object[] 
                { 
                    memberValueAsString, context.Configuration?.Culture ?? CultureInfo.InvariantCulture 
                });

                return obj;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to deserialize type '{memberValue.GetBestMemberType().GetSafeFullName(false)}' using Parse(string, IFormatProvider)");
                return null;
            }
        }
    }
}

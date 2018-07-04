// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.deserialization.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using ApiCop.Rules;
    using Collections;
    using IoC;
    using Logging;
    using Data;
    using Reflection;
    using Scoping;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContextInfo>
    {
        #region Events
        /// <summary>
        /// Occurs when an object is about to be serialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs> Serializing;

        /// <summary>
        /// Occurs when an object is about to serialize a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs> SerializingMember;

        /// <summary>
        /// Occurs when an object has just serialized a specific member.
        /// </summary>
        public event EventHandler<MemberSerializationEventArgs> SerializedMember;

        /// <summary>
        /// Occurs when an object has just been serialized.
        /// </summary>
        public event EventHandler<SerializationEventArgs> Serialized;
        #endregion

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

        #region Methods
        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The deserialized model.
        /// </returns>
        public virtual object Deserialize(object model, Stream stream, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetContext(model, model.GetType(), stream, SerializationContextMode.Deserialization, configuration))
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
        public object Deserialize(object model, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null)
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
        public virtual object Deserialize(object model, TSerializationContextInfo serializationContext, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", serializationContext);

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
        protected virtual object Deserialize(object model, ISerializationContext<TSerializationContextInfo> context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

            Log.Debug("Using '{0}' serializer modifiers to deserialize type '{1}'", serializerModifiers.Length, context.ModelTypeName);

            var serializingEventArgs = new SerializationEventArgs(context);

            Deserializing.SafeInvoke(this, serializingEventArgs);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserializing(context, model);
            }

            BeforeDeserialization(context);

            DeserializeMembers(context);

            // Always use the deserialized model (might be a value type)
            model = context.Model;

            AfterDeserialization(context);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserialized(context, model);
            }

            Deserialized.SafeInvoke(this, serializingEventArgs);

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
        public virtual object Deserialize(Type modelType, Stream stream, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

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
        public object Deserialize(Type modelType, ISerializationContextInfo serializationContext, ISerializationConfiguration configuration = null)
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
        public virtual object Deserialize(Type modelType, TSerializationContextInfo serializationContext, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializationContext);

            var model = TypeFactory.CreateInstance(modelType);

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
        public virtual List<MemberValue> DeserializeMembers(Type modelType, Stream stream, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var context = GetContext(modelType, stream, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembers(context);
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
        public List<MemberValue> DeserializeMembers(Type modelType, ISerializationContextInfo serializationContextInfo, ISerializationConfiguration configuration = null)
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
        public virtual List<MemberValue> DeserializeMembers(Type modelType, TSerializationContextInfo serializationContext, ISerializationConfiguration configuration = null)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializationContext);

            using (GetCurrentSerializationScopeManager(configuration))
            {
                configuration = GetCurrentSerializationConfiguration(configuration);

                using (var finalContext = GetContext(modelType, serializationContext, SerializationContextMode.Deserialization, configuration))
                {
                    return DeserializeMembers(finalContext);
                }
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of member values.</returns>
        protected virtual List<MemberValue> DeserializeMembers(ISerializationContext<TSerializationContextInfo> context)
        {
            ApiCop.UpdateRule<InitializationApiCopRule>("SerializerBase.WarmupAtStartup",
                x => x.SetInitializationMode(InitializationMode.Lazy, GetType().GetSafeFullName(false)));

            var deserializedMemberValues = new List<MemberValue>();

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType).Reverse();

            var membersToDeserialize = GetSerializableMembers(context, context.Model);
            foreach (var member in membersToDeserialize)
            {
                var memberSerializationEventArgs = new MemberSerializationEventArgs(context, member);

                DeserializingMember.SafeInvoke(this, memberSerializationEventArgs);

                BeforeDeserializeMember(context, member);

                var serializationObject = DeserializeMember(context, member);
                if (serializationObject.IsSuccessful)
                {
                    // Note that we need to sync the member values every time
                    var memberValue = new MemberValue(member.MemberGroup, member.ModelType, member.MemberType, member.Name,
                        member.NameForSerialization, serializationObject.MemberValue);

                    if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
                    {
                        var targetDictionary = TypeFactory.CreateInstance(member.MemberType) as IDictionary;
                        if (targetDictionary == null)
                        {
                            throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                                context.ModelTypeName);
                        }

                        var enumerable = memberValue.Value as List<SerializableKeyValuePair>;
                        if (enumerable != null)
                        {
                            foreach (var item in enumerable)
                            {
                                targetDictionary.Add(item.Key, item.Value);
                            }
                        }
                        else
                        {
                            var sourceDictionary = memberValue.Value as IDictionary;
                            if (sourceDictionary != null)
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

                        if (member.MemberType.IsArrayEx())
                        {
                            var elementType = member.MemberType.GetElementTypeEx();
                            member.Value = sourceCollection.ToArray(elementType);
                        }
                        else
                        {
                            var targetCollection = TypeFactory.CreateInstance(member.MemberType) as IList;
                            if (targetCollection == null)
                            {
                                throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                                    context.ModelTypeName);
                            }

                            if (sourceCollection != null)
                            {
                                foreach (var item in sourceCollection)
                                {
                                    targetCollection.Add(item);
                                }
                            }

                            member.Value = targetCollection;
                        }
                    }
                    else
                    {
                        member.Value = memberValue.Value;
                    }

                    deserializedMemberValues.Add(memberValue);

                    foreach (var serializerModifier in serializerModifiers)
                    {
                        serializerModifier.DeserializeMember(context, member);
                        memberValue.Value = member.Value;
                    }

                    AfterDeserializeMember(context, member);
                    memberValue.Value = member.Value;

                    DeserializedMember.SafeInvoke(this, memberSerializationEventArgs);
                    memberValue.Value = member.Value;
                }
            }

            if (deserializedMemberValues.Count > 0)
            {
                var firstMember = deserializedMemberValues[0];
                if (firstMember.MemberGroup == SerializationMemberGroup.SimpleRootObject)
                {
                    // Completely replace root object (this is a basic (non-reference) type)
                    context.Model = firstMember.Value;
                }
                else if (firstMember.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var targetDictionary = context.Model as IDictionary;
                    if (targetDictionary == null)
                    {
                        throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                            context.ModelTypeName);
                    }

                    targetDictionary.Clear();

                    var sourceDictionary = firstMember.Value as IDictionary;
                    if (sourceDictionary != null)
                    {
                        foreach (var key in sourceDictionary.Keys)
                        {
                            targetDictionary.Add(key, sourceDictionary[key]);
                        }
                    }
                }
                else if (firstMember.MemberGroup == SerializationMemberGroup.Collection)
                {
                    if (context.ModelType.IsArrayEx())
                    {
                        context.Model = firstMember.Value;
                    }
                    else
                    {
                        var targetCollection = context.Model as IList;
                        if (targetCollection == null)
                        {
                            throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                                context.ModelTypeName);
                        }

                        targetCollection.Clear();

                        var sourceCollection = firstMember.Value as IEnumerable;
                        if (sourceCollection != null)
                        {
                            foreach (var item in sourceCollection)
                            {
                                targetCollection.Add(item);
                            }
                        }
                    }
                }
                else
                {
                    PopulateModel(context.Model, deserializedMemberValues.ToArray());
                }
            }

            return deserializedMemberValues;
        }

        /// <summary>
        /// Deserializes the object using the <c>Parse(string, IFormatProvider)</c> method.
        /// </summary>
        /// <returns>The deserialized object.</returns>
        protected virtual object DeserializeUsingObjectParse(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
            // Note: don't use GetBestMemberType, it could return string type
            var parseMethod = GetObjectParseMethod(memberValue.MemberType);
            if (parseMethod == null)
            {
                return null;
            }

            var memberValueAsString = memberValue.Value as string;
            if (memberValueAsString == null)
            {
                return null;
            }
            
            try
            {
                var obj = parseMethod.Invoke(null, new object[] { memberValueAsString, context.Configuration.Culture });
                return obj;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to deserialize type '{memberValue.GetBestMemberType().GetSafeFullName(false)}' using Parse(string, IFormatProvider)");
                return null;
            }
        }
        #endregion
    }
}

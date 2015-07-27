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
    using System.IO;
    using System.Linq;
    using ApiCop.Rules;
    using IoC;
    using Logging;
    using Data;
    using Reflection;

    /// <summary>
    /// Base class for all serializers.
    /// </summary>
    public partial class SerializerBase<TSerializationContext>
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
        public virtual void Deserialize(object model, Stream stream)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("stream", stream);

            using (var context = GetContext(model, stream, SerializationContextMode.Deserialization))
            {
                Deserialize(model, context);
            }
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        public void Deserialize(object model, ISerializationContextInfo serializationContext)
        {
            Deserialize(model, (TSerializationContext)serializationContext);
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        public virtual void Deserialize(object model, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", serializationContext);

            using (var finalContext = GetContext(model, serializationContext, SerializationContextMode.Deserialization))
            {
                Deserialize(model, finalContext);
            }
        }

        /// <summary>
        /// Deserializes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The serialization context.</param>
        protected virtual void Deserialize(object model, ISerializationContext<TSerializationContext> context)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            var serializerModifiers = SerializationManager.GetSerializerModifiers(context.ModelType);

            Log.Debug("Using '{0}' serializer modifiers to deserialize type '{1}'", serializerModifiers.Length, context.ModelType.GetSafeFullName());

            var serializingEventArgs = new SerializationEventArgs(context);

            Deserializing.SafeInvoke(this, serializingEventArgs);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserializing(context, model);
            }

            BeforeDeserialization(context);

            DeserializeMembers(context);

            AfterDeserialization(context);

            foreach (var serializerModifier in serializerModifiers)
            {
                serializerModifier.OnDeserialized(context, model);
            }

            Deserialized.SafeInvoke(this, serializingEventArgs);
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <returns>The deserialized <see cref="object"/>.</returns>
        public virtual object Deserialize(Type modelType, Stream stream)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            using (var context = GetContext(modelType, stream, SerializationContextMode.Deserialization))
            {
                var model = context.Model;

                Deserialize(model, context.Context);

                return model;
            }
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <returns>The deserialized <see cref="object"/>.</returns>
        public object Deserialize(Type modelType, ISerializationContextInfo serializationContext)
        {
            return Deserialize(modelType, (TSerializationContext)serializationContext);
        }

        /// <summary>
        /// Deserializes the specified model type.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializationContext">The serialization context.</param>
        /// <returns>The deserialized <see cref="object"/>.</returns>
        public virtual object Deserialize(Type modelType, TSerializationContext serializationContext)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", serializationContext);

            var model = TypeFactory.CreateInstance(modelType);

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

            using (var context = GetContext(modelType, stream, SerializationContextMode.Deserialization))
            {
                return DeserializeMembers(context);
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="serializedContext">The serialized context.</param>
        /// <returns>The deserialized list of member values.</returns>
        public List<MemberValue> DeserializeMembers(Type modelType, ISerializationContextInfo serializedContext)
        {
            return DeserializeMembers(modelType, (TSerializationContext)serializedContext);
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

            using (var finalContext = GetContext(modelType, serializedContext, SerializationContextMode.Deserialization))
            {
                return DeserializeMembers(finalContext);
            }
        }

        /// <summary>
        /// Deserializes the members.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>The deserialized list of member values.</returns>
        protected virtual List<MemberValue> DeserializeMembers(ISerializationContext<TSerializationContext> context)
        {
            ApiCop.UpdateRule<InitializationApiCopRule>("SerializerBase.WarmupAtStartup",
                x => x.SetInitializationMode(InitializationMode.Lazy, GetType().GetSafeFullName()));

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
                    var memberValue = new MemberValue(member.MemberGroup, member.ModelType, member.MemberType, member.Name, serializationObject.MemberValue);

                    if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
                    {
                        var targetDictionary = TypeFactory.CreateInstance(member.MemberType) as IDictionary;
                        if (targetDictionary == null)
                        {
                            Log.ErrorAndThrowException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                                context.ModelType.GetSafeFullName());
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
                        var targetCollection = TypeFactory.CreateInstance(member.MemberType) as IList;
                        if (targetCollection == null)
                        {
                            Log.ErrorAndThrowException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                                context.ModelType.GetSafeFullName());
                        }

                        var sourceCollection = memberValue.Value as IEnumerable;
                        if (sourceCollection != null)
                        {
                            foreach (var item in sourceCollection)
                            {
                                targetCollection.Add(item);
                            }
                        }

                        member.Value = targetCollection;
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
                if (firstMember.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var targetDictionary = context.Model as IDictionary;
                    if (targetDictionary == null)
                    {
                        Log.ErrorAndThrowException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                            context.ModelType.GetSafeFullName());
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
                    var targetCollection = context.Model as IList;
                    if (targetCollection == null)
                    {
                        Log.ErrorAndThrowException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                            context.ModelType.GetSafeFullName());
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
                else
                {
                    PopulateModel(context.Model, deserializedMemberValues.ToArray());
                }
            }

            return deserializedMemberValues;
        }
        #endregion
    }
}
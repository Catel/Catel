// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.general.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel.ApiCop;
    using Catel.ApiCop.Rules;
    using Catel.Caching;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Threading;

    /// <summary>
    /// Base class for serializers that can serializer the <see cref="ModelBase" />.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the T serialization context.</typeparam>
    public abstract partial class SerializerBase<TSerializationContext> : IModelBaseSerializer<TSerializationContext>
        where TSerializationContext : class
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The API cop.
        /// </summary>
        private static readonly IApiCop ApiCop = ApiCopManager.GetCurrentClassApiCop();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="SerializerBase{TSerializationContext}"/> class.
        /// </summary>
        static SerializerBase()
        {
            ApiCop.RegisterRule(new InitializationApiCopRule("SerializerBase.WarmupAtStartup", "It is recommended to warm up the serializers at application startup", ApiCopRuleLevel.Hint, InitializationMode.Eager,
                "https://catelproject.atlassian.net/wiki/display/CTL/Introduction+to+serialization#Introductiontoserialization-Warmingupserialization"));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase{TSerializationContext}"/> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        protected SerializerBase(ISerializationManager serializationManager)
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            SerializationManager = serializationManager;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the serialization manager.
        /// </summary>
        /// <value>The serialization manager.</value>
        protected ISerializationManager SerializationManager { get; private set; }
        #endregion

        #region IModelBaseSerializer<TSerializationContext> Members
        /// <summary>
        /// Gets the serializable members for the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <returns>The list of members to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public virtual List<MemberValue> GetSerializableMembers(ModelBase model, params string[] membersToIgnore)
        {
            Argument.IsNotNull("model", model);

            var membersToIgnoreHashSet = new HashSet<string>(membersToIgnore);

            var modelType = model.GetType();

            //string cacheKey = modelType.GetSafeFullName();
            //foreach (var memberToIgnore in membersToIgnore)
            //{
            //    cacheKey += string.Format("_{0}", memberToIgnore);
            //}

            var catelTypeInfo = PropertyDataManager.Default.GetCatelTypeInfo(modelType);

            var catelPropertyNames = SerializationManager.GetCatelPropertyNames(modelType);
            var fieldsToSerialize = SerializationManager.GetFieldsToSerialize(modelType);
            var propertiesToSerialize = SerializationManager.GetPropertiesToSerialize(modelType);

            var listToSerialize = new List<MemberValue>();
            var checkedMemberNames = new List<string>();

            foreach (var fieldToSerialize in fieldsToSerialize)
            {
                checkedMemberNames.Add(fieldToSerialize);

                if (membersToIgnoreHashSet.Contains(fieldToSerialize) || ShouldIgnoreMember(model, fieldToSerialize))
                {
                    Log.Debug("Field '{0}' is being ignored for serialization", fieldToSerialize);
                    continue;
                }

                try
                {
                    Log.Debug("Adding field '{0}' to list of objects to serialize", fieldToSerialize);

                    var fieldInfo = modelType.GetFieldEx(fieldToSerialize);
                    var fieldValue = new MemberValue(SerializationMemberGroup.Field, modelType, fieldInfo.FieldType, fieldInfo.Name, fieldInfo.GetValue(model));

                    listToSerialize.Add(fieldValue);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to get value of member '{0}.{1}', skipping item during serialization", modelType.GetSafeFullName(), fieldToSerialize);
                }
            }

            foreach (var propertyToSerialize in propertiesToSerialize)
            {
                checkedMemberNames.Add(propertyToSerialize);

                if (membersToIgnoreHashSet.Contains(propertyToSerialize) || ShouldIgnoreMember(model, propertyToSerialize))
                {
                    Log.Debug("Property '{0}' is being ignored for serialization", propertyToSerialize);
                    continue;
                }

                try
                {
                    Log.Debug("Adding property '{0}' to list of objects to serialize", propertyToSerialize);

                    if (catelPropertyNames.Contains(propertyToSerialize))
                    {
                        var propertyData =  catelTypeInfo.GetPropertyData(propertyToSerialize);
                        var actualPropertyValue = model.GetValueFast(propertyToSerialize);
                        var propertyValue = new MemberValue(SerializationMemberGroup.CatelProperty, modelType, propertyData.Type, propertyData.Name, actualPropertyValue);

                        listToSerialize.Add(propertyValue);
                    }
                    else
                    {
                        var propertyInfo = modelType.GetPropertyEx(propertyToSerialize);
                        var propertyValue = new MemberValue(SerializationMemberGroup.RegularProperty, modelType, propertyInfo.PropertyType, propertyInfo.Name, propertyInfo.GetValue(model, null));

                        listToSerialize.Add(propertyValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to get value of member '{0}.{1}', skipping item during serialization", modelType.GetSafeFullName(), propertyToSerialize);
                }
            }

            return listToSerialize;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Warms up the specified types. If the <paramref name="types" /> is <c>null</c>, all types known
        /// in the <see cref="TypeCache" /> will be initialized.
        /// <para />
        /// Note that it is not required to call this, but it can help to prevent an additional performance
        /// impact the first time a type is serialized.
        /// </summary>
        /// <param name="types">The types to warmp up. If <c>null</c>, all types will be initialized.</param>
        /// <param name="typesPerThread">The types per thread. If <c>-1</c>, all types will be initialized on the same thread.</param>
        public void Warmup(IEnumerable<Type> types, int typesPerThread = 1000)
        {
            ApiCop.UpdateRule<InitializationApiCopRule>("SerializerBase.WarmupAtStartup", 
                x => x.SetInitializationMode(InitializationMode.Eager, GetType().GetSafeFullName()));

            if (types == null)
            {
                types = TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x));
            }

            var allTypes = new List<Type>(types);

            ParallelHelper.ExecuteInParallel(allTypes, type =>
            {
                // General warmup
                SerializationManager.Warmup(type);

                // Specific (customized) warmup
                Warmup(type);
            }, typesPerThread, "warmup serializer for types");
        }

        /// <summary>
        /// Warms up the specified type.
        /// </summary>
        /// <param name="type">The type to warmup.</param>
        protected abstract void Warmup(Type type);

        /// <summary>
        /// Determines whether the specified member on the specified model should be ignored by the serialization engine.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="propertyName">Name of the member.</param>
        /// <returns><c>true</c> if the member should be ignored, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldIgnoreMember(ModelBase model, string propertyName)
        {
            return false;
        }

        /// <summary>
        /// Gets the context for the specified model type.
        /// <para />
        /// Use this method when no model instance is available. This method will create one.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContext> GetContext(Type modelType, TSerializationContext context, SerializationContextMode contextMode)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", context);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);
            return GetContext(model, context, contextMode);
        }

        /// <summary>
        /// Gets the context for the specified model type.
        /// <para />
        /// Use this method when no model instance is available. This method will create one.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContext> GetContext(Type modelType, Stream stream, SerializationContextMode contextMode)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("stream", stream);

            var model = (ModelBase)TypeFactory.Default.CreateInstance(modelType);
            return GetContext(model, stream, contextMode);
        }

        /// <summary>
        /// Gets the context for the specified model instance.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        protected virtual ISerializationContext<TSerializationContext> GetContext(ModelBase model, TSerializationContext context, SerializationContextMode contextMode)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("context", context);

            return new SerializationContext<TSerializationContext>(model, context, contextMode);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        protected abstract ISerializationContext<TSerializationContext> GetContext(ModelBase model, Stream stream, SerializationContextMode contextMode);

        /// <summary>
        /// Appends the serialization context to the specified stream. This way each serializer can handle the serialization
        /// its own way and write the contents to the stream via this method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        protected abstract void AppendContextToStream(ISerializationContext<TSerializationContext> context, Stream stream);

        /// <summary>
        /// Populates the model with the specified members.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="members">The members.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="members"/> is <c>null</c>.</exception>
        protected virtual void PopulateModel(ModelBase model, params MemberValue[] members)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("properties", members);

            var modelType = model.GetType();

            var catelProperties = SerializationManager.GetCatelPropertyNames(modelType);
            var fieldsToSerialize = SerializationManager.GetFieldsToSerialize(modelType);
            var propertiesToSerialize = SerializationManager.GetPropertiesToSerialize(modelType);

            foreach (var member in members)
            {
                try
                {
                    if (catelProperties.Contains(member.Name))
                    {
                        model.SetValue(member.Name, member.Value);
                    }
                    else if (fieldsToSerialize.Contains(member.Name))
                    {
                        var fieldInfo = modelType.GetFieldEx(member.Name);
                        if (fieldInfo == null)
                        {
                            Log.Warning("Failed to set field '{0}.{1}' because the member cannot be found on the model", modelType.GetSafeFullName(), member.Name);
                        }
                        else
                        {
                            fieldInfo.SetValue(model, member.Value);
                        }
                    }
                    else if (propertiesToSerialize.Contains(member.Name))
                    {
                        var propertyInfo = modelType.GetPropertyEx(member.Name);
                        if (propertyInfo == null)
                        {
                            Log.Warning("Failed to set property '{0}.{1}' because the member cannot be found on the model", modelType.GetSafeFullName(), member.Name);
                        }
                        else
                        {
                            propertyInfo.SetValue(model, member.Value, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to populate '{0}.{1}', setting the member value threw an exception", modelType.GetSafeFullName(), member.Name);
                }
            }
        }

        /// <summary>
        /// Gets the member group.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>The <see cref="SerializationMemberGroup"/>.</returns>
        protected SerializationMemberGroup GetMemberGroup(Type modelType, string memberName)
        {
            var catelProperties = SerializationManager.GetCatelPropertyNames(modelType);
            if (catelProperties.Contains(memberName))
            {
                return SerializationMemberGroup.CatelProperty;
            }

            var regularProperties = SerializationManager.GetRegularPropertyNames(modelType);
            if (regularProperties.Contains(memberName))
            {
                return SerializationMemberGroup.RegularProperty;
            }

            return SerializationMemberGroup.Field;
        }

        /// <summary>
        /// Gets the type of the member.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="memberName">Name of the member.</param>
        /// <returns>The <see cref="Type"/> of the member.</returns>
        protected Type GetMemberType(Type modelType, string memberName)
        {
            var catelProperties = SerializationManager.GetCatelProperties(modelType);
            if (catelProperties.ContainsKey(memberName))
            {
                return catelProperties[memberName].MemberType;
            }

            var regularProperties = SerializationManager.GetRegularProperties(modelType);
            if (regularProperties.ContainsKey(memberName))
            {
                return regularProperties[memberName].MemberType;
            }

            var fields = SerializationManager.GetFields(modelType);
            if (fields.ContainsKey(memberName))
            {
                return fields[memberName].MemberType;
            }

            return null;
        }
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.general.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
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
    /// Base class for serializers that can serialize any object.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the T serialization context.</typeparam>
    public abstract partial class SerializerBase<TSerializationContext> : ISerializer<TSerializationContext>
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

        #region Fields
        private readonly CacheStorage<Type, SerializationModelInfo> _serializationModelCache = new CacheStorage<Type, SerializationModelInfo>();
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
        /// Initializes a new instance of the <see cref="SerializerBase{TSerializationContext}" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="objectAdapter">The object adapter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        protected SerializerBase(ISerializationManager serializationManager, ITypeFactory typeFactory, IObjectAdapter objectAdapter)
        {
            Argument.IsNotNull("serializationManager", serializationManager);
            Argument.IsNotNull("typeFactory", typeFactory);
            Argument.IsNotNull("objectAdapter", objectAdapter);

            SerializationManager = serializationManager;
            TypeFactory = typeFactory;
            ObjectAdapter = objectAdapter;

            SerializationManager.CacheInvalidated += OnSerializationManagerCacheInvalidated;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the serialization manager.
        /// </summary>
        /// <value>The serialization manager.</value>
        protected ISerializationManager SerializationManager { get; private set; }

        /// <summary>
        /// Gets the type factory.
        /// </summary>
        /// <value>The type factory.</value>
        protected ITypeFactory TypeFactory { get; private set; }

        /// <summary>
        /// Gets the object adapter.
        /// </summary>
        /// <value>The object adapter.</value>
        protected IObjectAdapter ObjectAdapter { get; private set; }
        #endregion

        #region ISerializer<TSerializationContext> Members
        /// <summary>
        /// Gets the serializable members for the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <returns>The list of members to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public virtual List<MemberValue> GetSerializableMembers(object model, params string[] membersToIgnore)
        {
            Argument.IsNotNull("model", model);

            var membersToIgnoreHashSet = new HashSet<string>(membersToIgnore);

            var modelType = model.GetType();

            var modelInfo = _serializationModelCache.GetFromCacheOrFetch(modelType, () =>
            {
                var catelPropertyNames = SerializationManager.GetCatelPropertyNames(modelType);
                var fieldsToSerialize = SerializationManager.GetFieldsToSerialize(modelType);
                var propertiesToSerialize = SerializationManager.GetPropertiesToSerialize(modelType);

                return new SerializationModelInfo(modelType, catelPropertyNames, fieldsToSerialize, propertiesToSerialize);
            });

            var listToSerialize = new List<MemberValue>();
            var checkedMemberNames = new HashSet<string>();

            foreach (var propertyName in modelInfo.PropertiesByName.Keys)
            {
                if (checkedMemberNames.Contains(propertyName))
                {
                    continue;
                }

                checkedMemberNames.Add(propertyName);

                if (membersToIgnoreHashSet.Contains(propertyName) || ShouldIgnoreMember(model, propertyName))
                {
                    Log.Debug("Property '{0}' is being ignored for serialization", propertyName);
                    continue;
                }

                var propertyValue = ObjectAdapter.GetMemberValue(model, propertyName, modelInfo);
                if (propertyValue != null)
                {
                    listToSerialize.Add(propertyValue);
                }
            }

            foreach (var field in modelInfo.Fields)
            {
                if (checkedMemberNames.Contains(field.Name))
                {
                    continue;
                }

                checkedMemberNames.Add(field.Name);

                if (membersToIgnoreHashSet.Contains(field.Name) || ShouldIgnoreMember(model, field.Name))
                {
                    Log.Debug("Field '{0}' is being ignored for serialization", field.Name);
                    continue;
                }

                var fieldValue = ObjectAdapter.GetMemberValue(model, field.Name, modelInfo);
                if (fieldValue != null)
                {
                    listToSerialize.Add(fieldValue);
                }
            }

            return listToSerialize;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the <see cref="E:SerializationManagerCacheInvalidated" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CacheInvalidatedEventArgs"/> instance containing the event data.</param>
        private void OnSerializationManagerCacheInvalidated(object sender, CacheInvalidatedEventArgs e)
        {
            _serializationModelCache.Remove(e.Type);
        }

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
        protected virtual bool ShouldIgnoreMember(object model, string propertyName)
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

            var model = TypeFactory.CreateInstance(modelType);
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

            var model = TypeFactory.CreateInstance(modelType);
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
        protected virtual ISerializationContext<TSerializationContext> GetContext(object model, TSerializationContext context, SerializationContextMode contextMode)
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
        protected abstract ISerializationContext<TSerializationContext> GetContext(object model, Stream stream, SerializationContextMode contextMode);

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
        protected virtual void PopulateModel(object model, params MemberValue[] members)
        {
            Argument.IsNotNull("model", model);
            Argument.IsNotNull("properties", members);

            var modelType = model.GetType();

            var modelInfo = _serializationModelCache.GetFromCacheOrFetch(modelType, () =>
            {
                var catelPropertyNames = SerializationManager.GetCatelPropertyNames(modelType);
                var fieldsToSerialize = SerializationManager.GetFieldsToSerialize(modelType);
                var propertiesToSerialize = SerializationManager.GetPropertiesToSerialize(modelType);

                return new SerializationModelInfo(modelType, catelPropertyNames, fieldsToSerialize, propertiesToSerialize);
            });

            foreach (var member in members)
            {
                ObjectAdapter.SetMemberValue(model, member, modelInfo);
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
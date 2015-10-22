// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.general.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using Catel.ApiCop;
    using Catel.ApiCop.Rules;
    using Catel.Caching;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Base class for serializers that can serialize any object.
    /// </summary>
    /// <typeparam name="TSerializationContext">The type of the T serialization context.</typeparam>
    public abstract partial class SerializerBase<TSerializationContext> : ISerializer
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

        /// <summary>
        /// The root object name.
        /// </summary>
        protected const string RootObjectName = "Value";

        /// <summary>
        /// The collection name.
        /// </summary>
        protected const string CollectionName = "Items";

        /// <summary>
        /// The dictionary name.
        /// </summary>
        protected const string DictionaryName = "Pairs";
        #endregion

        #region Fields
        private readonly CacheStorage<Type, SerializationModelInfo> _serializationModelCache = new CacheStorage<Type, SerializationModelInfo>();

        private readonly CacheStorage<Type, bool> _shouldSerializeAsCollectionCache = new CacheStorage<Type, bool>(); 
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
        /// <param name="context">The serialization context.</param>
        /// <param name="model">The model.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <returns>The list of members to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public virtual List<MemberValue> GetSerializableMembers(ISerializationContext<TSerializationContext> context, object model, params string[] membersToIgnore)
        {
            Argument.IsNotNull("model", model);

            var listToSerialize = new List<MemberValue>();
            var membersToIgnoreHashSet = new HashSet<string>(membersToIgnore);

            var modelType = model.GetType();

            // If a basic type, we need to directly deserialize as member and replace the context model
            if (ShouldExternalSerializerHandleMember(context.ModelType, context.Model))
            {
                listToSerialize.Add(new MemberValue(SerializationMemberGroup.SimpleRootObject, modelType, modelType, RootObjectName, RootObjectName, model));
                return listToSerialize;
            }

            if (ShouldSerializeAsDictionary(modelType))
            {
                // CTL-688: only support json for now. In the future, these checks (depth AND type) should be removed
                if (SupportsDictionarySerialization(context))
                {
                    listToSerialize.Add(new MemberValue(SerializationMemberGroup.Dictionary, modelType, modelType, CollectionName, CollectionName, model));
                    return listToSerialize;
                }
            }

            if (ShouldSerializeAsCollection(modelType))
            {
                listToSerialize.Add(new MemberValue(SerializationMemberGroup.Collection, modelType, modelType, CollectionName, CollectionName, model));
                return listToSerialize;
            }

            if (modelType == typeof(SerializableKeyValuePair))
            {
                var keyValuePair = (SerializableKeyValuePair)model;

                var keyType = typeof(object);
                var valueType = typeof(object);

                // Search max 2 levels deep, if not found, then we failed
                var parentDictionary = context.FindParentType(x => x.IsDictionary(), 2);
                if (parentDictionary != null)
                {
                    var genericTypeDefinition = parentDictionary.GetGenericArgumentsEx();

                    keyType = genericTypeDefinition[0];
                    valueType = genericTypeDefinition[1];
                }

                listToSerialize.Add(new MemberValue(SerializationMemberGroup.RegularProperty, modelType, keyType, "Key", "Key", keyValuePair.Key));
                listToSerialize.Add(new MemberValue(SerializationMemberGroup.RegularProperty, modelType, valueType, "Value", "Value", keyValuePair.Value));

                return listToSerialize;
            }

            var modelInfo = _serializationModelCache.GetFromCacheOrFetch(modelType, () =>
            {
                var catelProperties = SerializationManager.GetCatelPropertiesToSerialize(modelType);
                var fields = SerializationManager.GetFieldsToSerialize(modelType);
                var properties = SerializationManager.GetRegularPropertiesToSerialize(modelType);

                return new SerializationModelInfo(modelType, catelProperties, fields, properties);
            });

            var members = new List<MemberMetadata>();
            members.AddRange(modelInfo.CatelPropertiesByName.Values);
            members.AddRange(modelInfo.PropertiesByName.Values);
            members.AddRange(modelInfo.FieldsByName.Values);

            foreach (var memberMetadata in members)
            {
                var memberName = memberMetadata.MemberName;
                if (membersToIgnoreHashSet.Contains(memberName) || ShouldIgnoreMember(model, memberName))
                {
                    Log.Debug("Member '{0}' is being ignored for serialization", memberName);
                    continue;
                }

                // TODO: why get value but not store it?
                var memberValue = ObjectAdapter.GetMemberValue(model, memberName, modelInfo);
                if (memberValue != null)
                {
                    listToSerialize.Add(memberValue);
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
                types = TypeCache.GetTypes(x => x.IsModelBase());
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
            Argument.IsNotNull("type", modelType);
            Argument.IsNotNull("context", context);

            var model = CreateModelInstance(modelType);
            return GetContext(model, modelType, context, contextMode);
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
            Argument.IsNotNull("type", modelType);
            Argument.IsNotNull("stream", stream);

            var model = CreateModelInstance(modelType);
            return GetContext(model, modelType, stream, contextMode);
        }

        /// <summary>
        /// Gets the context for the specified model instance.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        protected virtual ISerializationContext<TSerializationContext> GetContext(object model, Type modelType, TSerializationContext context, SerializationContextMode contextMode)
        {
            Argument.IsNotNull("modelType", modelType);
            Argument.IsNotNull("context", context);

            return new SerializationContext<TSerializationContext>(model, modelType, context, contextMode);
        }

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        protected abstract ISerializationContext<TSerializationContext> GetContext(object model, Type modelType, Stream stream, SerializationContextMode contextMode);

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
                var catelProperties = SerializationManager.GetCatelProperties(modelType);
                var fields = SerializationManager.GetFields(modelType);
                var regularProperties = SerializationManager.GetRegularProperties(modelType);

                return new SerializationModelInfo(modelType, catelProperties, fields, regularProperties);
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


        /// <summary>
        /// Returns whether the model should be serialized as collection. Note that this method will
        /// return <c>false</c> if the method does not derive from <c>ModelBase</c>.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns><c>true</c> if the model should be serialized as a collection, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldSerializeModelAsCollection(Type memberType)
        {
            if (AttributeHelper.IsDecoratedWithAttribute<SerializeAsCollectionAttribute>(memberType))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns whether the member value should be serialized as collection.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the member value should be serialized as collection, <c>false</c> otherwise.</returns>
        protected bool ShouldSerializeAsCollection(MemberValue memberValue)
        {
            if (memberValue.MemberGroup == SerializationMemberGroup.Collection)
            {
                return true;
            }

            return ShouldSerializeAsCollection(memberValue.GetBestMemberType());
        }

        /// <summary>
        /// Returns whether the member value should be serialized as collection.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns><c>true</c> if the member value should be serialized as collection, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldSerializeAsCollection(Type memberType)
        {
            return _shouldSerializeAsCollectionCache.GetFromCacheOrFetch(memberType, () =>
            {
                if (memberType == typeof (byte[]))
                {
                    return false;
                }

                // An exception is ModelBase, we will always serialize ourselves (even when it is a collection)
                if (memberType.IsModelBase())
                {
                    return ShouldSerializeModelAsCollection(memberType);
                }

                if (memberType.IsCollection())
                {
                    return true;
                }

                if (memberType == typeof (IEnumerable))
                {
                    return true;
                }

                if (memberType.IsGenericTypeEx())
                {
                    var genericDefinition = memberType.GetGenericTypeDefinitionEx();
                    if (genericDefinition == typeof (IEnumerable<>) ||
                        typeof (IEnumerable<>).IsAssignableFromEx(genericDefinition))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// Returns whether the member value should be serialized as dictionary.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the member value should be serialized as dictionary, <c>false</c> otherwise.</returns>
        protected bool ShouldSerializeAsDictionary(MemberValue memberValue)
        {
            if (memberValue.MemberGroup == SerializationMemberGroup.Dictionary)
            {
                return true;
            }

            return ShouldSerializeAsDictionary(memberValue.GetBestMemberType());
        }

        /// <summary>
        /// Returns whether the member value should be serialized as dictionary.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns><c>true</c> if the member value should be serialized as dictionary, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldSerializeAsDictionary(Type memberType)
        {
            // TODO: add caching

            if (memberType.IsDictionary())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Converts a dictionary into a serializable collection.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The list of serializable key value pairs.</returns>
        protected List<SerializableKeyValuePair> ConvertDictionaryToCollection(object memberValue)
        {
            var collection = new List<SerializableKeyValuePair>();

            var dictionary = memberValue as IDictionary;
            if (dictionary != null)
            {
                var genericArguments = memberValue.GetType().GetGenericArgumentsEx();
                var keyType = genericArguments[0];
                var valueType = genericArguments[1];

                foreach (var key in dictionary.Keys)
                {
                    var serializableKeyValuePair = new SerializableKeyValuePair
                    {
                        Key = key,
                        KeyType = keyType,
                        Value = dictionary[key],
                        ValueType = valueType
                    };

                    collection.Add(serializableKeyValuePair);
                }
            }

            return collection;
        }

        /// <summary>
        /// Returns whether json.net should handle the member.
        /// <para />
        /// By default it only handles non-class types.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if json.net should handle the type, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldExternalSerializerHandleMember(MemberValue memberValue)
        {
            if (memberValue.MemberGroup == SerializationMemberGroup.Collection)
            {
                return false;
            }

            return ShouldExternalSerializerHandleMember(memberValue.GetBestMemberType(), memberValue.Value);
        }

        /// <summary>
        /// Returns whether json.net should handle the member.
        /// <para />
        /// By default it only handles non-class types.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if json.net should handle the type, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldExternalSerializerHandleMember(Type memberType, object memberValue)
        {
            if (memberType == typeof(IEnumerable))
            {
                return false;
            }

            if (!memberType.IsClassType())
            {
                return true;
            }

            if (memberType == typeof(string))
            {
                return true;
            }

            if (memberType == typeof(Guid))
            {
                return true;
            }

            if (memberType == typeof(Uri))
            {
                return true;
            }

            if (memberType == typeof(byte[]))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates the model instance. When a type is an array or IEnumerable, this will use a collection as model instance.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>The instantiated type.</returns>
        protected virtual object CreateModelInstance(Type type)
        {
            Type elementType = null;

            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsBasicType())
            {
                return Activator.CreateInstance(type);
            }

            if (type == typeof(IEnumerable))
            {
                elementType = typeof(object);
            }

            if (type.IsArrayEx())
            {
                elementType = type.GetElementTypeEx();
                return Array.CreateInstance(elementType, 0);
            }

            if (type.IsGenericTypeEx() && typeof(IEnumerable<>) == type.GetGenericTypeDefinitionEx())
            {
                elementType = type.GetGenericArgumentsEx()[0];
            }

            if (elementType != null)
            {
                var collectionType = typeof(List<>);
                var genericCollectionType = collectionType.MakeGenericType(elementType);

                type = genericCollectionType;
            }

            return TypeFactory.CreateInstance(type);
        }

        /// <summary>
        /// Determines whether the specified member value is a root dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the specified member value is a root dictionary; otherwise, <c>false</c>.</returns>
        protected virtual bool IsRootDictionary(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
        {
            return IsRootObject(context, memberValue, x => ShouldSerializeAsDictionary(x));
        }

        /// <summary>
        /// Determines whether the specified member value is a root collection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the specified member value is a root collection; otherwise, <c>false</c>.</returns>
        protected virtual bool IsRootCollection(ISerializationContext<TSerializationContext> context, MemberValue memberValue)
        {
            return IsRootObject(context, memberValue, x => ShouldSerializeAsCollection(x));
        }

        /// <summary>
        /// Determines whether the specified member value is a root object.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns><c>true</c> if the specified member value is a root object; otherwise, <c>false</c>.</returns>
        protected virtual bool IsRootObject(ISerializationContext<TSerializationContext> context, MemberValue memberValue, Func<MemberValue, bool> predicate)
        {
            if (context.Depth > 0)
            {
                return false;
            }

            if (!ReferenceEquals(context.Model, memberValue.Value))
            {
                return false;
            }

            if (!predicate(memberValue))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Supportses the dictionary serialization.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected virtual bool SupportsDictionarySerialization(ISerializationContext<TSerializationContext> context)
        {
            // NOTE: This method must be deleted in the future

            // CTL-688: only support json for now. In the future, these checks (depth AND type) should be removed
            if (context.Depth == 0 || GetType().Name != "XmlSerializer")
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
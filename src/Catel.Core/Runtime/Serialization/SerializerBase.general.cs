namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using Catel.Caching;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Scoping;

    /// <summary>
    /// Base class for serializers that can serialize any object.
    /// </summary>
    /// <typeparam name="TSerializationContextInfo">The type of the serialization context.</typeparam>
    public abstract partial class SerializerBase<TSerializationContextInfo> : ISerializer
        where TSerializationContextInfo : class, ISerializationContextInfo
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The default serialization configuration.
        /// </summary>
        private static readonly ISerializationConfiguration DefaultSerializationConfiguration = new SerializationConfiguration
        {
            Culture = CultureInfo.InvariantCulture
        };

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

        private readonly CacheStorage<Type, SerializationModelInfo> _serializationModelCache = new CacheStorage<Type, SerializationModelInfo>();

        private readonly CacheStorage<Type, bool> _shouldSerializeAsCollectionCache = new CacheStorage<Type, bool>();
        private readonly CacheStorage<Type, bool> _shouldSerializeAsDictionaryCache = new CacheStorage<Type, bool>();
        private readonly CacheStorage<Type, bool> _shouldSerializeByExternalSerializerCache = new CacheStorage<Type, bool>();
        private readonly CacheStorage<string, bool> _shouldSerializeUsingParseCache = new CacheStorage<string, bool>();

        private readonly CacheStorage<Type, MethodInfo?> _parseMethodCache = new CacheStorage<Type, MethodInfo?>();
        private readonly CacheStorage<Type, MethodInfo?> _toStringMethodCache = new CacheStorage<Type, MethodInfo?>();

        private readonly CacheStorage<string, bool> _shouldSerializeEnumAsStringCache = new CacheStorage<string, bool>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase{TSerializationContextInfo}" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="typeFactory">The type factory.</param>
        /// <param name="objectAdapter">The object adapter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serializationManager" /> is <c>null</c>.</exception>
        protected SerializerBase(ISerializationManager serializationManager, ITypeFactory typeFactory, IObjectAdapter objectAdapter)
        {
            SerializationManager = serializationManager;
            TypeFactory = typeFactory;
            ObjectAdapter = objectAdapter;

            SerializationManager.CacheInvalidated += OnSerializationManagerCacheInvalidated;
        }

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

        /// <summary>
        /// Gets the serializable members for the specified model.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="model">The model.</param>
        /// <param name="membersToIgnore">The members to ignore.</param>
        /// <returns>The list of members to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model"/> is <c>null</c>.</exception>
        public virtual List<MemberValue> GetSerializableMembers(ISerializationContext<TSerializationContextInfo> context, object model, params string[] membersToIgnore)
        {
            var listToSerialize = new List<MemberValue>();
            var membersToIgnoreHashSet = new HashSet<string>(membersToIgnore);

            var modelType = model.GetType();

            // If a basic type, we need to directly deserialize as member and replace the context model
            if (ShouldExternalSerializerHandleMember(context.ModelType))
            {
                listToSerialize.Add(new MemberValue(SerializationMemberGroup.SimpleRootObject, modelType, modelType, RootObjectName, RootObjectName, model));
                return listToSerialize;
            }

            if (ShouldSerializeAsDictionary(modelType))
            {
                listToSerialize.Add(new MemberValue(SerializationMemberGroup.Dictionary, modelType, modelType, CollectionName, CollectionName, model));
                return listToSerialize;
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
                if (parentDictionary is not null)
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

                var memberValue = ObjectAdapter.GetMemberValue(model, memberName, modelInfo);
                if (memberValue is not null)
                {
                    listToSerialize.Add(memberValue);
                }
            }

            return listToSerialize;
        }

        /// <summary>
        /// Handles the <see cref="E:SerializationManagerCacheInvalidated" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="CacheInvalidatedEventArgs"/> instance containing the event data.</param>
        private void OnSerializationManagerCacheInvalidated(object? sender, CacheInvalidatedEventArgs e)
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
        public void Warmup(IEnumerable<Type>? types = null, int typesPerThread = 1000)
        {
            if (types is null)
            {
                types = TypeCache.GetTypes(x => x.IsModelBase(), false);
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
        /// Gets the current serialization scope.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        protected virtual ScopeManager<SerializationScope> GetCurrentSerializationScopeManager(ISerializationConfiguration? configuration)
        {
            var scopeName = SerializationContextHelper.GetSerializationScopeName();
            var scopeManager = ScopeManager<SerializationScope>.GetScopeManager(scopeName, () => new SerializationScope(this, configuration ?? DefaultSerializationConfiguration));
            return scopeManager;
        }

        /// <summary>
        /// Gets the current serialization configuration.
        /// </summary>
        /// <param name="configuration">The configuration that might override the existing scope configuration.</param>
        /// <returns></returns>
        protected virtual ISerializationConfiguration GetCurrentSerializationConfiguration(ISerializationConfiguration? configuration)
        {
            using (var scopeManager = GetCurrentSerializationScopeManager(configuration))
            {
                var scopeObject = scopeManager.ScopeObject;

                if (configuration is not null)
                {
                    scopeObject.Configuration = configuration;
                }
                else
                {
                    if (scopeObject.Configuration is null)
                    {
                        scopeObject.Configuration = DefaultSerializationConfiguration;
                    }
                }

                return scopeObject.Configuration;
            }
        }

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
        /// <param name="configuration">The configuration.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContextInfo> GetContext(Type modelType, TSerializationContextInfo context,
            SerializationContextMode contextMode, ISerializationConfiguration? configuration = null)
        {
            var model = CreateModelInstance(modelType);
            if (model is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Cannot get context of null model");
            }

            return GetContext(model, modelType, context, contextMode, configuration);
        }

        /// <summary>
        /// Gets the context for the specified model type.
        /// <para />
        /// Use this method when no model instance is available. This method will create one.
        /// </summary>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The serialization context.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="stream" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        protected ISerializationContext<TSerializationContextInfo> GetContext(Type modelType, Stream stream,
            SerializationContextMode contextMode, ISerializationConfiguration? configuration = null)
        {
            var model = CreateModelInstance(modelType);
            if (model is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Cannot get context of null model");
            }

            return GetSerializationContextInfo(model, modelType, stream, contextMode, configuration);
        }

        /// <summary>
        /// Gets the context for the specified model instance.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="context">The context.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The serialization context.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        protected virtual ISerializationContext<TSerializationContextInfo> GetContext(object model, Type modelType,
            TSerializationContextInfo context, SerializationContextMode contextMode, ISerializationConfiguration? configuration = null)
        {
            var finalContext = new SerializationContext<TSerializationContextInfo>(model, modelType, context, contextMode, configuration);
            return finalContext;
        }

        /// <summary>
        /// Gets the serializer specific serialization context info.
        /// </summary>
        /// <param name="model">The model, can be <c>null</c> for value types.</param>
        /// <param name="modelType">Type of the model.</param>
        /// <param name="stream">The stream.</param>
        /// <param name="contextMode">The context mode.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// The serialization context.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="model" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="modelType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> is <c>null</c>.</exception>
        protected abstract ISerializationContext<TSerializationContextInfo> GetSerializationContextInfo(object model, Type modelType, Stream stream,
            SerializationContextMode contextMode, ISerializationConfiguration? configuration);

        /// <summary>
        /// Appends the serialization context to the specified stream. This way each serializer can handle the serialization
        /// its own way and write the contents to the stream via this method.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="stream">The stream.</param>
        protected abstract void AppendContextToStream(ISerializationContext<TSerializationContextInfo> context, Stream stream);

        /// <summary>
        /// Populates the model with the specified members.
        /// </summary>
        /// <param name="context">The serialization context.</param>
        /// <param name="members">The members.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="members"/> is <c>null</c>.</exception>
        protected virtual void PopulateModel(ISerializationContext<TSerializationContextInfo> context, List<MemberValue> members)
        {
            if (members.Count > 0)
            {
                var firstMember = members[0];
                if (firstMember.MemberGroup == SerializationMemberGroup.SimpleRootObject)
                {
                    // Completely replace root object (this is a basic (non-reference) type)
                    // CHECK IF WE SHOULD ALLOW NULL HERE
                    context.Model = firstMember.Value!;
                }
                else if (firstMember.MemberGroup == SerializationMemberGroup.Dictionary)
                {
                    var targetDictionary = context.Model as IDictionary;
                    if (targetDictionary is null)
                    {
                        throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a dictionary, but target model cannot be updated because it does not implement IDictionary",
                            context.ModelTypeName);
                    }

                    targetDictionary.Clear();

                    var sourceDictionary = firstMember.Value as IDictionary;
                    if (sourceDictionary is not null)
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
                        // CHECK IF WE SHOULD ALLOW NULL HERE
                        context.Model = firstMember.Value!;
                    }
                    else
                    {
                        var targetCollection = context.Model as IList;
                        if (targetCollection is null)
                        {
                            throw Log.ErrorAndCreateException<NotSupportedException>("'{0}' seems to be a collection, but target model cannot be updated because it does not implement IList",
                                context.ModelTypeName);
                        }

                        targetCollection.Clear();

                        var sourceCollection = firstMember.Value as IEnumerable;
                        if (sourceCollection is not null)
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
                    // Populate using properties
                    var model = context.Model;
                    var modelType = context.ModelType;

                    var modelInfo = _serializationModelCache.GetFromCacheOrFetch(modelType, () =>
                    {
                        var catelProperties = SerializationManager.GetCatelProperties(modelType);
                        var fields = SerializationManager.GetFields(modelType);
                        var regularProperties = SerializationManager.GetRegularProperties(modelType);

                        return new SerializationModelInfo(modelType, catelProperties, fields, regularProperties);
                    });

                    if (modelInfo is null)
                    {
                        throw Log.ErrorAndCreateException<CatelException>($"Failed to find model info for '{modelType.GetSafeFullName(false)}'");
                    }

                    foreach (var member in members)
                    {
                        ObjectAdapter.SetMemberValue(model, member, modelInfo);
                    }
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
        protected Type? GetMemberType(Type modelType, string memberName)
        {
            var catelProperties = SerializationManager.GetCatelProperties(modelType);
            if (catelProperties.TryGetValue(memberName, out var catelProperty))
            {
                return catelProperty.MemberType;
            }

            var regularProperties = SerializationManager.GetRegularProperties(modelType);
            if (regularProperties.TryGetValue(memberName, out var regularProperty))
            {
                return regularProperty.MemberType;
            }

            var fields = SerializationManager.GetFields(modelType);
            if (fields.TryGetValue(memberName, out var field))
            {
                return field.MemberType;
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
            if (memberType.IsGenericTypeEx())
            {
                var genericTypeDefinition = memberType.GetGenericTypeDefinitionEx();
                if (genericTypeDefinition == typeof(List<>) ||
                    genericTypeDefinition == typeof(ObservableCollection<>))
                {
                    return true;
                }
            }

            if (memberType.IsDecoratedWithAttribute<SerializeAsCollectionAttribute>())
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
                var serializerModifiers = SerializationManager.GetSerializerModifiers(memberType);
                if (serializerModifiers is not null)
                {
                    foreach (var serializerModifier in serializerModifiers)
                    {
                        var shouldSerializeAsCollection = serializerModifier.ShouldSerializeAsCollection();
                        if (shouldSerializeAsCollection.HasValue)
                        {
                            return shouldSerializeAsCollection.Value;
                        }
                    }
                }

                if (memberType == typeof(byte[]))
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

                if (memberType == typeof(IEnumerable))
                {
                    return true;
                }

                if (memberType.IsGenericTypeEx())
                {
                    var genericDefinition = memberType.GetGenericTypeDefinitionEx();
                    if (genericDefinition == typeof(IEnumerable<>) ||
                        typeof(IEnumerable<>).IsAssignableFromEx(genericDefinition))
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
            return _shouldSerializeAsDictionaryCache.GetFromCacheOrFetch(memberType, () =>
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(memberType);
                if (serializerModifiers is not null)
                {
                    foreach (var serializerModifier in serializerModifiers)
                    {
                        var shouldSerializeAsDictionary = serializerModifier.ShouldSerializeAsDictionary();
                        if (shouldSerializeAsDictionary.HasValue)
                        {
                            return shouldSerializeAsDictionary.Value;
                        }
                    }
                }

                if (memberType.IsDictionary())
                {
                    return true;
                }

                return false;
            });
        }

        /// <summary>
        /// Returns whether the member value should be serialized using <c>ToString(IFormatProvider)</c> and deserialized using <c>Parse(string, IFormatProvider)</c>.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <param name="checkActualMemberType">if set to <c>true</c>, check the actual member type.</param>
        /// <returns>
        ///   <c>true</c> if the member should be serialized using parse.
        /// </returns>
        protected virtual bool ShouldSerializeUsingParseAndToString(MemberValue memberValue, bool checkActualMemberType)
        {
            var cacheKey = $"{memberValue.ModelTypeName}|{memberValue.Name}|{checkActualMemberType.ToString()}";

            return _shouldSerializeUsingParseCache.GetFromCacheOrFetch(cacheKey, () =>
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(memberValue.ModelType);

                var useParseAndToString = false;

                var fieldInfo = memberValue.ModelType.GetFieldEx(memberValue.Name);
                if (fieldInfo is not null)
                {
                    useParseAndToString = fieldInfo.IsDecoratedWithAttribute<SerializeUsingParseAndToStringAttribute>();
                }

                if (!useParseAndToString)
                {
                    var propertyInfo = memberValue.ModelType.GetPropertyEx(memberValue.Name);
                    if (propertyInfo is not null)
                    {
                        useParseAndToString = propertyInfo.IsDecoratedWithAttribute<SerializeUsingParseAndToStringAttribute>();
                    }
                }

                // Note: serializer modifiers can always win
                foreach (var serializerModifier in serializerModifiers)
                {
                    var value = serializerModifier.ShouldSerializeMemberUsingParse(memberValue);
                    if (value.HasValue)
                    {
                        if (!value.Value)
                        {
                            return false;
                        }

                        // At least 1 serializer modifier wants this to be using parse and tostring
                        useParseAndToString = true;
                    }
                }

                if (!useParseAndToString)
                {
                    return false;
                }

                var memberType = checkActualMemberType ? memberValue.ActualMemberType : memberValue.MemberType;
                if (memberType is null)
                {
                    memberType = memberValue.MemberType;
                }

                var toStringMethod = GetObjectToStringMethod(memberType);
                if (toStringMethod is null)
                {
                    return false;
                }

                var parseMethod = GetObjectParseMethod(memberType);
                if (parseMethod is null)
                {
                    return false;
                }

                return true;
            });
        }


        /// <summary>
        /// Returns whether the enum member value should be serialized as string.
        /// </summary>
        /// <param name="memberValue"></param>
        /// <param name="checkActualMemberType"></param>
        /// <returns></returns>
        protected virtual bool ShouldSerializeEnumAsString(MemberValue memberValue, bool checkActualMemberType)
        {
            var cacheKey = $"{memberValue.ModelTypeName}|{memberValue.Name}|{checkActualMemberType.ToString()}";

            return _shouldSerializeEnumAsStringCache.GetFromCacheOrFetch(cacheKey, () =>
            {
                var serializerModifiers = SerializationManager.GetSerializerModifiers(memberValue.ModelType);

                var fieldInfo = memberValue.ModelType.GetFieldEx(memberValue.Name);
                if (fieldInfo is not null)
                {
                    if (fieldInfo.IsDecoratedWithAttribute<SerializeEnumAsStringAttribute>())
                    {
                        return true;
                    }
                }

                var propertyInfo = memberValue.ModelType.GetPropertyEx(memberValue.Name);
                if (propertyInfo is not null)
                {
                    if (propertyInfo.IsDecoratedWithAttribute<SerializeEnumAsStringAttribute>())
                    {
                        return true;
                    }
                }

                // Note: serializer modifiers can always win
                foreach (var serializerModifier in serializerModifiers)
                {
                    var value = serializerModifier.ShouldSerializeEnumMemberUsingToString(memberValue);
                    if (value.HasValue)
                    {
                        return value.Value;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// Gets the <c>ToString(IFormatProvider)</c> method.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns></returns>
        protected virtual MethodInfo? GetObjectToStringMethod(Type memberType)
        {
            var toStringMethod = _toStringMethodCache.GetFromCacheOrFetch(memberType, () =>
            {
                var method = memberType.GetMethodEx("ToString", TypeArray.From<IFormatProvider>(), BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                return method;
            });

            return toStringMethod;
        }

        /// <summary>
        /// Gets the <c>Parse(string, IFormatProvider)</c> method.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns></returns>
        protected virtual MethodInfo? GetObjectParseMethod(Type memberType)
        {
            var parseMethod = _parseMethodCache.GetFromCacheOrFetch(memberType, () =>
            {
                var method = memberType.GetMethodEx("Parse", TypeArray.From<string, IFormatProvider>(), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
                return method;
            });

            return parseMethod;
        }

        /// <summary>
        /// Converts a dictionary into a serializable collection.
        /// </summary>
        /// <param name="memberValue">The member value.</param>
        /// <returns>The list of serializable key value pairs.</returns>
        protected List<SerializableKeyValuePair> ConvertDictionaryToCollection(object? memberValue)
        {
            var collection = new List<SerializableKeyValuePair>();

            var dictionary = memberValue as IDictionary;
            if (dictionary is not null)
            {
                var genericArguments = memberValue!.GetType().GetGenericArgumentsEx();
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

            return ShouldExternalSerializerHandleMember(memberValue.GetBestMemberType());
        }

        /// <summary>
        /// Returns whether json.net should handle the member.
        /// <para />
        /// By default it only handles non-class types.
        /// </summary>
        /// <param name="memberType">Type of the member.</param>
        /// <returns><c>true</c> if json.net should handle the type, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldExternalSerializerHandleMember(Type memberType)
        {
            return _shouldSerializeByExternalSerializerCache.GetFromCacheOrFetch(memberType, () =>
            {
                if (memberType == typeof(IEnumerable))
                {
                    return false;
                }

                if (memberType.IsEnumEx())
                {
                    return true;
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
            });
        }

        /// <summary>
        /// Creates the model instance. When a type is an array or IEnumerable, this will use a collection as model instance.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>The instantiated type.</returns>
        protected virtual object CreateModelInstance(Type type)
        {
            Type? elementType = null;

            if (type == typeof(string))
            {
                return string.Empty;
            }

            if (type.IsBasicType())
            {
                return Activator.CreateInstance(type)!;
            }

            if (type == typeof(IEnumerable))
            {
                elementType = typeof(object);
            }

            if (type.IsArrayEx())
            {
                elementType = type.GetElementTypeEx();
                if (elementType is null)
                {
                    elementType = typeof(object);
                }

                return Array.CreateInstance(elementType, 0);
            }

            if (type.IsGenericTypeEx() && typeof(IEnumerable<>) == type.GetGenericTypeDefinitionEx())
            {
                elementType = type.GetGenericArgumentsEx()[0];
            }

            if (elementType is not null)
            {
                var collectionType = typeof(List<>);
#pragma warning disable HAA0101 // Array allocation for params parameter
                var genericCollectionType = collectionType.MakeGenericType(elementType);
#pragma warning restore HAA0101 // Array allocation for params parameter

                type = genericCollectionType;
            }

            return TypeFactory.CreateRequiredInstance(type);
        }

        /// <summary>
        /// Determines whether the specified member value is a root dictionary.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the specified member value is a root dictionary; otherwise, <c>false</c>.</returns>
        protected virtual bool IsRootDictionary(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
        {
            return IsRootObject(context, memberValue, x => ShouldSerializeAsDictionary(x));
        }

        /// <summary>
        /// Determines whether the specified member value is a root collection.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="memberValue">The member value.</param>
        /// <returns><c>true</c> if the specified member value is a root collection; otherwise, <c>false</c>.</returns>
        protected virtual bool IsRootCollection(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue)
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
        protected virtual bool IsRootObject(ISerializationContext<TSerializationContextInfo> context, MemberValue memberValue, Func<MemberValue, bool> predicate)
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
    }
}

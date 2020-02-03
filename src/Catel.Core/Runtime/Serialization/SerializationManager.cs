// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Catel.Caching;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Manager which is responsible for discovering what fields and properties of an object should be serialized.
    /// </summary>
    public class SerializationManager : ISerializationManager
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory = TypeFactory.Default;

        private readonly object _lock = new object();

        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _fieldsToSerializeCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _catelPropertiesToSerializeCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _regularPropertiesToSerializeCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

        private readonly ICacheStorage<string, HashSet<string>> _catelPropertyNamesCache = new CacheStorage<string, HashSet<string>>();
        private readonly ICacheStorage<string, Dictionary<string, MemberMetadata>> _catelPropertiesCache = new CacheStorage<string, Dictionary<string, MemberMetadata>>();

        private readonly ICacheStorage<Type, HashSet<string>> _regularPropertyNamesCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _regularPropertiesCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

        private readonly ICacheStorage<Type, HashSet<string>> _fieldNamesCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _fieldsCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

        private readonly Dictionary<Type, List<Type>> _serializationModifierDefinitionsPerTypeCache = new Dictionary<Type, List<Type>>();
        private readonly ICacheStorage<Type, ISerializerModifier> _serializerModifierCache = new CacheStorage<Type, ISerializerModifier>();
        private readonly ICacheStorage<Type, ISerializerModifier[]> _serializationModifiersPerTypeCache = new CacheStorage<Type, ISerializerModifier[]>();

        /// <summary>
        /// Occurs when the cache for a specific type has been invalidated.
        /// </summary>
        public event EventHandler<CacheInvalidatedEventArgs> CacheInvalidated;

        /// <summary>
        /// Warmups the specified type by calling all the methods for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public void Warmup(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_lock)
            {
                GetFieldsToSerialize(type);
                GetCatelPropertiesToSerialize(type);
                GetRegularPropertiesToSerialize(type);

                GetCatelPropertyNames(type);
                GetCatelProperties(type);

                GetRegularPropertyNames(type);
                GetRegularProperties(type);

                GetFieldNames(type);
                GetFields(type);

                GetSerializerModifiers(type);
            }
        }

        /// <summary>
        /// Clears the specified type from cache so it will be evaluated.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public void Clear(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_lock)
            {
                _fieldsToSerializeCache.Remove(type);
                _catelPropertiesToSerializeCache.Remove(type);
                _regularPropertiesToSerializeCache.Remove(type);

                var key1 = GetCacheKey(type, true);
                var key2 = GetCacheKey(type, false);

                _catelPropertyNamesCache.Remove(key1);
                _catelPropertyNamesCache.Remove(key2);
                _catelPropertiesCache.Remove(key1);
                _catelPropertiesCache.Remove(key2);

                _regularPropertyNamesCache.Remove(type);
                _regularPropertiesCache.Remove(type);

                _fieldNamesCache.Remove(type);
                _fieldsCache.Remove(type);

                _serializerModifierCache.Remove(type);
                _serializationModifierDefinitionsPerTypeCache.Remove(type);
                _serializationModifiersPerTypeCache.Remove(type);
            }

            CacheInvalidated?.Invoke(this, new CacheInvalidatedEventArgs(type));
        }

        /// <summary>
        /// Gets the fields to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of fields to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual Dictionary<string, MemberMetadata> GetFieldsToSerialize(Type type)
        {
            Argument.IsNotNull("type", type);

            return _fieldsToSerializeCache.GetFromCacheOrFetch(type, () =>
            {
                var serializableMembers = new Dictionary<string, MemberMetadata>();

                var fields = GetFields(type);
                foreach (var typeField in fields)
                {
                    var memberMetadata = typeField.Value;
                    var fieldInfo = (FieldInfo)memberMetadata.Tag;

                    // Exclude fields by default
                    var include = false;

                    if (fieldInfo.IsDecoratedWithAttribute<IncludeInSerializationAttribute>())
                    {
                        include = true;
                    }

                    if (fieldInfo.IsDecoratedWithAttribute<ExcludeFromSerializationAttribute>())
                    {
                        include = false;
                    }

                    if (include)
                    {
                        serializableMembers.Add(typeField.Key, memberMetadata);
                    }
                }

                return serializableMembers;
            });
        }

        /// <summary>
        /// Gets the catel properties to serialize.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual Dictionary<string, MemberMetadata> GetCatelPropertiesToSerialize(Type type)
        {
            Argument.IsNotNull("type", type);

            return _catelPropertiesToSerializeCache.GetFromCacheOrFetch(type, () =>
            {
                var serializableMembers = new Dictionary<string, MemberMetadata>();

                var properties = GetCatelProperties(type);
                foreach (var modelProperty in properties)
                {
                    var memberMetadata = modelProperty.Value;
                    var propertyData = (PropertyData)memberMetadata.Tag;

                    bool isSerializable = propertyData.IsSerializable || propertyData.Type.IsModelBase();
                    if (!isSerializable)
                    {
                        // CTL-550
                        var cachedPropertyInfo = propertyData.GetPropertyInfo(type);
                        if (cachedPropertyInfo.IsDecoratedWithAttribute<IncludeInSerializationAttribute>())
                        {
                            isSerializable = true;
                        }
                    }

                    if (!isSerializable)
                    {
                        Log.Warning("Property '{0}' is not serializable, so will be excluded from the serialization. If this property needs to be included, use the 'IncludeInSerializationAttribute'", propertyData.Name);
                        continue;
                    }

                    if (!propertyData.IncludeInSerialization)
                    {
                        Log.Debug("Property '{0}' is flagged to be excluded from serialization", propertyData.Name);
                        continue;
                    }

                    var propertyInfo = propertyData.GetPropertyInfo(type);
                    if (propertyInfo != null)
                    {
                        if (!propertyInfo.IsDecoratedWithAttribute<ExcludeFromSerializationAttribute>())
                        {
                            serializableMembers.Add(modelProperty.Key, memberMetadata);
                        }
                    }
                    else
                    {
                        // Dynamic property, always include
                        serializableMembers.Add(modelProperty.Key, memberMetadata);
                    }
                }

                return serializableMembers;
            });
        }

        /// <summary>
        /// Gets the properties to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual Dictionary<string, MemberMetadata> GetRegularPropertiesToSerialize(Type type)
        {
            Argument.IsNotNull("type", type);

            return _regularPropertiesToSerializeCache.GetFromCacheOrFetch(type, () =>
            {
                var serializableMembers = new Dictionary<string, MemberMetadata>();

                var catelPropertyNames = new HashSet<string>();

                var isModelBase = type.IsModelBase();
                if (isModelBase)
                {
                    catelPropertyNames = GetCatelPropertyNames(type, true);
                }

                var regularProperties = GetRegularProperties(type);
                foreach (var typeProperty in regularProperties)
                {
                    var memberMetadata = typeProperty.Value;
                    var propertyInfo = (PropertyInfo)memberMetadata.Tag;

                    if (!catelPropertyNames.Contains(memberMetadata.MemberName))
                    {
                        // If not a ModelBase, include by default
                        var include = !isModelBase;

                        if (propertyInfo.IsDecoratedWithAttribute<IncludeInSerializationAttribute>())
                        {
                            include = true;
                        }

                        if (propertyInfo.IsDecoratedWithAttribute<ExcludeFromSerializationAttribute>())
                        {
                            include = false;
                        }

                        if (include)
                        {
                            serializableMembers.Add(typeProperty.Key, memberMetadata);
                        }
                    }
                }

                return serializableMembers;
            });
        }

        /// <summary>
        /// Gets the catel property names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <param name="includeModelBaseProperties">if set to <c>true</c>, also include model base properties.</param>
        /// <returns>A hash set containing the Catel property names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public HashSet<string> GetCatelPropertyNames(Type type, bool includeModelBaseProperties = false)
        {
            Argument.IsNotNull("type", type);

            var key = GetCacheKey(type, includeModelBaseProperties);

            return _catelPropertyNamesCache.GetFromCacheOrFetch(key, () =>
            {
                var catelProperties = GetCatelProperties(type, includeModelBaseProperties);

                var finalProperties = new HashSet<string>();
                foreach (var property in catelProperties)
                {
                    finalProperties.Add(property.Key);
                }

                return finalProperties;
            });
        }

        /// <summary>
        /// Gets the catel properties.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <param name="includeModelBaseProperties">if set to <c>true</c>, also include model base properties.</param>
        /// <returns>A hash set containing the Catel properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public Dictionary<string, MemberMetadata> GetCatelProperties(Type type, bool includeModelBaseProperties = false)
        {
            Argument.IsNotNull("type", type);

            var key = GetCacheKey(type, includeModelBaseProperties);

            return _catelPropertiesCache.GetFromCacheOrFetch(key, () =>
            {
                var dictionary = new Dictionary<string, MemberMetadata>();

                var propertyDataManager = PropertyDataManager.Default;
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(type);
                var properties = (from property in catelTypeInfo.GetCatelProperties()
                                  select property.Value);

                if (!includeModelBaseProperties)
                {
                    properties = properties.Where(x => !x.IsModelBaseProperty);
                }

                foreach (var property in properties)
                {
                    var memberMetadata = new MemberMetadata(type, property.Type, SerializationMemberGroup.CatelProperty, property.Name)
                    {
                        Tag = property
                    };

                    var propertyInfo = property.GetPropertyInfo(type);
                    if (propertyInfo != null && propertyInfo.PropertyInfo != null)
                    {
                        var nameOverride = GetNameOverrideForSerialization(propertyInfo.PropertyInfo);
                        if (!string.IsNullOrWhiteSpace(nameOverride))
                        {
                            memberMetadata.MemberNameForSerialization = nameOverride;
                        }
                    }

                    dictionary[property.Name] = memberMetadata;
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Gets the regular property names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the regular property names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public HashSet<string> GetRegularPropertyNames(Type type)
        {
            Argument.IsNotNull("type", type);

            return _regularPropertyNamesCache.GetFromCacheOrFetch(type, () =>
            {
                var regularPropertyNames = GetRegularProperties(type);

                var finalProperties = new HashSet<string>();
                foreach (var propertyName in regularPropertyNames)
                {
                    finalProperties.Add(propertyName.Key);
                }

                return finalProperties;
            });
        }

        /// <summary>
        /// Gets the regular properties.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the regular properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public Dictionary<string, MemberMetadata> GetRegularProperties(Type type)
        {
            Argument.IsNotNull("type", type);

            return _regularPropertiesCache.GetFromCacheOrFetch(type, () =>
            {
                var dictionary = new Dictionary<string, MemberMetadata>();

                var catelPropertyNames = GetCatelPropertyNames(type, true);

                var regularProperties = type.GetPropertiesEx();
                foreach (var propertyInfo in regularProperties)
                {
                    if (catelPropertyNames.Contains(propertyInfo.Name) ||
                        propertyInfo.DeclaringType == typeof(ModelBase))
                    {
                        continue;
                    }

                    var memberMetadata = new MemberMetadata(type, propertyInfo.PropertyType, SerializationMemberGroup.RegularProperty, propertyInfo.Name)
                    {
                        Tag = propertyInfo
                    };

                    var nameOverride = GetNameOverrideForSerialization(propertyInfo);
                    if (!string.IsNullOrWhiteSpace(nameOverride))
                    {
                        memberMetadata.MemberNameForSerialization = nameOverride;
                    }

                    dictionary[propertyInfo.Name] = memberMetadata;
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Gets the field names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the field names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public HashSet<string> GetFieldNames(Type type)
        {
            Argument.IsNotNull("type", type);

            return _fieldNamesCache.GetFromCacheOrFetch(type, () =>
            {
                var fieldNames = GetFields(type);

                var finalFields = new HashSet<string>();
                foreach (var fieldName in fieldNames)
                {
                    finalFields.Add(fieldName.Key);
                }

                return finalFields;
            });
        }

        /// <summary>
        /// Gets the fields
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the fields.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public Dictionary<string, MemberMetadata> GetFields(Type type)
        {
            Argument.IsNotNull("type", type);

            return _fieldsCache.GetFromCacheOrFetch(type, () =>
            {
                var dictionary = new Dictionary<string, MemberMetadata>();

                var fields = type.GetFieldsEx();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.Name.Contains("__BackingField") ||
                        fieldInfo.DeclaringType == typeof (ModelBase))
                    {
                        continue;
                    }

                    var memberMetadata = new MemberMetadata(type, fieldInfo.FieldType, SerializationMemberGroup.Field, fieldInfo.Name)
                    {
                        Tag = fieldInfo
                    };

                    var nameOverride = GetNameOverrideForSerialization(fieldInfo);
                    if (!string.IsNullOrWhiteSpace(nameOverride))
                    {
                        memberMetadata.MemberNameForSerialization = nameOverride;
                    }

                    dictionary[fieldInfo.Name] = memberMetadata;
                }

                return dictionary;
            });
        }

        /// <summary>
        /// Adds the serializer modifier for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerModifierType">Type of the serializer modifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializerModifierType"/> is <c>null</c>.</exception>
        public void AddSerializerModifier(Type type, Type serializerModifierType)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("serializerModifierType", serializerModifierType);

            if (!_serializationModifierDefinitionsPerTypeCache.TryGetValue(type, out var serializerModifierTypes))
            {
                serializerModifierTypes = new List<Type>();
                _serializationModifierDefinitionsPerTypeCache[type] = serializerModifierTypes;
            }

            Log.Debug($"Adding serializer modifier '{serializerModifierType.Name}' for '{type.Name}'");

            if (serializerModifierTypes.Contains(serializerModifierType))
            {
                Log.Debug($"Serializer modifier '{serializerModifierType.Name}' for '{type.Name}' is already registered");
                return;
            }

            serializerModifierTypes.Add(serializerModifierType);
            _serializationModifiersPerTypeCache.Remove(type);
        }

        /// <summary>
        /// Removes the serializer modifier for a specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerModifierType">Type of the serializer modifier.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serializerModifierType"/> is <c>null</c>.</exception>
        public void RemoveSerializerModifier(Type type, Type serializerModifierType)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("serializerModifierType", serializerModifierType);

            if (!_serializationModifierDefinitionsPerTypeCache.TryGetValue(type, out var serializerModifierTypes))
            {
                serializerModifierTypes = new List<Type>();
                _serializationModifierDefinitionsPerTypeCache[type] = serializerModifierTypes;
            }

            Log.Debug($"Removing serializer modifier '{serializerModifierType.Name}' for '{type.Name}'");

            if (!serializerModifierTypes.Contains(serializerModifierType))
            {
                Log.Debug($"Serializer modifier '{serializerModifierType.Name}' for '{type.Name}' is not registered");
                return;
            }

            serializerModifierTypes.Remove(serializerModifierType);
            _serializationModifiersPerTypeCache.Remove(type);
        }

        /// <summary>
        /// Gets the serializer modifiers for the specified type.
        /// <para />
        /// Note that the order is important because the modifiers will be called in the returned order during serialization
        /// and in reversed order during deserialization.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An array containing the modifiers. Never <c>null</c>, but can be an empty array.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual ISerializerModifier[] GetSerializerModifiers(Type type)
        {
            Argument.IsNotNull("type", type);

            return _serializationModifiersPerTypeCache.GetFromCacheOrFetch(type, () =>
            {
                var serializers = new List<ISerializerModifier>();
                var serializerModifierTypes = FindSerializerModifiers(type);

                foreach (var serializerModifierType in serializerModifierTypes)
                {
                    var innerAttribute = serializerModifierType;
                    serializers.Add(_serializerModifierCache.GetFromCacheOrFetch(serializerModifierType, () =>
                    {
                        return (ISerializerModifier)_typeFactory.CreateInstance(serializerModifierType);
                    }));
                }

                return serializers.ToArray();
            });
        }

        /// <summary>
        /// Finds the serializer modifiers.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of modifier attributes found.</returns>
        protected virtual List<Type> FindSerializerModifiers(Type type)
        {
            var modifiers = new List<Type>();

            if (_serializationModifierDefinitionsPerTypeCache.TryGetValue(type, out var customModifierTypes))
            {
                modifiers.AddRange(customModifierTypes);
            }

            var attributes = type.GetCustomAttributesEx(typeof(SerializerModifierAttribute), true);
            foreach (var attribute in attributes)
            {
                modifiers.Add(((SerializerModifierAttribute)attribute).SerializerModifierType);
            }

            modifiers.Reverse();

            return modifiers;
        }

        private string GetNameOverrideForSerialization(MemberInfo memberInfo)
        {
            var name = string.Empty;

            DataMemberAttribute dataMemberAttribute = null;
            if (memberInfo.TryGetAttribute(out dataMemberAttribute))
            {
                if (!string.IsNullOrWhiteSpace(dataMemberAttribute.Name))
                {
                    name = dataMemberAttribute.Name;
                }
            }

            IncludeInSerializationAttribute includeInSerializationAttribute = null;
            if (memberInfo.TryGetAttribute(out includeInSerializationAttribute))
            {
                if (!string.IsNullOrWhiteSpace(includeInSerializationAttribute.Name))
                {
                    name = includeInSerializationAttribute.Name;
                }
            }

            return name;
        }

        private string GetCacheKey(Type type, bool additionalValue)
        {
            var key = $"{type.AssemblyQualifiedName}_{additionalValue.ToString()}";
            return key;
        }
    }
}

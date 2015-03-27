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

        private readonly ICacheStorage<Type, HashSet<string>> _fieldsToSerializeCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, HashSet<string>> _propertiesToSerializeCache = new CacheStorage<Type, HashSet<string>>();

        private readonly ICacheStorage<Type, HashSet<string>> _catelPropertyNamesCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _catelPropertiesCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

        private readonly ICacheStorage<Type, HashSet<string>> _regularPropertyNamesCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _regularPropertiesCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

        private readonly ICacheStorage<Type, HashSet<string>> _fieldNamesCache = new CacheStorage<Type, HashSet<string>>();
        private readonly ICacheStorage<Type, Dictionary<string, MemberMetadata>> _fieldsCache = new CacheStorage<Type, Dictionary<string, MemberMetadata>>();

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
            Argument.IsNotNull(() =>  type);

            lock (_lock)
            {
                GetFieldsToSerialize(type);
                GetPropertiesToSerialize(type);

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
                _propertiesToSerializeCache.Remove(type);

                _catelPropertyNamesCache.Remove(type);
                _catelPropertiesCache.Remove(type);

                _regularPropertyNamesCache.Remove(type);
                _regularPropertiesCache.Remove(type);

                _fieldNamesCache.Remove(type);
                _fieldsCache.Remove(type);

                _serializerModifierCache.Remove(type);
                _serializationModifiersPerTypeCache.Remove(type);
            }

            CacheInvalidated.SafeInvoke(this, new CacheInvalidatedEventArgs(type));
        }

        /// <summary>
        /// Gets the fields to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of fields to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual HashSet<string> GetFieldsToSerialize(Type type)
        {
            Argument.IsNotNull("type", type);

            return _fieldsToSerializeCache.GetFromCacheOrFetch(type, () =>
            {
                var fields = new HashSet<string>();

                var typeFields = type.GetFieldsEx();
                foreach (var typeField in typeFields)
                {
                    if (AttributeHelper.IsDecoratedWithAttribute<IncludeInSerializationAttribute>(typeField))
                    {
                        fields.Add(typeField.Name);
                    }
                }

                return fields;
            });
        }

        /// <summary>
        /// Gets the properties to serialize for the specified object.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of properties to serialize.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public virtual HashSet<string> GetPropertiesToSerialize(Type type)
        {
            Argument.IsNotNull("type", type);

            return _propertiesToSerializeCache.GetFromCacheOrFetch(type, () =>
            {
                var properties = new HashSet<string>();

                var propertyDataManager = PropertyDataManager.Default;
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(type);
                var catelProperties = catelTypeInfo.GetCatelProperties();
                var catelPropertyNames = new HashSet<string>(catelProperties.Keys);
                foreach (var modelProperty in catelProperties)
                {
                    var propertyData = modelProperty.Value;

                    bool isSerializable = typeof(ModelBase).IsAssignableFromEx(propertyData.Type) || propertyData.IsSerializable;
                    if (!isSerializable)
                    {
                        // CTL-550
                        var cachedPropertyInfo = propertyData.GetPropertyInfo(type);
                        if (AttributeHelper.IsDecoratedWithAttribute<IncludeInSerializationAttribute>(cachedPropertyInfo.PropertyInfo))
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
                        if (!AttributeHelper.IsDecoratedWithAttribute<ExcludeFromSerializationAttribute>(propertyInfo.PropertyInfo))
                        {
                            properties.Add(modelProperty.Key);
                        }
                    }
                    else
                    {
                        // Dynamic property, always include
                        properties.Add(modelProperty.Key);
                    }
                }

                var typeProperties = type.GetPropertiesEx();
                foreach (var typeProperty in typeProperties)
                {
                    if (!catelPropertyNames.Contains(typeProperty.Name))
                    {
                        if (AttributeHelper.IsDecoratedWithAttribute<IncludeInSerializationAttribute>(typeProperty))
                        {
                            properties.Add(typeProperty.Name);
                        }
                    }
                }

                return properties;
            });
        }

        /// <summary>
        /// Gets the catel property names.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the Catel property names.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public HashSet<string> GetCatelPropertyNames(Type type)
        {
            Argument.IsNotNull("type", type);

            return _catelPropertyNamesCache.GetFromCacheOrFetch(type, () =>
            {
                var propertyDataManager = PropertyDataManager.Default;
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(type);
                var properties = (from property in catelTypeInfo.GetCatelProperties()
                                  where !property.Value.IsModelBaseProperty
                                  select property.Key);

                return new HashSet<string>(properties);
            });
        }

        /// <summary>
        /// Gets the catel properties.
        /// </summary>
        /// <param name="type">Type of the model.</param>
        /// <returns>A hash set containing the Catel properties.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public Dictionary<string, MemberMetadata> GetCatelProperties(Type type)
        {
            Argument.IsNotNull("type", type);

            return _catelPropertiesCache.GetFromCacheOrFetch(type, () =>
            {
                var dictionary = new Dictionary<string, MemberMetadata>();

                var propertyDataManager = PropertyDataManager.Default;
                var catelTypeInfo = propertyDataManager.GetCatelTypeInfo(type);
                var properties = (from property in catelTypeInfo.GetCatelProperties()
                                  where !property.Value.IsModelBaseProperty
                                  select property.Value).ToList();

                foreach (var property in properties)
                {
                    dictionary[property.Name] = new MemberMetadata(type, property.Type, SerializationMemberGroup.CatelProperty, property.Name);
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
                var catelPropertyNames = GetCatelPropertyNames(type);
                var propertyNames = GetPropertiesToSerialize(type);

                var finalProperties = new HashSet<string>();
                foreach (var propertyName in propertyNames)
                {
                    if (!catelPropertyNames.Contains(propertyName))
                    {
                        finalProperties.Add(propertyName);
                    }
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

                var regularPropertyNames = GetRegularPropertyNames(type);
                foreach (var property in regularPropertyNames)
                {
                    var propertyInfo = type.GetPropertyEx(property);
                    if (propertyInfo != null)
                    {
                        dictionary[property] = new MemberMetadata(type, propertyInfo.PropertyType, SerializationMemberGroup.RegularProperty, property);
                    }
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
                var fieldNames = GetFieldsToSerialize(type);

                var finalFields = new HashSet<string>();
                foreach (var fieldName in fieldNames)
                {
                    finalFields.Add(fieldName);
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

                var fieldNames = GetFieldNames(type);
                foreach (var field in fieldNames)
                {
                    var fieldInfo = type.GetFieldEx(field);
                    if (fieldInfo != null)
                    {
                        dictionary[field] = new MemberMetadata(type, fieldInfo.FieldType, SerializationMemberGroup.Field, field);
                    }
                }

                return dictionary;
            });
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
                var serializerModifierAttributes = FindSerializerModifiers(type);

                foreach (var serializerModifierAttribute in serializerModifierAttributes)
                {
                    var innerAttribute = serializerModifierAttribute;
                    serializers.Add(_serializerModifierCache.GetFromCacheOrFetch(innerAttribute.SerializerModifierType, () =>
                    {
                        return (ISerializerModifier)_typeFactory.CreateInstance(innerAttribute.SerializerModifierType);
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
        private List<SerializerModifierAttribute> FindSerializerModifiers(Type type)
        {
            var modifiers = new List<SerializerModifierAttribute>();

            var attributes = type.GetCustomAttributesEx(typeof(SerializerModifierAttribute), true);
            foreach (var attribute in attributes)
            {
                modifiers.Add((SerializerModifierAttribute)attribute);
            }

            modifiers.Reverse();

            return modifiers;
        }
    }
}
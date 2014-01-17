// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataContractSerializerFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Catel.Caching;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Default implementation of the <see cref="IDataContractSerializerFactory" /> interface.
    /// </summary>
    public class DataContractSerializerFactory : IDataContractSerializerFactory
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Cache for the <see cref="DataContractSerializer"/> per name.
        /// </summary>
        private readonly CacheStorage<string, DataContractSerializer> _dataContractSerializersCache = new CacheStorage<string, DataContractSerializer>(storeNullValues: true);

        /// <summary>
        /// Cache for known attributes per type.
        /// </summary>
        private readonly CacheStorage<string, Type[]> _knownTypesByAttributesCache = new CacheStorage<string, Type[]>();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContractSerializerFactory"/> class.
        /// </summary>
        public DataContractSerializerFactory()
        {
        }
        #endregion

        #region IDataContractSerializerFactory Members
        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="rootNamespace">The root namespace.</param>
        /// <param name="serializingObject">The object to create the serializer for. When the object is not <c>null</c>, the types that are
        /// a child object of this object are added to the known types of the serializer.</param>
        /// <param name="additionalKnownTypes">A list of additional types to add to the known types.</param>
        /// <returns><see cref="DataContractSerializer" /> for the given type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName" /> is <c>null</c> or whitespace.</exception>
        public virtual DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, string rootNamespace = null, object serializingObject = null, List<Type> additionalKnownTypes = null)
        {
            Argument.IsNotNull("serializingType", serializingType);
            Argument.IsNotNull("typeToSerialize", typeToSerialize);
            Argument.IsNotNullOrWhitespace("xmlName", xmlName);

            string key = string.Format("{0}|{1}", typeToSerialize.GetSafeFullName(), xmlName);

            return _dataContractSerializersCache.GetFromCacheOrFetch(key, () =>
            {
                Log.Debug("Getting known types for xml serialization of '{0}'", typeToSerialize.GetSafeFullName());

                var serializerTypeInfo = new XmlSerializerTypeInfo(serializingType, typeToSerialize, additionalKnownTypes);

                if (serializingObject != null)
                {
                    GetKnownTypesForInstance(serializingObject, serializerTypeInfo);
                }
                else
                {
                    GetKnownTypes(typeToSerialize, serializerTypeInfo);
                }

                var knownTypesViaAttributes = GetKnownTypesViaAttributes(serializingType);
                foreach (var knownTypeViaAttribute in knownTypesViaAttributes)
                {
                    GetKnownTypes(knownTypeViaAttribute, serializerTypeInfo);
                }

                if (additionalKnownTypes != null)
                {
                    foreach (var additionalKnownType in additionalKnownTypes)
                    {
                        GetKnownTypes(additionalKnownType, serializerTypeInfo);
                    }
                }

                var xmlSerializer = new DataContractSerializer(typeToSerialize, xmlName, rootNamespace ?? string.Empty, serializerTypeInfo.KnownTypes);
                return xmlSerializer;
            });
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the known types for a specific object instance.
        /// </summary>
        /// <param name="obj">The object to retrieve the known types for.</param>
        /// <param name="serializerTypeInfo">The serializer type info.</param>
        /// <returns>Array of <see cref="Type"/> that are found in the object instance.</returns>
        protected virtual void GetKnownTypesForInstance(object obj, XmlSerializerTypeInfo serializerTypeInfo)
        {
            if (obj == null)
            {
                return;
            }

            Type objectType = obj.GetType();

            if (ShouldTypeBeIgnored(objectType, serializerTypeInfo))
            {
                return;
            }

            GetKnownTypes(objectType, serializerTypeInfo);

            // Note: the code below is specific for instants of objects, cannot be moved to the GetKnownTypes

            if (objectType == typeof(List<KeyValuePair<string, object>>))
            {
                foreach (var keyValuePair in ((List<KeyValuePair<string, object>>)obj))
                {
                    GetKnownTypesForInstance(keyValuePair.Value, serializerTypeInfo);
                }
            }
            else if (objectType == typeof(List<PropertyValue>))
            {
                foreach (var propertyValue in ((List<PropertyValue>)obj))
                {
                    GetKnownTypesForInstance(propertyValue.Value, serializerTypeInfo);
                }
            }

            // Collections might contain interface types, so if this is an IEnumerable, we need to loop all the instances (performance warning!)
            else if ((obj is IEnumerable) && (!(obj is string)))
            {
                var objAsIEnumerable = (IEnumerable)obj;
                foreach (object item in objAsIEnumerable)
                {
                    if (item != null)
                    {
                        GetKnownTypesForInstance(item, serializerTypeInfo);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the known types inside the specific type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerTypeInfo">The serializer type info.</param>
        /// <returns>Array of <see cref="Type"/> that are found in the object type.</returns>
        protected virtual void GetKnownTypes(Type type, XmlSerializerTypeInfo serializerTypeInfo)
        {
            if (ShouldTypeBeIgnored(type, serializerTypeInfo))
            {
                return;
            }

            Log.Debug("Getting known types for '{0}'", type.GetSafeFullName());

            // If this is an interface, HOUSTON, WE HAVE A PROBLEM
            if (type.IsInterfaceEx())
            {
                if (!serializerTypeInfo.IsTypeAlreadyHandled(type))
                {
                    Log.Debug("Type is an interface, checking all types deriving from this interface");

                    // Don't check this interface again in children checks
                    serializerTypeInfo.AddTypeAsHandled(type);

                    // Interfaces are not a type, and in fact a LOT of types can be added (in fact every object implementing the interface). For
                    // serialization, this is not a problem (we know the exact type), but for deserialization this IS an issue because we should
                    // expect EVERY type that implements the type in the whole AppDomain.
                    // This is huge performance hit, but it's the cost for dynamic easy on-the-fly serialization in WPF and Silverlight. Luckily
                    // we already implemented caching.
                    var typesDerivingFromInterface = TypeCache.GetTypes(t => t.ImplementsInterfaceEx(type));
                    foreach (var typeDerivingFromInterface in typesDerivingFromInterface)
                    {
                        if (typeDerivingFromInterface != type)
                        {
                            GetKnownTypes(typeDerivingFromInterface, serializerTypeInfo);
                        }
                    }

                    Log.Debug("Finished checking all types deriving from this interface");
                }

                // The interface itself is ignored
                return;
            }

            if (serializerTypeInfo.IsSpecialCollectionType(type) && !type.IsInterfaceEx())
            {
                Log.Debug("Type is a special collection type, adding it to the array of known types");

                AddTypeToKnownTypesIfSerializable(type, serializerTypeInfo);
            }

            // Fix generics
            if (type.GetSafeFullName().StartsWith("System."))
            {
                var genericArguments = type.GetGenericArgumentsEx();
                foreach (var genericArgument in genericArguments)
                {
                    Log.Debug("Retrieving known types for generic argument '{0}' of '{1}'", genericArgument.GetSafeFullName(), type.GetSafeFullName());

                    GetKnownTypes(genericArgument, serializerTypeInfo);
                }

                return;
            }

            if (!AddTypeToKnownTypesIfSerializable(type, serializerTypeInfo))
            {
                serializerTypeInfo.AddTypeAsHandled(type);
            }

            AddTypeMembers(type, serializerTypeInfo);

            // If this isn't the base type, check that as well
            var baseType = type.GetBaseTypeEx();
            if (baseType != null)
            {
                Log.Debug("Checking base type of '{0}' for known types", type.GetSafeFullName());

                if (baseType.FullName != null)
                {
                    GetKnownTypes(baseType, serializerTypeInfo);
                }
                else
                {
                    serializerTypeInfo.AddTypeAsHandled(baseType);
                }
            }

            // Last but not least, check if the type is decorated with KnownTypeAttributes
            var knowTypesByAttributes = GetKnownTypesViaAttributes(type);
            if (knowTypesByAttributes.Length > 0)
            {
                Log.Debug("Found {0} additional known types for type '{1}'", knowTypesByAttributes.Length, type.GetSafeFullName());

                foreach (var knownTypeByAttribute in knowTypesByAttributes)
                {
                    var attributeType = knownTypeByAttribute;
                    if (attributeType.FullName != null)
                    {
                        GetKnownTypes(knownTypeByAttribute, serializerTypeInfo);
                    }
                    else
                    {
                        serializerTypeInfo.AddTypeAsHandled(attributeType);
                    }
                }
            }
        }

        private void AddTypeMembers(Type type, XmlSerializerTypeInfo serializerTypeInfo)
        {
            var isModelBase = (type == typeof(ModelBase)) || typeof(ModelBase).IsAssignableFromEx(type);
            if (isModelBase)
            {
                var catelTypeInfo = PropertyDataManager.Default.GetCatelTypeInfo(type);
                var modelBaseProperties = catelTypeInfo.GetCatelProperties();
                foreach (var modelBaseProperty in modelBaseProperties)
                {
                    var propertyType = modelBaseProperty.Value.Type;
                    if (propertyType.FullName != null)
                    {
                        GetKnownTypes(propertyType, serializerTypeInfo);
                    }
                    else
                    {
                        serializerTypeInfo.AddTypeAsHandled(propertyType);
                    }
                }
            }
            else
            {
                bool allowNonPublicReflection = AllowNonPublicReflection(type);

                // Fields
                var fields = type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(false, false, allowNonPublicReflection));
                foreach (var field in fields)
                {
                    var fieldType = field.FieldType;
                    if (fieldType.FullName != null)
                    {
                        GetKnownTypes(fieldType, serializerTypeInfo);
                    }
                    else
                    {
                        serializerTypeInfo.AddTypeAsHandled(fieldType);
                    }
                }

                // Properties
                var properties = type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(false, false, allowNonPublicReflection));
                foreach (var property in properties)
                {
                    var propertyType = property.PropertyType;
                    if (propertyType.FullName != null)
                    {
                        GetKnownTypes(propertyType, serializerTypeInfo);
                    }
                    else
                    {
                        serializerTypeInfo.AddTypeAsHandled(propertyType);
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the type should be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="serializerTypeInfo">The serializer type info.</param>
        /// <returns><c>true</c> if the type should be handled; otherwise, <c>false</c>.</returns>
        protected virtual bool ShouldTypeBeIgnored(Type type, XmlSerializerTypeInfo serializerTypeInfo)
        {
            if (type == null)
            {
                return true;
            }

            // Note, although resharper says this isn't possible, it might be
            if (type.FullName == null)
            {
                serializerTypeInfo.AddTypeAsHandled(type);
                return true;
            }

            // Ignore non-generic .NET
            if (!type.IsGenericTypeEx() && type.GetSafeFullName().StartsWith("System."))
            {
                // Log.Debug("Non-generic .NET system type, can be ignored");
                serializerTypeInfo.AddTypeAsHandled(type);
                return true;
            }

            return serializerTypeInfo.ContainsKnownType(type) || 
                   serializerTypeInfo.IsTypeAlreadyHandled(type) || 
                   serializerTypeInfo.IsCollectionAlreadyHandled(type);
        }

        /// <summary>
        /// Gets the known types via attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of known types via the <see cref="KnownTypeAttribute"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        protected virtual Type[] GetKnownTypesViaAttributes(Type type)
        {
            Argument.IsNotNull("type", type);

            string typeName = type.AssemblyQualifiedName;

            return _knownTypesByAttributesCache.GetFromCacheOrFetch(typeName, () =>
            {
                var additionalTypes = new List<Type>();
                var knownTypeAttributes = type.GetCustomAttributesEx(typeof(KnownTypeAttribute), true);
                foreach (var attr in knownTypeAttributes)
                {
                    var ktattr = attr as KnownTypeAttribute;
                    if (ktattr != null)
                    {
                        if (ktattr.MethodName != null)
                        {
                            var mi = type.GetMethodEx(ktattr.MethodName, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                            // this can be null because we are also getting here through the recursive behaviour
                            // of GetCustomAttributesEx. We are getting at this point once per class derived from a
                            // base class having a KnownType() with a method. This can be ignored
                            if (mi != null)
                            {
                                var types = mi.Invoke(null, null) as IEnumerable<Type>;
                                if (types != null)
                                {
                                    additionalTypes.AddRange(types);
                                }
                            }
                        }
                        else
                        {
                            additionalTypes.Add(ktattr.Type);
                        }
                    }
                }

                return additionalTypes.ToArray();
            });
        }

        /// <summary>
        /// Adds the type to the known types if the type is serializable.
        /// </summary>
        /// <param name="typeToAdd">The type to add.</param>
        /// <param name="serializerTypeInfo">The serializer type info.</param>
        /// <returns><c>true</c> if the type is serializable; otherwise <c>false</c>.</returns>
        protected virtual bool AddTypeToKnownTypesIfSerializable(Type typeToAdd, XmlSerializerTypeInfo serializerTypeInfo)
        {
            // Collection first, this collection of types is smaller so if we have a hit, we exit sooner
            if (serializerTypeInfo.IsCollectionAlreadyHandled(typeToAdd))
            {
                return true;
            }

            if (serializerTypeInfo.ContainsKnownType(typeToAdd))
            {
                return true;
            }

            serializerTypeInfo.AddCollectionAsHandled(typeToAdd);

            // If this is a special collection type (generic), then we need to make sure that if the inner type is
            // an interface, we do not add it again if already added.
            // See this issue http://catel.codeplex.com/workitem/7167
            if (serializerTypeInfo.IsSpecialCollectionType(typeToAdd))
            {
                // Always ignore as a test
                return false;

                // TODO: Also check interfaces
                //    // It might be a base type, loop all
                //    var baseType = typeToAdd;
                //    while (baseType != null)
                //    {
                //        if (baseType.IsGenericTypeEx())
                //        {
                //            if (baseType.GetGenericArgumentsEx()[0].IsInterfaceEx())
                //            {
                //                var genericTypeDefinition = baseType.GetGenericTypeDefinitionEx();

                //                var allPossibleMatchingTypes = (from type in serializerTypeInfo.SpecialGenericCollectionTypes
                //                                                where type.GetGenericTypeDefinitionEx() == genericTypeDefinition
                //                                                select type).ToList();

                //                if (allPossibleMatchingTypes.Count > 0)
                //                {
                //                    Log.Debug("Skipping type '{0}' because there already exists such a type which does the same", typeToAdd.GetSafeFullName());
                //                    return false;
                //                }
                //            }
                //        }

                //        baseType = baseType.GetBaseTypeEx();
                //    }
            }

            return serializerTypeInfo.AddKnownType(typeToAdd);
        }

        /// <summary>
        /// Returns whether non-public reflection is allowed on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if non-public reflection is allowed, <c>false</c> otherwise.</returns>
        protected virtual bool AllowNonPublicReflection(Type type)
        {
#if NET
            bool allowNonPublicReflection = (type == typeof(ModelBase)) || !typeof(ModelBase).IsAssignableFromEx(type);
#else
            bool allowNonPublicReflection = false;
#endif

            return allowNonPublicReflection;
        }
        #endregion

        #region Nested type: XmlSerializerTypeInfo
        /// <summary>
        /// Class containing serializer type info.
        /// </summary>
        protected class XmlSerializerTypeInfo
        {
            #region Fields
            private readonly HashSet<Type> _collectionTypesAlreadyHandled = new HashSet<Type>();
            private readonly Dictionary<Type, bool> _isSpecialCollectionCache = new Dictionary<Type, bool>();

            private readonly HashSet<Type> _knownTypes = new HashSet<Type>();
            private readonly HashSet<Type> _specialCollectionTypes = new HashSet<Type>();
            private readonly HashSet<Type> _specialGenericCollectionTypes = new HashSet<Type>();
            private readonly HashSet<Type> _typesAlreadyHandled = new HashSet<Type>();

            private readonly CacheStorage<Type, bool> _isTypeSerializableCache = new CacheStorage<Type, bool>();
            #endregion

            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="XmlSerializerTypeInfo" /> class.
            /// </summary>
            /// <param name="serializingType">Type of the serializing.</param>
            /// <param name="typeToSerialize">The type to serialize.</param>
            /// <param name="additionalKnownTypes">The additional known types.</param>
            public XmlSerializerTypeInfo(Type serializingType, Type typeToSerialize, IEnumerable<Type> additionalKnownTypes = null)
            {
                SerializingType = serializingType;
                TypeToSerialize = typeToSerialize;

                if (additionalKnownTypes != null)
                {
                    foreach (var additionalKnownType in additionalKnownTypes)
                    {
                        _knownTypes.Add(additionalKnownType);
                    }
                }
            }
            #endregion

            #region Properties
            /// <summary>
            /// Gets the serializing type.
            /// </summary>
            /// <value>The serializing type.</value>
            public Type SerializingType { get; private set; }

            /// <summary>
            /// Gets the type to serialize.
            /// </summary>
            /// <value>The type to serialize.</value>
            public Type TypeToSerialize { get; private set; }

            /// <summary>
            /// Gets the known types.
            /// </summary>
            /// <value>The known types.</value>
            public IEnumerable<Type> KnownTypes
            {
                get { return _knownTypes; }
            }

            /// <summary>
            /// Gets the types already handled.
            /// </summary>
            /// <value>The types already handled.</value>
            public IEnumerable<Type> TypesAlreadyHandled
            {
                get { return _typesAlreadyHandled; }
            }

            /// <summary>
            /// Gets the special collection types.
            /// </summary>
            /// <value>The special collection types.</value>
            public IEnumerable<Type> SpecialCollectionTypes
            {
                get { return _specialCollectionTypes; }
            }

            /// <summary>
            /// Gets the special generic collection types.
            /// </summary>
            /// <value>The special generic collection types.</value>
            public IEnumerable<Type> SpecialGenericCollectionTypes
            {
                get { return _specialGenericCollectionTypes; }
            }
            #endregion

            #region Methods
            /// <summary>
            /// Adds the type to the list of known types.
            /// </summary>
            /// <param name="type">The type.</param>
            public bool AddKnownType(Type type)
            {
                if (!IsTypeSerializable(type))
                {
                    return false;
                }

                _knownTypes.Add(type);

                if (IsSpecialCollectionType(type))
                {
                    _specialCollectionTypes.Add(type);

                    if (type.IsGenericTypeEx())
                    {
                        _specialGenericCollectionTypes.Add(type);
                    }
                }

                return true;
            }

            /// <summary>
            /// Determines whether the specified type is a known type.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns><c>true</c> if the specified type is known type; otherwise, <c>false</c>.</returns>
            public bool ContainsKnownType(Type type)
            {
                return _knownTypes.Contains(type);
            }

            /// <summary>
            /// Adds the type as handled.
            /// </summary>
            /// <param name="type">The type.</param>
            public void AddTypeAsHandled(Type type)
            {
                _typesAlreadyHandled.Add(type);
            }

            /// <summary>
            /// Determines whether the specified type is already handled, which doesn't mean that it is also
            /// a known type. It means that the type has already been inspected once.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns><c>true</c> if the type is already handled; otherwise, <c>false</c>.</returns>
            public bool IsTypeAlreadyHandled(Type type)
            {
                return _typesAlreadyHandled.Contains(type);
            }

            /// <summary>
            /// Adds the collection type as handled.
            /// </summary>
            /// <param name="type">The type.</param>
            public void AddCollectionAsHandled(Type type)
            {
                _collectionTypesAlreadyHandled.Add(type);
            }

            /// <summary>
            /// Determines whether the specified collection type is already handled.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns><c>true</c> if the collection type is already handled; otherwise, <c>false</c>.</returns>
            public bool IsCollectionAlreadyHandled(Type type)
            {
                return _collectionTypesAlreadyHandled.Contains(type);
            }

            /// <summary>
            /// Determines whether the specified type is a special .NET collection type which should be
            /// added to the serialization known types.
            /// <para />
            /// All generic collections in the <c>System.Collections.Generic</c> namespace are considered
            /// special. Besides these classes, the <c>ObservableCollection{T}</c> is also considered
            /// special.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns><c>true</c> if the specified type is a special collection type; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
            public bool IsSpecialCollectionType(Type type)
            {
                if (_isSpecialCollectionCache.ContainsKey(type))
                {
                    return _isSpecialCollectionCache[type];
                }

                // Check all sub types as well (a type might be deriving from IEnumerable)
                var baseType = type;
                while (baseType != null)
                {
                    if (IsSpecificTypeSpecialCollection(baseType))
                    {
                        // Note: this is not a bug, we need to add type to the collection
                        _isSpecialCollectionCache[type] = true;
                        return true;
                    }

                    baseType = baseType.GetBaseTypeEx();
                }

                var implementedInterfaces = type.GetInterfacesEx();
                foreach (var implementedInterface in implementedInterfaces)
                {
                    if (IsSpecificTypeSpecialCollection(implementedInterface))
                    {
                        // Note: this is not a bug, we need to add type to the collection
                        _isSpecialCollectionCache[type] = true;
                        return true;
                    }
                }

                _isSpecialCollectionCache[type] = false;
                return false;
            }

            /// <summary>
            /// Determines whether the specified type is serializable.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <returns><c>true</c> if the specified type is serializable; otherwise, <c>false</c>.</returns>
            /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
            public bool IsTypeSerializable(Type type)
            {
                return _isTypeSerializableCache.GetFromCacheOrFetch(type, () =>
                {
                    if (type.IsAbstractEx())
                    {
                        return false;
                    }

                    if (type.IsInterfaceEx())
                    {
                        return false;
                    }

                    if (type.IsEnumEx())
                    {
                        return true;
                    }

                    if (IsSpecialCollectionType(type))
                    {
                        return true;
                    }

                    // Should have an empty constructor
                    if (type.GetConstructorEx(new Type[0]) == null)
                    {
                        return false;
                    }

                    // Type must be public
                    if (!type.IsPublicEx() && !type.IsNestedPublicEx())
                    {
                        return false;
                    }

                    // TODO: Add more checks?

                    return true;
                });
            }

            private bool IsSpecificTypeSpecialCollection(Type type)
            {
                if (type.IsGenericTypeEx())
                {
                    var fullName = type.GetSafeFullName();

                    if (fullName.StartsWith("System.Collections.ObjectModel.ObservableCollection") ||
                        fullName.StartsWith("System.Collections.Generic."))
                    {
                        // Note: this is not a bug, we need to add type to the collection
                        return true;
                    }
                }

                return false;
            }
            #endregion
        }
        #endregion
    }
}
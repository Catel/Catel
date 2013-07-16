// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using Collections;
    using Data;
    using Logging;
    using Reflection;

#if NET
    using System.IO;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;
#elif NETFX_CORE
    using Windows.Storage.Streams;
#elif PCL
    // Not supported in Portable Class Library
#else
    using System.IO.IsolatedStorage;
#endif

    /// <summary>
    /// Class that makes serialization much easier and safer.
    /// </summary>
    public static class SerializationHelper
    {
        #region Fields
        /// <summary>
        /// The <see cref="ILog">log</see> object.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET
        /// <summary>
        /// Cache for the <see cref="XmlSerializer"/> per name.
        /// </summary>
        private static readonly Dictionary<string, System.Xml.Serialization.XmlSerializer> _xmlSerializers = new Dictionary<string, System.Xml.Serialization.XmlSerializer>();
#endif

        /// <summary>
        /// Cache for the <see cref="DataContractSerializer"/> per name.
        /// </summary>
        private static readonly Dictionary<string, DataContractSerializer> _dataContractSerializers = new Dictionary<string, DataContractSerializer>();

        /// <summary>
        /// Cache for known attributes per type.
        /// </summary>
        private static readonly Dictionary<string, Type[]> _knownTypesByAttributesCache = new Dictionary<string, Type[]>();
        #endregion

        #region XmlSerializer
#if NET
        /// <summary>
        /// Gets the XML serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="type">The type to get the xml serializer for.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <returns><see cref="XmlSerializer"/> for the given type.</returns>
        public static System.Xml.Serialization.XmlSerializer GetXmlSerializer(Type type, string xmlName)
        {
            string key = string.Format("{0}|{1}", type.Name, xmlName);

            if (_xmlSerializers.ContainsKey(key))
            {
                return _xmlSerializers[key];
            }

            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(type, new XmlRootAttribute(xmlName));

            _xmlSerializers.Add(key, xmlSerializer);

            return xmlSerializer;
        }

        /// <summary>
        /// Gets the <see cref="BinaryFormatter"/> for binary (de)serialization.
        /// </summary>
        /// <param name="supportRedirects">if set to <c>true</c>, redirects of types are supported. This is substantially slower.</param>
        /// <returns><see cref="BinaryFormatter"/> with the requested options.</returns>
        public static BinaryFormatter GetBinarySerializer(bool supportRedirects)
        {
            Log.Debug("Creating binary serializer");

            var formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.FilterLevel = TypeFilterLevel.Full;
            formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;

            if (supportRedirects)
            {
                formatter.Binder = new RedirectDeserializationBinder();
            }

            Log.Debug("Created binary serializer");

            return formatter;
        }

        /// <summary>
        /// Retrieves a string from a SerializationInfo object.
        /// </summary>
        /// <param name="info">SerializationInfo object.</param>
        /// <param name="name">Name of the value to retrieve.</param>
        /// <param name="defaultValue">Default value when value does not exist.</param>
        /// <returns>String value.</returns>
        public static string GetString(SerializationInfo info, string name, string defaultValue)
        {
            return GetObject(info, name, defaultValue);
        }

        /// <summary>
        /// Retrieves an integer from a SerializationInfo object.
        /// </summary>
        /// <param name="info">SerializationInfo object</param>
        /// <param name="name">Name of the value to retrieve.</param>
        /// <param name="defaultValue">Default value when value does not exist.</param>
        /// <returns>Integer value.</returns>
        public static int GetInt(SerializationInfo info, string name, int defaultValue)
        {
            return GetObject(info, name, defaultValue);
        }

        /// <summary>
        /// Retrieves a boolean from a SerializationInfo object.
        /// </summary>
        /// <param name="info">SerializationInfo object.</param>
        /// <param name="name">Name of the value to retrieve.</param>
        /// <param name="defaultValue">Default value when value does not exist.</param>
        /// <returns>Boolean value.</returns>
        public static bool GetBool(SerializationInfo info, string name, bool defaultValue)
        {
            return GetObject(info, name, defaultValue);
        }

        /// <summary>
        /// Retrieves an object from a SerializationInfo object.
        /// </summary>
        /// <typeparam name="T">Type of the value to read from the serialization information.</typeparam>
        /// <param name="info">SerializationInfo object.</param>
        /// <param name="name">Name of the value to retrieve.</param>
        /// <param name="defaultValue">Default value when value does not exist.</param>
        /// <returns>object value.</returns>
        public static T GetObject<T>(SerializationInfo info, string name, T defaultValue)
        {
            Type type = typeof(T);
            object value = GetObject(info, name, type, defaultValue);
            return ((value != null) && (value is T)) ? (T)value : defaultValue;
        }

        /// <summary>
        /// Retrieves an object from a SerializationInfo object.
        /// </summary>
        /// <param name="info">SerializationInfo object.</param>
        /// <param name="name">Name of the value to retrieve.</param>
        /// <param name="type">Type of the object to retrieve.</param>
        /// <param name="defaultValue">Default value when value does not exist.</param>
        /// <returns>object value.</returns>
        public static object GetObject(SerializationInfo info, string name, Type type, object defaultValue)
        {
            try
            {
                object obj = info.GetValue(name, type);
                return obj ?? defaultValue;
            }
            catch (InvalidCastException)
            {
                Log.Debug("Value for '{0}' must implement IConvertible, probably because it's trying to convert nothing to an object", name);
                return defaultValue;
            }
            catch (SerializationException)
            {
                Log.Debug("Name '{0}' is not found in the SerializationInfo, returning default value '{1}'", name, ObjectToStringHelper.ToString(defaultValue));
                return defaultValue;
            }
        }

        /// <summary>
        /// Serializes the XML.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="obj">The object.</param>
        /// <returns><c>true</c> if the object is serialized to xml successfully; otherwise <c>false</c>.</returns>
        public static bool SerializeXml(string fileName, object obj)
        {
            // Failed by default
            bool succeeded = false;

            try
            {
                if (obj != null)
                {
                    Directory.CreateDirectory(IO.Path.GetParentDirectory(fileName));

                    using (var fs = File.Create(fileName))
                    {
                        var xs = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                        xs.Serialize(fs, obj);

                        succeeded = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to serialize object");
            }

            return succeeded;
        }

        /// <summary>
        /// Deserializes the specified file name to an object.
        /// </summary>
        /// <typeparam name="T">Type of the object that is contained in the file.</typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Deserialized type or <c>null</c> if not successful.</returns>
        public static T DeserializeXml<T>(string fileName)
            where T : class, new()
        {
            T result = null;

            try
            {
                Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read);

                var xs = new System.Xml.Serialization.XmlSerializer(typeof(T));

                result = (T)xs.Deserialize(stream);

                stream.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to deserialize object");
            }

            return result ?? new T();
        }
#endif
        #endregion

        #region DataContractSerializer
        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="loadFromCache">if set to <c>true</c>, the serializer is retrieved from the cache if possible.</param>
        /// <returns>
        /// <see cref="DataContractSerializer"/> for the given type.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public static DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, bool loadFromCache = true)
        {
            return GetDataContractSerializer(serializingType, typeToSerialize, xmlName, null, loadFromCache);
        }

        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="additionalKnownTypes">A list of additional types to add to the known types.</param>
        /// <param name="loadFromCache">if set to <c>true</c>, the serializer is retrieved from the cache if possible.</param>
        /// <returns>
        /// <see cref="DataContractSerializer"/> for the given type.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public static DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, List<Type> additionalKnownTypes, bool loadFromCache = true)
        {
            return GetDataContractSerializer(serializingType, typeToSerialize, xmlName, null, additionalKnownTypes, loadFromCache);
        }

        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="obj">The object to create the serializer for. When the object is not <c>null</c>, the types that are
        /// a child object of this object are added to the known types of the serializer.</param>
        /// <param name="loadFromCache">if set to <c>true</c>, the serializer is retrieved from the cache if possible.</param>
        /// <returns>
        /// <see cref="DataContractSerializer"/> for the given type.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public static DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, object obj, bool loadFromCache = true)
        {
            return GetDataContractSerializer(serializingType, typeToSerialize, xmlName, obj, null, loadFromCache);
        }

        /// <summary>
        /// Gets the Data Contract serializer for a specific type. This method caches serializers so the
        /// performance can be improved when a serializer is used more than once.
        /// </summary>
        /// <param name="serializingType">The type that is currently (de)serializing.</param>
        /// <param name="typeToSerialize">The type to (de)serialize.</param>
        /// <param name="xmlName">Name of the property as known in XML.</param>
        /// <param name="obj">The object to create the serializer for. When the object is not <c>null</c>, the types that are
        /// a child object of this object are added to the known types of the serializer.</param>
        /// <param name="additionalKnownTypes">A list of additional types to add to the known types.</param>
        /// <param name="loadFromCache">if set to <c>true</c>, the serializer is retrieved from the cache if possible.</param>
        /// <returns>
        /// <see cref="DataContractSerializer"/> for the given type.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="serializingType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToSerialize"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="xmlName"/> is <c>null</c> or whitespace.</exception>
        public static DataContractSerializer GetDataContractSerializer(Type serializingType, Type typeToSerialize, string xmlName, object obj, List<Type> additionalKnownTypes, bool loadFromCache = true)
        {
            Argument.IsNotNull("serializingType", serializingType);
            Argument.IsNotNull("typeToSerialize", typeToSerialize);
            Argument.IsNotNullOrWhitespace("xmlName", xmlName);

            string key = string.Empty;

            if (loadFromCache)
            {
                key = string.Format("{0}|{1}", typeToSerialize.FullName, xmlName);
                if (_dataContractSerializers.ContainsKey(key))
                {
                    return _dataContractSerializers[key];
                }
            }

            Log.Debug("Getting known types for xml serialization of '{0}'", typeToSerialize.Name);

            var alreadyCheckedTypes = new HashSet<Type>();

            Type[] knownTypes = (obj != null) ? GetKnownTypesForInstance(obj, new HashSet<Type>(), alreadyCheckedTypes) : GetKnownTypes(typeToSerialize, new HashSet<Type>(), alreadyCheckedTypes);

            var knownTypesViaAttributes = GetKnownTypesViaAttributes(serializingType);
            foreach (var knownTypeViaAttribute in knownTypesViaAttributes)
            {
                knownTypes = GetKnownTypes(knownTypeViaAttribute, new HashSet<Type>(knownTypes), alreadyCheckedTypes);
            }

            if (additionalKnownTypes != null)
            {
                foreach (var additionalKnownType in additionalKnownTypes)
                {
                    knownTypes = GetKnownTypes(additionalKnownType, new HashSet<Type>(knownTypes), alreadyCheckedTypes);
                }
            }

            var xmlSerializer = new DataContractSerializer(typeToSerialize, xmlName, string.Empty, knownTypes);

            if (!string.IsNullOrEmpty(key))
            {
                _dataContractSerializers[key] = xmlSerializer;
            }

            return xmlSerializer;
        }

        /// <summary>
        /// Gets the known types for a specific object instance.
        /// </summary>
        /// <param name="obj">The object to retrieve the known types for.</param>
        /// <param name="knownTypeList">The known type list.</param>
        /// <param name="alreadyCheckedTypes">The already checked types.</param>
        /// <returns>
        /// Array of <see cref="Type"/> that are found in the object instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="knownTypeList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="alreadyCheckedTypes"/> is <c>null</c>.</exception>
        private static Type[] GetKnownTypesForInstance(object obj, HashSet<Type> knownTypeList, HashSet<Type> alreadyCheckedTypes)
        {
            Argument.IsNotNull("knownTypeList", knownTypeList);
            Argument.IsNotNull("alreadyCheckedTypes", alreadyCheckedTypes);

            if (obj == null)
            {
                return knownTypeList.ToArray();
            }

            Type objectType = obj.GetType();

            if (!ShouldTypeBeHandled(objectType, knownTypeList, alreadyCheckedTypes))
            {
                return knownTypeList.ToArray();
            }

            Log.Debug("Getting known types for instance of '{0}'", objectType.FullName);

            GetKnownTypes(objectType, knownTypeList, alreadyCheckedTypes);

            if (objectType.FullName == null)
            {
                alreadyCheckedTypes.Add(objectType);
                return knownTypeList.ToArray();
            }

            // Collections might contain interface types, so if this is an IEnumerable, we need to loop all the instances (performance warning!)
            var objAsIEnumerable = obj as IEnumerable;
            if ((objAsIEnumerable != null) && (!(obj is string)))
            {
                foreach (object item in objAsIEnumerable)
                {
                    if (item != null)
                    {
                        GetKnownTypesForInstance(item, knownTypeList, alreadyCheckedTypes);
                    }
                }
            }

            if (objectType == typeof(List<KeyValuePair<string, object>>))
            {
                foreach (var keyValuePair in ((List<KeyValuePair<string, object>>)obj))
                {
                    GetKnownTypesForInstance(keyValuePair.Value, knownTypeList, alreadyCheckedTypes);
                }
            }
            else if (objectType == typeof(List<PropertyValue>))
            {
                foreach (var propertyValue in ((List<PropertyValue>)obj))
                {
                    GetKnownTypesForInstance(propertyValue.Value, knownTypeList, alreadyCheckedTypes);
                }
            }
            // Generic collections are special in Silverlight and WP7 (WHY?!)
            else if (IsSpecialCollectionType(objectType) && !objectType.IsInterfaceEx())
            {
                AddTypeToKnownTypesIfSerializable(knownTypeList, objectType);
            }
            else if (!objectType.FullName.StartsWith("System."))
            {
                var fields = objectType.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(false, false));
                foreach (var field in fields)
                {
                    try
                    {
                        object value = field.GetValue(obj);
                        GetKnownTypes(value == null ? field.FieldType : value.GetType(), knownTypeList, alreadyCheckedTypes);
                    }
                    catch (Exception)
                    {
                        Log.Warning("Failed to get value for field '{0}' of type '{1}'", field.Name, objectType.Name);
                    }
                }

                var properties = objectType.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(false, false));
                foreach (var property in properties)
                {
                    try
                    {
                        object value = property.GetValue(obj, null);
                        GetKnownTypes(value == null ? property.PropertyType : value.GetType(), knownTypeList, alreadyCheckedTypes);
                    }
                    catch (Exception)
                    {
                        Log.Warning("Failed to get value for property '{0}' of type '{1}'", property.Name, objectType.Name);
                    }
                }
            }

            return knownTypeList.ToArray();
        }

        /// <summary>
        /// Gets the known types inside the specific type.
        /// </summary>
        /// <param name="type">The type to retrieve the known types for.</param>
        /// <param name="knownTypeList">The known type list.</param>
        /// <param name="alreadyCheckedTypes">The already checked types.</param>
        /// <returns>
        /// Array of <see cref="Type"/> that are found in the object type.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="knownTypeList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="alreadyCheckedTypes"/> is <c>null</c>.</exception>
        private static Type[] GetKnownTypes(Type type, HashSet<Type> knownTypeList, HashSet<Type> alreadyCheckedTypes)
        {
            if (!ShouldTypeBeHandled(type, knownTypeList, alreadyCheckedTypes))
            {
                return knownTypeList.ToArray();
            }

            Log.Debug("Getting known types for '{0}'", type.FullName);

            // If this is an interface, HOUSTON, WE HAVE A PROBLEM
            if (type.IsInterfaceEx())
            {
                if (!alreadyCheckedTypes.Contains(type))
                {
                    Log.Debug("Type is an interface, checking all types deriving from this interface");

                    // Don't check this interface again in children checks
                    alreadyCheckedTypes.Add(type);

                    // Interfaces are not a type, and in fact a LOT of types can be added (in fact every object implementing the interface). For
                    // serialization, this is not a problem (we know the exact type), but for deserialization this IS an issue because we should
                    // expect EVERY type that implements the type in the whole AppDomain.
                    //
                    // This is huge performance hit, but it's the cost for dynamic easy on-the-fly serialization in WPF and Silverlight. Luckily
                    // we already implemented caching.
                    var typesDerivingFromInterface = TypeCache.GetTypes(t => t.ImplementsInterfaceEx(type));
                    foreach (var typeDerivingFromInterface in typesDerivingFromInterface)
                    {
                        if (typeDerivingFromInterface != type)
                        {
                            GetKnownTypes(typeDerivingFromInterface, knownTypeList, alreadyCheckedTypes);
                        }
                    }

                    Log.Debug("Finished checking all types deriving from this interface");
                }

                // The interface itself is ignored
                return knownTypeList.ToArray();
            }

            if (IsSpecialCollectionType(type) && !type.IsInterfaceEx())
            {
                Log.Debug("Type is a special collection type, adding it to the array of known types");

                AddTypeToKnownTypesIfSerializable(knownTypeList, type);
            }

            // Fix generics
            if (type.FullName.StartsWith("System."))
            {
                var genericArguments = type.GetGenericArgumentsEx();
                foreach (var genericArgument in genericArguments)
                {
                    Log.Debug("Retrieving known types for generic argument '{0}' of '{1}'", genericArgument.FullName, type.FullName);

                    GetKnownTypes(genericArgument, knownTypeList, alreadyCheckedTypes);
                }

                return knownTypeList.ToArray();
            }

            if (!AddTypeToKnownTypesIfSerializable(knownTypeList, type))
            {
                alreadyCheckedTypes.Add(type);
            }

            // Fields
            var fields = type.GetFieldsEx(BindingFlagsHelper.GetFinalBindingFlags(false, false));
            foreach (var field in fields)
            {
                Log.Debug("Getting known types for field '{0}' of '{1}'", field.Name, type.FullName);

                var fieldType = field.FieldType;
                if (fieldType.FullName != null)
                {
                    GetKnownTypes(fieldType, knownTypeList, alreadyCheckedTypes);
                }
                else
                {
                    alreadyCheckedTypes.Add(fieldType);
                }
            }

            // Properties
            var properties = type.GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(false, false));
            foreach (var property in properties)
            {
                Log.Debug("Getting known types for property '{0}' of '{1}'", property.Name, type.FullName);

                var propertyType = property.PropertyType;
                if (propertyType.FullName != null)
                {
                    GetKnownTypes(propertyType, knownTypeList, alreadyCheckedTypes);
                }
                else
                {
                    alreadyCheckedTypes.Add(propertyType);
                }
            }

            // If this isn't the base type, check that as well
            var baseType = type.GetBaseTypeEx();
            if (baseType != null)
            {
                Log.Debug("Checking base type of '{0}' for known types", type.FullName);

                if (baseType.FullName != null)
                {
                    GetKnownTypes(baseType, knownTypeList, alreadyCheckedTypes);
                }
                else
                {
                    alreadyCheckedTypes.Add(baseType);
                }
            }

            // Last but not least, check if the type is decorated with KnownTypeAttributes
            var knowTypesByAttributes = GetKnownTypesViaAttributes(type);
            if (knowTypesByAttributes.Length > 0)
            {
                Log.Debug("Found {0} additional known types for type '{1}'", knowTypesByAttributes.Length, type.FullName);

                foreach (var knownTypeByAttribute in knowTypesByAttributes)
                {
                    var attributeType = knownTypeByAttribute;
                    if (attributeType.FullName != null)
                    {
                        GetKnownTypes(knownTypeByAttribute, knownTypeList, alreadyCheckedTypes);
                    }
                    else
                    {
                        alreadyCheckedTypes.Add(attributeType);
                    }
                }
            }

            return knownTypeList.ToArray();
        }

        /// <summary>
        /// Determines whether the type should be handled.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="knownTypeList">The known type list.</param>
        /// <param name="alreadyCheckedTypes">The already checked types.</param>
        /// <returns><c>true</c> if the type should be handled; otherwise, <c>false</c>.</returns>
        /// <remarks></remarks>
        private static bool ShouldTypeBeHandled(Type type, HashSet<Type> knownTypeList, HashSet<Type> alreadyCheckedTypes)
        {
            if (type == null)
            {
                return false;
            }

            if (type.FullName == null)
            {
                alreadyCheckedTypes.Add(type);
                return false;
            }

            // Ignore non-generic .NET
            if (!type.IsGenericTypeEx() && type.FullName.StartsWith("System."))
            {
                //Log.Debug("Non-generic .NET system type, can be ignored");
                alreadyCheckedTypes.Add(type);
                return false;
            }

            return !knownTypeList.Contains(type) && !alreadyCheckedTypes.Contains(type);
        }

        /// <summary>
        /// Gets the known types via attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The list of known types via the <see cref="KnownTypeAttribute"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static Type[] GetKnownTypesViaAttributes(Type type)
        {
            Argument.IsNotNull("type", type);

            string typeName = type.AssemblyQualifiedName;
            lock (_knownTypesByAttributesCache)
            {
                if (!_knownTypesByAttributesCache.ContainsKey(typeName))
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

                    _knownTypesByAttributesCache.Add(typeName, additionalTypes.ToArray());
                }

                return _knownTypesByAttributesCache[typeName];
            }
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
        /// <returns>
        /// <c>true</c> if the specified type is a special collection type; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static bool IsSpecialCollectionType(Type type)
        {
            Argument.IsNotNull("type", type);

            if (!type.IsGenericTypeEx())
            {
                return false;
            }

            if (type.FullName == null)
            {
                return false;
            }

            if (type.FullName.StartsWith("System.Collections.ObjectModel.ObservableCollection") ||
                type.FullName.StartsWith("System.Collections.Generic."))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds the type to the known types if the type is serializable.
        /// </summary>
        /// <param name="knownTypesList">The known types list.</param>
        /// <param name="typeToAdd">The type to add.</param>
        /// <returns><c>true</c> if the type is serializable; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="knownTypesList"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeToAdd"/> is <c>null</c>.</exception>
        private static bool AddTypeToKnownTypesIfSerializable(HashSet<Type> knownTypesList, Type typeToAdd)
        {
            Argument.IsNotNull("knownTypesList", knownTypesList);
            Argument.IsNotNull("typeToAdd", typeToAdd);

            if (knownTypesList.Contains(typeToAdd))
            {
                return true;
            }

            // If this is a special collection type (generic), then we need to make sure that if the inner type is
            // an interface, we do not add it again if already added.
            //
            // See this issue http://catel.codeplex.com/workitem/7167
            if (IsSpecialCollectionType(typeToAdd))
            {
                if (typeToAdd.GetGenericArgumentsEx()[0].IsInterfaceEx())
                {
                    var genericTypeDefinition = typeToAdd.GetGenericTypeDefinitionEx();

                    if ((from type in knownTypesList
                         where type.IsGenericTypeEx() && IsSpecialCollectionType(type) && type.GetGenericArgumentsEx()[0].IsInterfaceEx() &&
                               type.GetGenericTypeDefinitionEx() == genericTypeDefinition
                         select type).Any())
                    {
                        return false;
                    }
                }
            }

            if (IsSerializable(typeToAdd))
            {
                knownTypesList.Add(typeToAdd);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type is serializable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is serializable; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static bool IsSerializable(Type type)
        {
            Argument.IsNotNull("type", type);

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
        }

#if !PCL
        /// <summary>
        /// Serializes the XML.
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// <c>true</c> if the object is serialized to xml successfully; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <param ref="fileStream" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <param ref="obj" /> is <c>null</c>.</exception>
#if NET
        public static bool SerializeXml(FileStream fileStream, object obj)
#elif NETFX_CORE
        public static bool SerializeXml(IRandomAccessStream fileStream, object obj)
#else
        public static bool SerializeXml(IsolatedStorageFileStream fileStream, object obj)
#endif
        {
            Argument.IsNotNull("fileStream", fileStream);
            Argument.IsNotNull("obj", obj);

            bool succeeded = false;

            try
            {
                if (obj != null)
                {
#if NETFX_CORE
                    throw new NotSupportedException("Unfortunately, this is not yet supported in WinRT");
                    //var stream = fileStream.GetOutputStreamAt(0);
                    //var dataWriter = new DataWriter(stream);
                    //serializer.WriteObject(dataWriter, obj);
#else
                    var serializer = new DataContractSerializer(obj.GetType());
                    serializer.WriteObject(fileStream, obj);
                    succeeded = true;
#endif
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to serialize object");
            }

            return succeeded;
        }

        /// <summary>
        /// Deserializes the specified file name to an object.
        /// </summary>
        /// <typeparam name="T">Type of the object that is contained in the file.</typeparam>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>
        /// Deserialized type or <c>null</c> if not successful.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <param ref="fileStream" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <param ref="obj" /> is <c>null</c>.</exception>
#if NET
        public static T DeserializeXml<T>(FileStream fileStream)
#elif NETFX_CORE
        public static T DeserializeXml<T>(IRandomAccessStream fileStream)
#else
        public static T DeserializeXml<T>(IsolatedStorageFileStream fileStream)
#endif
 where T : class, new()
        {
            Argument.IsNotNull("fileStream", fileStream);

            T result = null;

            try
            {
                var serializer = new DataContractSerializer(typeof(T));
#if NETFX_CORE
                throw new NotSupportedException("Unfortunately, this is not yet supported in WinRT");
#else
                result = (T)serializer.ReadObject(fileStream);
#endif
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to deserialize object");
            }

            if (result == null)
            {
                result = new T();
            }

            return result;
        }
#endif // PLC
        #endregion
    }
}
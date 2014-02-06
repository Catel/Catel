// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializerHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Caching;
    using IoC;
    using Reflection;

    /// <summary>
    /// The binary serializer helper.
    /// </summary>
    public class BinarySerializerHelper
    {
        #region Constants
        /// <summary>
        /// The cache
        /// </summary>
        private static readonly ICacheStorage<string, IObjectMetaData> Cache = new CacheStorage<string, IObjectMetaData>();
        #endregion

        #region Methods
        /// <summary>
        /// Discovers the and serialize.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The object.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectType"/> is <c>null</c>.</exception>
        public static void DiscoverAndSerialize(Stream stream, object obj, Type objectType)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("obj", obj);
            Argument.IsNotNull("objectType", objectType);

            lock (Cache)
            {
                IObjectMetaData objectMetaData;
                if (Cache.Contains(objectType.FullName))
                {
                    objectMetaData = Cache[objectType.FullName];
                }
                else
                {
                    objectMetaData = new ObjectMetaData();
                    DiscoverTypeInfo(objectType, objectMetaData);
                    Cache.Add(objectType.FullName, objectMetaData);
                }

                Serialize(stream, obj, objectMetaData);
            }
        }

        /// <summary>
        /// Discovers the and decimal serialize.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectType"/> is <c>null</c>.</exception>
        public static object DiscoverAndDeSerialize(Stream stream, Type objectType)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("objectType", objectType);

            lock (Cache)
            {
                IObjectMetaData objectMetaData;
                if (Cache.Contains(objectType.FullName))
                {
                    objectMetaData = Cache[objectType.FullName];
                }
                else
                {
                    objectMetaData = new ObjectMetaData();
                    DiscoverTypeInfo(objectType, objectMetaData);
                    Cache.Add(objectType.FullName, objectMetaData);
                }
                return DeSerialize(stream, objectMetaData);
            }
        }

        /// <summary>
        /// Serializes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="obj">The object.</param>
        /// <param name="objectMetaData">The object meta data.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectMetaData"/> is <c>null</c>.</exception>
        public static void Serialize(Stream stream, object obj, IObjectMetaData objectMetaData)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("objectMetaData", objectMetaData);

            if (obj == null || objectMetaData.DataType == DataType.Undefined)
            {
                Write(stream, "NuLl");
            }
            else if (objectMetaData.DataType != DataType.Collection && objectMetaData.DataType != DataType.Object)
            {
                Write(stream, obj);
            }
            else if (objectMetaData.DataType == DataType.Collection)
            {
                var collection = obj as ICollection;
                if (collection == null)
                {
                    return;
                }

                Write(stream, collection.Count);
                foreach (var item in collection)
                {
                    Serialize(stream, item, objectMetaData.ChildObjMetaData);
                }
            }
            else
            {
                foreach (var propertyMetaData in objectMetaData.PropertiesMetaData)
                {
                    Serialize(stream, propertyMetaData.PropertyInfo.GetValue(obj, null), propertyMetaData);
                }
            }
        }

        /// <summary>
        /// Decimals the serialize.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="objectMetaData">The object meta data.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectMetaData"/> is <c>null</c>.</exception>
        public static object DeSerialize(Stream stream, IObjectMetaData objectMetaData)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("objectMetaData", objectMetaData);

            var position = stream.Position;
            if ((string) Read(stream, typeof (string)) == "NuLl" || objectMetaData.DataType == DataType.Undefined)
            {
                return null;
            }
            if (objectMetaData.DataType != DataType.Collection && objectMetaData.DataType != DataType.Object)
            {
                stream.Position = position;
                return Read(stream, objectMetaData.Type);
            }
            var obj = TypeFactory.Default.CreateInstance(objectMetaData.Type);
            stream.Position = position;

            if (obj is ICollection)
            {
                var count = (int) Read(stream, typeof (int));

                for (var i = 0; i < count; i++)
                {
                    obj.GetType()
                        .GetMethod("Add")
                        .Invoke(obj, new[] {DeSerialize(stream, objectMetaData.ChildObjMetaData)});
                }
                return obj;
            }

            foreach (var propertyMetaData in objectMetaData.PropertiesMetaData)
            {
                propertyMetaData.PropertyInfo.SetValue(obj, DeSerialize(stream, propertyMetaData), null);
            }
            return obj;
        }

        /// <summary>
        /// Writes the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="System.Exception">Data Type not supported.</exception>
        private static void Write(Stream stream, object value)
        {
            while (true)
            {
                int size;

                if (value is Int16)
                {
                    size = sizeof (Int16);
                    var val = Convert.ToInt16(value);
                    for (var i = 0; i < size; i++)
                    {
                        stream.WriteByte((byte) val);
                        val >>= 8;
                    }
                }
                else if (value is Int32)
                {
                    size = sizeof (Int32);
                    var val = Convert.ToInt32(value);
                    for (var i = 0; i < size; i++)
                    {
                        stream.WriteByte((byte) val);
                        val >>= 8;
                    }
                }
                else if (value is Int64)
                {
                    size = sizeof (Int64);
                    var val = Convert.ToInt64(value);
                    for (var i = 0; i < size; i++)
                    {
                        stream.WriteByte((byte) val);
                        val >>= 8;
                    }
                }
                else if (value is char)
                {
                    size = sizeof (char);
                    var val = Convert.ToChar(value);
                    for (var i = 0; i < size; i++)
                    {
                        stream.WriteByte((byte) val);
                        val >>= 8;
                    }
                }
                else if (value is bool)
                {
                    value = (short) (Convert.ToBoolean(value) ? 1 : 0);
                    continue;
                }
                else if (value is Single || value is Double || value is Decimal || value is DateTime || value is string)
                {
                    WriteString(stream, value.ToString());
                }
                else
                {
                    throw new Exception("Data Type not supported.");
                }
                break;
            }
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Data Type not supported.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static object Read(Stream stream, Type type)
        {
            Argument.IsNotNull("stream", stream);
            Argument.IsNotNull("type", type);
            
            int size;

            if (type == typeof (Int16))
            {
                size = sizeof (Int16);

                var result = 0;
                for (var i = 0; i < size; i++)
                {
                    var temp = stream.ReadByte();
                    result |= temp << (i*8);
                }
                return result;
            }
            if (type == typeof (Int32))
            {
                size = sizeof (Int32);

                var result = 0;
                for (var i = 0; i < size; i++)
                {
                    var temp = stream.ReadByte();
                    result |= temp << (i*8);
                }
                return result;
            }
            if (type == typeof (Int64))
            {
                size = sizeof (Int64);

                Int64 result = 0;
                for (var i = 0; i < size; i++)
                {
                    Int64 temp = stream.ReadByte();
                    result |= temp << (i*8);
                }
                return result;
            }
            if (type == typeof (char))
            {
                size = sizeof (char);

                var result = 0;
                for (var i = 0; i < size; i++)
                {
                    var temp = stream.ReadByte();
                    result |= temp << (i*8);
                }
                return (char) result;
            }
            if (type == typeof (bool))
            {
                return Convert.ToInt16(Read(stream, typeof (Int16))) == 1;
            }
            if (type == typeof (Single))
            {
                return Single.Parse(ReadString(stream));
            }
            if (type == typeof (Double))
            {
                return Double.Parse(ReadString(stream));
            }
            if (type == typeof (Decimal))
            {
                return Decimal.Parse(ReadString(stream));
            }
            if (type == typeof (DateTime))
            {
                return DateTime.Parse(ReadString(stream));
            }
            if (type == typeof (string))
            {
                return ReadString(stream);
            }

            throw new Exception("Data Type not supported.");
        }

        /// <summary>
        /// Writes the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="value">The value.</param>
        private static void WriteString(Stream stream, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            Write(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Reads the string.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private static string ReadString(Stream stream)
        {
            var size = (int) Read(stream, typeof (int));
            if (size <= 0)
            {
                return string.Empty;
            }

            var buffer = new byte[size];
            stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Discovers the type information.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="objectMetaData">The object meta data.</param>
        private static void DiscoverTypeInfo(Type objectType, IObjectMetaData objectMetaData)
        {
            while (true)
            {
                if (objectType == null)
                {
                    return;
                }

                objectMetaData.Type = objectType;
                var dataType = GetType(objectType);

                if (dataType != DataType.Undefined)
                {
                    return;
                }

                var collectionElementType = GetElementType(objectType);
                if (collectionElementType != null)
                {
                    objectMetaData.DataType = DataType.Collection;
                    objectMetaData.CollectionItemType = collectionElementType;

                    objectMetaData.ChildObjMetaData = new ObjectMetaData();
                    objectType = collectionElementType;
                    objectMetaData = objectMetaData.ChildObjMetaData;
                    continue;
                }
                objectMetaData.DataType = DataType.Object;
                objectMetaData.PropertiesMetaData = new List<IObjectMetaData>();

                foreach (var propertyInfo in objectType.GetProperties().OrderBy(property => property.Name))
                {
                    if (!propertyInfo.PropertyType.IsPublicEx() || propertyInfo.GetSetMethod() == null)
                    {
                        continue;
                    }
                    var propertyMetaData = new ObjectMetaData {PropertyInfo = propertyInfo};

                    DiscoverTypeInfo(propertyInfo.PropertyType, propertyMetaData);

                    objectMetaData.PropertiesMetaData.Add(propertyMetaData);
                }
                break;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        private static DataType GetType(Type type)
        {
            return type == typeof (Int16)
                ? DataType.Int16
                : (type == typeof (Int32)
                    ? DataType.Int32
                    : (type == typeof (Int64)
                        ? DataType.Int64
                        : (type == typeof (Decimal)
                            ? DataType.Decimal
                            : (type == typeof (Double)
                                ? DataType.Double
                                : (type == typeof (Single)
                                    ? DataType.Single
                                    : (type == typeof (bool)
                                        ? DataType.Bool
                                        : (type == typeof (DateTime)
                                            ? DataType.DateTime
                                            : (type == typeof (Char)
                                                ? DataType.Char
                                                : (type == typeof (String) ? DataType.String : DataType.Undefined)))))))));
        }

        /// <summary>
        /// Gets the type of the element.
        /// </summary>
        /// <param name="collectionType">Type of the collection.</param>
        /// <returns></returns>
        private static Type GetElementType(Type collectionType)
        {
            return (collectionType.GetInterfacesEx().Where(intType => intType.IsGenericTypeEx() &&
                                                                      (intType.GetGenericTypeDefinitionEx() ==
                                                                       typeof (IList<>) ||
                                                                       intType.GetGenericTypeDefinitionEx() ==
                                                                       typeof (ICollection<>)))
                .Select(intType => intType.GetGenericArgumentsEx()[0])).FirstOrDefault();
        }
        #endregion
    }
}
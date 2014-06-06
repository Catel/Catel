// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.type.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#pragma warning disable 1591

//#define ENABLE_CACHE

namespace Catel.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
#if ENABLE_CACHE
        /// <summary>
        /// The _constructors cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, ConstructorInfo[]> _constructorsCache = new CacheStorage<ReflectionCacheKey, ConstructorInfo[]>(storeNullValues: true);

        /// <summary>
        /// The _constructor cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, ConstructorInfo> _constructorCache = new CacheStorage<ReflectionCacheKey, ConstructorInfo>(storeNullValues: true);

        /// <summary>
        /// The _fields cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, FieldInfo[]> _fieldsCache = new CacheStorage<ReflectionCacheKey, FieldInfo[]>(storeNullValues: true);

        /// <summary>
        /// The _field cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, FieldInfo> _fieldCache = new CacheStorage<ReflectionCacheKey, FieldInfo>(storeNullValues: true);

        /// <summary>
        /// The _properties cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, PropertyInfo[]> _propertiesCache = new CacheStorage<ReflectionCacheKey, PropertyInfo[]>(storeNullValues: true);

        /// <summary>
        /// The _property cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, PropertyInfo> _propertyCache = new CacheStorage<ReflectionCacheKey, PropertyInfo>(storeNullValues: true);

        /// <summary>
        /// The _events cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, EventInfo[]> _eventsCache = new CacheStorage<ReflectionCacheKey, EventInfo[]>(storeNullValues: true);

        /// <summary>
        /// The _event cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, EventInfo> _eventCache = new CacheStorage<ReflectionCacheKey, EventInfo>(storeNullValues: true);

        /// <summary>
        /// The _methods cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, MethodInfo[]> _methodsCache = new CacheStorage<ReflectionCacheKey, MethodInfo[]>(storeNullValues: true);

        /// <summary>
        /// The _method cache.
        /// </summary>
        private static readonly CacheStorage<ReflectionCacheKey, MethodInfo> _methodCache = new CacheStorage<ReflectionCacheKey, MethodInfo>(storeNullValues: true);
#endif

        /// <summary>
        /// Dictionary containing all possible implicit conversions of system types.
        /// </summary>
        private static readonly Dictionary<Type, List<Type>> _convertableDictionary = new Dictionary<Type, List<Type>>
            {
                {typeof (decimal), new List<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char)}},
                {typeof (double), new List<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char), typeof (float)}},
                {typeof (float), new List<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char), typeof (float)}},
                {typeof (ulong), new List<Type> {typeof (byte), typeof (ushort), typeof (uint), typeof (char)}},
                {typeof (long), new List<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (char)}},
                {typeof (uint), new List<Type> {typeof (byte), typeof (ushort), typeof (char)}},
                {typeof (int), new List<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (char)}},
                {typeof (ushort), new List<Type> {typeof (byte), typeof (char)}},
                {typeof (short), new List<Type> {typeof (byte)}}
            };

        /// <summary>
        /// Gets the parent types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if ((type == null) || (type.GetBaseTypeEx() == null))
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfacesEx())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.GetBaseTypeEx();
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.GetBaseTypeEx();
            }
        }

        /// <summary>
        /// Gets the full name of the type in a safe way. This means it checks for null first.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The safe full name.</returns>
        public static string GetSafeFullName(this Type type)
        {
            if (type == null)
            {
                return "NullType";
            }

            if (type.FullName == null)
            {
                return type.Name;
            }

            return type.FullName;
        }

        /// <summary>
        /// The get custom attribute ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">The attribute type.</param>
        /// <param name="inherit">The inherit.</param>
        /// <returns>The get custom attribute ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static Attribute GetCustomAttributeEx(this Type type, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("attributeType", attributeType);

            var attributes = GetCustomAttributesEx(type, attributeType, inherit);
            return (attributes.Length > 0) ? attributes[0] : null;
        }

        /// <summary>
        /// The get custom attributes ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="inherit">The inherit.</param>
        /// <returns>System.Object[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Attribute[] GetCustomAttributesEx(this Type type, bool inherit)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().GetCustomAttributes(inherit).ToArray();
#else
            return type.GetCustomAttributes(inherit).ToAttributeArray();
#endif
        }

        /// <summary>
        /// The get custom attributes ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">The attribute type.</param>
        /// <param name="inherit">The inherit.</param>
        /// <returns>System.Object[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static Attribute[] GetCustomAttributesEx(this Type type, Type attributeType, bool inherit)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("attributeType", attributeType);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
#else
            return type.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
#endif
        }

        /// <summary>
        /// Determines whether the specified type contains generic parameters.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type contains generic parameters; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool ContainsGenericParametersEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().ContainsGenericParameters;
#else
            return type.ContainsGenericParameters;
#endif
        }

        /// <summary>
        /// The get assembly ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Assembly.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Assembly GetAssemblyEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().Assembly;
#else
            return type.Assembly;
#endif
        }

        /// <summary>
        /// The get assembly full name ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The get assembly full name ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static string GetAssemblyFullNameEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().Assembly.FullName;
#else
            return type.Assembly.FullName;
#endif
        }

        /// <summary>
        /// The has base type ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeToCheck">The type to check.</param>
        /// <returns>The has base type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool HasBaseTypeEx(this Type type, Type typeToCheck)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("typeToCheck", typeToCheck);

#if NETFX_CORE || PCL
            return (type.GetTypeInfo().BaseType == typeToCheck);
#else
            return type.BaseType == typeToCheck;
#endif
        }

        /// <summary>
        /// The is serializable ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is serializable ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsSerializableEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsSerializable;
#elif NET
            return type.IsSerializable;
#else
            return true;
#endif
        }

        /// <summary>
        /// The is public ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is public ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsPublicEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
        }

        /// <summary>
        /// The is nested public ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is nested public ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsNestedPublicEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsNestedPublic;
#else
            return type.IsNestedPublic;
#endif
        }

        /// <summary>
        /// The is interface ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is interface ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsInterfaceEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsInterface;
#else
            return type.IsInterface;
#endif
        }

        /// <summary>
        /// Determines whether the specified type is abstract.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is abstract; otherwise, <c>false</c>.</returns>
        public static bool IsAbstractEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsAbstract;
#else
            return type.IsAbstract;
#endif
        }

        /// <summary>
        /// Determines whether the specified type is a class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a class; otherwise, <c>false</c>.</returns>
        public static bool IsClassEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        /// <summary>
        /// The is value type ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is value type ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsValueTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }

        /// <summary>
        /// The is generic type ex.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The is generic type ex.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="type"/> is <c>null</c>.
        /// </exception>
        public static bool IsGenericTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        /// <summary>
        /// Returns whether the specified type implements the specified interface.
        /// </summary>
        /// <typeparam name="TInterface">The type of the t interface.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type implements the interface; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool ImplementsInterfaceEx<TInterface>(this Type type)
        {
            Argument.IsNotNull("type", type);

            return ImplementsInterfaceEx(type, typeof(TInterface));
        }

        /// <summary>
        /// Returns whether the specified type implements the specified interface.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns><c>true</c> if the type implements the interface; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> is <c>null</c>.</exception>
        public static bool ImplementsInterfaceEx(this Type type, Type interfaceType)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("interfaceType", interfaceType);

            return IsAssignableFromEx(interfaceType, type);
        }

        /// <summary>
        /// Returns whether the specified type is a primitive type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The primitive type specification.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsPrimitiveEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }

        /// <summary>
        /// The is enum ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is enum ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsEnumEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        /// <summary>
        /// Determines whether the specified type is a COM object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCOMObjectEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return false;
#else
            return type.IsCOMObject;
#endif
        }

        /// <summary>
        /// Gets the generic type definition of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The generic type definition.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The specified type is not a generic type.</exception>
        public static Type GetGenericTypeDefinitionEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            if (!IsGenericTypeEx(type))
            {
                throw new NotSupportedException(string.Format("The type '{0}' is not generic, cannot get generic type", type.FullName));
            }

#if NETFX_CORE || PCL
            return type.GetTypeInfo().GetGenericTypeDefinition();
#else
            return type.GetGenericTypeDefinition();
#endif
        }

        /// <summary>
        /// The get generic arguments ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type[][].</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type[] GetGenericArgumentsEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        /// <summary>
        /// The get interfaces ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type[] GetInterfacesEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().ImplementedInterfaces.ToArray();
#else
            return type.GetInterfaces();
#endif
        }

        /// <summary>
        /// The get base type ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type GetBaseTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        /// <summary>
        /// The is assignable from ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeToCheck">The type to check.</param>
        /// <returns>The is assignable from ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeToCheck" /> is <c>null</c>.</exception>
        public static bool IsAssignableFromEx(this Type type, Type typeToCheck)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("typeToCheck", typeToCheck);

#if NETFX_CORE || PCL
            return type.GetTypeInfo().IsAssignableFrom(typeToCheck.GetTypeInfo());
#else
            return type.IsAssignableFrom(typeToCheck);
#endif
        }

        /// <summary>
        /// The is instance of type ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="objectToCheck">The object to check.</param>
        /// <returns>The is instance of type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectToCheck" /> is <c>null</c>.</exception>
        public static bool IsInstanceOfTypeEx(this Type type, object objectToCheck)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("objectToCheck", objectToCheck);

            var instanceType = objectToCheck.GetType();

            if (_convertableDictionary.ContainsKey(type) && _convertableDictionary[type].Contains(instanceType))
            {
                return true;
            }

            if (type.IsAssignableFromEx(instanceType))
            {
                return true;
            }

            bool castable = (from method in type.GetMethodsEx(BindingFlags.Public | BindingFlags.Static)
                             where method.ReturnType == instanceType &&
                                   method.Name.Equals("op_Implicit", StringComparison.Ordinal) ||
                                   method.Name.Equals("op_Explicit", StringComparison.Ordinal)
                             select method).Any();

            return castable;
        }

        /// <summary>
        /// The get constructor ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="types">The types.</param>
        /// <returns>ConstructorInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="types" /> is <c>null</c>.</exception>
        public static ConstructorInfo GetConstructorEx(this Type type, Type[] types)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("types", types);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Constructor, BindingFlags.Default, types);
#if NETFX_CORE || PCL
            return _constructorCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetConstructor(types, BindingFlagsHelper.GetFinalBindingFlags(false, false)));
#else
            return _constructorCache.GetFromCacheOrFetch(cacheKey, () => type.GetConstructor(types));
#endif
#else
#if NETFX_CORE || PCL
            return type.GetTypeInfo().GetConstructor(types, BindingFlagsHelper.GetFinalBindingFlags(false, false));
#else
            return type.GetConstructor(types);
#endif
#endif
        }

        /// <summary>
        /// The get constructors ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>ConstructorInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static ConstructorInfo[] GetConstructorsEx(this Type type)
        {
            Argument.IsNotNull("type", type);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Constructor, BindingFlags.Default, "allctors");
#if NETFX_CORE || PCL
            return _constructorsCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().DeclaredConstructors.ToArray());
#else
            return _constructorsCache.GetFromCacheOrFetch(cacheKey, type.GetConstructors);
#endif
#else
#if NETFX_CORE || PCL
            return type.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            return type.GetConstructors();
#endif
#endif
        }

        /// <summary>
        /// The get field ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <returns>FieldInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static FieldInfo GetFieldEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetFieldEx(type, name, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get field ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>FieldInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static FieldInfo GetFieldEx(this Type type, string name, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);
#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Field, bindingFlags, name);
            return _fieldCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetField(name, bindingFlags));
#else
            return type.GetTypeInfo().GetField(name, bindingFlags);
#endif
        }

        /// <summary>
        /// The get fields ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <returns>FieldInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static FieldInfo[] GetFieldsEx(this Type type, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetFieldsEx(type, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get fields ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>FieldInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static FieldInfo[] GetFieldsEx(this Type type, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);
#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Field, bindingFlags);
            return _fieldsCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetFields(bindingFlags));
#else
            return type.GetTypeInfo().GetFields(bindingFlags);
#endif
        }

        /// <summary>
        /// The get property ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <returns>PropertyInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static PropertyInfo GetPropertyEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            BindingFlags bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers);
            return GetPropertyEx(type, name, bindingFlags);
        }

        /// <summary>
        /// The get property ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>PropertyInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static PropertyInfo GetPropertyEx(this Type type, string name, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Property, bindingFlags, name);
            return _propertyCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetProperty(name, bindingFlags));
#else
            return type.GetTypeInfo().GetProperty(name, bindingFlags);
#endif
        }

        /// <summary>
        /// The get properties ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <returns>PropertyInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static PropertyInfo[] GetPropertiesEx(this Type type, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetPropertiesEx(type, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get properties ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>PropertyInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static PropertyInfo[] GetPropertiesEx(this Type type, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Property, bindingFlags);
            return _propertiesCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetProperties(bindingFlags));
#else
            return type.GetTypeInfo().GetProperties(bindingFlags);
#endif
        }

        /// <summary>
        /// The get event ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten Hierarchy.</param>
        /// <param name="allowStaticMembers">The allow Static Members.</param>
        /// <returns>EventInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static EventInfo GetEventEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetEventEx(type, name, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get event ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>EventInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static EventInfo GetEventEx(this Type type, string name, BindingFlags bindingFlags)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("type", type);
#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Event, bindingFlags, name);
            return _eventCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetEvent(name, bindingFlags));
#else
            return type.GetTypeInfo().GetEvent(name, bindingFlags);
#endif
        }

        /// <summary>
        /// The get events ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flattenHierarchy">The flatten Hierarchy.</param>
        /// <param name="allowStaticMembers">The allow Static Members.</param>
        /// <returns>EventInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static EventInfo[] GetEventsEx(this Type type, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            Argument.IsNotNull("type", type);
            BindingFlags bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Event, bindingFlags);
            return _eventsCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetEvents(bindingFlags));
#else
            return type.GetTypeInfo().GetEvents(bindingFlags);
#endif
        }

        /// <summary>
        /// The get method ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten Hierarchy.</param>
        /// <param name="allowStaticMembers">The allow Static Members.</param>
        /// <returns>MethodInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static MethodInfo GetMethodEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetMethodEx(type, name, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get method ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>MethodInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static MethodInfo GetMethodEx(this Type type, string name, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Method, bindingFlags, name);
            return _methodCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetMethod(name, bindingFlags));
#else
            return type.GetTypeInfo().GetMethod(name, bindingFlags);
#endif
        }

        /// <summary>
        /// The get method ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="types">The types.</param>
        /// <param name="flattenHierarchy">The flatten Hierarchy.</param>
        /// <param name="allowStaticMembers">The allow Static Members.</param>
        /// <returns>MethodInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static MethodInfo GetMethodEx(this Type type, string name, Type[] types, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetMethodEx(type, name, types, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get method ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="types">The types.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>MethodInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static MethodInfo GetMethodEx(this Type type, string name, Type[] types, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Method, bindingFlags, new object[] {name, types});

#if NETFX_CORE || PCL
            return _methodCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetMethod(name, bindingFlags));
#else
            return _methodCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetMethod(name, bindingFlags, null, types, null));
#endif

#else
#if WP80
            return type.GetTypeInfo().GetMethod(name, bindingFlags, null, types, null);
#elif XAMARIN
            return type.GetTypeInfo().GetMethod(name, types);
#else
            return type.GetTypeInfo().GetMethod(name, types, bindingFlags);
#endif
#endif
        }

        /// <summary>
        /// The get methods ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="flattenHierarchy">The flatten Hierarchy.</param>
        /// <param name="allowStaticMembers">The allow Static Members.</param>
        /// <returns>MethodInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static MethodInfo[] GetMethodsEx(this Type type, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetMethodsEx(type, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// The get methods ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>MethodInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static MethodInfo[] GetMethodsEx(this Type type, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);

#if ENABLE_CACHE
            var cacheKey = new ReflectionCacheKey(type, ReflectionTypes.Method, bindingFlags);
            return _methodsCache.GetFromCacheOrFetch(cacheKey, () => type.GetTypeInfo().GetMethods(bindingFlags));
#else
            return type.GetTypeInfo().GetMethods(bindingFlags);
#endif
        }

#if NET40 || SILVERLIGHT

        /// <summary>
        /// The type infos cache.
        /// </summary>
        private static readonly Dictionary<Type, TypeInfo> _typeInfos = new Dictionary<Type, TypeInfo>();

        /// <summary>
        /// The _sync obj.
        /// </summary>
        private static readonly object _syncObj = new object();

        /// <summary>
        /// Gets the type info.
        /// </summary>
        /// <param name="this">
        /// The this.
        /// </param>
        /// <returns>
        /// The <see cref="TypeInfo"/> instance of the current <see cref="Type"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="this"/> is <c>null</c>.
        /// </exception>
        public static TypeInfo GetTypeInfo(this Type @this)
        {
            Argument.IsNotNull("@this", @this);
            TypeInfo typeInfo;

            // TODO: Create with this code for a readonly cache storage. 
            if (!_typeInfos.ContainsKey(@this))
            {
                // NOTE: Use MultipleReaderExclusiveWriterSynchronizer here!!!.
                lock (_syncObj)
                {
                    if (_typeInfos.ContainsKey(@this))
                    {
                        typeInfo = _typeInfos[@this];
                    }
                    else
                    {
                        typeInfo = new TypeInfo(@this);
                        _typeInfos.Add(@this, typeInfo);
                    }
                }
            }
            else
            {
                // The cache is readonly and never is cleared so we can do this out of lock.
                typeInfo = _typeInfos[@this];
            }

            // TODO: Evaluate if just do 'return new TypeInfo(@this);' is enough
            return typeInfo;
        }

#endif
    }
}
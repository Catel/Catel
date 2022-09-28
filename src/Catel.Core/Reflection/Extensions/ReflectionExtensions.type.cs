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
        /// <summary>
        /// Dictionary containing all possible implicit conversions of system types.
        /// </summary>
        private static readonly Dictionary<Type, HashSet<Type>> ConvertableDictionary = new Dictionary<Type, HashSet<Type>>
            {
                {typeof (decimal), new HashSet<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char)}},
                {typeof (double), new HashSet<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char), typeof (float)}},
                {typeof (float), new HashSet<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (long), typeof (ulong), typeof (char), typeof (float)}},
                {typeof (ulong), new HashSet<Type> {typeof (byte), typeof (ushort), typeof (uint), typeof (char)}},
                {typeof (long), new HashSet<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (int), typeof (uint), typeof (char)}},
                {typeof (uint), new HashSet<Type> {typeof (byte), typeof (ushort), typeof (char)}},
                {typeof (int), new HashSet<Type> {typeof (sbyte), typeof (byte), typeof (short), typeof (ushort), typeof (char)}},
                {typeof (ushort), new HashSet<Type> {typeof (byte), typeof (char)}},
                {typeof (short), new HashSet<Type> {typeof (byte)}}
            };

        /// <summary>
        /// Determines whether the specified type is a Catel type.
        /// </summary>
        /// <param name="type">Type of the target.</param>
        /// <returns>
        /// <c>true</c> if the specified type is a Catel type; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static bool IsCatelType(this Type type)
        {
            Argument.IsNotNull("type", type);

            var assemblyName = type.GetAssemblyFullNameEx();

            return assemblyName.StartsWith("Catel.Core") ||
                assemblyName.StartsWith("Catel.MVVM") ||
                assemblyName.StartsWith("Catel.Serialization");
        }

        /// <summary>
        /// Gets the parent types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if ((type is null) || (type.GetBaseTypeEx() is null))
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
            while (currentBaseType is not null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.GetBaseTypeEx();
            }
        }

        /// <summary>
        /// Gets the full name of the type in a safe way. This means it checks for null first.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="fullyQualifiedAssemblyName">if set to <c>true</c>, include the assembly name in the type name.</param>
        /// <returns>The safe full name.</returns>
        public static string GetSafeFullName(this Type type, bool fullyQualifiedAssemblyName /* in v5, set = false */)
        {
            if (type is null)
            {
                return "NullType";
            }

            var fullName = string.Empty;

            if (type.FullName is not null)
            {
                fullName = type.FullName;
            }
            else
            {
                fullName = type.Name;
            }

            if (fullyQualifiedAssemblyName)
            {
                var assemblyName = "unknown_assembly";

                var assembly = type.GetAssemblyEx();
                if (assembly is not null)
                {
                    assemblyName = assembly.FullName;
                }

                fullName += ", " + assemblyName;
            }

            return fullName;
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
        /// The get custom attribute ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeArgument">The type argument.</param>
        /// <returns>The generic type.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeArgument" /> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type typeArgument)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("typeArgument", typeArgument);

            return MakeGenericTypeEx(type, new [] { typeArgument });
        }

        /// <summary>
        /// The get custom attribute ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="typeArguments">The type arguments.</param>
        /// <returns>The generic type.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="typeArguments" /> is <c>null</c> or empty array.</exception>
        public static Type MakeGenericTypeEx(this Type type, params Type[] typeArguments)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrEmptyArray("typeArguments", typeArguments);

            return type.MakeGenericType(typeArguments);
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

            return type.GetCustomAttributes(inherit).ToAttributeArray();
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

            return type.GetCustomAttributes(attributeType, inherit).ToAttributeArray();
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

            return type.ContainsGenericParameters;
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

            return type.Assembly;
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

            return type.Assembly.FullName;
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

            return type.BaseType == typeToCheck;
        }

        /// <summary>
        /// The is serializable ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is serializable ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsSerializableEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsSerializable;
        }

        /// <summary>
        /// The is public ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is public ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsPublicEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsPublic;
        }

        /// <summary>
        /// The is nested public ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is nested public ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsNestedPublicEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsNestedPublic;
        }

        /// <summary>
        /// The is interface ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is interface ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsInterfaceEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsInterface;
        }

        /// <summary>
        /// Determines whether the specified type is abstract.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is abstract; otherwise, <c>false</c>.</returns>
        public static bool IsAbstractEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsAbstract;
        }

        /// <summary>
        /// Determines whether the specified type is an array.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is an array; otherwise, <c>false</c>.</returns>
        public static bool IsArrayEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsArray;
        }

        /// <summary>
        /// Determines whether the specified type is a class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a class; otherwise, <c>false</c>.</returns>
        public static bool IsClassEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsClass;
        }

        /// <summary>
        /// Determines whether the specified type is a value type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is value type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsValueTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsValueType;
        }

        /// <summary>
        /// The is generic type ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is generic type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsGenericTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsGenericType;
        }

        /// <summary>
        /// The is generic type definition ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The is generic type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsGenericTypeDefinitionEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsGenericTypeDefinition;
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

            return type.IsPrimitive;
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

            return type.IsEnum;
        }

        /// <summary>
        /// Determines whether the specified type is a COM object.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsCOMObjectEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.IsCOMObject;
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

            return type.GetGenericTypeDefinition();
        }

        /// <summary>
        /// The get generic arguments ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type[] GetGenericArgumentsEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.GetGenericArguments();
        }

        /// <summary>
        /// Gets the element type of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type GetElementTypeEx(this Type type)
        {
            Argument.IsNotNull("type", type);

            return type.GetElementType();
        }

        /// <summary>
        /// Gets the element type of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="ignoreCase">if set to <c>true</c> [ignore case].</param>
        /// <returns>
        /// Type.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static Type GetInterfaceEx(this Type type, string name, bool ignoreCase)
        {
            Argument.IsNotNull("type", type);

            return type.GetInterface(name, ignoreCase);
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

            return type.GetInterfaces();
        }

        /// <summary>
        /// Gets the distance between types.
        /// </summary>
        /// <param name="fromType">The type</param>
        /// <param name="toType">The base type</param>
        /// <returns>The distance distance between types or -1 if the <paramref name="toType"/> is not assignable from the <paramref name="fromType"/></returns>
        public static int GetTypeDistance(this Type fromType, Type toType)
        {
            Argument.IsNotNull("type", fromType);
            Argument.IsNotNull("baseType", toType);

            return GetTypeDistanceInternal(fromType, toType);
        }

        /// <summary>
        /// Gets the distance between types.
        /// </summary>
        /// <param name="fromType">The type</param>
        /// <param name="toType">The base type</param>
        /// <returns>The distance distance between types or -1 if the <paramref name="toType"/> is not assignable from the <paramref name="fromType"/></returns>
        /// <remarks>
        /// Don't use this method directly use <see cref="GetTypeDistance"/> instead.
        /// </remarks>
        private static int GetTypeDistanceInternal(Type fromType, Type toType)
        {
            if (!toType.IsAssignableFromEx(fromType))
            {
                return -1;
            }

            var distance = 0;
            while (fromType != toType && !(toType.IsInterfaceEx() && !fromType.ImplementsInterfaceEx(toType)))
            {
                fromType = fromType.GetBaseTypeEx();
                distance++;
            }

            return distance;
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

            return type.BaseType;
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

            return type.IsAssignableFrom(typeToCheck);
        }

        /// <summary>
        /// The is instance of type ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="objectToCheck">The object to check.</param>
        /// <returns>The is instance of type ex.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="objectToCheck" /> is <c>null</c>.</exception>
        public static bool IsInstanceOfTypeEx<T>(this Type type, T objectToCheck)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("objectToCheck", objectToCheck);

            var instanceType = objectToCheck.GetType();

            if (ConvertableDictionary.TryGetValue(type, out var convertableHashSet))
            {
                if (convertableHashSet.Contains(instanceType))
                {
                    return true;
                }
            }

            if (type.IsAssignableFromEx(instanceType))
            {
                return true;
            }

            var castable = (from method in type.GetMethodsEx(BindingFlags.Public | BindingFlags.Static)
                            where method.ReturnType == instanceType &&
                                  method.Name.Equals("op_Implicit", StringComparison.Ordinal) ||
                                  method.Name.Equals("op_Explicit", StringComparison.Ordinal)
                            select method).Any();

            return castable;
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

            if (ConvertableDictionary.TryGetValue(type, out var convertableHashSet))
            {
                if (convertableHashSet.Contains(instanceType))
                {
                    return true;
                }
            }

            if (type.IsAssignableFromEx(instanceType))
            {
                return true;
            }

            var castable = (from method in type.GetMethodsEx(BindingFlags.Public | BindingFlags.Static)
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

            return type.GetConstructor(types);
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

            return type.GetConstructors();
        }

        /// <summary>
        /// Gets the member on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <returns>MemberInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static MemberInfo[] GetMemberEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false)
        {
            return GetMemberEx(type, name, BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers));
        }

        /// <summary>
        /// Gets the member on the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <returns>MemberInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static MemberInfo[] GetMemberEx(this Type type, string name, BindingFlags bindingFlags)
        {
            Argument.IsNotNull("type", type);

            // Explicitly use Catel.Reflection.TypeInfoExtensions, see https://github.com/Catel/Catel/issues/1617
            //return Catel.Reflection.TypeInfoExtensions.GetMember(type.GetTypeInfo(), name, bindingFlags);
            return type.GetMember(name, bindingFlags);
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

            // Explicitly use Catel.Reflection.TypeInfoExtensions, see https://github.com/Catel/Catel/issues/1617
            //return Catel.Reflection.TypeInfoExtensions.GetField(type.GetTypeInfo(), name, bindingFlags);
            return type.GetField(name, bindingFlags);
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
            return GetFieldsEx(type, bindingFlags, false);
        }

        /// <summary>
        /// The get fields ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <param name="flattenMembers">Flattens members if set to <c>true</c>.</param>
        /// <returns>FieldInfo[][].</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static FieldInfo[] GetFieldsEx(this Type type, BindingFlags bindingFlags, bool flattenMembers)
        {
            Argument.IsNotNull("type", type);

            if (!flattenMembers)
            {
                // Fast way out
                return type.GetFields(bindingFlags);
            }

            var fields = new List<FieldInfo>(type.GetFields(bindingFlags));

            // We want flattened stuff to leak through as well
            if (Enum<BindingFlags>.Flags.IsFlagSet(bindingFlags, BindingFlags.FlattenHierarchy))
            {
                var baseType = type.BaseType;
                if ((baseType is not null) && (baseType != typeof(object)))
                {
                    foreach (var member in GetFieldsEx(baseType, bindingFlags, true))
                    {
                        if (!fields.Contains(member))
                        {
                            fields.Add(member);
                        }
                    }
                }
            }

            return fields.ToArray();
        }

        /// <summary>
        /// The get property ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="flattenHierarchy">The flatten hierarchy.</param>
        /// <param name="allowStaticMembers">The allow static members.</param>
        /// <param name="allowExplicitInterfaceProperties">if set to <c>true</c>, this method will check for explicit interface implementations when the property is not found.</param>
        /// <returns>PropertyInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static PropertyInfo GetPropertyEx(this Type type, string name, bool flattenHierarchy = true, bool allowStaticMembers = false,
            bool allowExplicitInterfaceProperties = true)
        {
            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers);
            return GetPropertyEx(type, name, bindingFlags, allowExplicitInterfaceProperties);
        }

        /// <summary>
        /// The get property ex.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="bindingFlags">The binding Flags.</param>
        /// <param name="allowExplicitInterfaceProperties">if set to <c>true</c>, this method will check for explicit interface implementations when the property is not found.</param>
        /// <returns>PropertyInfo.</returns>
        /// <exception cref="System.ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="System.ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        public static PropertyInfo GetPropertyEx(this Type type, string name, BindingFlags bindingFlags, bool allowExplicitInterfaceProperties = true)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNullOrWhitespace("name", name);

            PropertyInfo propertyInfo = null;

            try
            {
                propertyInfo = type.GetTypeInfo().GetProperty(name, bindingFlags);
            }
            catch (AmbiguousMatchException)
            {
                // Fallback mechanism
                var allProperties = type.GetPropertiesEx(bindingFlags);

                // Assume the "best" one is first in the list
                propertyInfo = allProperties.FirstOrDefault(x => x.Name.Equals(name));
            }

            if (propertyInfo is null)
            {
                if (allowExplicitInterfaceProperties)
                {
                    foreach (var iface in type.GetInterfacesEx())
                    {
                        propertyInfo = iface.GetPropertyEx(name, bindingFlags, false);
                        if (propertyInfo is not null)
                        {
                            break;
                        }
                    }
                }
            }

            return propertyInfo;
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
            return GetPropertiesEx(type, bindingFlags, false);
        }

        public static PropertyInfo[] GetPropertiesEx(this Type type, BindingFlags bindingFlags, bool flattenMembers)
        {
            Argument.IsNotNull("type", type);

            if (!flattenMembers)
            {
                // Fast way out
                return type.GetProperties(bindingFlags);
            }

            var properties = new List<PropertyInfo>(type.GetProperties(bindingFlags));

            // We want flattened stuff to leak through as well
            if (Enum<BindingFlags>.Flags.IsFlagSet(bindingFlags, BindingFlags.FlattenHierarchy))
            {
                var baseType = type.BaseType;
                if ((baseType is not null) && (baseType != typeof(object)))
                {
                    foreach (var member in GetPropertiesEx(baseType, bindingFlags, true))
                    {
                        if (!properties.Contains(member))
                        {
                            properties.Add(member);
                        }
                    }
                }
            }

            return properties.ToArray();
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

            // Explicitly use Catel.Reflection.TypeInfoExtensions, see https://github.com/Catel/Catel/issues/1617
            //return Catel.Reflection.TypeInfoExtensions.GetEvent(type.GetTypeInfo(), name, bindingFlags);
            return type.GetEvent(name, bindingFlags);
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

            var bindingFlags = BindingFlagsHelper.GetFinalBindingFlags(flattenHierarchy, allowStaticMembers);

            // Explicitly use Catel.Reflection.TypeInfoExtensions, see https://github.com/Catel/Catel/issues/1617
            //return Catel.Reflection.TypeInfoExtensions.GetEvents(type.GetTypeInfo(), bindingFlags);
            return type.GetEvents(bindingFlags);
        }

        public static EventInfo[] GetEventsEx(this Type type, BindingFlags bindingFlags)
        {
            return type.GetEvents(bindingFlags);
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

            // Explicitly use Catel.Reflection.TypeInfoExtensions, see https://github.com/Catel/Catel/issues/1617
            //return Catel.Reflection.TypeInfoExtensions.GetMethod(type.GetTypeInfo(), name, bindingFlags);
            return type.GetMethod(name, bindingFlags);
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

            return type.GetMethod(name, bindingFlags, null, types, null);
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
            return GetMethodsEx(type, bindingFlags, false);
        }

        public static MethodInfo[] GetMethodsEx(this Type type, BindingFlags bindingFlags, bool flattenMembers)
        {
            Argument.IsNotNull("type", type);

            if (!flattenMembers)
            {
                // Fast way out
                return type.GetMethods(bindingFlags);
            }

            var methods = new List<MethodInfo>(type.GetMethods(bindingFlags));

            // We want flattened stuff to leak through as well
            if (Enum<BindingFlags>.Flags.IsFlagSet(bindingFlags, BindingFlags.FlattenHierarchy))
            {
                var baseType = type.BaseType;
                if ((baseType is not null) && (baseType != typeof(object)))
                {
                    foreach (var member in GetMethodsEx(baseType, bindingFlags, true))
                    {
                        if (!methods.Contains(member))
                        {
                            methods.Add(member);
                        }
                    }
                }
            }

            return methods.ToArray();
        }
    }
}

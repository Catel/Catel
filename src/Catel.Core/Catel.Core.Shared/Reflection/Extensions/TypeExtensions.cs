// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Reflection
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

    /// <summary>
    /// Extensions for the type class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified type is a class type, meaning it is not a value type but also not a string
        /// or any of the primitive types in .NET.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if this type is a class type; otherwise, <c>false</c>.</returns>
        public static bool IsClassType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsValueTypeEx())
            {
                return false;
            }

            if (type == typeof(string))
            {
                return false;
            }

            return type.IsClassEx();
        }

        /// <summary>
        /// Determines whether the specified type is a collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a collection; otherwise, <c>false</c>.</returns>
        public static bool IsCollection(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFromEx(type);
        }

        /// <summary>
        /// Determines whether the specified type is a dictionary.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a dictionary; otherwise, <c>false</c>.</returns>
        public static bool IsDictionary(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type == typeof(string))
            {
                return false;
            }

            if (!type.IsGenericTypeEx())
            {
                return false;
            }

            var genericDefinition = type.GetGenericTypeDefinitionEx();
            return genericDefinition == typeof(Dictionary<,>);
        }

        /// <summary>
        /// Returns whether a type is nullable or not.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type is nullable, otherwise false.</returns>
        public static bool IsNullableType(this Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (!type.IsValueTypeEx())
            {
                return true;
            }

            if (Nullable.GetUnderlyingType(type) != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type is a basic type. A basic type is one that can be wholly expressed
        /// as an XML attribute. All primitive data types and <see cref="String"/> and <see cref="DateTime"/> are basic types.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the specified type is a basic type; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static bool IsBasicType(this Type type)
        {
            Argument.IsNotNull("type", type);

            if (type == typeof(string) || type.IsPrimitiveEx() || type.IsEnumEx() || type == typeof(DateTime) || type == typeof(decimal) || type == typeof(Guid))
            {
                return true;
            }

            if (IsNullableType(type))
            {
                var underlyingNullableType = Nullable.GetUnderlyingType(type);
                if (underlyingNullableType != null)
                {
                    return IsBasicType(underlyingNullableType);
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified type derives from <see cref="ModelBase" />.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the specified type is a model base; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsModelBase(this Type type)
        {
            Argument.IsNotNull("type", type);

            if (type == typeof (ModelBase))
            {
                return true;
            }

            return typeof (ModelBase).IsAssignableFromEx(type);
        }

        /// <summary>
        /// Gets the element type of the collection.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static Type GetCollectionElementType(this Type type)
        {
            Argument.IsNotNull("type", type);

            if (type.IsArrayEx())
            {
                var arrayElementType = type.GetElementTypeEx();
                return arrayElementType;
            }

            var interfaces = type.GetInterfacesEx();
            var genericEnumerableInterface = (from iface in interfaces
                                              where iface.Name.StartsWith("IEnumerable") && iface.IsGenericTypeEx()
                                              select iface).FirstOrDefault();
            if (genericEnumerableInterface == null)
            {
                return null;
            }

            var elementType = genericEnumerableInterface.GetGenericArgumentsEx()[0];
            return elementType;
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameterType1">The parameter type 1</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType1"/> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type parameterType1)
        {
            Argument.IsNotNull("type", type);

            return type.MakeGenericType(TypeArray.From(parameterType1));
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameterType1">The parameter type 1</param>
        /// <param name="parameterType2">The parameter type 2</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType2"/> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type parameterType1, Type parameterType2)
        {
            Argument.IsNotNull("type", type);

            return type.MakeGenericType(TypeArray.From(parameterType1, parameterType2));
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameterType1">The parameter type 1</param>
        /// <param name="parameterType2">The parameter type 2</param>
        /// <param name="parameterType3">The parameter type 3</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType3"/> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type parameterType1, Type parameterType2, Type parameterType3)
        {
            Argument.IsNotNull("type", type);

            return type.MakeGenericType(TypeArray.From(parameterType1, parameterType2, parameterType3));
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameterType1">The parameter type 1</param>
        /// <param name="parameterType2">The parameter type 2</param>
        /// <param name="parameterType3">The parameter type 3</param>
        /// <param name="parameterType4">The parameter type 4</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType3"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType4"/> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type parameterType1, Type parameterType2, Type parameterType3, Type parameterType4)
        {
            Argument.IsNotNull("type", type);

            return type.MakeGenericType(TypeArray.From(parameterType1, parameterType2, parameterType3, parameterType4));
        }

        /// <summary>
        /// Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="parameterType1">The parameter type 1</param>
        /// <param name="parameterType2">The parameter type 2</param>
        /// <param name="parameterType3">The parameter type 3</param>
        /// <param name="parameterType4">The parameter type 4</param>
        /// <param name="parameterType5">The parameter type 5</param>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType1"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType2"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType3"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType4"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType5"/> is <c>null</c>.</exception>
        public static Type MakeGenericTypeEx(this Type type, Type parameterType1, Type parameterType2, Type parameterType3, Type parameterType4, Type parameterType5)
        {
            Argument.IsNotNull("type", type);

            return type.MakeGenericType(TypeArray.From(parameterType1, parameterType2, parameterType3, parameterType4, parameterType5));
        }
    }
}
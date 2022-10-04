namespace Catel.Reflection
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Reflection extension class.
    /// </summary>
    public static partial class ReflectionExtensions
    {
        /// <summary>
        /// Gets the attribute from the member.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="memberInfo">The member Info.</param>
        /// <returns>The attribute or <c>null</c> of the member is not decorated with the attribute.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo" /> is <c>null</c>.</exception>
        public static TAttribute? GetAttribute<TAttribute>(this MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            TryGetAttribute(memberInfo, typeof(TAttribute), out var tempAttribute);

            return tempAttribute as TAttribute;
        }

        /// <summary>
        /// Gets the attribute from the member.
        /// </summary>
        /// <param name="memberInfo">The member Info.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The attribute or <c>null</c> of the member is not decorated with the attribute.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static Attribute? GetAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            TryGetAttribute(memberInfo, attributeType, out var attribute);

            return attribute;
        }

        /// <summary>
        /// Gets the attribute from the type.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns>The attribute or <c>null</c> of the member is not decorated with the attribute.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static TAttribute? GetAttribute<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            TryGetAttribute(type, typeof(TAttribute), out var tempAttribute);

            return tempAttribute as TAttribute;
        }

        /// <summary>
        /// Gets the attribute from the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>The attribute or <c>null</c> of the member is not decorated with the attribute.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static Attribute? GetAttribute(this Type type, Type attributeType)
        {
            TryGetAttribute(type, attributeType, out var attribute);

            return attribute;
        }

        /// <summary>
        /// Tries to the get attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="memberInfo">The member Info.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>
        /// <c>true</c> if the attribute is retrieved successfully; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo"/> is <c>null</c>.</exception>
        public static bool TryGetAttribute<TAttribute>(this MemberInfo memberInfo, out TAttribute? attribute)
            where TAttribute : Attribute
        {
            var result = TryGetAttribute(memberInfo, typeof(TAttribute), out var tempAttribute);

            attribute = tempAttribute as TAttribute;
            return result;
        }

        /// <summary>
        /// Tries to the get attribute.
        /// </summary>
        /// <param name="memberInfo">The member Info.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns>
        ///   <c>true</c> if the attribute is retrieved successfully; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType"/> is <c>null</c>.</exception>
        public static bool TryGetAttribute(this MemberInfo memberInfo, Type attributeType, out Attribute? attribute)
        {
            attribute = null;
            var attributes = memberInfo.GetCustomAttributes(attributeType, false) as Attribute[];

            if ((attributes is not null) && (attributes.Length > 0))
            {
                attribute = attributes[0];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to the get attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="type">The type.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns><c>true</c> if the attribute is retrieved successfully; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool TryGetAttribute<TAttribute>(this Type type, out TAttribute? attribute)
            where TAttribute : Attribute
        {
            var result = TryGetAttribute(type, typeof(TAttribute), out var tempAttribute);

            attribute = tempAttribute as TAttribute;
            return result;
        }

        /// <summary>
        /// Tries to the get attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <param name="attribute">The attribute.</param>
        /// <returns><c>true</c> if the attribute is retrieved successfully; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static bool TryGetAttribute(this Type type, Type attributeType, out Attribute? attribute)
        {
            attribute = null;
            var attributes = type.GetCustomAttributesEx(attributeType, false) as Attribute[];

            if ((attributes is not null) && (attributes.Length > 0))
            {
                attribute = attributes[0];
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified member is decorated with the specified attribute type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="memberInfo">The member info.</param>
        /// <returns>
        ///   <c>true</c> if the member is decorated with the attribute; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo"/> is <c>null</c>.</exception>
        public static bool IsDecoratedWithAttribute<TAttribute>(this MemberInfo memberInfo)
        {
            return IsDecoratedWithAttribute(memberInfo, typeof(TAttribute));
        }

        /// <summary>
        /// Determines whether the specified member is decorated with the specified attribute type.
        /// </summary>
        /// <param name="memberInfo">The member info.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns>
        ///   <c>true</c> if the member is decorated with the attribute; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="memberInfo"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType"/> is <c>null</c>.</exception>
        public static bool IsDecoratedWithAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            return TryGetAttribute(memberInfo, attributeType, out var tempAttribute);
        }

        /// <summary>
        /// Determines whether the specified member is decorated with the specified attribute type.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the member is decorated with the attribute; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        public static bool IsDecoratedWithAttribute<TAttribute>(this Type type)
        {
            return IsDecoratedWithAttribute(type, typeof(TAttribute));
        }

        /// <summary>
        /// Determines whether the specified member is decorated with the specified attribute type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="attributeType">Type of the attribute.</param>
        /// <returns><c>true</c> if the member is decorated with the attribute; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="attributeType" /> is <c>null</c>.</exception>
        public static bool IsDecoratedWithAttribute(this Type type, Type attributeType)
        {
            return TryGetAttribute(type, attributeType, out var tempAttribute);
        }
    }
}

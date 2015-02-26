// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectToStringHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Globalization;
    using Reflection;

    /// <summary>
    /// Simple static class that turns an object to string.
    /// </summary>
    public static class ObjectToStringHelper
    {
        /// <summary>
        /// Initializes static members of the <see cref="StringToObjectHelper"/> class.
        /// </summary>
        static ObjectToStringHelper()
        {
            DefaultCulture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the default culture to use for parsing.
        /// </summary>
        /// <value>The default culture.</value>
        public static CultureInfo DefaultCulture { get; set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance, can be <c>null</c>.</param>
        /// <returns>A <see cref="string"/> that represents the instance.</returns>
        public static string ToString(object instance)
        {
            return ToString(instance, DefaultCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the instance.
        /// <para />
        /// If the <paramref name="instance" /> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance, can be <c>null</c>.</param>
        /// <param name="cultureInfo">The culture information.</param>
        /// <returns>A <see cref="string" /> that represents the instance.</returns>
        public static string ToString(object instance, CultureInfo cultureInfo)
        {
            if (instance == null)
            {
                return "null";
            }

#if !NETFX_CORE && !PCL
            if (instance == DBNull.Value)
            {
                return "dbnull";
            }
#endif

#if !NETFX_CORE
            // Note: cannot be used on NETFX_CORE

            // Check if there is a culture specific version
            var instanceType = instance.GetType();
            var toStringMethod = instanceType.GetMethodEx("ToString", new[] {typeof (IFormatProvider)});
            if (toStringMethod != null)
            {
                return (string)toStringMethod.Invoke(instance, new object[] { cultureInfo });
            }
#endif

            return instance.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the type name of the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="string"/> that represents the type of the instance.</returns>
        public static string ToTypeString(object instance)
        {
            if (instance == null)
            {
                return "null";
            }

            var instanceAsType = instance as Type;
            if (instanceAsType != null)
            {
                return instanceAsType.Name;
            }

            return instance.GetType().Name;
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the full type name of the instance.
        /// <para />
        /// If the <paramref name="instance"/> is <c>null</c>, this method will return "null". This
        /// method is great when the value of a property must be logged.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>A <see cref="string"/> that represents the type of the instance.</returns>
        public static string ToFullTypeString(object instance)
        {
            if (instance == null)
            {
                return "null";
            }

            var instanceAsType = instance as Type;
            if (instanceAsType != null)
            {
                return instanceAsType.GetSafeFullName();
            }

            return instance.GetType().GetSafeFullName();
        }
    }
}
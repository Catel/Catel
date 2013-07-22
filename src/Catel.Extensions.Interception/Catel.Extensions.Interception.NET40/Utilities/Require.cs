// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Require.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Utilities
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// 
    /// </summary>
    public static class Require
    {
        #region Members
        /// <summary>
        /// Overridables the method.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void OverridableMethod(MethodCallExpression expression)
        {
            if (!expression.Method.IsVirtual || expression.Method.IsFinal)
            {
                throw new InvalidOperationException(string.Format("Method must be virtual and not final: {0}.",
                                                                  expression.Method.Name));
            }
        }

        /// <summary>
        /// Overridables the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property.</param>
        public static void OverridableProperty<T>(string property)
        {
            OverridableProperty(typeof (T), property);
        }

        /// <summary>
        /// Overridables the property.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="property">The property.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public static void OverridableProperty(Type type, string property)
        {
            Argument.IsNotNull(() => type);
            Argument.IsNotNullOrWhitespace(() => property);

            if (!type.GetMethod(property).IsVirtual || type.GetMethod(property).IsFinal)
            {
                throw new InvalidOperationException(string.Format("Property must be virtual and not final: {0}.{1}.",
                                                                  type.FullName, property));
            }
        }
        #endregion
    }
}
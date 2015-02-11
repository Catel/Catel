// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangedEventArgsExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.ComponentModel
{
    using System;
    using Linq.Expressions;
    using Catel;
    using Catel.Reflection;

    /// <summary>
    /// Extensions for the <see cref="PropertyChangedEventArgs "/> class.
    /// </summary>
    public static class PropertyChangedEventArgsExtensions
    {
        /// <summary>
        /// Returns whether the specified instance of the <see cref="PropertyChangedEventArgs"/> represents that all properties
        /// of an object have changed. This is the case when the <see cref="PropertyChangedEventArgs.PropertyName"/> is <c>null</c>
        /// or empty.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <returns><c>true</c> if the <see cref="PropertyChangedEventArgs.PropertyName"/> is <c>null</c> or empty, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e" /> is <c>null</c>.</exception>
        public static bool AllPropertiesChanged(this PropertyChangedEventArgs e)
        {
            Argument.IsNotNull("e", e);

            return string.IsNullOrEmpty(e.PropertyName);
        }

        /// <summary>
        /// Determines whether the specified instance of the <see cref="PropertyChangedEventArgs" /> represents a change notification
        /// for the property specified by the property name.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if <see cref="PropertyChangedEventArgs.PropertyName"/> equals the property from the property expression; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="propertyName" /> is <c>null</c> or whitespace.</exception>
        public static bool HasPropertyChanged(this PropertyChangedEventArgs e, string propertyName)
        {
            Argument.IsNotNull("e", e);
            Argument.IsNotNullOrWhitespace("propertyName", propertyName);

            return string.Equals(e.PropertyName, propertyName);
        }

        /// <summary>
        /// Determines whether the specified instance of the <see cref="PropertyChangedEventArgs"/> represents a change notification
        /// for the property specified by the property expression.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">if set to <c>true</c>, nested properties are allowed.</param>
        /// <returns><c>true</c> if <see cref="PropertyChangedEventArgs.PropertyName"/> equals the property from the property expression; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static bool HasPropertyChanged<TValue>(this PropertyChangedEventArgs e, Expression<Func<TValue>> propertyExpression, bool allowNested = false)
        {
            Argument.IsNotNull("e", e);
            Argument.IsNotNull("propertyExpression", propertyExpression);

            return string.Equals(e.PropertyName, PropertyHelper.GetPropertyName(propertyExpression, allowNested), StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether the specified instance of the <see cref="PropertyChangedEventArgs" /> represents a change notification
        /// for the property specified by the property expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="allowNested">if set to <c>true</c>, nested properties are allowed.</param>
        /// <returns><c>true</c> if <see cref="PropertyChangedEventArgs.PropertyName" /> equals the property from the property expression; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="e" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="e" /> is <c>null</c>.</exception>
        public static bool HasPropertyChanged<TModel, TValue>(this PropertyChangedEventArgs e, Expression<Func<TModel, TValue>> propertyExpression, bool allowNested = false)
        {
            Argument.IsNotNull("e", e);
            Argument.IsNotNull("propertyExpression", propertyExpression);

            return string.Equals(e.PropertyName, PropertyHelper.GetPropertyName(propertyExpression, allowNested), StringComparison.Ordinal);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObjectConverterService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;

    /// <summary>
    /// Service to convert objects to strings and vice versa.
    /// </summary>
    public interface IObjectConverterService
    {
        /// <summary>
        /// Converts the specified object to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string value.</returns>
        string ConvertFromObjectToString(object value);

        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The object value.</returns>
        object ConvertFromStringToObject(string value, Type targetType);

        /// <summary>
        /// Converts the specified object to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The object value.</returns>
        object ConvertFromObjectToObject(object value, Type targetType);
    }
}
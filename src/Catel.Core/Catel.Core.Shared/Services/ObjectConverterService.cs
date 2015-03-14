// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeConverterService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;

    /// <summary>
    /// Service to convert objects to strings and vice versa.
    /// </summary>
    public class ObjectConverterService : IObjectConverterService
    {
        /// <summary>
        /// Converts the specified object to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string value.</returns>
        public virtual string ConvertFromObjectToString(object value)
        {
            return ObjectToStringHelper.ToString(value);
        }

        /// <summary>
        /// Converts the specified string value to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The object value.</returns>
        public virtual object ConvertFromStringToObject(string value, Type targetType)
        {
            return StringToObjectHelper.ToRightType(targetType, value);
        }

        /// <summary>
        /// Converts the specified object to an object.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns>The object value.</returns>
        public virtual object ConvertFromObjectToObject(object value, Type targetType)
        {
            var stringValue = ConvertFromObjectToString(value);
            return ConvertFromStringToObject(stringValue, targetType);
        }
    }
}
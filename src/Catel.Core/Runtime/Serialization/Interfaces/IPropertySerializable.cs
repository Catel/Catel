// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFastSerializable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Runtime.Serialization
{
    /// <summary>
    /// Fast serialization interaction. By default the serialization engine uses reflection to get and set values. To improve
    /// performance, once can implement this interface.
    /// </summary>
    public interface IPropertySerializable
    {
        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is retrieved successfully; otherwise, <c>false</c>.</returns>
        bool GetPropertyValue(string propertyName, ref object value);

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is set successfully; otherwise, <c>false</c>.</returns>
        bool SetPropertyValue(string propertyName, object value);
    }
}
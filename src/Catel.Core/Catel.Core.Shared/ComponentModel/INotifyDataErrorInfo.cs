// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyDataErrorInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40

namespace System.ComponentModel
{
    using Collections;

    /// <summary>
    /// WPF implementation of the INotifyDataErrorInfo as it is known in Silverlight.
    /// </summary>
    public interface INotifyDataErrorInfo
    {
        /// <summary>
        /// Gets a value indicating whether this object contains any field or business errors.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }

        /// <summary>
        /// Occurs when the errors have changed.
        /// </summary>
        event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Gets the errors for the specific property name.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see cref="IEnumerable"/> of errors.</returns>
        IEnumerable GetErrors(string propertyName);
    }
}

#endif
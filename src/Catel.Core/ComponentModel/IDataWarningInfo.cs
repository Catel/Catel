// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyDataWarningInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace System.ComponentModel
{
    /// <summary>
    /// Interface that is based on the <see cref="IDataErrorInfo"/> interface, but supports warnings instead of errors.
    /// </summary>
    public interface IDataWarningInfo
    {
        /// <summary>
        /// Gets the warning.
        /// </summary>
        /// <value>
        /// A warning message indicating what is possibly wrong with this object. The default is
        /// an empty string ("").
        /// </value>
        string Warning { get; }

        /// <summary>
        /// Gets the warning message for the property with the given name.
        /// </summary>
        /// <value></value>
        /// <param name="columnName">The name of the property whose warning message to get.</param>
        string this[string columnName] { get; }
    }
}
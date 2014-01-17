// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationClosingEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// Event args class for an event when an application gets closed.
    /// </summary>
    public class ApplicationClosingEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the closing of the application should be canceled.
        /// </summary>
        /// <value><c>true</c> if the action should be canceled; otherwise, <c>false</c>.</value>
        /// <remarks></remarks>
        public bool Cancel { get; set; }
    }
}
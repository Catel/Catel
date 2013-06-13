// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CameraOperationCompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;

    /// <summary>
    /// <see cref="EventArgs"/> for camera operations.
    /// </summary>
    public class CameraOperationCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CameraOperationCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="ex">The exception that occurred. If no exception has occurred, pass <c>null</c>.</param>
        public CameraOperationCompletedEventArgs(Exception ex)
        {
            Exception = ex;
            Succeeded = (Exception != null);
        }

        /// <summary>
        /// Gets the exception that occurred if any exception occurred. If no exception occurred,
        /// this value is <c>null</c>.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the operation is succeeded.
        /// </summary>
        /// <value><c>true</c> if succeeded; otherwise, <c>false</c>.</value>
        public bool Succeeded { get; private set; }
    }
}

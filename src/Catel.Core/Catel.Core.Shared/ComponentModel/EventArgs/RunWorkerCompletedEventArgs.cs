// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RunWorkerCompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE || PCL

namespace System.ComponentModel
{
    /// <summary>
    /// Event arguments passed to the RunWorkerCompleted handler.
    /// </summary>
    public class RunWorkerCompletedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        public RunWorkerCompletedEventArgs()
        {
        }

        /// <summary>
        /// Creates an instance of the type.
        /// </summary>
        /// <param name="cancelled">Sets the cancelled value.</param>
        /// <param name="error">Sets the error value.</param>
        /// <param name="result">Sets the result value.</param>
        public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled)
        {
            Result = result;
            Error = error;
            Cancelled = cancelled;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value containing any exception
        /// that terminated the background task.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets a value containing the result
        /// of the operation.
        /// </summary>
        public object Result { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// operation was cancelled prior to completion.
        /// </summary>
        public bool Cancelled { get; set; }
        #endregion
    }
}

#endif
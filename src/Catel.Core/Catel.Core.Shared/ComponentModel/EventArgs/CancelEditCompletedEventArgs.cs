// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CancelEditCompletedEventArgs.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace System.ComponentModel
{
    /// <summary>
    /// Event args implementation called when the objects cancel edit operation has completed.
    /// </summary>
    public class CancelEditCompletedEventArgs : EventArgs
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CancelEditCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="wasCanceled">  If true, the cancel operation was canceled.
        /// If false, the cancel operation ran to completion.</param>
        public CancelEditCompletedEventArgs(bool wasCanceled)
        {
            IsCancelOperationCanceled = wasCanceled;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the value indicating if the cancel operation canceled.
        /// </summary>
        /// <remarks>If <c>true</c>, the cancel operation was canceled and the operation is complete.
        /// If <c>false</c>, the cancel operation was allowed to continue and all cancel operations
        /// are complete.</remarks>
        public bool IsCancelOperationCanceled { get; private set; }
        #endregion
    }
}
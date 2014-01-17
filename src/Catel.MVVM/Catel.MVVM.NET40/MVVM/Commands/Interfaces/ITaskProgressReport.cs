// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskProgressReport.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !NET40 && !SILVERLIGHT && !WP7

namespace Catel.MVVM
{
    /// <summary>
    /// Interface for task progress report.
    /// </summary>
    public interface ITaskProgressReport
    {
        #region Properties
        /// <summary>
        /// Status of the task progress.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; }
        #endregion
    }
}

#endif
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITaskProgressReport.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRetryPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface IRetryPolicy
    {
        #region Properties
        /// <summary>
        /// Gets or sets the number of attempts.
        /// </summary>
        /// <value>
        /// The number of attempts.
        /// </value>
        int Attempts { get; }

        /// <summary>
        /// Gets or sets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        TimeSpan Interval { get; }
        #endregion
    }
}
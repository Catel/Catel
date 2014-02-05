// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represent the base policy interface in exception handling
    /// </summary>
    public interface IPolicy
    {
        #region Properties
        /// <summary>
        /// Gets the number of times.
        /// </summary>
        /// <value>
        /// The number of times.
        /// </value>
        int NumberOfTimes { get; }

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
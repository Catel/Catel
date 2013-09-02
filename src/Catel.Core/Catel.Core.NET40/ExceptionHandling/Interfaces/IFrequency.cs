// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFrequency.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represent the frequency tolerance interface in exception handling
    /// </summary>
    public interface IFrequency
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
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        TimeSpan Duration { get; }
        #endregion
    }
}
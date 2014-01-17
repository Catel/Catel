// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PolicyBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// The policy base.
    /// </summary>
    public class PolicyBase : IPolicy
    {
        #region IPolicy Members
        /// <summary>
        /// Gets the number of times.
        /// </summary>
        /// <value>
        /// The number of times.
        /// </value>
        public int NumberOfTimes { get; protected set; }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval { get; protected set; }
        #endregion
    }
}
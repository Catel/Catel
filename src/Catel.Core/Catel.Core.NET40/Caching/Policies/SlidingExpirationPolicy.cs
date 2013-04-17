// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
    /// The cache item will expire using the duration property as the sliding expiration.
    /// </summary>
    public sealed class SlidingExpirationPolicy : DurationExpirationPolicy
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidingExpirationPolicy"/> class.
        /// </summary>
        /// <param name="durationTimeSpan">
        /// The expiration.
        /// </param>
        internal SlidingExpirationPolicy(TimeSpan durationTimeSpan)
            : base(durationTimeSpan, true)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// The reset.
        /// </summary>
        protected override void OnReset()
        {
            AbsoluteExpirationDateTime = DateTime.Now.Add(DurationTimeSpan);
        }

        #endregion
    }
}
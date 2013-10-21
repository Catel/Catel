// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represents the retry policy.
    /// </summary>
    public class RetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class.
        /// </summary>
        /// <param name="numberOfAttempts">The number of attempts.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="numberOfAttempts"/> is larger than <c>1</c>.</exception>
        public RetryPolicy(int numberOfAttempts, TimeSpan interval)
        {
            Argument.IsMinimal("numberOfAttempts", numberOfAttempts,  1);

            NumberOfAttempts = numberOfAttempts;
            Interval = interval;
        }

        #region IRetryPolicy Members
        /// <summary>
        /// Gets the number of attempts.
        /// </summary>
        /// <value>
        /// The number of attempts.
        /// </value>
        public int NumberOfAttempts { get; private set; }

        /// <summary>
        /// Gets the interval.
        /// </summary>
        /// <value>
        /// The interval.
        /// </value>
        public TimeSpan Interval { get; private set; }
        #endregion
    }
}
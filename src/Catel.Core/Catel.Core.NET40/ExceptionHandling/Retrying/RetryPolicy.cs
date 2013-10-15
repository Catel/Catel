// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public class RetryPolicy : IRetryPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class.
        /// </summary>
        /// <param name="attempts">The attempts.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="attempts"/> is larger than <c>1</c>.</exception>
        public RetryPolicy(int attempts, TimeSpan interval)
        {
            Argument.IsMinimal(() => attempts,  1);

            Attempts = attempts;
            Interval = interval;
        }

        #region IRetryPolicy Members
        /// <summary>
        /// Gets the attempts.
        /// </summary>
        /// <value>
        /// The attempts.
        /// </value>
        public int Attempts { get; private set; }

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
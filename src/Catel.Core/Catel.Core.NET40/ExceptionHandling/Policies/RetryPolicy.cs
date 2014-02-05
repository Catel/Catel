// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represents the retry policy.
    /// </summary>
    public class RetryPolicy : PolicyBase, IRetryPolicy
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryPolicy"/> class.
        /// </summary>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is larger than <c>1</c>.</exception>
        public RetryPolicy(int numberOfTimes, TimeSpan interval)
        {
            Argument.IsMinimal("numberOfTimes", numberOfTimes, 1);

            NumberOfTimes = numberOfTimes;
            Interval = interval;
        }
        #endregion
    }
}
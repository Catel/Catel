// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Frequency.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represent the frequency tolerance implementation in exception handling
    /// </summary>
    public class Frequency : IFrequency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Frequency"/> class.
        /// </summary>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <param name="duration">The duration.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is out of range.</exception>
        public Frequency(int numberOfTimes, TimeSpan duration)
        {
            Argument.IsMinimal(() => numberOfTimes, 1);

            NumberOfTimes = numberOfTimes;
            Duration = duration;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of times.
        /// </summary>
        /// <value>
        /// The number of times.
        /// </value>
        public int NumberOfTimes { get; private set; }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// The overrided ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} times per {1}", NumberOfTimes, Duration);
        }
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Frequency.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;

    /// <summary>
    /// Represent the buffer policy implementation in exception handling
    /// </summary>
    public class BufferPolicy : PolicyBase, IBufferPolicy
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPolicy"/> class.
        /// </summary>
        /// <param name="numberOfTimes">The number of times.</param>
        /// <param name="interval">The interval.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="numberOfTimes"/> is out of range.</exception>
        public BufferPolicy(int numberOfTimes, TimeSpan interval)
        {
            Argument.IsMinimal("numberOfTimes", numberOfTimes, 1);

            NumberOfTimes = numberOfTimes;
            Interval = interval;
        }
        #endregion

        #region Methods
        /// <summary>
        /// The overrided ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} times per {1}", NumberOfTimes, Interval);
        }
        #endregion
    }
}
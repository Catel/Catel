// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProgressContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;

    /// <summary>
    /// Never calculate progress manually anymore.
    /// </summary>
    public class ProgressContext : Disposable
    {
        private readonly double _onePercentage;

        private readonly long _refreshInterval;

        private readonly long _refreshIntervalCount;
        private readonly double _smallRefreshIntervalCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressContext"/> class.
        /// </summary>
        /// <param name="totalCount">The total count that this progress context represents.</param>
        /// <param name="numberOfRefreshes">The number of refreshes required during progress.</param>
        public ProgressContext(long totalCount, int numberOfRefreshes)
        {
            Argument.IsMinimal("numberOfRefreshes", numberOfRefreshes, 1);

            TotalCount = totalCount;
            NumberOfRefreshes = numberOfRefreshes;

            _onePercentage = TotalCount / 100d;
            _refreshIntervalCount = totalCount / numberOfRefreshes;
            if (_refreshIntervalCount == 0)
            {
                _smallRefreshIntervalCounter = totalCount / (double)numberOfRefreshes;
            }

            _refreshInterval = TotalCount / NumberOfRefreshes;
            if (_refreshInterval == 0)
            {
                _refreshInterval = 1;
            }
        }

        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public long TotalCount { get; private set; }

        /// <summary>
        /// Gets the number of refreshes.
        /// </summary>
        /// <value>The number of refreshes.</value>
        public int NumberOfRefreshes { get; private set; }

        /// <summary>
        /// Gets the current refresh number. This represents a value that is calculated 
        /// based on the <see cref="TotalCount"/>, <see cref="CurrentCount"/> and <see cref="NumberOfRefreshes"/>.
        /// </summary>
        /// <value>The current step.</value>
        public int CurrentRefreshNumber
        {
            get
            {
                var currentCount = CurrentCount;

                if (_refreshIntervalCount == 0)
                {
                    var currentRefreshNumberAsDouble = (currentCount / _smallRefreshIntervalCounter);
                    return (int)currentRefreshNumberAsDouble;
                }

                var currentRefreshNumber = (currentCount / _refreshIntervalCount);
                return (int)currentRefreshNumber;
            }
        }

        /// <summary>
        /// Gets the percentage of the progress.
        /// </summary>
        /// <value>The percentage.</value>
        public double Percentage
        {
            get
            {
                var percentage = (CurrentCount / _onePercentage);
                return percentage;
            }
        }

        /// <summary>
        /// Gets or sets the current count.
        /// </summary>
        /// <value>The current count.</value>
        public long CurrentCount { get; set; }

        /// <summary>
        /// Gets a value indicating whether an update is required. An update is required
        /// at the moment that the <see cref="CurrentCount"/> exactly meets the count representing
        /// a single refresh.
        /// </summary>
        /// <value><c>true</c> if this instance is update required; otherwise, <c>false</c>.</value>
        public bool IsRefreshRequired
        {
            get
            {
                var remainder = CurrentCount % _refreshInterval;
                return remainder == 0L;
            }
        }
    }
}
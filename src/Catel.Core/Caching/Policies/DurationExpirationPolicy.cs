namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
	///	The cache item will expire using the duration to calculate the absolute expiration from now.
    /// </summary>
    public class DurationExpirationPolicy : AbsoluteExpirationPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurationExpirationPolicy"/> class.
        /// </summary>
        /// <param name="durationTimeSpan">
        /// The expiration.
        /// </param>
        internal DurationExpirationPolicy(TimeSpan durationTimeSpan)
            : this(durationTimeSpan, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DurationExpirationPolicy"/> class.
        /// </summary>
        /// <param name="durationTimeSpan">
        /// The expiration.
        /// </param>
        /// <param name="canReset">
        /// The can reset.
        /// </param>
        protected DurationExpirationPolicy(TimeSpan durationTimeSpan, bool canReset)
            : base(FastDateTime.Now.Add(durationTimeSpan), canReset)
        {
            DurationTimeSpan = durationTimeSpan;
        }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        protected TimeSpan DurationTimeSpan { get; set; }
    }
}

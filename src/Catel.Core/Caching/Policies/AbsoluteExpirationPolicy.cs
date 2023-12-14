namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
    /// The cache item will expire on the absolute expiration date time.
    /// </summary>
    public class AbsoluteExpirationPolicy : ExpirationPolicy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteExpirationPolicy"/> class.
        /// </summary>
        /// <param name="absoluteExpirationDateTime">
        /// The expiration date time.
        /// </param>
        internal AbsoluteExpirationPolicy(DateTime absoluteExpirationDateTime)
            : this(absoluteExpirationDateTime, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbsoluteExpirationPolicy"/> class.
        /// </summary>
        /// <param name="absoluteExpirationDateTime">
        /// The expiration date time.
        /// </param>
        /// <param name="canReset">
        /// The can reset.
        /// </param>
        protected AbsoluteExpirationPolicy(DateTime absoluteExpirationDateTime, bool canReset)
            : base(canReset)
        {
            AbsoluteExpirationDateTime = absoluteExpirationDateTime;
        }

        /// <summary>
        /// Gets or sets the expiration date time.
        /// </summary>
        protected DateTime AbsoluteExpirationDateTime { get; set; }

        /// <summary>
        /// Gets a value indicating whether is expired.
        /// </summary>
        public override bool IsExpired
        {
            get
            {
                return FastDateTime.Now > AbsoluteExpirationDateTime;
            }
        }
    }
}

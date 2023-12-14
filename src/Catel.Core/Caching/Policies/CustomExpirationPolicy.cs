namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
    /// The custom expiration policy.
    /// </summary>
    public sealed class CustomExpirationPolicy : ExpirationPolicy
    {
        /// <summary>
        /// The function to check if the policy is expired.
        /// </summary>
        private readonly Func<bool>? _isExpiredFunc;

        /// <summary>
        ///  The action that will be executed if the item is read before expiration.
        /// </summary>
        private readonly Action? _resetAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExpirationPolicy"/> class.
        /// </summary>
        /// <param name="isExpiredFunc">
        /// The function to check if the policy is expired.
        /// </param>
        /// <param name="resetAction">
        /// The action that will be executed if the item is read before expiration.
        /// </param>
        public CustomExpirationPolicy(Func<bool>? isExpiredFunc = null, Action? resetAction = null)
            : base(resetAction is not null)
        {
            _isExpiredFunc = isExpiredFunc;
            _resetAction = resetAction;
        }

        /// <summary>
        /// Gets a value indicating whether is expired.
        /// </summary>
        public override bool IsExpired
        {
            get
            {
                return _isExpiredFunc is null || _isExpiredFunc.Invoke();
            }
        }

		/// <summary>
        /// Called when the policy is resetting.
        /// </summary>
        protected override void OnReset()
        {
            _resetAction?.Invoke();
        }
    }
}

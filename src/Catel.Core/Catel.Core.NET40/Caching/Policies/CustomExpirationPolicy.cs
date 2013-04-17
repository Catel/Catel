// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomExpirationPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Caching.Policies
{
    using System;

    /// <summary>
    /// The custom expiration policy.
    /// </summary>
    public sealed class CustomExpirationPolicy : ExpirationPolicy
    {
        #region Fields

        /// <summary>
        /// The function to check if the policy is expired.
        /// </summary>
        private readonly Func<bool> _isExpiredFunc;

        /// <summary>
        ///  The action that will be executed if the item is read before expiration.
        /// </summary>
        private readonly Action _resetAction;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomExpirationPolicy"/> class.
        /// </summary>
        /// <param name="isExpiredFunc">
        /// The function to check if the policy is expired.
        /// </param>
        /// <param name="resetAction">
        /// The action that will be executed if the item is read before expiration.
        /// </param>
        internal CustomExpirationPolicy(Func<bool> isExpiredFunc = null, Action resetAction = null)
            : base(resetAction != null)
        {
            _isExpiredFunc = isExpiredFunc;
            _resetAction = resetAction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether is expired.
        /// </summary>
        public override bool IsExpired
        {
            get
            {
                return _isExpiredFunc == null || _isExpiredFunc.Invoke();
            }
        }
        #endregion

        #region Methods

		/// <summary>
        /// Called when the policy is resetting.
        /// </summary>
        protected override void OnReset()
        {
            _resetAction.Invoke();
        }

        #endregion
    }
}
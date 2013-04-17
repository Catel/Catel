// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeExpirationPolicy.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Caching.Policies
{
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Threading;

    /// <summary>
    /// The composite expiration policy.
    /// </summary>
    public sealed class CompositeExpirationPolicy : ExpirationPolicy
    {
        #region Fields

        /// <summary>
        /// The expiration policies.
        /// </summary>
        private readonly List<ExpirationPolicy> _expirationPolicies = new List<ExpirationPolicy>();

        /// <summary>
        /// The synchronization context.
        /// </summary>
        private readonly SynchronizationContext _synchronizationContext = new SynchronizationContext();

        /// If <c>true</c> indicates that the cache will expires only if <b>All</b> policies of the composition expires, 
        /// otherwise the cache will expires if <b>Any</b> policy does.
        private readonly bool _expiresOnlyIfAllPoliciesExpires;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeExpirationPolicy"/> class.
        /// </summary>
        /// <param name="expiresOnlyIfAllPoliciesExpires">
        /// If <c>true</c> indicates that the cache will expires only if <b>All</b> policies of the composition expires, 
        /// otherwise the cache will expires if <b>Any</b> policy does.
        /// </param>
        public CompositeExpirationPolicy(bool expiresOnlyIfAllPoliciesExpires = false)
        {
            _expiresOnlyIfAllPoliciesExpires = expiresOnlyIfAllPoliciesExpires;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether can reset.
        /// </summary>
        public override bool CanReset
        {
            get
            {
                _synchronizationContext.Acquire();

                bool canReset = _expirationPolicies.Any(policy => policy.CanReset);

                if (!IsResting || !canReset)
                {
                    _synchronizationContext.Release();
                }

                return canReset;
            }
        }

        /// <summary>
        /// Gets a value indicating whether is expired.
        /// </summary>
        public override bool IsExpired
        {
            get
            {
                return _expiresOnlyIfAllPoliciesExpires ? _synchronizationContext.Execute(() => _expirationPolicies.All(policy => policy.IsExpired)) : _synchronizationContext.Execute(() => _expirationPolicies.Any(policy => policy.IsExpired));
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Called when the policy is resetting.
        /// </summary>
        protected override void OnReset()
        {
            try
            {
                foreach (var expirationPolicy in _expirationPolicies.Where(expirationPolicy => expirationPolicy.CanReset))
                {
                    expirationPolicy.Reset();
                }
            }
            finally
            {
                if (IsResting)
                {
                    _synchronizationContext.Release();
                }
            }
        }

        /// <summary>
        /// Adds an expiration policy to the composition.
        /// </summary>
        /// <param name="expirationPolicy">
        /// The expiration policy.
        /// </param>
        /// <returns>
        /// The <see cref="CompositeExpirationPolicy"/>.
        /// </returns>
        public CompositeExpirationPolicy Add(ExpirationPolicy expirationPolicy)
        {
            _synchronizationContext.Execute(() => _expirationPolicies.Add(expirationPolicy));

            return this;
        }

        #endregion
    }
}
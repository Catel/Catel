// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Tests.Caching.Policies
{
    using System;

    using Catel.Caching.Policies;

    using NUnit.Framework;

    /// <summary>
    /// The sliding expiration policy facts.
    /// </summary>
    public class SlidingExpirationPolicyFacts
    {
        #region Nested type: TheCanResetProperty

        /// <summary>
        /// The can reset property.
        /// </summary>
        [TestFixture]
        public class TheCanResetProperty
        {
            #region Methods

            /// <summary>
            /// The returns true.
            /// </summary>
            [TestCase]
            public void ReturnsTrue()
            {
                Assert.IsTrue(new SlidingExpirationPolicy(TimeSpan.FromTicks(0)).CanReset);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheIsExpiredProperty

        /// <summary>
        /// The the is expired property.
        /// </summary>
        [TestFixture]
        public class TheIsExpiredProperty
        {
            #region Methods

            /// <summary>
            /// The returns true if the expiration date time is the pass.
            /// </summary>
            [TestCase]
            public void ReturnsTrueIfTheExpirationDateTimeIsThePass()
            {
                Assert.IsTrue(new SlidingExpirationPolicy(TimeSpan.FromDays(-1)).IsExpired);
            }

            /// <summary>
            /// The returns false if the expiration date time is the future.
            /// </summary>
            [TestCase]
            public void ReturnsFalseIfTheExpirationDateTimeIsTheFuture()
            {
                Assert.IsFalse(new SlidingExpirationPolicy(TimeSpan.FromDays(1)).IsExpired);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheResetMethod

        /// <summary>
        /// The can reset property.
        /// </summary>
        [TestFixture]
        public class TheResetMethod
        {
            #region Methods

            /// <summary>
            /// The reset the expiration state.
            /// </summary>
            [TestCase]
            public void TurnTheIsExpiredPropertyToFalse()
            {
                var policy = new SlidingExpirationPolicy(new TimeSpan(0, 0, 0, 0, 250));

                ThreadHelper.Sleep(500);

                Assert.IsTrue(policy.IsExpired);

                policy.Reset();
                Assert.IsFalse(policy.IsExpired);
            }

            #endregion
        }
        #endregion
    }
}
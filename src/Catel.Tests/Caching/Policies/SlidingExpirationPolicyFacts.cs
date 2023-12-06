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
                Assert.That(new SlidingExpirationPolicy(TimeSpan.FromTicks(0)).CanReset, Is.True);
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
                Assert.That(new SlidingExpirationPolicy(TimeSpan.FromDays(-1)).IsExpired, Is.True);
            }

            /// <summary>
            /// The returns false if the expiration date time is the future.
            /// </summary>
            [TestCase]
            public void ReturnsFalseIfTheExpirationDateTimeIsTheFuture()
            {
                Assert.That(new SlidingExpirationPolicy(TimeSpan.FromDays(1)).IsExpired, Is.False);
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

                Assert.That(policy.IsExpired, Is.True);

                policy.Reset();
                Assert.That(policy.IsExpired, Is.False);
            }

            #endregion
        }
        #endregion
    }
}
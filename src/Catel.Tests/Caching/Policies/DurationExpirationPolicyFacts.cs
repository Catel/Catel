namespace Catel.Tests.Caching.Policies
{
    using System;

    using Catel.Caching.Policies;

    using NUnit.Framework;

    /// <summary>
    /// The duration expiration policy facts.
    /// </summary>
    public class DurationExpirationPolicyFacts
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
            /// The returns false.
            /// </summary>
            [TestCase]
            public void ReturnsFalse()
            {
                Assert.That(new DurationExpirationPolicy(new TimeSpan(0)).CanReset, Is.False);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheFromMethod
        #endregion

        #region Nested type: TheIsExpiredPropety

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
                Assert.That(new DurationExpirationPolicy(TimeSpan.FromDays(-1)).IsExpired, Is.True);
            }

            /// <summary>
            /// The returns false if the expiration date time is the future.
            /// </summary>
            [TestCase]
            public void ReturnsFalseIfTheExpirationDateTimeIsTheFuture()
            {
                Assert.That(new DurationExpirationPolicy(TimeSpan.FromDays(1)).IsExpired, Is.False);
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
            /// The throws invalid operation exception.
            /// </summary>
            [TestCase]
            public void ThrowsInvalidOperationException()
            {
                Assert.Throws<InvalidOperationException>(() => new DurationExpirationPolicy(new TimeSpan(0)).Reset());
            }

            #endregion
        }
        #endregion
    }
}
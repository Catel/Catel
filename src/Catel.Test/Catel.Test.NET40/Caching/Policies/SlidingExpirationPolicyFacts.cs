// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Caching.Policies
{
    using System;

    using Catel.Caching.Policies;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    /// <summary>
    /// The sliding expiration policy facts.
    /// </summary>
    public class SlidingExpirationPolicyFacts
    {
        #region Nested type: TheCanResetProperty

        /// <summary>
        /// The can reset property.
        /// </summary>
        [TestClass]
        public class TheCanResetProperty
        {
            #region Methods

            /// <summary>
            /// The returns true.
            /// </summary>
            [TestMethod]
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
        [TestClass]
        public class TheIsExpiredProperty
        {
            #region Methods

            /// <summary>
            /// The returns true if the expiration date time is the pass.
            /// </summary>
            [TestMethod]
            public void ReturnsTrueIfTheExpirationDateTimeIsThePass()
            {
                Assert.IsTrue(new SlidingExpirationPolicy(TimeSpan.FromDays(-1)).IsExpired);
            }

            /// <summary>
            /// The returns false if the expiration date time is the future.
            /// </summary>
            [TestMethod]
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
        [TestClass]
        public class TheResetMethod
        {
            #region Methods

            /// <summary>
            /// The reset the expiration state.
            /// </summary>
            [TestMethod]
            public void TurnTheIsExpiredPropertyToFalse()
            {
                var policy = new SlidingExpirationPolicy(new TimeSpan(0, 0, 1));

                ThreadHelper.Sleep(1500);

                Assert.IsTrue(policy.IsExpired);

                policy.Reset();
                Assert.IsFalse(policy.IsExpired);
            }

            #endregion
        }
        #endregion
    }
}
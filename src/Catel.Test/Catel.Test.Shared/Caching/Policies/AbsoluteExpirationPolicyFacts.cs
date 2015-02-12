// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbsoluteExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Caching.Policies
{
    using System;

    using Catel.Caching.Policies;

    using NUnit.Framework;

    /// <summary>
    /// The absolute expiration policy facts.
    /// </summary>
    public class AbsoluteExpirationPolicyFacts
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
                Assert.IsFalse(new AbsoluteExpirationPolicy(DateTime.Now.AddDays(1)).CanReset);
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
                Assert.IsTrue(new AbsoluteExpirationPolicy(DateTime.Now.AddDays(-1)).IsExpired);
            }

            /// <summary>
            /// The returns false if the expiration date time is the future.
            /// </summary>
            [TestCase]
            public void ReturnsFalseIfTheExpirationDateTimeIsTheFuture()
            {
                Assert.IsFalse(new AbsoluteExpirationPolicy(DateTime.Now.AddDays(1)).IsExpired);
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
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => new AbsoluteExpirationPolicy(DateTime.Now.AddDays(1)).Reset());
            }

            #endregion
        }
        #endregion
    }
}
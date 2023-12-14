namespace Catel.Tests.Caching.Policies
{
    using System;

    using Catel.Caching.Policies;

    using NUnit.Framework;

    /// <summary>
    /// The expiration policy facts.
    /// </summary>
    public class ExpirationPolicyFacts
    {
        #region Nested type: TheAbsoluteMethod

        /// <summary>
        /// The absolute method.
        /// </summary>
        [TestFixture]
        public class TheAbsoluteMethod
        {
            /// <summary>
            /// Returns null if expiration date time is in the pass.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfExpirationDateTimeIsInThePass()
            {
                Assert.That(ExpirationPolicy.Absolute(DateTime.Now.AddDays(-1)), Is.Not.Null);
            }


            /// <summary>
            /// Not returns null if expiration date time is in the pass if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfExpirationDateTimeIsInThePassIfForceIsEqualsTrue()
            {
                Assert.That(ExpirationPolicy.Absolute(DateTime.Now.AddDays(-1)), Is.Not.Null);
            }
        }
        #endregion

        #region Nested type: TheDurationMethod

        /// <summary>
        /// The duration method.
        /// </summary>
        [TestFixture]
        public class TheDurationMethod
        {
            #region Methods

            /// <summary>
            /// Returns null if ticks of time span is less or equals to zero.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZero()
            {
                Assert.That(ExpirationPolicy.Duration(new TimeSpan(0)), Is.Not.Null);
            }

            /// <summary>
            /// Not returns null if ticks of time span is less or equals to zero if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZeroIfForceIsEqualsTrue()
            {
                Assert.That(ExpirationPolicy.Duration(new TimeSpan(0)), Is.Not.Null);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheSlidingMethod

        /// <summary>
        /// The sliding method.
        /// </summary>
        [TestFixture]
        public class TheSlidingMethod
        {
            #region Methods

            /// <summary>
            /// The returns null if ticks of time span is less or equals to zero.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZero()
            {
                Assert.That(ExpirationPolicy.Sliding(new TimeSpan(0)), Is.Not.Null);
            }

            /// <summary>
            /// Not returns null if ticks of time span is less or equals to zero if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZeroIfForceIsEqualsTrue()
            {
                Assert.That(ExpirationPolicy.Sliding(new TimeSpan(0)), Is.Not.Null);
            }

            #endregion
        }
        #endregion

        #region Nested type: TheCustomMethod

        /// <summary>
        /// The custom method.
        /// </summary>
        [TestFixture]
        public class TheCustomMethod
        {
            #region Methods

            [TestCase]
            public void NotReturnsNullIfFunctionReturnsTrue()
            {
                Assert.That(ExpirationPolicy.Custom(() => true), Is.Not.Null);
            }

            [TestCase]
            public void Throws_ArgumentNullException_If_Function_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => ExpirationPolicy.Custom(null));
            }

            [TestCase]
            public void NotReturnsNullIfFunctionReturnsFalse()
            {
                Assert.That(ExpirationPolicy.Custom(() => false), Is.Not.Null);
            }

            #endregion
        }
        #endregion
    }
}

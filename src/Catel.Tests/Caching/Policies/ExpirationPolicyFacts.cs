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
                Assert.IsNotNull(ExpirationPolicy.Absolute(DateTime.Now.AddDays(-1)));
            }


            /// <summary>
            /// Not returns null if expiration date time is in the pass if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfExpirationDateTimeIsInThePassIfForceIsEqualsTrue()
            {
                Assert.IsNotNull(ExpirationPolicy.Absolute(DateTime.Now.AddDays(-1)));
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
                Assert.IsNotNull(ExpirationPolicy.Duration(new TimeSpan(0)));
            }

            /// <summary>
            /// Not returns null if ticks of time span is less or equals to zero if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZeroIfForceIsEqualsTrue()
            {
                Assert.IsNotNull(ExpirationPolicy.Duration(new TimeSpan(0)));
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
                Assert.IsNotNull(ExpirationPolicy.Sliding(new TimeSpan(0)));
            }

            /// <summary>
            /// Not returns null if ticks of time span is less or equals to zero if force is equals true.
            /// </summary>
            [TestCase]
            public void NotReturnsNullIfTicksOfTimeSpanIsLessOrEqualsToZeroIfForceIsEqualsTrue()
            {
                Assert.IsNotNull(ExpirationPolicy.Sliding(new TimeSpan(0)));
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
                Assert.IsNotNull(ExpirationPolicy.Custom(() => true));
            }

            [TestCase]
            public void Throws_ArgumentNullException_If_Function_Is_Null()
            {
                Assert.Throws<ArgumentNullException>(() => ExpirationPolicy.Custom(null));
            }

            [TestCase]
            public void NotReturnsNullIfFunctionReturnsFalse()
            {
                Assert.IsNotNull(ExpirationPolicy.Custom(() => false));
            }

            #endregion
        }
        #endregion
    }
}

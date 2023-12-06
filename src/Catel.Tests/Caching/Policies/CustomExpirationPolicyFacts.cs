namespace Catel.Tests.Caching.Policies
{
    using Catel.Caching.Policies;

    using NUnit.Framework;

    public class CustomExpirationPolicyFacts
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
            /// The returns true if an action is specified.
            /// </summary>
            [TestCase]
            public void ReturnsTrueIfAnActionIsSpecified()
            {
                Assert.That(new CustomExpirationPolicy(() => true, () => ThreadHelper.Sleep(0)).CanReset, Is.True);
            }

            [TestCase]
            public void ReturnsFalseIfAnActionNotIsSpecified()
            {
                Assert.That(new CustomExpirationPolicy(() => true).CanReset, Is.False);
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

            [TestCase]
            public void ReturnsTrueIfFunctionRetursTrue()
            {
                Assert.That(new CustomExpirationPolicy(() => true).IsExpired, Is.True);
            }

            [TestCase]
            public void ReturnsTrueIfActionIsNull()
            {
                Assert.That(new CustomExpirationPolicy().IsExpired, Is.True);
            }

            [TestCase]
            public void ReturnsTrueIfFunctionRetursFalse()
            {
                Assert.That(new CustomExpirationPolicy(() => false).IsExpired, Is.False);
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
            /// Invokes the action.
            /// </summary>
            [TestCase]
            public void InvokesTheAction()
            {
                bool actionInvoked = false;

                var customExpirationPolicy = new CustomExpirationPolicy(() => true, () => actionInvoked = true);
                customExpirationPolicy.Reset();

                Assert.That(actionInvoked, Is.True);
            }

            #endregion
        }
        #endregion
    }
}
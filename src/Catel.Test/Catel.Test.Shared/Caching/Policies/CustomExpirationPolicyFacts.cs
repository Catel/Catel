// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Caching.Policies
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
                Assert.IsTrue(new CustomExpirationPolicy(() => true, () => ThreadHelper.Sleep(0)).CanReset);
            }         
            
            [TestCase]
            public void ReturnsFalseIfAnActionNotIsSpecified()
            {
                Assert.IsFalse(new CustomExpirationPolicy(() => true).CanReset);
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
                Assert.IsTrue(new CustomExpirationPolicy(() => true).IsExpired);
            }         
            
            [TestCase]
            public void ReturnsTrueIfActionIsNull()
            {
                Assert.IsTrue(new CustomExpirationPolicy().IsExpired);
            }

            [TestCase]
            public void ReturnsTrueIfFunctionRetursFalse()
            {
                Assert.IsFalse(new CustomExpirationPolicy(() => false).IsExpired);
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
                
                Assert.IsTrue(actionInvoked);
            }

            #endregion
        }
        #endregion
    }
}
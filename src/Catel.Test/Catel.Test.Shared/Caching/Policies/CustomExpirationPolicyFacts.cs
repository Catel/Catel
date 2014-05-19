// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Caching.Policies
{
    using Catel.Caching.Policies;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CustomExpirationPolicyFacts
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
            /// The returns true if an action is specified.
            /// </summary>
            [TestMethod]
            public void ReturnsTrueIfAnActionIsSpecified()
            {
                Assert.IsTrue(new CustomExpirationPolicy(() => true, () => ThreadHelper.Sleep(0)).CanReset);
            }         
            
            [TestMethod]
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
        [TestClass]
        public class TheIsExpiredProperty
        {
            #region Methods

            [TestMethod]
            public void ReturnsTrueIfFunctionRetursTrue()
            {
                Assert.IsTrue(new CustomExpirationPolicy(() => true).IsExpired);
            }         
            
            [TestMethod]
            public void ReturnsTrueIfActionIsNull()
            {
                Assert.IsTrue(new CustomExpirationPolicy().IsExpired);
            }

            [TestMethod]
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
        [TestClass]
        public class TheResetMethod
        {
            #region Methods

            /// <summary>
            /// Invokes the action.
            /// </summary>
            [TestMethod]
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
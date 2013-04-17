// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SlidingExpirationPolicyFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Caching.Policies
{
    using System;
    using System.Threading;

    using Catel.Caching.Policies;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CompositeExpirationPolicyFacts
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
            /// Returns true if any policy can be reset.
            /// </summary>
            [TestMethod]
            public void ReturnsTrueIfAnyPolicyCanBeReset()
            {
                Assert.IsTrue(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true, () => ThreadHelper.Sleep(0))).Add(new CustomExpirationPolicy(() => true)).CanReset);
            }

            /// <summary>
            /// Returns false if all policy can not be reset.
            /// </summary>
            [TestMethod]
            public void ReturnsFalseIfAllPolicyCanNotBeReset()
            {
                Assert.IsFalse(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true)).CanReset);
            }         
            
            #if NET 
            
            [TestMethod]
            public void DoesNotCauseDeathLock()
            {
                CompositeExpirationPolicy policy = new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true));
                var events = new [] { new AutoResetEvent(false), new AutoResetEvent(false)};

                new Thread(() => 
                { 
                    Assert.IsFalse(policy.CanReset);
                    events[0].Set();
                }).Start();

                new Thread(() =>
                {
                    Assert.IsFalse(policy.CanReset);
                    events[1].Set();
                }).Start();

                // How specify the apartment statte to with MSTest: 
                // WaitHandle.WaitAll(events,TimeSpan.FromSeconds(10))
                events[0].WaitOne(TimeSpan.FromSeconds(10));
                events[1].WaitOne(TimeSpan.FromSeconds(10));
            }
            
            #endif
            
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
            public void ReturnsTrueIfAnyPolicyExpires()
            {
                Assert.IsTrue(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => true)).IsExpired);
            }


            [TestMethod]
            public void ReturnsFalseIfAnyPolicyExpiresButWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.IsFalse(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => true)).IsExpired);
            }


            [TestMethod]
            public void ReturnsTrueIfAllPolicyExpiresButWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.IsTrue(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true)).IsExpired);
            }

            [TestMethod]
            public void ReturnsFalseIfAllPolicyNonExpiresWasConfiguredToExpireOnlyIfAllPolicyExpires()
            {
                Assert.IsFalse(new CompositeExpirationPolicy(true).Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => false)).IsExpired);
            }         

            [TestMethod]
            public void ReturnsFalseIfAllPolicyNonExpires()
            {
                Assert.IsFalse(new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => false)).Add(new CustomExpirationPolicy(() => false)).IsExpired);
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
            public void InvokesTheResetActions()
            {
                bool actionInvoked1 = false;
                bool actionInvoked2 = false;
           
                new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true, () => actionInvoked1 = true)).Add(new CustomExpirationPolicy(() => true, () => actionInvoked2 = true)).Reset();
                
                Assert.IsTrue(actionInvoked1);
                Assert.IsTrue(actionInvoked2);
            }

            #if NET

            [TestMethod]
            public void DoesNotCauseDeathLockIfPolicyCanNotBeResetAndThrowsInvalidOperationException()
            {
                var events = new[] { new AutoResetEvent(false), new AutoResetEvent(false) };
                CompositeExpirationPolicy policy = new CompositeExpirationPolicy().Add(new CustomExpirationPolicy(() => true)).Add(new CustomExpirationPolicy(() => true));

                new Thread(() =>
                    {
                        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(policy.Reset);
                        events[0].Set();
                    }).Start();

                new Thread(() =>
                    {
                        ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(policy.Reset);
                        events[1].Set();
                    }).Start();

                // How specify the apartment statte to with MSTest: 
                // WaitHandle.WaitAll(events,TimeSpan.FromSeconds(10))
                events[0].WaitOne(TimeSpan.FromSeconds(10));
                events[1].WaitOne(TimeSpan.FromSeconds(10));
            }
            
            #endif 

            #endregion
        }
        #endregion
    }
}
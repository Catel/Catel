// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationContextTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Threading
{
    using System;
    using System.Threading;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    using SynchronizationContext = Catel.Threading.SynchronizationContext;

    public class SynchronizationContextFacts
    {
        [TestClass]
        public class TheAcquireMethod
        {
            #region Public Methods and Operators

            [TestMethod]
            public void SetsIsLockAquiredToTrue()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Acquire();
                Assert.IsTrue(context.IsLockAcquired);
            }

            #endregion
        }

        [TestClass]
        public class TheExecuteMethod
        {
            #region Public Methods and Operators

            [TestMethod]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEnd()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Execute(() => Assert.IsTrue(context.IsLockAcquired));
                Assert.IsFalse(context.IsLockAcquired);
            }

            [TestMethod]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEndAndReturnAValue()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                int expected = new Random().Next(50, 100);
                int result = context.Execute(() =>
                    {
                        Assert.IsTrue(context.IsLockAcquired);
                        return expected;
                    });
                Assert.IsFalse(context.IsLockAcquired);
                Assert.AreEqual(expected, result);
            }

            [TestMethod]
            public void NestedExecuteCallAreAllowed()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                int expected = new Random().Next(50, 100);
                int result = context.Execute(() =>
                    {
                        int nestedResult = context.Execute(() =>
                            {
                                Assert.IsTrue(context.IsLockAcquired);
                                return expected;
                            });
                        Assert.IsTrue(context.IsLockAcquired);
                        return nestedResult;
                    });

                Assert.IsFalse(context.IsLockAcquired);
                Assert.AreEqual(expected, result);
            }

            #endregion
        }

        [TestClass]
        public class TheEnqueueMethod
        {
            #region Public Methods and Operators

            [TestMethod]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEnd()
            {
                var @event = new AutoResetEvent(false);
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Enqueue(() => Assert.IsTrue(context.IsLockAcquired), (sender, args) => @event.Set());
                @event.WaitOne();
                Assert.IsFalse(context.IsLockAcquired);
            }

            [TestMethod]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEndAndReturnAValue()
            {
                var @event = new AutoResetEvent(false);
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                int expected = new Random().Next(50, 100);
                int result = 0;
                context.Enqueue(() =>
                {
                    Assert.IsTrue(context.IsLockAcquired);
                    return expected;
                }, (sender, args) =>
                    {
                        result = (int)args.Result;
                        @event.Set();
                    });
                @event.WaitOne();
                Assert.IsFalse(context.IsLockAcquired);
                Assert.AreEqual(expected, result);
            }

            #endregion
        }

        [TestClass]
        public class TheReleaseMethod
        {
            #region Public Methods and Operators

            [TestMethod]
            public void KeepsIsLockAquiredInFalse()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Release();
                Assert.IsFalse(context.IsLockAcquired);
            }

            #endregion
        }
    }
}
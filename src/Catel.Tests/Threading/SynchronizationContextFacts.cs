namespace Catel.Tests.Threading
{
    using System;
    using System.Threading;
    using NUnit.Framework;

    using SynchronizationContext = Catel.Threading.SynchronizationContext;

    public class SynchronizationContextFacts
    {
        [TestFixture]
        public class TheAcquireMethod
        {
            #region Public Methods and Operators

            [TestCase]
            public void SetsIsLockAquiredToTrue()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Acquire();
                Assert.IsTrue(context.IsLockAcquired);
            }

            #endregion
        }

        [TestFixture]
        public class TheExecuteMethod
        {
            #region Public Methods and Operators

            [TestCase]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEnd()
            {
                var context = new SynchronizationContext();
                Assert.IsFalse(context.IsLockAcquired);
                context.Execute(() => Assert.IsTrue(context.IsLockAcquired));
                Assert.IsFalse(context.IsLockAcquired);
            }

            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class TheReleaseMethod
        {
            #region Public Methods and Operators

            [TestCase]
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
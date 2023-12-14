namespace Catel.Tests.Threading
{
    using System;
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
                Assert.That(context.IsLockAcquired, Is.False);
                context.Acquire();
                Assert.That(context.IsLockAcquired, Is.True);
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
                Assert.That(context.IsLockAcquired, Is.False);
                context.Execute(() => Assert.That(context.IsLockAcquired, Is.True));
                Assert.That(context.IsLockAcquired, Is.False);
            }

            [TestCase]
            public void AdquiresTheLockDuringTheExecutionAndReleaseItAtTheEndAndReturnAValue()
            {
                var context = new SynchronizationContext();
                Assert.That(context.IsLockAcquired, Is.False);
                int expected = new Random().Next(50, 100);
                int result = context.Execute(() =>
                    {
                        Assert.That(context.IsLockAcquired, Is.True);
                        return expected;
                    });
                Assert.That(context.IsLockAcquired, Is.False);
                Assert.That(result, Is.EqualTo(expected));
            }

            [TestCase]
            public void NestedExecuteCallAreAllowed()
            {
                var context = new SynchronizationContext();
                Assert.That(context.IsLockAcquired, Is.False);
                int expected = new Random().Next(50, 100);
                int result = context.Execute(() =>
                    {
                        int nestedResult = context.Execute(() =>
                            {
                                Assert.That(context.IsLockAcquired, Is.True);
                                return expected;
                            });
                        Assert.That(context.IsLockAcquired, Is.True);
                        return nestedResult;
                    });

                Assert.That(context.IsLockAcquired, Is.False);
                Assert.That(result, Is.EqualTo(expected));
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
                Assert.That(context.IsLockAcquired, Is.False);
                context.Release();
                Assert.That(context.IsLockAcquired, Is.False);
            }

            #endregion
        }
    }
}
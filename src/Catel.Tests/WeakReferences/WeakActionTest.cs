namespace Catel.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class WeakActionTest
    {
        #region Test classes
        public class ActionTarget
        {
            #region Properties
            public int PublicActionExecutedCount { get; private set; }

            public int PrivateActionExecutedCount { get; private set; }

            public int PublicActionWithParameterExecutedCount { get; private set; }

            public int PrivateActionWithParameterExecutedCount { get; private set; }
            #endregion

            #region Methods
            public void PublicActionToExecute()
            {
                PublicActionExecutedCount++;
            }

            public void PrivateActionToExecute()
            {
                PrivateActionExecutedCount++;
            }

            public void PublicActionWithParameterToExecute(int parameter)
            {
                PublicActionWithParameterExecutedCount++;
            }

            public void PrivateActionWithParameterToExecute(int parameter)
            {
                PrivateActionWithParameterExecutedCount++;
            }
            #endregion
        }
        #endregion

        #region Methods
        [TestCase]
        public void MemoryLeakFreeWithNoInvocation()
        {
            var target = CreateActionTarget();
            var weakAction = new WeakAction(target, target.PublicActionToExecute);

            target = null;

            GCHelper.CollectAndFreeMemory();

            Assert.That(weakAction.IsTargetAlive, Is.False);
        }

        [TestCase]
        public void NonGeneric_PublicMethod()
        {
            var target = CreateActionTarget();
            var weakAction = new WeakAction(target, target.PublicActionToExecute);

            Assert.That(weakAction.Execute(), Is.True);

            Assert.That(target.PublicActionExecutedCount, Is.EqualTo(1));

            target = null;

            GCHelper.CollectAndFreeMemory();

            Assert.That(weakAction.IsTargetAlive, Is.False);
        }

        [TestCase]
        public void NonGeneric_AnonymousDelegate()
        {
            int count = 0;

            Assert.Throws<NotSupportedException>(() => new WeakAction(null, () => count++));

            //weakAction.Execute();

            //Assert.AreEqual(1, count);
        }

        [TestCase]
        public void Generic_PublicMethod()
        {
            var target = CreateActionTarget();
            var weakAction = new WeakAction<int>(target, target.PublicActionWithParameterToExecute);

            Assert.That(weakAction.Execute(1), Is.True);

            Assert.That(target.PublicActionWithParameterExecutedCount, Is.EqualTo(1));

            target = null;

            GCHelper.CollectAndFreeMemory();

            Assert.That(weakAction.IsTargetAlive, Is.False);
        }

        [TestCase]
        public void Generic_AnonymousDelegate()
        {
            int count = 0;

            Assert.Throws<NotSupportedException>(() => new WeakAction<int>(null, i => count = i));

            Assert.That(count, Is.EqualTo(0));
        }

        // Require separate method to allow GC.Collect to do its magic
        private ActionTarget CreateActionTarget()
        {
            return new ActionTarget();
        }
        #endregion
    }
}

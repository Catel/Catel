// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakActionTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests
{
    using System;
    using NUnit.Framework;

    [TestFixture]
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
            var target = new ActionTarget();
            var weakAction = new WeakAction(target, target.PublicActionToExecute);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakAction.IsTargetAlive);
        }

        [TestCase]
        public void NonGeneric_PublicMethod()
        {
            var target = new ActionTarget();
            var weakAction = new WeakAction(target, target.PublicActionToExecute);

            Assert.IsTrue(weakAction.Execute());

            Assert.AreEqual(1, target.PublicActionExecutedCount);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakAction.IsTargetAlive);
        }

        [TestCase]
        public void NonGeneric_AnonymousDelegate()
        {
            int count = 0;

            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => new WeakAction(null, () => count++));

            //weakAction.Execute();

            //Assert.AreEqual(1, count);
        }

        [TestCase]
        public void Generic_PublicMethod()
        {
            var target = new ActionTarget();
            var weakAction = new WeakAction<int>(target, target.PublicActionWithParameterToExecute);

            Assert.IsTrue(weakAction.Execute(1));

            Assert.AreEqual(1, target.PublicActionWithParameterExecutedCount);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakAction.IsTargetAlive);
        }

        [TestCase]
        public void Generic_AnonymousDelegate()
        {
            int count = 0;

            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => new WeakAction<int>(null, i => count = i));

            Assert.AreEqual(0, count);
        }
        #endregion
    }
}
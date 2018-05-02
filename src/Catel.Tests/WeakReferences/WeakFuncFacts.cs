// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakFuncFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests
{
    using System;

    using NUnit.Framework;

    [TestFixture]
    public class WeakFuncFacts
    {
        #region Test classes
        public class FuncTarget
        {
            #region Properties
            public int PublicFuncExecutedCount { get; private set; }

            public int PrivateFuncExecutedCount { get; private set; }

            public int PublicFuncWithParameterExecutedCount { get; private set; }

            public int PrivateFuncWithParameterExecutedCount { get; private set; }
            #endregion

            #region Methods
            public bool PublicFuncToExecute()
            {
                PublicFuncExecutedCount++;
                return true;
            }

            public bool PrivateFuncToExecute()
            {
                PrivateFuncExecutedCount++;
                return true;
            }

            public bool PublicFuncWithParameterToExecute(int parameter)
            {
                PublicFuncWithParameterExecutedCount++;
                return true;
            }

            public bool PrivateFuncWithParameterToExecute(int parameter)
            {
                PrivateFuncWithParameterExecutedCount++;
                return true;
            }
            #endregion
        }
        #endregion

        #region Methods
        [TestCase]
        public void MemoryLeakFreeWithNoInvocation()
        {
            var target = new FuncTarget();
            var weakAction = new WeakFunc<bool>(target, target.PublicFuncToExecute);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakAction.IsTargetAlive);
        }

        [TestCase]
        public void NonGeneric_PublicMethod()
        {
            var target = new FuncTarget();
            var weakFunc = new WeakFunc<bool>(target, target.PublicFuncToExecute);

            bool result;
            Assert.IsTrue(weakFunc.Execute(out result));

            Assert.AreEqual(1, target.PublicFuncExecutedCount);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakFunc.IsTargetAlive);
        }

        [TestCase]
        public void NonGeneric_AnonymousDelegate()
        {
            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => new WeakFunc<bool>(null, () => true));
        }

        [TestCase]
        public void Generic_PublicMethod()
        {
            var target = new FuncTarget();
            var weakFunc = new WeakFunc<int, bool>(target, target.PublicFuncWithParameterToExecute);

            bool result;
            Assert.IsTrue(weakFunc.Execute(1, out result));

            Assert.AreEqual(1, target.PublicFuncWithParameterExecutedCount);

            target = null;
            GC.Collect();

            Assert.IsFalse(weakFunc.IsTargetAlive);
        }

        [TestCase]
        public void Generic_AnonymousDelegate()
        {
            var count = 0;

            ExceptionTester.CallMethodAndExpectException<NotSupportedException>(
                () => new WeakFunc<int, bool>(
                    null,
                    i =>
                        {
                            count = i;
                            return true;
                        }));

            Assert.AreEqual(0, count);
        }
        #endregion
    }
}
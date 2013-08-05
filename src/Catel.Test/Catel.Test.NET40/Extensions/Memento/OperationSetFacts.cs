// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSetFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using Catel.Memento;
    using Mocks;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class OperationSetFacts
    {
        [TestClass]
        public class TheOperationSetUndoMethod
        {
            [TestMethod]
            public void UndoTest()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo(true);

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                operationSet.Undo();

                Assert.IsTrue(mock1.UndoCalled);
                Assert.IsTrue(mock2.UndoCalled);
            }

            [TestMethod]
            public void UndoOrderTest()
            {
                var finalValue = 0;
                var mock1 = new MockUndo(true);

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(new ActionUndo(this, () => finalValue = 1));
                operationSet.Add(new ActionUndo(this, () => finalValue = 2));

                operationSet.Undo();

                Assert.IsTrue(mock1.UndoCalled);
                Assert.AreEqual(1, finalValue);
            }
        }

        [TestClass]
        public class ThePropertyRedoMethod
        {
            [TestMethod()]
            public void CanRedoOk()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo(true);

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                Assert.IsTrue(operationSet.CanRedo);
            }

            [TestMethod()]
            public void CanRedoNak()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo();

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                Assert.IsFalse(operationSet.CanRedo);
            }


            [TestMethod()]
            public void RedoTest()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo(true);

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                operationSet.Redo();
                Assert.IsTrue(mock1.RedoCalled);
                Assert.IsTrue(mock2.RedoCalled);
            }
        }
    }
}
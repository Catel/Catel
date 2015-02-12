// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperationSetFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using Catel.Memento;
    using Mocks;

    using NUnit.Framework;

    public class OperationSetFacts
    {
        [TestFixture]
        public class TheOperationSetUndoMethod
        {
            [TestCase]
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

            [TestCase]
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

        [TestFixture]
        public class ThePropertyRedoMethod
        {
            [TestCase]
            public void CanRedoOk()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo(true);

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                Assert.IsTrue(operationSet.CanRedo);
            }

            [TestCase]
            public void CanRedoNak()
            {
                var mock1 = new MockUndo(true);
                var mock2 = new MockUndo();

                var operationSet = new OperationSet(this);
                operationSet.Add(mock1);
                operationSet.Add(mock2);

                Assert.IsFalse(operationSet.CanRedo);
            }

            [TestCase]
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
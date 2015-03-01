// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionUndoTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;
    using Data;
    using Mocks;

    using NUnit.Framework;

    public class ActionUndoFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ActionUndo(null, () => MockModel.Change("test")));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullUndoMethod()
            {
                var obj = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new ActionUndo(obj, null));
            }
        }

        [TestFixture]
        public class TheUndoMethod
        {
            [TestCase]
            public void SetsOldValue()
            {
                var action = new ActionUndo(this, () => MockModel.Change("previousValue"));

                action.Undo();
                Assert.AreEqual("previousValue", MockModel.Name);
            }

            [TestCase]
            public void SetsNewValue()
            {
                var action = new ActionUndo(this, () => MockModel.Change("previousValue"), () => MockModel.Change("nextValue"));

                action.Redo();
                Assert.AreEqual("nextValue", MockModel.Name);
            }

            [TestCase]
            public void SetsOldAndNewValue()
            {
                var action = new ActionUndo(this, () => MockModel.Change("previousValue"), () => MockModel.Change("nextValue"));

                action.Undo();
                Assert.AreEqual("previousValue", MockModel.Name);

                action.Redo();
                Assert.AreEqual("nextValue", MockModel.Name);

                action.Undo();
                Assert.AreEqual("previousValue", MockModel.Name);
            }
        }

        [TestFixture]
        public class TheActionsThroughMementoServiceMethod
        {
            [TestCase]
            public void CallActions()
            {
                var value = false;
                var mementoService = new MementoService();
                var action = new ActionUndo(this, () => value = true, () => value = false);

                mementoService.Add(action);
                Assert.IsFalse(value);

                mementoService.Undo();
                Assert.IsTrue(value);

                mementoService.Redo();
                Assert.IsFalse(value);
            }

            [TestCase]
            public void SetProperty()
            {
                var instance = new IniEntry();
                var action = new PropertyChangeUndo(instance, "Key", "previousValue", "nextValue");
                var mementoService = new MementoService();

                mementoService.Add(action);
                Assert.AreEqual(IniEntry.KeyProperty.GetDefaultValue(), instance.Key);

                mementoService.Undo();
                Assert.AreEqual("previousValue", instance.Key);

                mementoService.Redo();
                Assert.AreEqual("nextValue", instance.Key);
            }
        }
    }
}
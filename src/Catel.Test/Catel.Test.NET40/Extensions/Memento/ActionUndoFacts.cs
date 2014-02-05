// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActionUndoTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;
    using Data;
    using Mocks;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ActionUndoFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ActionUndo(null, () => MockModel.Change("test")));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullUndoMethod()
            {
                var obj = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new ActionUndo(obj, null));
            }
        }

        [TestClass]
        public class TheUndoMethod
        {
            [TestMethod]
            public void SetsOldValue()
            {
                var action = new ActionUndo(this, () => MockModel.Change("previousValue"));

                action.Undo();
                Assert.AreEqual("previousValue", MockModel.Name);
            }

            [TestMethod]
            public void SetsNewValue()
            {
                var action = new ActionUndo(this, () => MockModel.Change("previousValue"), () => MockModel.Change("nextValue"));

                action.Redo();
                Assert.AreEqual("nextValue", MockModel.Name);
            }

            [TestMethod]
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

        [TestClass]
        public class TheActionsThroughMementoServiceMethod
        {
            [TestMethod]
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

            [TestMethod]
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MementoServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Memento
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Memento;
    using Mocks;
    
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class MementoServiceFacts
    {
        [TestClass]
        public class TheConstructor
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentOutOfRangeExceptionForNegativeParameter()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => new MementoService(-1));
            }

            [TestMethod]
            public void ExpectDefaultMaximumSupportedActionsValue()
            {
                var mementoService = new MementoService();
                Assert.AreEqual(300, mementoService.MaximumSupportedBatches);
            }
            #endregion
        }

        [TestClass]
        public class TheMaximumSupportedProperty
        {
            #region Methods
            [TestMethod]
            public void MaximumSupportedOperationsTest()
            {
                var mementoService = new MementoService(5);
                var listUndoOps = new List<MockUndo>();

                for (var i = 0; i < 10; i++)
                {
                    var memento = new MockUndo() {Value = i};
                    mementoService.Add(memento);
                    listUndoOps.Add(memento);
                }

                var count = 0;
                while (mementoService.CanUndo)
                {
                    mementoService.Undo();
                    count++;
                }

                for (var i = 0; i < 5; i++)
                {
                    Assert.IsFalse(listUndoOps[i].UndoCalled);
                }

                for (var i = 5; i < 10; i++)
                {
                    Assert.IsTrue(listUndoOps[i].UndoCalled);
                }

                Assert.AreEqual(count, mementoService.MaximumSupportedBatches);
            }
            #endregion
        }

        [TestClass]
        public class TheIsEnabledProperty
        {
            [TestMethod]
            public void IsTrueByDefault()
            {
                var mementoService = new MementoService();

                Assert.IsTrue(mementoService.IsEnabled);
            }

            [TestMethod]
            public void PreventsAdditionsWhenDisabled()
            {
                var mementoService = new MementoService();
                mementoService.IsEnabled = false;

                var undo1 = new MockUndo(true);
                mementoService.Add(undo1);

                Assert.IsFalse(mementoService.CanRedo);
            }
        }

        [TestClass]
        public class TheBeginBatchMethod
        {
            [TestMethod]
            public void BeginsNewBatchWhenThereAlreadyIsABatch()
            {
                var mementoService = new MementoService();
                var model = new MockModel();

                var firstBatch = mementoService.BeginBatch("FirstBatch");
                mementoService.Add(new PropertyChangeUndo(model, "Value", model.Value));
                Assert.AreEqual(1, firstBatch.ActionCount);

                var secondBatch = mementoService.BeginBatch("SecondBatch");
                mementoService.Add(new PropertyChangeUndo(model, "Value", model.Value));
                Assert.AreEqual(1, secondBatch.ActionCount);

                // Also check if the first batch was closed
                Assert.AreEqual(1, mementoService.UndoBatches.Count());
                Assert.AreEqual(1, firstBatch.ActionCount);
            }
        }

        [TestClass]
        public class TheEndBatchMethod
        {
            [TestMethod]
            public void EndsBatchWhenThereAlreadyIsABatch()
            {
                var mementoService = new MementoService();
                var model = new MockModel();

                var firstBatch = mementoService.BeginBatch("FirstBatch");
                mementoService.Add(new PropertyChangeUndo(model, "Value", model.Value));
                Assert.AreEqual(1, firstBatch.ActionCount);

                var secondBatch = mementoService.BeginBatch("SecondBatch");
                mementoService.Add(new PropertyChangeUndo(model, "Value", model.Value));
                Assert.AreEqual(1, secondBatch.ActionCount);
                mementoService.EndBatch();

                Assert.AreEqual(2, mementoService.UndoBatches.Count());
            }
        }

        [TestClass]
        public class TheRedoMethod
        {
            #region Methods
            [TestMethod]
            public void RedoTest()
            {
                var mementoService = new MementoService();
                var undo1 = new MockUndo(true);

                mementoService.Add(undo1);
                mementoService.Undo();
                Assert.IsTrue(undo1.UndoCalled);
                Assert.IsFalse(undo1.RedoCalled);
                Assert.IsTrue(mementoService.CanRedo);

                mementoService.Redo();
                Assert.IsTrue(undo1.RedoCalled);
                Assert.IsFalse(mementoService.CanRedo);
            }

            [TestMethod]
            public void HandlesDoubleRedo()
            {
                var obj = new MockModel {Value = "value1"};
                var service = new MementoService();

                service.RegisterObject(obj);
                obj.Value = "value2";
                obj.Value = "value3";

                service.Undo();
                Assert.AreEqual("value2", obj.Value);

                service.Undo();
                Assert.AreEqual("value1", obj.Value);

                service.Redo();
                Assert.AreEqual("value2", obj.Value);

                service.Redo();
                Assert.AreEqual("value3", obj.Value);
            }

            [TestMethod]
            public void CanRedoTest()
            {
                var mementoService = new MementoService();
                Assert.IsFalse(mementoService.CanUndo);

                mementoService.Add(new MockUndo());
                Assert.IsTrue(mementoService.CanUndo);

                mementoService.Undo();
                Assert.IsFalse(mementoService.CanRedo);

                mementoService.Add(new MockUndo(true));
                Assert.IsTrue(mementoService.CanUndo);

                mementoService.Undo();
                Assert.IsFalse(mementoService.CanUndo);
                Assert.IsTrue(mementoService.CanRedo);

                mementoService.Redo();
                Assert.IsTrue(mementoService.CanUndo);
                Assert.IsFalse(mementoService.CanRedo);
            }
            #endregion
        }

        [TestClass]
        public class TheUndoMethod
        {
            #region Methods
            [TestMethod]
            public void UndoTest()
            {
                var mementoService = new MementoService();
                var undo1 = new MockUndo();
                var undo2 = new MockUndo();

                mementoService.Add(undo1);
                mementoService.Add(undo2);

                mementoService.Undo();
                Assert.IsTrue(undo2.UndoCalled);
                Assert.IsFalse(undo1.UndoCalled);
                Assert.IsTrue(mementoService.CanUndo);
            }

            [TestMethod]
            public void HandlesDoubleUndo()
            {
                var obj = new MockModel {Value = "value1"};
                var service = new MementoService();

                service.RegisterObject(obj);

                obj.Value = "value2";
                obj.Value = "value3";

                service.Undo();
                Assert.AreEqual("value2", obj.Value);

                service.Undo();
                Assert.AreEqual("value1", obj.Value);
            }

            [TestMethod]
            public void CanUndoTest()
            {
                var mementoService = new MementoService();
                Assert.IsFalse(mementoService.CanUndo);

                mementoService.Add(new MockUndo());
                Assert.IsTrue(mementoService.CanUndo);

                mementoService.Undo();
                Assert.IsFalse(mementoService.CanUndo);
            }
            #endregion
        }
        
        [TestClass]
        public class TheUnregisterObjectMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                var service = new MementoService();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => service.UnregisterObject(null));
            }

            [TestMethod]
            public void CancelsSubscriptionForInstance()
            {
                var obj = new MockModel {Value = "value1"};
                var service = new MementoService();

                service.RegisterObject(obj);
                service.UnregisterObject(obj);

                obj.Value = "newvalue";

                Assert.IsFalse(service.CanUndo);
            }

            [TestMethod]
            public void ClearsCurrentUndoRedoStackForInstance()
            {
                var obj = new MockModel {Value = "value1"};
                var service = new MementoService();

                service.RegisterObject(obj);

                obj.Value = "newvalue1";
                Assert.IsFalse(service.CanRedo);

                service.UnregisterObject(obj);

                Assert.IsFalse(service.CanUndo);
            }
            #endregion
        }
    }
}
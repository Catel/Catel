// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BatchFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Memento
{
    using Catel.Memento;

    using NUnit.Framework;

    public class BatchFacts
    {
        [TestFixture]
        public class TheActionCountProperty
        {
            [TestCase]
            public void ReturnsZeroForEmptyBatch()
            {
                var batch = new Batch();

                Assert.AreEqual(0, batch.ActionCount);
            }

            [TestCase]
            public void ReturnsOneForBatchWithOneAction()
            {
                var model = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model, "Value", model.Value));

                Assert.AreEqual(1, batch.ActionCount);
            }
        }

        [TestFixture]
        public class TheIsEmptyBatchProperty
        {
            [TestCase]
            public void ReturnsTrueForEmptyBatch()
            {
                var batch = new Batch();
                
                Assert.IsTrue(batch.IsEmptyBatch);
            }

            [TestCase]
            public void ReturnsFalseForBatchWithOneAction()
            {
                var model = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model, "Value", model.Value));

                Assert.IsFalse(batch.IsEmptyBatch);
            }

            [TestCase]
            public void ReturnsFalseForBatchWithMultipleActions()
            {
                var model1 = new Mocks.MockModel();
                var model2 = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model1, "Value", model1.Value));
                batch.AddAction(new PropertyChangeUndo(model2, "Value", model2.Value));

                Assert.IsFalse(batch.IsEmptyBatch);
            }
        }

        [TestFixture]
        public class TheIsSingleActionBatchProperty
        {
            [TestCase]
            public void ReturnsFalseForEmptyBatch()
            {
                var batch = new Batch();

                Assert.IsFalse(batch.IsSingleActionBatch);
            }

            [TestCase]
            public void ReturnsTrueForBatchWithOneAction()
            {
                var model = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model, "Value", model.Value));

                Assert.IsTrue(batch.IsSingleActionBatch);
            }

            [TestCase]
            public void ReturnsFalseForBatchWithMultipleActions()
            {
                var model1 = new Mocks.MockModel();
                var model2 = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model1, "Value", model1.Value));
                batch.AddAction(new PropertyChangeUndo(model2, "Value", model2.Value));

                Assert.IsFalse(batch.IsSingleActionBatch);
            }
        }

        [TestFixture]
        public class TheCanRedoProperty
        {
            [TestCase]
            public void IsFalseWhenNoActionsCanRedo()
            {
                var model1 = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new ActionUndo(model1, () => model1.Value = "Value"));

                Assert.IsFalse(batch.CanRedo);
            }

            [TestCase]
            public void IsTrueWhenAtLeastOneActionCanRedo()
            {
                var model1 = new Mocks.MockModel();
                var model2 = new Mocks.MockModel();

                var batch = new Batch();
                batch.AddAction(new PropertyChangeUndo(model1, "Value", model1.Value));
                batch.AddAction(new PropertyChangeUndo(model2, "Value", model2.Value));

                Assert.IsTrue(batch.CanRedo);
            }
        }
    }
}
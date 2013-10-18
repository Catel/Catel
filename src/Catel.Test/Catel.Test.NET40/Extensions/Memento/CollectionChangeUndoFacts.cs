// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionChangeUndoTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using System.Collections.Generic;
    using Catel.Collections;
    using Catel.Memento;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CollectionChangeUndoFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new CollectionChangeUndo(null, CollectionChangeType.Add, 0, 0, null, null));
            }

            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var table = new List<object>();
                var collectionChangeUndo = new CollectionChangeUndo(table, CollectionChangeType.Add, 0, 0, "currentValue", "nextValue");

                Assert.IsNotNull(collectionChangeUndo.Collection);
                Assert.AreEqual(CollectionChangeType.Add, collectionChangeUndo.ChangeType);
                Assert.AreEqual(table, collectionChangeUndo.Collection);
                Assert.AreEqual("currentValue", collectionChangeUndo.OldValue);
                Assert.AreEqual("nextValue", collectionChangeUndo.NewValue);
                Assert.AreEqual(true, collectionChangeUndo.CanRedo);
            }
        }

        [TestClass]
        public class TheUndoMethod
        {
            [TestMethod]
            public void HandlesCollectionAddCorrectly()
            {
                var table = new List<string>(new[] {"currentValue"});
                var tableAfter = new List<string>();

                var collectionChangeUndo = new CollectionChangeUndo(table, CollectionChangeType.Add, 0, 1, null, "currentValue");
                collectionChangeUndo.Undo();

                Assert.IsTrue(CollectionHelper.IsEqualTo(table, tableAfter));
            }

            // TODO: Write replace, remove, move
        }

        [TestClass]
        public class TheRedoMethod
        {
            [TestMethod]
            public void HandlesCollectionAddCorrectly()
            {
                var table = new List<string>();
                var tableAfter = new List<string>(new[] { "currentValue" });

                var collectionChangeUndo = new CollectionChangeUndo(table, CollectionChangeType.Add, 0, 1, null, "currentValue");
                collectionChangeUndo.Redo();

                Assert.IsTrue(CollectionHelper.IsEqualTo(table, tableAfter));
            }

            // TODO: Write replace, remove, move
        }
    }
}
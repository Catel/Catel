// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeUndoTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;
    using Mocks;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class PropertyChangeUndoFacts
    {
        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new PropertyChangeUndo(null, "PropertyName", null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForNullPropertyName()
            {
                var obj = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new PropertyChangeUndo(obj, null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new PropertyChangeUndo(obj, string.Empty, null));
            }

            [TestMethod]
            public void SetsValuesCorrectly()
            {
                var obj = new { MyProperty = "currentValue" };

                var propertyChangeUndo = new PropertyChangeUndo(obj, "MyProperty", "currentValue", "nextValue");
                Assert.AreEqual("currentValue", obj.MyProperty);
                Assert.AreEqual("MyProperty", propertyChangeUndo.PropertyName);
                Assert.AreEqual(obj, propertyChangeUndo.Target);
                Assert.AreEqual("currentValue", propertyChangeUndo.OldValue);
                Assert.AreEqual("nextValue", propertyChangeUndo.NewValue);
                Assert.AreEqual(true, propertyChangeUndo.CanRedo);
            }
        }

        [TestClass]
        public class TheUndoMethod
        {
            [TestMethod]
            public void SetsOldValue()
            {
                var obj = new MockModel { Value = "currentValue" };

                var propertyChangeUndo = new PropertyChangeUndo(obj, "Value", "previousValue", "nextValue");
                propertyChangeUndo.Undo();

                Assert.AreEqual("previousValue", obj.Value);
            }
        }

        [TestClass]
        public class TheRedoMethod
        {
            [TestMethod]
            public void SetsNewValue()
            {
                var obj = new MockModel { Value = "currentValue" };

                var propertyChangeUndo = new PropertyChangeUndo(obj, "Value", "previousValue", "nextValue");
                propertyChangeUndo.Redo();

                Assert.AreEqual("nextValue", obj.Value);
            }
        }
    }
}
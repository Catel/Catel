// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyChangeUndoTest.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Memento
{
    using System;
    using Catel.Memento;
    using Mocks;

    using NUnit.Framework;

    public class PropertyChangeUndoFacts
    {
        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullInstance()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new PropertyChangeUndo(null, "PropertyName", null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForNullPropertyName()
            {
                var obj = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new PropertyChangeUndo(obj, null, null));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => new PropertyChangeUndo(obj, string.Empty, null));
            }

            [TestCase]
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

        [TestFixture]
        public class TheUndoMethod
        {
            [TestCase]
            public void SetsOldValue()
            {
                var obj = new MockModel { Value = "currentValue" };

                var propertyChangeUndo = new PropertyChangeUndo(obj, "Value", "previousValue", "nextValue");
                propertyChangeUndo.Undo();

                Assert.AreEqual("previousValue", obj.Value);
            }
        }

        [TestFixture]
        public class TheRedoMethod
        {
            [TestCase]
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
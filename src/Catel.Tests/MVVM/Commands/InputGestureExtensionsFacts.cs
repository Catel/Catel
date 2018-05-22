// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGestureExtensionsFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests
{
    using System.Windows.Input;
    using Windows.Input;
    using NUnit.Framework;
    using Catel.Windows.Input;
    using InputGesture = Catel.Windows.Input.InputGesture;

    [TestFixture]
    public class InputGestureExtensionsFacts
    {
        [TestCase(Key.A, ModifierKeys.Control, false)]
        [TestCase(Key.A, ModifierKeys.Shift, false)]
        [TestCase(Key.A, ModifierKeys.None, false)]
        [TestCase(Key.None, ModifierKeys.Control, false)]
        [TestCase(Key.None, ModifierKeys.None, true)]
        public void TheIsEmptyMethod(Key key, ModifierKeys modifierKeys, bool expectedValue)
        {
            var inputGesture = new InputGesture(key, modifierKeys);

            Assert.AreEqual(expectedValue, inputGesture.IsEmpty());
        }
    }
}
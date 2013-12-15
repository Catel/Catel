// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGestureFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Windows.Input
{
    using InputGesture = Catel.Windows.Input.InputGesture;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

    using ModifierKeys = global::Windows.System.VirtualKeyModifiers;
    using Key = global::Windows.System.VirtualKey;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
#endif

    public class InputGestureFacts
    {
        [TestClass]
        public class TheToStringMethod
        {
            [TestMethod]
            public void CorrectlyReturnsStringWithoutModifiers()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.None);

                Assert.AreEqual("A", inputGesture.ToString());
            }

            [TestMethod]
            public void CorrectlyReturnsStringWithSingleModifier()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control);

                Assert.AreEqual("Control + A", inputGesture.ToString());
            }

            [TestMethod]
            public void CorrectlyReturnsStringWithMultipleModifier()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                Assert.AreEqual("Control + Shift + A", inputGesture.ToString());
            }
        }
    }
}
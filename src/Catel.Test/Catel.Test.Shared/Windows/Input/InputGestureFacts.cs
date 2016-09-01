// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InputGestureFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Windows.Input
{
    using System.IO;
    using Catel.Runtime.Serialization;
    using InputGesture = Catel.Windows.Input.InputGesture;

    using NUnit.Framework;

#if NETFX_CORE
    using ModifierKeys = global::Windows.System.VirtualKeyModifiers;
    using Key = global::Windows.System.VirtualKey;
#else
    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;
#endif

    public class InputGestureFacts
    {
        [TestFixture]
        public class TheSerializationFunctionality
        {
            [TestCase]
            public void CorrectlySerializesAndDeserializes()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                var xmlSerializer = SerializationFactory.GetXmlSerializer();
                using (var memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(inputGesture, memoryStream, null);

                    memoryStream.Position = 0L;

                    var finalInputGesture = xmlSerializer.Deserialize(typeof (InputGesture), memoryStream, null);

                    Assert.AreEqual(inputGesture, finalInputGesture);
                }
            }
        }

        [TestFixture]
        public class TheToStringMethod
        {
            [TestCase]
            public void CorrectlyReturnsStringWithoutModifiers()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.None);

                Assert.AreEqual("A", inputGesture.ToString());
            }

            [TestCase]
            public void CorrectlyReturnsStringWithSingleModifier()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control);

                Assert.AreEqual("Control + A", inputGesture.ToString());
            }

            [TestCase]
            public void CorrectlyReturnsStringWithMultipleModifier()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                Assert.AreEqual("Control + Shift + A", inputGesture.ToString());
            }
        }
    }
}
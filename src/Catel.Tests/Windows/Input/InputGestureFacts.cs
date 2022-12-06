namespace Catel.Tests.Windows.Input
{
    using System.Diagnostics;
    using System.IO;
    using Catel.Runtime.Serialization;
    using InputGesture = Catel.Windows.Input.InputGesture;

    using NUnit.Framework;

    using System.Windows.Input;
    using ModifierKeys = System.Windows.Input.ModifierKeys;

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

                    var finalInputGesture = xmlSerializer.Deserialize(typeof(InputGesture), memoryStream, null);

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

            [TestCase]
            public void CorrectlyReturnsStringAfterUpdateKey()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                inputGesture.ToString();

                inputGesture.Key = Key.B;

                Assert.AreEqual("Control + Shift + B", inputGesture.ToString());
            }

            [TestCase]
            public void CorrectlyReturnsStringAfterUpdateModifierKeys()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                inputGesture.ToString();

                inputGesture.Modifiers |= ModifierKeys.Alt;
                Assert.AreEqual("Alt + Control + Shift + A", inputGesture.ToString());
            }

            [TestCase]
            public void SecondCallExecutesFasterThanFirstOne()
            {
                var inputGesture = new InputGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift);

                var stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                inputGesture.ToString();
                stopwatch1.Stop();

                var stopwatch2 = new Stopwatch();
                stopwatch2.Start();
                inputGesture.ToString();
                stopwatch2.Stop();

                Assert.Less(stopwatch2.Elapsed, stopwatch1.Elapsed);
            }
        }
    }
}

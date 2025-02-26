namespace Catel.Tests
{
    using System.Linq;
    using System.Windows.Input;
    using NUnit.Framework;
    using CommandManager = Catel.MVVM.CommandManager;
    using InputGesture = Catel.Windows.Input.InputGesture;

    public class CommandManagerExtensionsFacts
    {
        [TestFixture]
        public class The_FindCommandsByGesture_Method
        {
            [TestCase(Key.A, ModifierKeys.Control, true)]
            [TestCase(Key.A, ModifierKeys.Shift, false)]
            [TestCase(Key.A, ModifierKeys.None, false)]
            [TestCase(Key.B, ModifierKeys.Control, false)]
            public void Returns_Correct_Value(Key key, ModifierKeys modifierKeys, bool expectedToBeAvailable)
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("CtrlA", new InputGesture(Key.A, ModifierKeys.Control));

                var inputGesture = new InputGesture(key, modifierKeys);
                var existingCommands = commandManager.FindCommandsByGesture(inputGesture);

                Assert.That(existingCommands.Any(), Is.EqualTo(expectedToBeAvailable));
            }
        }

        [TestFixture]
        public class The_GetRequiredCommand_Method
        {
            [Test]
            public void Throws_Exception_When_Not_Registered()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("CtrlA", new InputGesture(Key.A, ModifierKeys.Control));

                Assert.Throws<CatelException>(() => commandManager.GetRequiredCommand("CtrlB"));
            }

            [Test]
            public void Returns_Command_When_Registered()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("CtrlA", new InputGesture(Key.A, ModifierKeys.Control));

                Assert.That(commandManager.GetRequiredCommand("CtrlA"), Is.Not.Null);
            }
        }
    }
}

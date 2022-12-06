namespace Catel.Tests.MVVM
{
    using System;
    using System.Threading.Tasks;
    using Catel.MVVM;
    using NUnit.Framework;

    public class CommandManagerFacts
    {
        [TestFixture]
        public class TheCreateCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.CreateCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.CreateCommand(" "));
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForAlreadyCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.Throws<InvalidOperationException>(() => commandManager.CreateCommand("MyCommand"));
            }

            [TestCase]
            public void CorrectlyCreatesTheCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }
        }

        [TestFixture]
        public class TheIsCommandCreatedMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(" "));
            }

            [TestCase]
            public void ReturnsTrueForCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }

            [TestCase]
            public void ReturnsFalseForNotCreatedCommand()
            {
                var commandManager = new CommandManager();

                Assert.IsFalse(commandManager.IsCommandCreated("MyCommand"));
            }
        }

        [TestFixture]
        public class TheGetCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.GetCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.GetCommand(" "));
            }

            [TestCase]
            public void ReturnsNullForNotCreatedCommand()
            {
                var commandManager = new CommandManager();

                Assert.IsNull(commandManager.GetCommand("MyCommand"));
            }

            [TestCase]
            public void ReturnsCommandForCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsNotNull(commandManager.GetCommand("MyCommand"));
            }
        }

        [TestFixture]
        public class TheExecuteCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.ExecuteCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.ExecuteCommand(" "));
            }

            [TestCase]
            public void ExecutesRegisteredCommands()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");
                commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

                commandManager.ExecuteCommand("MyCommand");

                Assert.IsTrue(vm.IsTestCommand1Executed);
            }

            [TestCase]
            public void DoesNotExecuteUnregisteredCommands()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");
                commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));

                commandManager.UnregisterCommand("MyCommand", vm.TestCommand1);

                commandManager.ExecuteCommand("MyCommand");

                Assert.IsFalse(vm.IsTestCommand1Executed);
            }
        }

        [TestFixture]
        public class TheRegisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.RegisterCommand(null, vm.TestCommand1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.RegisterCommand(" ", vm.TestCommand1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentNullException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }

        [TestFixture]
        public class TheUnregisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(" "));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                Assert.Throws<ArgumentNullException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }

        [TestFixture]
        public class TheRegisterAndUnregisterActionFunctionality
        {
            [TestCase]
            public async Task RegisteredActionsCanBeInvokedAsync()
            {
                var invoked = false;
                Action action = () => invoked = true;

                var commandManager = new CommandManager();

                commandManager.CreateCommand("TestAction");

                commandManager.RegisterAction("TestAction", action);

                commandManager.ExecuteCommand("TestAction");

                Assert.IsTrue(invoked);
            }

            [TestCase]
            public void RegisteredActionsCanBeUnregistered_DefinedAction()
            {
                var invoked = false;
                Action action = () => invoked = true;

                var commandManager = new CommandManager();

                commandManager.CreateCommand("TestAction");

                commandManager.RegisterAction("TestAction", action);
                commandManager.UnregisterAction("TestAction", action);

                commandManager.ExecuteCommand("TestAction");

                Assert.IsFalse(invoked);
            }

            [TestCase]
            public void RegisteredActionsCanBeUnregistered_DynamicAction()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("TestAction");

                commandManager.RegisterAction("TestAction", RegisteredActionsCanBeUnregistered_TestMethod);
                commandManager.UnregisterAction("TestAction", RegisteredActionsCanBeUnregistered_TestMethod);

                commandManager.ExecuteCommand("TestAction");

                Assert.IsFalse(_registeredActionsCanBeUnregistered_TestValue);
            }

            private bool _registeredActionsCanBeUnregistered_TestValue = false;

            private void RegisteredActionsCanBeUnregistered_TestMethod()
            {
                _registeredActionsCanBeUnregistered_TestValue = true;
            }
        }
    }
}

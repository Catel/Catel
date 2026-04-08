namespace Catel.Tests.MVVM;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

public class CommandManagerFacts
{
    [TestFixture]
    public class TheCreateCommandMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.CreateCommand(null));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.CreateCommand(" "));
        }

        [TestCase]
        public void ThrowsInvalidOperationExceptionForAlreadyCreatedCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("MyCommand");

            Assert.Throws<InvalidOperationException>(() => commandManager.CreateCommand("MyCommand"));
        }

        [TestCase]
        public void CorrectlyCreatesTheCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider); 

            commandManager.CreateCommand("MyCommand");

            Assert.That(commandManager.IsCommandCreated("MyCommand"), Is.True);
        }
    }

    [TestFixture]
    public class TheIsCommandCreatedMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(null));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(" "));
        }

        [TestCase]
        public void ReturnsTrueForCreatedCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("MyCommand");

            Assert.That(commandManager.IsCommandCreated("MyCommand"), Is.True);
        }

        [TestCase]
        public void ReturnsFalseForNotCreatedCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.That(commandManager.IsCommandCreated("MyCommand"), Is.False);
        }
    }

    [TestFixture]
    public class TheGetCommandMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.GetCommand(null));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.GetCommand(" "));
        }

        [TestCase]
        public void ReturnsNullForNotCreatedCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.That(commandManager.GetCommand("MyCommand"), Is.Null);
        }

        [TestCase]
        public void ReturnsCommandForCreatedCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("MyCommand");

            Assert.That(commandManager.GetCommand("MyCommand"), Is.Not.Null);
        }
    }

    [TestFixture]
    public class TheExecuteCommandMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.ExecuteCommand(null));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.ExecuteCommand(" "));
        }

        [TestCase]
        public void ExecutesRegisteredCommands()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

            commandManager.CreateCommand("MyCommand");
            commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

            commandManager.ExecuteCommand("MyCommand");

            Assert.That(vm.IsTestCommand1Executed, Is.True);
        }

        [TestCase]
        public void DoesNotExecuteUnregisteredCommands()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

            commandManager.CreateCommand("MyCommand");
            commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

            Assert.That(commandManager.IsCommandCreated("MyCommand"), Is.True);

            commandManager.UnregisterCommand("MyCommand", vm.TestCommand1);

            commandManager.ExecuteCommand("MyCommand");

            Assert.That(vm.IsTestCommand1Executed, Is.False);
        }
    }

    [TestFixture]
    public class TheRegisterCommandMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.RegisterCommand(null, vm.TestCommand1));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.RegisterCommand(" ", vm.TestCommand1));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => commandManager.RegisterCommand("MyCommand", null));
        }
    }

    [TestFixture]
    public class TheUnregisterCommandMethod
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(null));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            Assert.Throws<ArgumentException>(() => commandManager.IsCommandCreated(" "));
        }

        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);
            var vm = new CompositeCommandViewModel(serviceProvider);

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

            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("TestAction");

            commandManager.RegisterAction("TestAction", action);

            commandManager.ExecuteCommand("TestAction");

            Assert.That(invoked, Is.True);
        }

        [TestCase]
        public void RegisteredActionsCanBeUnregistered_DefinedAction()
        {
            var invoked = false;
            Action action = () => invoked = true;

            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("TestAction");

            commandManager.RegisterAction("TestAction", action);
            commandManager.UnregisterAction("TestAction", action);

            commandManager.ExecuteCommand("TestAction");

            Assert.That(invoked, Is.False);
        }

        [TestCase]
        public void RegisteredActionsCanBeUnregistered_DynamicAction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var commandManager = new CommandManager(new NullLogger<CommandManager>(), serviceProvider);

            commandManager.CreateCommand("TestAction");

            commandManager.RegisterAction("TestAction", RegisteredActionsCanBeUnregistered_TestMethod);
            commandManager.UnregisterAction("TestAction", RegisteredActionsCanBeUnregistered_TestMethod);

            commandManager.ExecuteCommand("TestAction");

            Assert.That(_registeredActionsCanBeUnregistered_TestValue, Is.False);
        }

        private bool _registeredActionsCanBeUnregistered_TestValue = false;

        private void RegisteredActionsCanBeUnregistered_TestMethod()
        {
            _registeredActionsCanBeUnregistered_TestValue = true;
        }
    }
}

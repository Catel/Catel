namespace Catel.Tests.MVVM;

using System;
using System.Threading.Tasks;
using Catel.MVVM;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class CompositeCommandFacts
{
    [TestFixture]
    public class The_CanExecute_State
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void CanExecuteEmptyCommandWithAtLeastOneMustBeExecutable(bool atLeastOneMustBeExecutable, bool expectedValue)
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            compositeCommand.AtLeastOneMustBeExecutable = atLeastOneMustBeExecutable;

            Assert.That(((ICatelCommand)compositeCommand).CanExecute(null), Is.EqualTo(expectedValue));
        }

        [TestCase(false, true)]
        [TestCase(true, false)]
        public void PreventsExecutionOfPartiallyExecutableCommand(bool checkCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand, bool expectedValue)
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            compositeCommand.RegisterCommand(new Command(serviceProvider, () => { }, () => false));
            compositeCommand.RegisterCommand(new Command(serviceProvider, () => { }, () => true));

            compositeCommand.CheckCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand = checkCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand;

            Assert.That(((ICatelCommand)compositeCommand).CanExecute(null), Is.EqualTo(expectedValue));
        }
    }

    [TestFixture]
    public class The_RegisterCommand_Method
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => compositeCommand.RegisterCommand(null));
        }

        [TestCase]
        public void RegistersCommandForExecution()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            var vm = new CompositeCommandViewModel(serviceProvider);
            compositeCommand.RegisterCommand(vm.TestCommand1, vm);

            compositeCommand.Execute();

            Assert.That(vm.IsTestCommand1Executed, Is.True);
        }

        [TestCase]
        public async Task AwaitsRegisteredTaskCommandExecution()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);
            var taskCompletionSource = new TaskCompletionSource<object?>();
            var childCommandStarted = false;

            var childCommand = new TaskCommand(serviceProvider, async () =>
            {
                childCommandStarted = true;
                await taskCompletionSource.Task;
            });

            compositeCommand.RegisterCommand(childCommand);

            compositeCommand.Execute();

            var compositeTask = compositeCommand.GetTask();

            Assert.That(childCommandStarted, Is.True);
            Assert.That(compositeTask.IsCompleted, Is.False);

            taskCompletionSource.SetResult(null);

            await compositeTask;

            Assert.That(compositeTask.IsCompleted, Is.True);
        }
    }

    [TestFixture]
    public class The_UnregisterCommand_Method
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullCommand()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => compositeCommand.UnregisterCommand(null));
        }

        [TestCase]
        public void UnregistersCommandForExecution()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            var vm = new CompositeCommandViewModel(serviceProvider); 
            
            compositeCommand.RegisterCommand(vm.TestCommand1, vm);
            compositeCommand.RegisterCommand(vm.TestCommand2, vm);

            compositeCommand.UnregisterCommand(vm.TestCommand1);

            compositeCommand.Execute();

            Assert.That(vm.IsTestCommand1Executed, Is.False);
            Assert.That(vm.IsTestCommand2Executed, Is.True);
        }
    }

    [TestFixture]
    public class The_RegisterGenericAction_Method
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullAction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => compositeCommand.RegisterAction((Action<object>)null));
        }

        [TestCase]
        public void RegistersActionForExecution()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            bool executed = false;
            var action = new Action<object>(obj => executed = true);

            compositeCommand.RegisterAction(action);
            compositeCommand.Execute();

            Assert.That(executed, Is.True);
        }
    }

    [TestFixture]
    public class The_UnregisterGenericAction_Method
    {
        [TestCase]
        public void ThrowsArgumentNullExceptionForNullAction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            Assert.Throws<ArgumentNullException>(() => compositeCommand.UnregisterAction((Action<object>)null));
        }

        [TestCase]
        public void UnregistersCommandForExecution()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            bool executed = false;
            var action = new Action<object>(obj => executed = true);

            compositeCommand.RegisterAction(action);
            compositeCommand.UnregisterAction(action);

            compositeCommand.Execute();

            Assert.That(executed, Is.False);
        }
    }

    [TestFixture]
    public class The_Register_And_Unregister_Action_Functionality
    {
        [TestCase]
        public void RegisteredActionsCanBeInvoked()
        {
            var invoked = false;
            Action action = () => invoked = true;

            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            compositeCommand.RegisterAction(action);

            compositeCommand.Execute(null);

            Assert.That(invoked, Is.True);
        }

        [TestCase]
        public void RegisteredActionsCanBeUnregistered_DefinedAction()
        {
            var invoked = false;
            Action action = () => invoked = true;

            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            compositeCommand.RegisterAction(action);
            compositeCommand.UnregisterAction(action);

            compositeCommand.Execute(null);

            Assert.That(invoked, Is.False);
        }

        [TestCase]
        public void RegisteredActionsCanBeUnregistered_DynamicAction()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            compositeCommand.RegisterAction(RegisteredActionsCanBeUnregistered_TestMethod);
            compositeCommand.UnregisterAction(RegisteredActionsCanBeUnregistered_TestMethod);

            compositeCommand.Execute(null);

            Assert.That(_registeredActionsCanBeUnregistered_TestValue, Is.False);
        }

        private bool _registeredActionsCanBeUnregistered_TestValue = false;

        private void RegisteredActionsCanBeUnregistered_TestMethod()
        {
            _registeredActionsCanBeUnregistered_TestValue = true;
        }
    }

    [TestFixture]
    public class The_Auto_Unsubscribe_Functionality
    {
        [TestCase]
        public async Task AutomaticallyUnsubscribesCommandOnViewModelClosedAsync()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            var compositeCommand = new CompositeCommand(serviceProvider);

            var vm = new CompositeCommandViewModel(serviceProvider);
            compositeCommand.RegisterCommand(vm.TestCommand1, vm);

            Assert.That(vm.IsTestCommand1Executed, Is.False);

            await vm.CloseViewModelAsync(false);

            compositeCommand.Execute();

            Assert.That(vm.IsTestCommand1Executed, Is.False);
        }
    }
}

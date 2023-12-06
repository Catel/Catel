﻿namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;
    using NUnit.Framework;
    using System.Threading.Tasks;

    public class CompositeCommandFacts
    {
        [TestFixture]
        public class TheCanExecuteState
        {
            [TestCase(false, true)]
            [TestCase(true, false)]
            public void CanExecuteEmptyCommandWithAtLeastOneMustBeExecutable(bool atLeastOneMustBeExecutable, bool expectedValue)
            {
                var compositeCommand = new CompositeCommand();
                compositeCommand.AtLeastOneMustBeExecutable = atLeastOneMustBeExecutable;

                Assert.That(((ICatelCommand)compositeCommand).CanExecute(null), Is.EqualTo(expectedValue));
            }

            [TestCase(false, true)]
            [TestCase(true, false)]
            public void PreventsExecutionOfPartiallyExecutableCommand(bool checkCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand, bool expectedValue)
            {
                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterCommand(new Command(() => { }, () => false));
                compositeCommand.RegisterCommand(new Command(() => { }, () => true));

                compositeCommand.CheckCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand = checkCanExecuteOfAllCommandsToDetermineCanExecuteForCompositeCommand;

                Assert.That(((ICatelCommand)compositeCommand).CanExecute(null), Is.EqualTo(expectedValue));
            }
        }

        [TestFixture]
        public class TheRegisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                Assert.Throws<ArgumentNullException>(() => compositeCommand.RegisterCommand(null));
            }

            [TestCase]
            public void RegistersCommandForExecution()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                compositeCommand.Execute();

                Assert.That(vm.IsTestCommand1Executed, Is.True);
            }
        }

        [TestFixture]
        public class TheUnregisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                Assert.Throws<ArgumentNullException>(() => compositeCommand.UnregisterCommand(null));
            }

            [TestCase]
            public void UnregistersCommandForExecution()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);
                compositeCommand.RegisterCommand(vm.TestCommand2, vm);

                compositeCommand.UnregisterCommand(vm.TestCommand1);

                compositeCommand.Execute();

                Assert.That(vm.IsTestCommand1Executed, Is.False);
                Assert.That(vm.IsTestCommand2Executed, Is.True);
            }
        }

        [TestFixture]
        public class TheRegisterGenericActionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullAction()
            {
                var compositeCommand = new CompositeCommand();

                Assert.Throws<ArgumentNullException>(() => compositeCommand.RegisterAction((Action<object>)null));
            }

            [TestCase]
            public void RegistersActionForExecution()
            {
                var compositeCommand = new CompositeCommand();

                bool executed = false;
                var action = new Action<object>(obj => executed = true);

                compositeCommand.RegisterAction(action);
                compositeCommand.Execute();

                Assert.That(executed, Is.True);
            }
        }

        [TestFixture]
        public class TheUnregisterGenericActionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullAction()
            {
                var compositeCommand = new CompositeCommand();

                Assert.Throws<ArgumentNullException>(() => compositeCommand.UnregisterAction((Action<object>)null));
            }

            [TestCase]
            public void UnregistersCommandForExecution()
            {
                var compositeCommand = new CompositeCommand();

                bool executed = false;
                var action = new Action<object>(obj => executed = true);

                compositeCommand.RegisterAction(action);
                compositeCommand.UnregisterAction(action);

                compositeCommand.Execute();

                Assert.That(executed, Is.False);
            }
        }

        [TestFixture]
        public class TheRegisterAndUnregisterActionFunctionality
        {
            [TestCase]
            public void RegisteredActionsCanBeInvoked()
            {
                var invoked = false;
                Action action = () => invoked = true;

                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterAction(action);

                compositeCommand.Execute(null);

                Assert.That(invoked, Is.True);
            }

            [TestCase]
            public void RegisteredActionsCanBeUnregistered_DefinedAction()
            {
                var invoked = false;
                Action action = () => invoked = true;

                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterAction(action);
                compositeCommand.UnregisterAction(action);

                compositeCommand.Execute(null);

                Assert.That(invoked, Is.False);
            }

            [TestCase]
            public void RegisteredActionsCanBeUnregistered_DynamicAction()
            {
                var compositeCommand = new CompositeCommand();

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
        public class TheAutoUnsubscribeFunctionality
        {
            [TestCase]
            public async Task AutomaticallyUnsubscribesCommandOnViewModelClosedAsync()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                Assert.That(vm.IsTestCommand1Executed, Is.False);

                await vm.CloseViewModelAsync(false);

                compositeCommand.Execute();

                Assert.That(vm.IsTestCommand1Executed, Is.False);
            }
        }
    }
}
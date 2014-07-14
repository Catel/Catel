// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommandFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using NUnit.Framework;

    public class CompositeCommandFacts
    {
        [TestFixture]
        public class TheRegisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.RegisterCommand(null));
            }

            [TestCase]
            public void RegistersCommandForExecution()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand(); 

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                compositeCommand.Execute();

                Assert.IsTrue(vm.IsTestCommand1Executed);
            }
        }

        [TestFixture]
        public class TheUnregisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.UnregisterCommand(null));
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

                Assert.IsFalse(vm.IsTestCommand1Executed);
                Assert.IsTrue(vm.IsTestCommand2Executed);
            }
        }

        [TestFixture]
        public class TheRegisterGenericActionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullAction()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.RegisterAction((Action<object>)null));
            }

            [TestCase]
            public void RegistersActionForExecution()
            {
                var compositeCommand = new CompositeCommand();

                bool executed = false;
                var action = new Action<object>(obj => executed = true);

                compositeCommand.RegisterAction(action);
                compositeCommand.Execute();

                Assert.IsTrue(executed);
            }
        }

        [TestFixture]
        public class TheUnregisterGenericActionMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullAction()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.UnregisterAction((Action<object>)null));
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

                Assert.IsFalse(executed);
            }
        }

        [TestFixture]
        public class TheAutoUnsubscribeFunctionality
        {
            [TestCase]
            public void AutomaticallyUnsubscribesCommandOnViewModelClosed()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand();

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                Assert.IsFalse(vm.IsTestCommand1Executed);

                vm.CloseViewModel(false);

                compositeCommand.Execute();

                Assert.IsFalse(vm.IsTestCommand1Executed);
            }
        }
    }
}
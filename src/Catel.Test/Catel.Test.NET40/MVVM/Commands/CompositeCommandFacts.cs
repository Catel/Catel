// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommandFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;
    using ViewModels.TestClasses;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CompositeCommandFacts
    {
        public class CompositeCommandViewModel : ViewModelBase
        {
            public CompositeCommandViewModel()
            {
                TestCommand1 = new Command(OnTestCommand1Execute, OnTestCommand1CanExecute);
                TestCommand2 = new Command(OnTestCommand2Execute, OnTestCommand2CanExecute);

                AllowTestCommand1Execution = true;
                AllowTestCommand2Execution = true;
            }

            public bool AllowTestCommand1Execution { get; private set; }
            public bool AllowTestCommand2Execution { get; private set; }

            public bool IsTestCommand1Executed { get; private set; }
            public bool IsTestCommand2Executed { get; private set; }

            public Command TestCommand1 { get; private set; }

            private bool OnTestCommand1CanExecute()
            {
                return AllowTestCommand1Execution;
            }

            private void OnTestCommand1Execute()
            {
                IsTestCommand1Executed = true;
            }

            public Command TestCommand2 { get; private set; }

            private bool OnTestCommand2CanExecute()
            {
                return AllowTestCommand2Execution;
            }

            private void OnTestCommand2Execute()
            {
                IsTestCommand2Executed = true;
            }
        }

        [TestClass]
        public class TheRegisterCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.RegisterCommand(null));
            }

            [TestMethod]
            public void RegistersCommandForExecution()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand(); 

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                compositeCommand.Execute();

                Assert.IsTrue(vm.IsTestCommand1Executed);
            }
        }

        [TestClass]
        public class TheUnregisterCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var compositeCommand = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => compositeCommand.UnregisterCommand(null));
            }

            [TestMethod]
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

        [TestClass]
        public class TheAutoUnsubscribeFunctionality
        {
            [TestMethod]
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommandFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM
{
    using System;
    using Catel.MVVM;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class CompositeCommandFacts
    {
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

            [TestMethod]
            public void RegistersCommandCanExecution()
            {
                var vm = new CompositeCommandViewModel();
                var compositeCommand = new CompositeCommand();

                Assert.IsFalse(compositeCommand.CanExecute());

                compositeCommand.RegisterCommand(vm.TestCommand1, vm);

                vm.AllowTestCommand1Execution = false;
                Assert.IsFalse(compositeCommand.CanExecute());

                vm.AllowTestCommand1Execution = true;
                Assert.IsTrue(compositeCommand.CanExecute());

                compositeCommand.RegisterCommand(vm.TestCommand2, vm);

                vm.AllowTestCommand1Execution = false;
                vm.AllowTestCommand2Execution = false;
                Assert.IsFalse(compositeCommand.CanExecute());

                vm.AllowTestCommand1Execution = false;
                vm.AllowTestCommand2Execution = true;
                Assert.IsTrue(compositeCommand.CanExecute());
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
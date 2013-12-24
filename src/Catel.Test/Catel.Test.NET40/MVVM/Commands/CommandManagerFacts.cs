// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManagerFacts.cs" company="Catel development team">
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

    public class CommandManagerFacts
    {
        [TestClass]
        public class TheCreateCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.CreateCommand(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstanceNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.AddCommand(null, null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullInstanceWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.AddCommand(null, " "));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForInstanceWhitespaceCommandName()
            {
                var commandManager = new CommandManager();
                var command = new CompositeCommand();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.AddCommand(command, " "));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.CreateCommand(" "));
            }

            [TestMethod]
            public void ThrowsInvalidOperationExceptionForAlreadyCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => commandManager.CreateCommand("MyCommand"));
            }

            [TestMethod]
            public void ThrowsInvalidOperationExceptionOnAddForAlreadyCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => commandManager.AddCommand(new CompositeCommand(), "MyCommand"));
            }

            [TestMethod]
            public void ThrowsInvalidOperationExceptionOnAddForAlreadyAddedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.AddCommand(new CompositeCommand(), "MyCommand");

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => commandManager.AddCommand(new CompositeCommand(), "MyCommand"));
            }

            [TestMethod]
            public void ThrowsInvalidOperationExceptionOnAddForAlreadyAddedCommandInstance()
            {
                var commandManager = new CommandManager();

                var cmd = new CompositeCommand();
                commandManager.AddCommand(cmd, "MyCommand");

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => commandManager.AddCommand(cmd, "MyCommand2"));
            }
            
            [TestMethod]
            public void CorrectlyCreatesTheCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }

            [TestMethod]
            public void CorrectlyAddTheCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }
        }

        [TestClass]
        public class TheIsCommandCreatedMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(" "));
            }

            [TestMethod]
            public void ReturnsTrueForCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }

            [TestMethod]
            public void ReturnsTrueForAddedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.AddCommand(new CompositeCommand(), "MyCommand");

                Assert.IsTrue(commandManager.IsCommandCreated("MyCommand"));
            }

            [TestMethod]
            public void ReturnsFalseForNotCreatedCommand()
            {
                var commandManager = new CommandManager();

                Assert.IsFalse(commandManager.IsCommandCreated("MyCommand"));
            }
        }

        [TestClass]
        public class TheGetCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.GetCommand(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.GetCommand(" "));
            }

            [TestMethod]
            public void ReturnsNullForNotCreatedCommand()
            {
                var commandManager = new CommandManager();

                Assert.IsNull(commandManager.GetCommand("MyCommand"));
            }

            [TestMethod]
            public void ReturnsCommandForCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                Assert.IsNotNull(commandManager.GetCommand("MyCommand"));
            }

            [TestMethod]
            public void ReturnsCommandForAddedCommand()
            {
                var commandManager = new CommandManager();

                var cmd = new CompositeCommand();
                commandManager.AddCommand(cmd, "MyCommand");

                Assert.IsTrue(cmd == commandManager.GetCommand("MyCommand"));
            }
        }

        [TestClass]
        public class TheExecuteCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.ExecuteCommand(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.ExecuteCommand(" "));
            }

            [TestMethod]
            public void ExecutesRegisteredCommands()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");
                commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

                commandManager.ExecuteCommand("MyCommand");

                Assert.IsTrue(vm.IsTestCommand1Executed);
            }

            [TestMethod]
            public void ExecutesOutsideRegisteredCommands()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                var cmd = new CompositeCommand();

                commandManager.AddCommand(cmd, "MyCommand");
                commandManager.RegisterCommand("MyCommand", vm.TestCommand1);

                cmd.Execute();

                Assert.IsTrue(vm.IsTestCommand1Executed);
            }

            [TestMethod]
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

        [TestClass]
        public class TheRegisterCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand((String)null, vm.TestCommand1));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand(" ", vm.TestCommand1));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }

        [TestClass]
        public class TheUnregisterCommandMethod
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(" "));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }
    }
}
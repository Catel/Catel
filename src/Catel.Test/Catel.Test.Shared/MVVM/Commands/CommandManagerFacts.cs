﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManagerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM
{
    using System;
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

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.CreateCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.CreateCommand(" "));
            }

            [TestCase]
            public void ThrowsInvalidOperationExceptionForAlreadyCreatedCommand()
            {
                var commandManager = new CommandManager();

                commandManager.CreateCommand("MyCommand");

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => commandManager.CreateCommand("MyCommand"));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(" "));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.GetCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.GetCommand(" "));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.ExecuteCommand(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.ExecuteCommand(" "));
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

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand(null, vm.TestCommand1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand(" ", vm.TestCommand1));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }

        [TestFixture]
        public class TheUnregisterCommandMethod
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(null));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForWhitespaceCommandName()
            {
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.IsCommandCreated(" "));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForNullCommand()
            {
                var vm = new CompositeCommandViewModel();
                var commandManager = new CommandManager();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => commandManager.RegisterCommand("MyCommand", null));
            }
        }
    }
}
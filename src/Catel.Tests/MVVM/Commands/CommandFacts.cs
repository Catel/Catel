// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FastObservableCollectionFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.MVVM.Commands
{
    using System;
    using System.Diagnostics;
    using Catel.Data;
    using Catel.MVVM;

    using NUnit.Framework;

    public class CommandFacts
    {
        [TestFixture]
        public class TheExecuteCommand
        {
            [Test]
            public void ExecuteThrowsException()
            {
                var command = new Command(() => { throw new Exception(); }, () => true);
                ExceptionTester.CallMethodAndExpectException<Exception>(() => command.Execute());
            }

            [Test]
            public void CanExecuteThrowsException()
            {
                var command = new Command(() => { }, () => { throw new Exception(); });
                ExceptionTester.CallMethodAndExpectException<Exception>(() => command.Execute());
            }
        }

        /// <summary>
        /// See https://github.com/Catel/Catel/issues/1192
        /// </summary>
        [TestFixture]
        public class CTL1192_WeakRef_CanExecute
        {
            public class TestDisplayClassViewModel : ViewModelBase
            {
                public TestDisplayClassViewModel()
                {
                    int localVariable = 1;
                    TestCommand = new Command(TestFunction, () =>
                    {
                        Console.WriteLine("CanExecute called " + BoxingCache.GetBoxedValue(localVariable++));
                        return false;
                    });
                }

                public void TestFunction()
                {
                }

                public Command TestCommand { get; set; }
            }

            [Test]
            public void CanExecuteWeakRefLostTest()
            {
                var vm = new TestDisplayClassViewModel();

                var canExecuteBefore = vm.TestCommand.CanExecute();
                Console.WriteLine("CanExecute before: " + canExecuteBefore);

                GC.Collect();

                var canExecuteAfter = vm.TestCommand.CanExecute();
                Console.WriteLine("CanExecute after: " + canExecuteAfter);
                Assert.That(canExecuteAfter, Is.False);
            }
        }
    }
}

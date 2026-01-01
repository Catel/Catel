namespace Catel.Tests.MVVM.Commands
{
    using System;
    using Catel.Data;
    using Catel.MVVM;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public class CommandFacts
    {
        [TestFixture]
        public class TheExecuteCommand
        {
            [Test]
            public void Execute_Throws_Exception()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var command = new Command(serviceProvider, () => { throw new Exception(); }, () => true);
                Assert.Throws<Exception>(() => command.Execute());
            }

            [Test]
            public void CanExecute_Throws_Exception()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var command = new Command(serviceProvider, () => { }, () => { throw new Exception(); });
                Assert.Throws<Exception>(() => command.Execute());
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
                public TestDisplayClassViewModel(IServiceProvider serviceProvider)
                    : base(serviceProvider)
                {
                    int localVariable = 1;
                    TestCommand = new Command(serviceProvider, TestFunction, () =>
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
            public void CanExecute_Weak_Ref_Lost_Test()
            {
                var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

                using var serviceProvider = serviceCollection.BuildServiceProvider();

                var vm = new TestDisplayClassViewModel(serviceProvider);

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

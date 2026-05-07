namespace Catel.Tests.MVVM;

using System;
using System.Windows.Input;
using Catel.MVVM;
using Catel.MVVM.Providers;
using Moq;
using NUnit.Framework;

public class AdditionalMvvmCoverageFacts
{
    [TestFixture]
    public class The_CommandEventArgs_Class
    {
        [Test]
        public void Stores_CommandParameter()
        {
            var eventArgs = new CommandEventArgs("value");

            Assert.That(eventArgs.CommandParameter, Is.EqualTo("value"));
        }
    }

    [TestFixture]
    public class The_CommandCanceledEventArgs_Class
    {
        [Test]
        public void Stores_CommandParameter_And_Default_Cancel()
        {
            var eventArgs = new CommandCanceledEventArgs("value");

            Assert.That(eventArgs.CommandParameter, Is.EqualTo("value"));
            Assert.That(eventArgs.Cancel, Is.False);
        }
    }

    [TestFixture]
    public class The_CommandCreatedEventArgs_Class
    {
        [Test]
        public void Stores_Command_And_Name()
        {
            var command = new Mock<ICommand>().Object;

            var eventArgs = new CommandCreatedEventArgs(command, "MyCommand");

            Assert.That(eventArgs.Command, Is.EqualTo(command));
            Assert.That(eventArgs.Name, Is.EqualTo("MyCommand"));
        }
    }

    [TestFixture]
    public class The_CommandExecutedEventArgs_Class
    {
        [Test]
        public void Stores_All_Values()
        {
            var command = new Mock<ICatelCommand>().Object;

            var eventArgs = new CommandExecutedEventArgs(command, "parameter", "MyCommand");

            Assert.That(eventArgs.Command, Is.EqualTo(command));
            Assert.That(eventArgs.CommandParameter, Is.EqualTo("parameter"));
            Assert.That(eventArgs.CommandPropertyName, Is.EqualTo("MyCommand"));
        }

        [Test]
        public void Throws_For_Null_Command()
        {
            Assert.Throws<ArgumentNullException>(() => new CommandExecutedEventArgs(null!));
        }
    }

    [TestFixture]
    public class The_CommandProgressChangedEventArgs_Class
    {
        [Test]
        public void Stores_Progress()
        {
            var eventArgs = new CommandProgressChangedEventArgs<int>(42);

            Assert.That(eventArgs.Progress, Is.EqualTo(42));
        }
    }

    [TestFixture]
    public class The_DetermineViewModelTypeEventArgs_Class
    {
        [Test]
        public void Stores_DataContext_And_ViewModelType()
        {
            var eventArgs = new DetermineViewModelTypeEventArgs("context")
            {
                ViewModelType = typeof(object)
            };

            Assert.That(eventArgs.DataContext, Is.EqualTo("context"));
            Assert.That(eventArgs.ViewModelType, Is.EqualTo(typeof(object)));
        }
    }

    [TestFixture]
    public class The_DetermineViewModelInstanceEventArgs_Class
    {
        [Test]
        public void Stores_DataContext_And_Allows_Setting_Properties()
        {
            var viewModel = new Mock<IViewModel>().Object;
            var eventArgs = new DetermineViewModelInstanceEventArgs("context")
            {
                DoNotCreateViewModel = true,
                ViewModel = viewModel
            };

            Assert.That(eventArgs.DataContext, Is.EqualTo("context"));
            Assert.That(eventArgs.DoNotCreateViewModel, Is.True);
            Assert.That(eventArgs.ViewModel, Is.EqualTo(viewModel));
        }
    }

    [TestFixture]
    public class The_CancelingEventArgs_Class
    {
        [Test]
        public void Defaults_Cancel_To_False()
        {
            var eventArgs = new CancelingEventArgs();

            Assert.That(eventArgs.Cancel, Is.False);
        }
    }

    [TestFixture]
    public class The_SavingEventArgs_Class
    {
        [Test]
        public void Allows_Setting_Cancel()
        {
            var eventArgs = new SavingEventArgs
            {
                Cancel = true
            };

            Assert.That(eventArgs.Cancel, Is.True);
        }
    }
}

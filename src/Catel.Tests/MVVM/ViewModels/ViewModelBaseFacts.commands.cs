namespace Catel.Tests.MVVM.ViewModels
{
    using System.Threading;
    using Catel.MVVM;
    using NUnit.Framework;
    using TestClasses;

    public partial class ViewModelBaseFacts
    {
        [TestCase]
        public void InvalidateCommands_Manual()
        {
            bool canExecuteChangedTriggered = false;
            using (var canExecuteChangedEvent = new ManualResetEvent(false))
            {
                var viewModel = new TestViewModel();
                viewModel.SetInvalidateCommandsOnPropertyChanged(false);

                ICatelCommand command = viewModel.GenerateData;
                command.CanExecuteChanged += delegate
                {
                    canExecuteChangedTriggered = true;
                    canExecuteChangedEvent.Set();
                };

                // By default, command can be executed
                Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

                viewModel.FirstName = "first name";

                Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

                canExecuteChangedEvent.WaitOne(1000, false);

                Assert.That(canExecuteChangedTriggered, Is.False);
            }
        }

        [TestCase]
        public void InvalidateCommands_AutomaticByPropertyChange()
        {
            bool canExecuteChangedTriggered = false;
            using (var canExecuteChangedEvent = new ManualResetEvent(false))
            {
                var viewModel = new TestViewModel();
                viewModel.SetInvalidateCommandsOnPropertyChanged(true);

                ICatelCommand command = viewModel.GenerateData;
                command.CanExecuteChanged += delegate
                {
                    canExecuteChangedTriggered = true;
                    canExecuteChangedEvent.Set();
                };

                // By default, command can be executed
                Assert.That(viewModel.GenerateData.CanExecute(null), Is.True);

                viewModel.FirstName = "first name";

                Assert.That(viewModel.GenerateData.CanExecute(null), Is.False);

                canExecuteChangedEvent.WaitOne(1000, false);

                Assert.That(canExecuteChangedTriggered, Is.True);
            }
        }
    }
}

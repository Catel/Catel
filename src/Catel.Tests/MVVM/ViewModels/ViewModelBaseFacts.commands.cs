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
                Assert.IsTrue(viewModel.GenerateData.CanExecute(null));

                viewModel.FirstName = "first name";

                Assert.IsFalse(viewModel.GenerateData.CanExecute(null));

                canExecuteChangedEvent.WaitOne(1000, false);

                Assert.IsFalse(canExecuteChangedTriggered);
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
                Assert.IsTrue(viewModel.GenerateData.CanExecute(null));

                viewModel.FirstName = "first name";

                Assert.IsFalse(viewModel.GenerateData.CanExecute(null));

                canExecuteChangedEvent.WaitOne(1000, false);

                Assert.IsTrue(canExecuteChangedTriggered);
            }
        }
    }
}

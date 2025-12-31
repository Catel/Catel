namespace Catel.Tests.MVVM
{
    using System;
    using Catel.MVVM;

    public class CompositeCommandViewModel : ViewModelBase
    {
        public CompositeCommandViewModel(IServiceProvider serviceProvider)
        {
            TestCommand1 = new Command(serviceProvider, OnTestCommand1Execute, OnTestCommand1CanExecute);
            TestCommand2 = new Command(serviceProvider, OnTestCommand2Execute, OnTestCommand2CanExecute);

            AllowTestCommand1Execution = true;
            AllowTestCommand2Execution = true;
        }
       
        public bool AllowTestCommand1Execution { get; private set; }
        public bool AllowTestCommand2Execution { get; private set; }

        public bool IsTestCommand1Executed { get; private set; }
        public bool IsTestCommand2Executed { get; private set; }

        public Command TestCommand1 { get; private set; }
        public Command TestCommand2 { get; private set; }
  

        private bool OnTestCommand1CanExecute()
        {
            return AllowTestCommand1Execution;
        }

        private void OnTestCommand1Execute()
        {
            IsTestCommand1Executed = true;
        }

        private bool OnTestCommand2CanExecute()
        {
            return AllowTestCommand2Execution;
        }

        private void OnTestCommand2Execute()
        {
            IsTestCommand2Executed = true;
        }
    }
}

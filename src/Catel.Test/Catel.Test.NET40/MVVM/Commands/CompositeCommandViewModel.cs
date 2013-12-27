// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeCommandViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.MVVM
{
    using Catel.MVVM;

    public class CompositeCommandViewModel : ViewModelBase
    {
        #region Constructors
        public CompositeCommandViewModel()
        {
            TestCommand1 = new Command(OnTestCommand1Execute, OnTestCommand1CanExecute);
            TestCommand2 = new Command(OnTestCommand2Execute, OnTestCommand2CanExecute);

            AllowTestCommand1Execution = true;
            AllowTestCommand2Execution = true;
        }
        #endregion

        #region Properties
        public bool AllowTestCommand1Execution { get; set; }
        public bool AllowTestCommand2Execution { get; set; }

        public bool IsTestCommand1Executed { get; private set; }
        public bool IsTestCommand2Executed { get; private set; }

        public Command TestCommand1 { get; private set; }
        public Command TestCommand2 { get; private set; }
        #endregion

        #region Methods
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
        #endregion
    }
}
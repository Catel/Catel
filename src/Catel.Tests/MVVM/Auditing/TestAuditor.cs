namespace Catel.Tests.MVVM.Auditing
{
    using System;
    using Catel.MVVM;
    using Catel.MVVM.Auditing;

    public class TestAuditor : AuditorBase
    {
        public bool OnViewModelCreatingCalled { get; set; }
        public Type OnViewModelCreatingType { get; set; }

        public override void OnViewModelCreating(Type viewModelType)
        {
            if (OnViewModelCreatingCalled)
            {
                return;
            }

            OnViewModelCreatingCalled = true;
            OnViewModelCreatingType = viewModelType;
        }

        public bool OnViewModelCreatedCalled { get; set; }
        public Type OnViewModelCreatedType { get; set; }

        public override void OnViewModelCreated(IViewModel viewModel)
        {
            if (OnViewModelCreatedCalled)
            {
                return;
            }

            OnViewModelCreatedCalled = true;
            OnViewModelCreatedType = viewModel.GetType();
        }


        public bool OnViewModelInitializedCalled { get; set; }
        public Type OnViewModelInitializedType { get; set; }

        public override void OnViewModelInitialized(IViewModel viewModel)
        {
            if (OnViewModelInitializedCalled)
            {
                return;
            }

            OnViewModelInitializedCalled = true;
            OnViewModelInitializedType = viewModel.GetType();
        }

        public bool OnPropertyChangedCalled { get; set; }
        public IViewModel OnPropertyChangedViewModel { get; set; }
        public string OnPropertyChangedPropertyName { get; set; }
        public object OnPropertyChangedNewValue { get; set; }

        public override void OnPropertyChanged(IViewModel viewModel, string propertyName, object newValue)
        {
            OnPropertyChangedCalled = true;
            OnPropertyChangedViewModel = viewModel;
            OnPropertyChangedPropertyName = propertyName;
            OnPropertyChangedNewValue = newValue;
        }

        public bool OnCommandExecutedCalled { get; set; }
        public IViewModel OnCommandExecutedViewModel { get; set; }
        public string OnCommandExecutedCommandName { get; set; }
        public ICatelCommand OnCommandExecutedCommand { get; set; }
        public object OnCommandExecutedCommandParameter { get; set; }

        public override void OnCommandExecuted(IViewModel viewModel, string commandName, ICatelCommand command, object commandParameter)
        {
            if (OnCommandExecutedCalled)
            {
                return;
            }

            OnCommandExecutedCalled = true;
            OnCommandExecutedViewModel = viewModel;
            OnCommandExecutedCommandName = commandName;
            OnCommandExecutedCommand = command;
            OnCommandExecutedCommandParameter = commandParameter;
        }

        public bool OnViewModelSavingCalled { get; set; }
        public IViewModel OnViewModelSavingViewModel { get; set; }

        public override void OnViewModelSaving(IViewModel viewModel)
        {
            if (OnViewModelSavingCalled)
            {
                return;
            }

            OnViewModelSavingCalled = true;
            OnViewModelSavingViewModel = viewModel;
        }

        public bool OnViewModelSavedCalled { get; set; }
        public IViewModel OnViewModelSavedViewModel { get; set; }

        public override void OnViewModelSaved(IViewModel viewModel)
        {
            if (OnViewModelSavedCalled)
            {
                return;
            }

            OnViewModelSavedCalled = true;
            OnViewModelSavedViewModel = viewModel;
        }

        public bool OnViewModelCancelingCalled { get; set; }
        public IViewModel OnViewModelCancelingViewModel { get; set; }

        public override void OnViewModelCanceling(IViewModel viewModel)
        {
            if (OnViewModelCancelingCalled)
            {
                return;
            }

            OnViewModelCancelingCalled = true;
            OnViewModelCancelingViewModel = viewModel;
        }

        public bool OnViewModelCanceledCalled { get; set; }
        public IViewModel OnViewModelCanceledViewModel { get; set; }

        public override void OnViewModelCanceled(IViewModel viewModel)
        {
            if (OnViewModelCanceledCalled)
            {
                return;
            }

            OnViewModelCanceledCalled = true;
            OnViewModelCanceledViewModel = viewModel;
        }

        public bool OnViewModelClosingCalled { get; set; }
        public IViewModel OnViewModelClosingViewModel { get; set; }

        public override void OnViewModelClosing(IViewModel viewModel)
        {
            if (OnViewModelClosingCalled)
            {
                return;
            }

            OnViewModelClosingCalled = true;
            OnViewModelClosingViewModel = viewModel;
        }

        public bool OnViewModelClosedCalled { get; set; }
        public IViewModel OnViewModelClosedViewModel { get; set; }

        public override void OnViewModelClosed(IViewModel viewModel)
        {
            if (OnViewModelClosedCalled)
            {
                return;
            }

            OnViewModelClosedCalled = true;
            OnViewModelClosedViewModel = viewModel;
        }
    }
}

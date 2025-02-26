namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IAuditingManager
    {
        bool IsAuditingEnabled { get; }
        int RegisteredAuditorsCount { get; }

        void Clear();
        void OnCommandExecuted(IViewModel? viewModel, string? commandName, ICatelCommand command, object? commandParameter);
        void OnPropertyChanged(IViewModel viewModel, string? propertyName, object? newValue);
        void OnViewModelCanceled(IViewModel viewModel);
        void OnViewModelCanceling(IViewModel viewModel);
        void OnViewModelClosed(IViewModel viewModel);
        void OnViewModelClosing(IViewModel viewModel);
        void OnViewModelCreated(IViewModel viewModel);
        void OnViewModelCreating(Type viewModelType);
        void OnViewModelInitialized(IViewModel viewModel);
        void OnViewModelSaved(IViewModel viewModel);
        void OnViewModelSaving(IViewModel viewModel);
        void RegisterAuditor(IAuditor auditor);
        void RegisterAuditor<TAuditor>() where TAuditor : class, IAuditor;
        void UnregisterAuditor(IAuditor auditor);
    }
}

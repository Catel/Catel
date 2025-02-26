namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Catel.Data;

    /// <summary>
    /// Helper for auditing which handles the complete subscription of an <see cref="IViewModel"/> instance
    /// to the current auditing manager.
    /// </summary>
    public class AuditingWrapper
    {
        private static readonly HashSet<string> KnownIgnoredPropertyNames = new HashSet<string>();

        private readonly IAuditingManager _auditingManager;
        private readonly IObjectAdapter _objectAdapter;
        private readonly IViewModel _viewModel;

        /// <summary>
        /// Initializes static members of the <see cref="AuditingWrapper"/> class.
        /// </summary>
        static AuditingWrapper()
        {
            KnownIgnoredPropertyNames.Add("IDataWarningInfo.Warning");
            KnownIgnoredPropertyNames.Add("INotifyDataWarningInfo.HasWarnings");
            KnownIgnoredPropertyNames.Add("IDataErrorInfo.Error");
            KnownIgnoredPropertyNames.Add("INotifyDataErrorInfo.HasErrors");
        }

        /// <summary>
        /// Registers the view model to the <see cref="AuditingManager"/>.
        /// <para />
        /// This helper will automatically unsubscribe from all events when the view model is closed.
        /// </summary>
        /// <param name="auditingManager">The auditing manager.</param>
        /// <param name="objectAdapter">The object adapter.</param>
        /// <param name="viewModel">The view model to register.</param>
        /// <remarks>
        /// This helper will call the <see cref="AuditingManager.OnViewModelCreating"/> and <see cref="AuditingManager.OnViewModelCreated"/>
        /// automatically.
        /// </remarks>
        public AuditingWrapper(IAuditingManager auditingManager, IObjectAdapter objectAdapter, IViewModel viewModel)
        {
            _auditingManager = auditingManager;
            _objectAdapter = objectAdapter;
            _viewModel = viewModel;

            var isAuditingEnabled = _auditingManager.IsAuditingEnabled;
            if (isAuditingEnabled)
            {
                _auditingManager.OnViewModelCreating(viewModel.GetType());
            }

            SubscribeEvents();

            if (isAuditingEnabled)
            {
                _auditingManager.OnViewModelCreated(viewModel);
            }
        }

        /// <summary>
        /// Subscribes to all events of the view model.
        /// </summary>
        private void SubscribeEvents()
        {
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
            _viewModel.CommandExecutedAsync += OnViewModelCommandExecutedAsync;
            _viewModel.InitializedAsync += OnViewModelInitializedAsync;
            _viewModel.SavingAsync += OnViewModelSavingAsync;
            _viewModel.SavedAsync += OnViewModelSavedAsync;
            _viewModel.CancelingAsync += OnViewModelCancelingAsync;
            _viewModel.CanceledAsync += OnViewModelCanceledAsync;
            _viewModel.ClosingAsync += OnViewModelClosingAsync;
            _viewModel.ClosedAsync += OnViewModelClosedAsync;
        }

        /// <summary>
        /// Unsubscribes from all events of the view model.
        /// </summary>
        private void UnsubscribeEvents()
        {
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _viewModel.CommandExecutedAsync -= OnViewModelCommandExecutedAsync;
            _viewModel.InitializedAsync -= OnViewModelInitializedAsync;
            _viewModel.SavingAsync -= OnViewModelSavingAsync;
            _viewModel.SavedAsync -= OnViewModelSavedAsync;
            _viewModel.CancelingAsync -= OnViewModelCancelingAsync;
            _viewModel.CanceledAsync -= OnViewModelCanceledAsync;
            _viewModel.ClosingAsync -= OnViewModelClosingAsync;
            _viewModel.ClosedAsync -= OnViewModelClosedAsync;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return;
            }

            object? propertyValue = null;
            if (!string.IsNullOrEmpty(e.PropertyName) && !KnownIgnoredPropertyNames.Contains(e.PropertyName))
            {
                _objectAdapter.TryGetMemberValue(viewModel, e.PropertyName, out propertyValue);
            }

            _auditingManager.OnPropertyChanged(viewModel, e.PropertyName, propertyValue);
        }

        private Task OnViewModelCommandExecutedAsync(object? sender, CommandExecutedEventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnCommandExecuted(viewModel, e.CommandPropertyName, e.Command, e.CommandParameter);

            return Task.CompletedTask;
        }

        private Task OnViewModelInitializedAsync(object? sender, EventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelInitialized(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelSavingAsync(object? sender, SavingEventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelSaving(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelSavedAsync(object? sender, EventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelSaved(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelCancelingAsync(object? sender, CancelingEventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelCanceling(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelCanceledAsync(object? sender, EventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelCanceled(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelClosingAsync(object? sender, EventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelClosing(viewModel);

            return Task.CompletedTask;
        }

        private Task OnViewModelClosedAsync(object? sender, EventArgs e)
        {
            if (!_auditingManager.IsAuditingEnabled)
            {
                return Task.CompletedTask;
            }

            var viewModel = sender as IViewModel;
            if (viewModel is null)
            {
                return Task.CompletedTask;
            }

            _auditingManager.OnViewModelClosed(viewModel);

            UnsubscribeEvents();

            return Task.CompletedTask;
        }
    }
}

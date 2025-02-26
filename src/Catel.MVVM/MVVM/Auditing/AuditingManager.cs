namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Handles the auditing for MVVM inside Catel.
    /// <para/>
    /// Use this manager to register custom auditors.
    /// </summary>
    public class AuditingManager : IAuditingManager
    {
        private readonly List<IAuditor> _auditors = new List<IAuditor>();
        private readonly IServiceProvider _serviceProvider;

        public AuditingManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets a value indicating whether auditing is enabled. Auditing is enabled when at least 1 auditor is registered.
        /// </summary>
        /// <value><c>true</c> if auditing is enabled; otherwise, <c>false</c>.</value>
        public bool IsAuditingEnabled { get; private set; }

        /// <summary>
        /// Gets the number of registered auditors.
        /// </summary>
        /// <value>The number of registered auditors.</value>
        public int RegisteredAuditorsCount
        {
            get { return _auditors.Count; }
        }

        /// <summary>
        /// Clears all the current auditors.
        /// </summary>
        public void Clear()
        {
            lock (_auditors)
            {
                _auditors.Clear();

                UpdateState();
            }
        }

        /// <summary>
        /// Registers a auditor and automatically instantiates it by using the <see cref="ActivatorUtilities.CreateInstance"/>.
        /// </summary>
        /// <typeparam name="TAuditor">The type of the auditor.</typeparam>
        public void RegisterAuditor<TAuditor>()
            where TAuditor : class, IAuditor
        {
            var auditor = ActivatorUtilities.CreateInstance<TAuditor>(_serviceProvider);

            RegisterAuditor(auditor);
        }

        /// <summary>
        /// Registers a specific auditor.
        /// </summary>
        /// <param name="auditor">The auditor.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="auditor"/> is <c>null</c>.</exception>
        public void RegisterAuditor(IAuditor auditor)
        {
            ArgumentNullException.ThrowIfNull(auditor);

            lock (_auditors)
            {
                if (!_auditors.Contains(auditor))
                {
                    _auditors.Add(auditor);
                }

                UpdateState();
            }
        }

        /// <summary>
        /// Unregisters a specific auditor.
        /// <para />
        /// If the auditor is not registered, nothing happens.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="auditor"/> is <c>null</c>.</exception>
        public void UnregisterAuditor(IAuditor auditor)
        {
            ArgumentNullException.ThrowIfNull(auditor);

            lock (_auditors)
            {
                _auditors.Remove(auditor);

                UpdateState();
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is being created.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        public void OnViewModelCreating(Type viewModelType)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelCreating(viewModelType);
                }
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is created.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelCreated(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelCreated(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a specific view model type is initialized.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelInitialized(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelInitialized(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when the property of a view model has just changed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new property value.</param>
        public void OnPropertyChanged(IViewModel viewModel, string? propertyName, object? newValue)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    if (propertyName is not null && auditor.PropertiesToIgnore is not null)
                    {
                        if (auditor.PropertiesToIgnore.Contains(propertyName))
                        {
                            continue;
                        }
                    }

                    auditor.OnPropertyChanged(viewModel, propertyName, newValue);
                }
            }
        }

        /// <summary>
        /// Must be called when a command of a view model has just been executed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="commandName">Name of the command, which is the name of the command property.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter.</param>
        public void OnCommandExecuted(IViewModel? viewModel, string? commandName, ICatelCommand command, object? commandParameter)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnCommandExecuted(viewModel, commandName, command, commandParameter);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelSaving(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelSaving(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelSaved(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelSaved(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelCanceling(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelCanceling(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelCanceled(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelCanceled(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model is about to be closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelClosing(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelClosing(viewModel);
                }
            }
        }

        /// <summary>
        /// Must be called when a view model has just been closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public void OnViewModelClosed(IViewModel viewModel)
        {
            lock (_auditors)
            {
                foreach (var auditor in _auditors)
                {
                    auditor.OnViewModelClosed(viewModel);
                }
            }
        }

        /// <summary>
        /// Updates the state.
        /// </summary>
        private void UpdateState()
        {
            lock (_auditors)
            {
                IsAuditingEnabled = _auditors.Count > 0;
            }
        }
    }
}

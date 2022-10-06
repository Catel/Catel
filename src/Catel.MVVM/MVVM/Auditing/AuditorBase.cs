namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Convenience implementation of the <see cref="IAuditor"/> interface so not all interface members
    /// have to be implemented by the developer.
    /// <para />
    /// This auditor also ignores the default properties such as <c>IsDirty</c> since hardly anyone
    /// would be interested in such changes. To enable the notification, clear the <see cref="PropertiesToIgnore"/>.
    /// </summary>
    public abstract class AuditorBase : IAuditor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditorBase"/> class.
        /// </summary>
        protected AuditorBase()
        {
            PropertiesToIgnore = new HashSet<string>
            {
                "IsDirty",
                "IsReadOnly",
                "ParentViewModel",
                "Title"
            };
        }

        /// <summary>
        /// Gets a list of properties that should be ignored.
        /// </summary>
        /// <value>The list of properties to ignore.</value>
        public HashSet<string> PropertiesToIgnore { get; private set; }

        /// <summary>
        /// Called when a specific view model type is being created.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        public virtual void OnViewModelCreating(Type viewModelType) { }

        /// <summary>
        /// Called when a specific view model type is created.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelCreated(IViewModel viewModel) { }

        /// <summary>
        /// Called when a specific view model type is initialized.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelInitialized(IViewModel viewModel) { }

        /// <summary>
        /// Called when the property of a view model has just changed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new property value.</param>
        public virtual void OnPropertyChanged(IViewModel? viewModel, string? propertyName, object? newValue) { }

        /// <summary>
        /// Called when a command of a view model has just been executed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="commandName">Name of the command, which is the name of the command property.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter.</param>
        public virtual void OnCommandExecuted(IViewModel? viewModel, string? commandName, ICatelCommand command, object? commandParameter) { }

        /// <summary>
        /// Called when a view model is about to be saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelSaving(IViewModel viewModel) { }

        /// <summary>
        /// Called when a view model has just been saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelSaved(IViewModel viewModel) { }

        /// <summary>
        /// Called when a view model is about to be canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelCanceling(IViewModel viewModel) { }

        /// <summary>
        /// Called when a view model has just been canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelCanceled(IViewModel viewModel) { }

        /// <summary>
        /// Called when a view model is about to be closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelClosing(IViewModel viewModel) { }

        /// <summary>
        /// Called when a view model has just been closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public virtual void OnViewModelClosed(IViewModel viewModel) { }
    }
}

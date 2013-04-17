// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuditor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Auditing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for auditors that can register itself with the <see cref="AuditingManager"/>.
    /// </summary>
    public interface IAuditor
    {
        /// <summary>
        /// Gets a list of properties that should be ignored.
        /// </summary>
        /// <value>The list of properties to ignore.</value>
        List<string> PropertiesToIgnore { get; }

        /// <summary>
        /// Called when a specific view model type is being created.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        void OnViewModelCreating(Type viewModelType);

        /// <summary>
        /// Called when a specific view model type is created.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        void OnViewModelCreated(Type viewModelType);

        /// <summary>
        /// Called when the property of a view model is about to change.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old property value.</param>
        void OnPropertyChanging(IViewModel viewModel, string propertyName, object oldValue);

        /// <summary>
        /// Called when the property of a view model has just changed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="newValue">The new property value.</param>
        void OnPropertyChanged(IViewModel viewModel, string propertyName, object newValue);

        /// <summary>
        /// Called when a command of a view model has just been executed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="commandName">Name of the command, which is the name of the command property.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter.</param>
        void OnCommandExecuted(IViewModel viewModel, string commandName, ICatelCommand command, object commandParameter);

        /// <summary>
        /// Called when a view model is about to be saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelSaving(IViewModel viewModel);

        /// <summary>
        /// Called when a view model has just been saved.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelSaved(IViewModel viewModel);

        /// <summary>
        /// Called when a view model is about to be canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelCanceling(IViewModel viewModel);

        /// <summary>
        /// Called when a view model has just been canceled.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelCanceled(IViewModel viewModel);

        /// <summary>
        /// Called when a view model is about to be closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelClosing(IViewModel viewModel);

        /// <summary>
        /// Called when a view model has just been closed.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        void OnViewModelClosed(IViewModel viewModel);
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INotifyableViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;

    /// <summary>
    /// Interface that view models must implement to support the <see cref="InterestedInAttribute"/>.
    /// </summary>
    public interface INotifyableViewModel
    {
        /// <summary>
        /// Called when a property on a view model has changed.
        /// </summary>
        /// <param name="viewModel">The view model of which the property has changed.</param>
        /// <param name="propertyName">Name of the property that has changed.</param>
        void ViewModelPropertyChanged(IViewModel viewModel, string propertyName);

        /// <summary>
        /// Called when a command on a view model is executed.
        /// </summary>
        /// <param name="viewModel">The view model of which the command is executed.</param>
        /// <param name="command">The command that has been executed.</param>
        /// <param name="commandParameter">The command parameter.</param>
        void ViewModelCommandExecuted(IViewModel viewModel, ICatelCommand command, object commandParameter);

        /// <summary>
        /// Called when a view model event occurs.
        /// </summary>
        /// <param name="viewModel">The view model that has raised the event.</param>
        /// <param name="viewModelEvent">The view model event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ViewModelEvent(IViewModel viewModel, ViewModelEvent viewModelEvent, EventArgs e);
    }
}
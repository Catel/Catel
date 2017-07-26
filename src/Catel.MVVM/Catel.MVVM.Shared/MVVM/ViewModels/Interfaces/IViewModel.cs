// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using Data;

    /// <summary>
    /// View model interface.
    /// </summary>
    public interface IViewModel : IValidatable, INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="CancelViewModelAsync"/>, <see cref="SaveViewModelAsync"/> or <see cref="CloseViewModelAsync"/>
        /// will have no effect.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        bool IsClosed { get; }

        /// <summary>
        /// Gets the title of the view model.
        /// </summary>
        /// <value>The title.</value>
        string Title { get; }

        /// <summary>
        /// Gets the unique identifier of the view model.
        /// </summary>
        /// <value>The unique identifier.</value>
        int UniqueIdentifier { get; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model has been initialized.
        /// </summary>
        event AsyncEventHandler<EventArgs> InitializedAsync;

        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        event AsyncEventHandler<CommandExecutedEventArgs> CommandExecutedAsync;

        /// <summary>
        /// Occurs when the view model is about to be saved.
        /// </summary>
        event AsyncEventHandler<SavingEventArgs> SavingAsync;

        /// <summary>
        /// Occurs when the view model is saved successfully.
        /// </summary>
        event AsyncEventHandler<EventArgs> SavedAsync;

        /// <summary>
        /// Occurs when the view model is about to be canceled.
        /// </summary>
        event AsyncEventHandler<CancelingEventArgs> CancelingAsync;

        /// <summary>
        /// Occurrs when the view model is canceled.
        /// </summary>
        event AsyncEventHandler<EventArgs> CanceledAsync;

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        event AsyncEventHandler<EventArgs> ClosingAsync;

        /// <summary>
        /// Occurs when the view model has been closed.
        /// </summary>
        event AsyncEventHandler<ViewModelClosedEventArgs> ClosedAsync;
        #endregion

        #region Methods
        /// <summary>
        /// Initializes the view model. Normally the initialization is done in the constructor, but sometimes this must be delayed
        /// to a state where the associated UI element (user control, window, ...) is actually loaded.
        /// <para />
        /// This method is called as soon as the associated UI element is loaded.
        /// </summary>
        /// <remarks>
        /// It's not recommended to implement the initialization of properties in this method. The initialization of properties
        /// should be done in the constructor. This method should be used to start the retrieval of data from a web service or something
        /// similar.
        /// <para />
        /// During unit tests, it is recommended to manually call this method because there is no external container calling this method.
        /// </remarks>
        Task InitializeViewModelAsync();

        /// <summary>
        /// Cancels the editing of the data.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        Task<bool> CancelViewModelAsync();

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        Task<bool> SaveViewModelAsync();

        /// <summary>
        /// Closes this instance. Always called after the <see cref="CancelViewModelAsync"/> of <see cref="SaveViewModelAsync"/> method.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        Task CloseViewModelAsync(bool? result);
        #endregion
    }
}
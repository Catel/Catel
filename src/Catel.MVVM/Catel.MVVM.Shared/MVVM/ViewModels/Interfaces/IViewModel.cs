// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewModel.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    /// <summary>
    /// View model interface.
    /// </summary>
    public interface IViewModel : INotifyPropertyChanged
    {
        #region Properties
        /// <summary>
        /// Gets a value indicating whether this instance has a dirty model.
        /// </summary>
        /// <value><c>true</c> if this instance has a dirty model; otherwise, <c>false</c>.</value>
        bool HasDirtyModel { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is closed. If a view model is closed, calling
        /// <see cref="CancelViewModel"/>, <see cref="SaveViewModel"/> or <see cref="CloseViewModel"/>
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

        /// <summary>
        /// Gets the view model construction time, which is used to get unique instances of view models.
        /// </summary>
        /// <value>The view model construction time.</value>
        DateTime ViewModelConstructionTime { get; }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the view model has been initialized.
        /// </summary>
        event EventHandler<EventArgs> Initialized;

        /// <summary>
        /// Occurs when a command on the view model has been executed.
        /// </summary>
        event EventHandler<CommandExecutedEventArgs> CommandExecuted;

        /// <summary>
        /// Occurs when the view model is about to be saved.
        /// </summary>
        event EventHandler<SavingEventArgs> Saving;

        /// <summary>
        /// Occurs when the view model is saved successfully.
        /// </summary>
        event EventHandler<EventArgs> Saved;

        /// <summary>
        /// Occurs when the view model is about to be canceled.
        /// </summary>
        event EventHandler<CancelingEventArgs> Canceling;

        /// <summary>
        /// Occurrs when the view model is canceled.
        /// </summary>
        event EventHandler<EventArgs> Canceled;

        /// <summary>
        /// Occurs when the view model is being closed.
        /// </summary>
        event EventHandler<EventArgs> Closing;

        /// <summary>
        /// Occurs when the view model has been closed.
        /// </summary>
        event EventHandler<ViewModelClosedEventArgs> Closed;
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
        Task InitializeViewModel();

        /// <summary>
        /// Validates the specified notify changed properties only.
        /// </summary>
        /// <param name="force">if set to <c>true</c>, a validation is forced (even if the object knows it is already validated).</param>
        /// <param name="notifyChangedPropertiesOnly">if set to <c>true</c> only the properties for which the warnings or errors have been changed
        /// will be updated via <see cref="INotifyPropertyChanged.PropertyChanged"/>; otherwise all the properties that
        /// had warnings or errors but not anymore and properties still containing warnings or errors will be updated.</param>
        /// <returns>
        /// <c>true</c> if validation succeeds; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This method is useful when the view model is initialized before the window, and therefore WPF does not update the errors and warnings.
        /// </remarks>
        bool ValidateViewModel(bool force = false, bool notifyChangedPropertiesOnly = true);

        /// <summary>
        /// Cancels the editing of the data.
        /// </summary>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        Task<bool> CancelViewModel();

        /// <summary>
        /// Cancels the editing of the data, but also closes the view model in the same call.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        Task<bool> CancelAndCloseViewModel();

        /// <summary>
        /// Saves the data.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        Task<bool> SaveViewModel();

        /// <summary>
        /// Saves the data, but also closes the view model in the same call if the save succeeds.
        /// </summary>
        /// <returns>
        /// <c>true</c> if successful; otherwise <c>false</c>.
        /// </returns>
        Task<bool> SaveAndCloseViewModel();

        /// <summary>
        /// Closes this instance. Always called after the <see cref="CancelViewModel"/> of <see cref="SaveViewModel"/> method.
        /// </summary>
        /// <param name="result">The result to pass to the view. This will, for example, be used as <c>DialogResult</c>.</param>
        Task CloseViewModel(bool? result);
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUIVisualizerService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using MVVM;

    /// <summary>
    /// This interface defines a UI controller which can be used to display dialogs
    /// in either modal or modaless form from a ViewModel.
    /// </summary>
    public interface IUIVisualizerService
    {
        #region Methods
        /// <summary>
        /// Registers the specified view model and the window type. This way, Catel knowns what
        /// window to show when a specific view model window is requested.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <param name="windowType">Type of the window.</param>
        /// <param name="throwExceptionIfExists">if set to <c>true</c>, this method will throw an exception when already registered.</param>
        /// <exception cref="ArgumentException">The <paramref name="name" /> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentException">The <paramref name="windowType" /> is not of the right type.</exception>
        void Register(string name, Type windowType, bool throwExceptionIfExists = true);

        /// <summary>
        /// This unregisters the specified view model.
        /// </summary>
        /// <param name="name">Name of the registered window.</param>
        /// <returns>
        /// 	<c>true</c> if the view model is unregistered; otherwise <c>false</c>.
        /// </returns>
        bool Unregister(string name);

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        Task<bool?> Show(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null);

        /// <summary>
        /// Shows a window that is registered with the specified view model in a non-modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// 	<c>true</c> if the popup window is successfully opened; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        Task<bool?> Show(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null);

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="viewModel"/> is <c>null</c>.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="viewModel"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        Task<bool?> ShowDialog(IViewModel viewModel, EventHandler<UICompletedEventArgs> completedProc = null);

        /// <summary>
        /// Shows a window that is registered with the specified view model in a modal state.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data to set as data context. If <c>null</c>, the data context will be untouched.</param>
        /// <param name="completedProc">The callback procedure that will be invoked as soon as the window is closed. This value can be <c>null</c>.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="WindowNotRegisteredException">The <paramref name="name"/> is not registered by the <see cref="Register(string,System.Type,bool)"/> method first.</exception>
        Task<bool?> ShowDialog(string name, object data, EventHandler<UICompletedEventArgs> completedProc = null);
        #endregion

        /// <summary>
        /// Determines whether the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        bool IsRegistered(string name);
    }
}

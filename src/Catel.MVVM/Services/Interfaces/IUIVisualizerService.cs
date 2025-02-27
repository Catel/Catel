﻿namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface defines a UI controller which can be used to display dialogs
    /// in either modal or non-modal form from a ViewModel.
    /// </summary>
    public interface IUIVisualizerService
    {
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
        /// Determines whether the specified name is registered.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified name is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        bool IsRegistered(string name);

        /// <summary>
        /// Shows a window that is registered with the specified view model, respecting the specified context.
        /// </summary>
        /// <param name="context">The context to use to show the window.</param>
        /// <returns>
        /// Nullable boolean representing the dialog result.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="context"/> is <c>null</c> or whitespace.</exception>
        Task<UIVisualizerResult> ShowContextAsync(UIVisualizerContext context);
    }
}

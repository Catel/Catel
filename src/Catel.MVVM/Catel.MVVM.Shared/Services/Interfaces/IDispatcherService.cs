// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        [ObsoleteEx(ReplacementTypeOrMember = "void Invoke(Action action, bool onlyInvokeWhenNoAccess);", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        void Invoke(Action action);

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void Invoke(Action action, bool onlyInvokeWhenNoAccess);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess);
    }
}
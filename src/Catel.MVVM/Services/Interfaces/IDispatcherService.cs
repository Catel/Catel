// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

#if !XAMARIN && !XAMARIN_FORMS
#if UWP
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif
#endif

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public interface IDispatcherService
    {
#if NET || NETCORE || UWP
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        Task InvokeAsync(Action action);

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        Task InvokeAsync(Func<Task> actionAsync);

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with supporting of cancellation token.
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        Task InvokeAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        Task InvokeAsync(Delegate method, params object[] args);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        Task<T> InvokeAsync<T>(Func<T> func);

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation.</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value.</param>
        /// <returns>The task representing the asynchronous operation with the returning value.</returns>
        Task<T> InvokeAsync<T>(Func<Task<T>> funcAsync);

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value with supporting of cancellation token.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation.</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value and supports cancelling.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task representing the asynchronous operation with the returning value.</returns>
        Task<T> InvokeAsync<T>(Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken);
#endif

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        void Invoke(Action action, bool onlyInvokeWhenNoAccess = true);

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true);
    }
}

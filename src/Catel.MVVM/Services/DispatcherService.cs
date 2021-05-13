// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;

#if ANDROID
    using global::Android.App;
    using global::Android.OS;
#elif IOS
    using global::CoreFoundation;
#elif UWP
    using Windows.Threading;
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#elif !XAMARIN_FORMS
    using Catel.Windows.Threading;
    using System.Windows.Threading;
    using DispatcherExtensions = Windows.Threading.DispatcherExtensions;
#else
    using Xamarin.Forms;
#endif

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if !XAMARIN && !XAMARIN_FORMS
        private readonly IDispatcherProviderService _dispatcherProviderService;
#endif

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherService"/> class.
        /// </summary>
        public DispatcherService(IDispatcherProviderService dispatcherProviderService)
        {
            Argument.IsNotNull(nameof(dispatcherProviderService), dispatcherProviderService);

            _dispatcherProviderService = dispatcherProviderService;
        }
#endif

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        protected virtual Dispatcher CurrentDispatcher
        {
            get
            {
                return _dispatcherProviderService.GetApplicationDispatcher() as Dispatcher;
            }
        }
#endif

#if NET || NETCORE || UWP
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task InvokeAsync(Action action)
        {
#if NET || NETCORE
            return CurrentDispatcher.InvokeAsync(action).Task;
#else
            return dispatcher.InvokeAsync(action);
#endif
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task InvokeAsync(Delegate method, params object[] args)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, method, args);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task<T> InvokeAsync<T>(Func<T> func)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, func);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value</param>
        /// <returns>The task representing the asynchronous operation</returns>
        public virtual Task InvokeTaskAsync(Func<Task> actionAsync)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, actionAsync);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with supporting of CancellationToken
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The task representing the asynchronous operation</returns>
        public virtual Task InvokeTaskAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, actionAsync, cancellationToken);
        }
        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value</param>
        /// <returns>The task representing the asynchronous operation with the returning value</returns>
        public virtual Task<T> InvokeTaskAsync<T>(Func<Task<T>> funcAsync)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, funcAsync);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value with supporting of CancellationToken
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value and supports cancelling</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The task representing the asynchronous operation with the returning value</returns>
        public virtual Task<T> InvokeTaskAsync<T>(Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, funcAsync, cancellationToken);
        }
#endif

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public virtual void Invoke(Action action, bool onlyInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            var synchronizationContext = SynchronizationContext.Current;
            if (synchronizationContext is not null)
            {
                synchronizationContext.Post(state => action(), null);
            }
            else
            {
                action();
            }
#elif ANDROID
            _handler.Post(action);
#elif IOS
            DispatchQueue.MainQueue.DispatchSync(() => action());
#else
            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.Invoke(dispatcher, action, onlyInvokeWhenNoAccess);
#endif
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public virtual void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            var synchronizationContext = SynchronizationContext.Current;
            if (synchronizationContext is not null)
            {
                synchronizationContext.Post(state => action(), null);
            }
            else
            {
                action();
            }
#elif ANDROID
            _handler.Post(action);
#elif IOS
            DispatchQueue.MainQueue.DispatchAsync(() => action());
#else
            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.BeginInvoke(dispatcher, action, onlyBeginInvokeWhenNoAccess);
#endif
        }
    }
}

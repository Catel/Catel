// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
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
    using System.Threading;
    using Xamarin.Forms;
#endif

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherService"/> class.
        /// </summary>
        public DispatcherService()
        {
            // Get current dispatcher to make sure we have one
            var currentDispatcher = DispatcherHelper.CurrentDispatcher;
            if (currentDispatcher != null)
            {
                Log.Debug("Successfully Initialized current dispatcher");
            }
            else
            {
                Log.Warning("Failed to retrieve the current dispatcher at this point, will try again later");
            }
        }
#endif

#if ANDROID
        private readonly Handler _handler = new Handler(Looper.MainLooper);
#elif !XAMARIN && !XAMARIN_FORMS
        /// <summary>
        /// Gets the current dispatcher.
        /// <para />
        /// Internally, this property uses the <see cref="DispatcherHelper"/>, but can be overriden if required.
        /// </summary>
        protected virtual Dispatcher CurrentDispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }
#endif

#if NET || NETCORE || UWP
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        public Task InvokeAsync(Action action)
        {
            var dispatcher = CurrentDispatcher;

#if NET || NETCORE
            return dispatcher.InvokeAsync(action).Task;
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
        public Task InvokeAsync(Delegate method, params object[] args)
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
        public Task<T> InvokeAsync<T>(Func<T> func)
        {
            var dispatcher = CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, func);
        }
#endif

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public void Invoke(Action action, bool onlyInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            var synchronizationContext = SynchronizationContext.Current;
            if (synchronizationContext != null)
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
        public void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true)
        {
            Argument.IsNotNull("action", action);
#if XAMARIN_FORMS
            var synchronizationContext = SynchronizationContext.Current;
            if (synchronizationContext != null)
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

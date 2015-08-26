// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Threading
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    using Logging;

#if NETFX_CORE
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#endif

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static partial class DispatcherExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET40
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Action action)
        {
            var tcs = new TaskCompletionSource<bool>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) =>
            {
                if (tcs.Task.IsCanceled)
                {
                    return;
                }

                if (!tcs.TrySetResult(true))
                {
                    Log.Warning("Failed to set the task result to true, task was already completed. Current status is '{0}'", tcs.Task.Status);
                }
            };
            dispatcherOperation.Aborted += (sender, e) =>
            {
                if (tcs.Task.IsCanceled)
                {
                    return;
                }

                if (!tcs.TrySetCanceled())
                {
                    Log.Warning("Failed to set the task as canceled, task was already completed or canceled before");
                }
            };

            return tcs.Task;
        }
#endif

#if NET
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            var tcs = new TaskCompletionSource<bool>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    method.DynamicInvoke(args);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) => tcs.SetResult(true);
            dispatcherOperation.Aborted += (sender, e) => tcs.SetCanceled();

            return tcs.Task;
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            var result = default(T);

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    result = func();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), null);

            dispatcherOperation.Completed += (sender, e) => tcs.SetResult(result);
            dispatcherOperation.Aborted += (sender, e) => tcs.SetCanceled();

            return tcs.Task;
        }
#endif
    }
}

#endif
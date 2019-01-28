// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Threading
{
    using System;
    using System.Threading.Tasks;
    using Logging;

#if UWP
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
    using System.Windows.Threading;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static partial class DispatcherExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET || NETCORE || UWP
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            return InvokeAsync(dispatcher, method, DispatcherPriority.Normal, args);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">The method.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Delegate method, DispatcherPriority priority, params object[] args)
        {
            return RunAsync(dispatcher, () =>
            {
                method.DynamicInvoke(args);
            }, priority);
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
            return InvokeAsync<T>(dispatcher, func, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="func">The function.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The task representing the action.</returns>
        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<T> func, DispatcherPriority priority)
        {
            return RunWithResultAsync(dispatcher, () =>
            {
                return func();
            }, priority);
        }

        private static Task RunAsync(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
#if UWP
            var task = dispatcher.RunAsync(priority.ToCoreDispatcherPriority(), () =>
            {
                action();
            });

            return task.AsTask();
#else
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
            }), priority, null);

            dispatcherOperation.Completed += (sender, e) => SetResult(tcs, true);
            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            return tcs.Task;
#endif
        }

        private static async Task<T> RunWithResultAsync<T>(this Dispatcher dispatcher, Func<T> function, DispatcherPriority priority)
        {
            var result = default(T);

#if UWP
            await dispatcher.RunAsync(priority.ToCoreDispatcherPriority(), () =>
            {
                result = function();
            });
#else
            var tcs = new TaskCompletionSource<T>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    result = function();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), priority, null);

            dispatcherOperation.Completed += (sender, e) => SetResult(tcs, result);
            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            await tcs.Task;
#endif

            return result;
        }

        private static bool SetResult<T>(TaskCompletionSource<T> tcs, T result)
        {
            if (tcs.Task.IsCanceled)
            {
                return false;
            }

            if (!tcs.TrySetResult(result))
            {
                Log.Warning("Failed to set the task result to '{0}', task was already completed. Current status is '{1}'", result, tcs.Task.Status);
                return false;
            }

            return true;
        }

        private static bool SetCanceled<T>(TaskCompletionSource<T> tcs)
        {
            if (tcs.Task.IsCanceled)
            {
                return true;
            }

            if (!tcs.TrySetCanceled())
            {
                Log.Warning("Failed to set the task as canceled, task was already completed or canceled before");
                return false;
            }

            return true;
        }
#endif
    }
}

#endif

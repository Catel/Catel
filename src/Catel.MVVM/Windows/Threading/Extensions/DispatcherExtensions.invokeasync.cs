// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using Catel.Threading;

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
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="actionAsync">The asynchronous operation without returning a value.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Func<Task> actionAsync)
        {
            return RunAsync(dispatcher, actionAsync, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with supporting of cancellation token.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="actionAsync">The cancellation token.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The task representing the asynchronous operation.</returns>
        public static Task InvokeAsync(this Dispatcher dispatcher, Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken)
        {
            return RunAsync(dispatcher, actionAsync, cancellationToken, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="funcAsync">The asynchronous operation which returns a value.</param>
        /// <returns>The task representing the asynchronous operation with the returning value.</returns>
        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<Task<T>> funcAsync)
        {
            return InvokeAsync(dispatcher, funcAsync, DispatcherPriority.Normal);
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value with supporting of cancellation token.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation.</typeparam>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="funcAsync">The asynchronous operation which returns a value and supports cancelling.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>The task representing the asynchronous operation with the returning value.</returns>
        public static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken)
        {
            return InvokeAsync(dispatcher, funcAsync, cancellationToken, DispatcherPriority.Normal);
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

        private static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<Task<T>> funcAsync, DispatcherPriority priority)
        {
            return RunWithResultAsync(dispatcher, funcAsync, priority);
        }

        private static Task<T> InvokeAsync<T>(this Dispatcher dispatcher, Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken, DispatcherPriority priority)
        {
            return RunWithResultAsync(dispatcher, funcAsync, cancellationToken, priority);
        }

        private static Task RunAsync(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
#if UWP
            throw Log.ErrorAndCreateException<PlatformNotSupportedException>();
#endif

            // Only invoke if we really have to
            if (dispatcher.CheckAccess())
            {
                action();
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    action();

                    SetResult(tcs, true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), priority, null);

            // IMPORTANT: don't handle 'dispatcherOperation.Completed' event.
            // We should only signal to awaiter when the operation is really done

            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            return tcs.Task;
        }

        private static async Task RunAsync(this Dispatcher dispatcher, Func<Task> actionAsync, DispatcherPriority priority)
        {
            await dispatcher.RunWithResultAsync(async token =>
            {
                await actionAsync();
                return (object)null;
            }, CancellationToken.None, priority);
        }

        private static async Task RunAsync(this Dispatcher dispatcher, Func<CancellationToken, Task> actionAsync,
            CancellationToken cancellationToken, DispatcherPriority priority)
        {
            await dispatcher.RunWithResultAsync(async token =>
            {
                await actionAsync(cancellationToken);
                return (object)null;
            }, cancellationToken, priority);
        }

        private static async Task<T> RunWithResultAsync<T>(this Dispatcher dispatcher, Func<T> function, DispatcherPriority priority)
        {
            var result = default(T);

#if UWP
            throw Log.ErrorAndCreateException<PlatformNotSupportedException>();
#endif

            // Only invoke if we really have to
            if (dispatcher.CheckAccess())
            {
                result = function();
                return result;
            }

            var tcs = new TaskCompletionSource<T>();

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    result = function();

                    SetResult(tcs, result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            }), priority, null);

            // IMPORTANT: don't handle 'dispatcherOperation.Completed' event.
            // We should only signal to awaiter when the operation is really done

            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            await tcs.Task;

            return result;
        }

        private static async Task<T> RunWithResultAsync<T>(this Dispatcher dispatcher, Func<Task<T>> functionAsync, DispatcherPriority priority)
        {
            return await dispatcher.RunWithResultAsync(token => functionAsync(), CancellationToken.None, priority);
        }

        private static async Task<T> RunWithResultAsync<T>(this Dispatcher dispatcher, Func<CancellationToken, Task<T>> functionAsync,
            CancellationToken cancellationToken, DispatcherPriority priority)
        {
            var tcs = new TaskCompletionSource<T>();

#if UWP
            throw Log.ErrorAndCreateException<PlatformNotSupportedException>();
#endif

            // Only invoke if we really have to
            if (dispatcher.CheckAccess())
            {
                var result = await functionAsync(cancellationToken);
                return result;
            }

            var dispatcherOperation = dispatcher.BeginInvoke(new Action(async () =>
            {
                try
                {
                    var task = functionAsync(cancellationToken);

                    await task;

                    if (task.IsFaulted)
                    {
                        tcs.TrySetException(task.Exception ?? new Exception("Unknown error"));
                        return;
                    }

                    if (task.IsCanceled)
                    {
                        SetCanceled(tcs);
                        return;
                    }

                    SetResult(tcs, task.Result);
                }
                catch (Exception ex)
                {
                    // NOTE: in theory, it could have been already set before
                    tcs.TrySetException(ex);
                }
            }), priority, null);

            // IMPORTANT: don't handle 'dispatcherOperation.Completed' event.
            // We should only signal to awaiter when the operation is really done

            dispatcherOperation.Aborted += (sender, e) => SetCanceled(tcs);

            return await tcs.Task;
        }

        private static bool SetResult<T>(TaskCompletionSource<T> tcs, T result)
        {
            if (tcs.Task.IsCanceled)
            {
                return false;
            }

            if (!tcs.TrySetResult(result))
            {
                Log.Warning($"Failed to set the task result to '{result.ToString()}', task was already completed. Current status is '{Enum<TaskStatus>.ToString(tcs.Task.Status)}'");
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

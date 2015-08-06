// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Helper class for tasks.
    /// </summary>
    /// <typeparam name="T">The type of the tasks.</typeparam>
    public static class TaskHelper<T>
    {
        private static readonly Dictionary<T, Task<T>> _fromResultCache = new Dictionary<T, Task<T>>();

        private static readonly Task<T> _defaultValue = TaskShim.FromResult(default(T));

        private static readonly Task<T> _canceled = CreateCanceledTask();

        private static Task<T> CreateCanceledTask()
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetCanceled();
            return tcs.Task;
        }

        /// <summary>
        /// A <see cref="Task"/> return the default value of <typeparamref name="T"/>.
        /// </summary>
        public static Task<T> DefaultValue
        {
            get { return _defaultValue; }
        }

        /// <summary>
        /// A <see cref="Task"/> representing a canceled task.
        /// </summary>
        public static Task<T> Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// Creates a <see cref="Task"/> using the <c>Task.FromResult</c> method, but caches the result for the next call.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> FromResult(T value)
        {
            Task<T> task;

            if (!_fromResultCache.ContainsKey(value))
            {
                task = TaskShim.FromResult(value);
                _fromResultCache[value] = task;
            }
            else
            {
                task = _fromResultCache[value];
            }

            return task;
        }
    }

    /// <summary>
    /// Helper class for tasks.
    /// </summary>
    public static class TaskHelper
    {
        private static readonly Task _completed = TaskHelper<bool>.FromResult(true);
        private static readonly Task _canceled = TaskHelper<bool>.Canceled;

        /// <summary>
        /// A <see cref="Task"/> that has been completed.
        /// </summary>
        public static Task Completed
        {
            get { return _completed; }
        }

        /// <summary>
        /// A <see cref="Task"/> that has been canceled.
        /// </summary>
        public static Task Canceled
        {
            get { return _canceled; }
        }

        /// <summary>
        /// The default configure await value.
        /// </summary>
        public const bool DefaultConfigureAwaitValue = false;

        /// <summary>
        /// Runs the specified action using Task.Run if available.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        /// <summary>
        /// Runs the specified action using Task.Run if available.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
            return Run(action, cancellationToken, DefaultConfigureAwaitValue);
        }

        /// <summary>
        /// Runs the specified action using Task.Run if available.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="configureAwait">The value to be passed into ConfigureAwait.</param>
        /// <returns>Task.</returns>
        public static async Task Run(Action action, CancellationToken cancellationToken, bool configureAwait)
        {
            var task = TaskShim.Run(action, cancellationToken);

            await task.ConfigureAwait(configureAwait);
        }

        /// <summary>
        /// Runs the specified function using Task.Run if available.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> func)
        {
            return Run(func, CancellationToken.None);
        }

        /// <summary>
        /// Runs the specified function using Task.Run if available.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
            return Run(func, cancellationToken, DefaultConfigureAwaitValue);
        }

        /// <summary>
        /// Runs the specified function using Task.Run if available.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="configureAwait">The value to be passed into ConfigureAwait.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static async Task<TResult> Run<TResult>(Func<TResult> func, CancellationToken cancellationToken, bool configureAwait)
        {
            var task = TaskShim.Run(func, cancellationToken);

            return await task.ConfigureAwait(configureAwait);
        }

        /// <summary>
        /// Runs all the specified actions in separate threads and waits for the to complete.
        /// </summary>
        /// <param name="actions">The actions to spawn in separate threads.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
        public static void RunAndWait(params Action[] actions)
        {
            Argument.IsNotNull("actions", actions);

#if !SILVERLIGHT && !PCL
            Parallel.Invoke(actions);
#elif PCL
            var list = actions.ToList();
            var tasks = new List<Task>();
            for (int i = 0; i < list.Count; i++)
            {
                var task = new Task(list[i]);
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());
#else
            var list = actions.ToList();
            var handles = new ManualResetEvent[list.Count];
            for (var i = 0; i < list.Count; i++)
            {
                handles[i] = new ManualResetEvent(false);

                var currentAction = list[i];
                var currentHandle = handles[i];

                Action wrappedAction = () =>
                {
                    try
                    {
                        currentAction();
                    }
                    finally
                    {
                        currentHandle.Set();
                    }
                };

                ThreadPool.QueueUserWorkItem(x => wrappedAction());
            }

            WaitHandle.WaitAll(handles);
#endif
        }

        /// <summary>
        /// Runs all the specified actions in separate threads and waits for the to complete.
        /// <para />
        /// The waiting for all threads is also done in a separate thread which makes this method asynchronous.
        /// </summary>
        /// <param name="actions">The actions to spawn in separate threads.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
        public static Task RunAndWaitAsync(params Action[] actions)
        {
            return Run(() => RunAndWait(actions));
        }

        /// <summary>
        /// Runs all the specified actions in separate threads and waits for the to complete.
        /// <para />
        /// The waiting for all threads is also done in a separate thread which makes this method asynchronous.
        /// </summary>
        /// <param name="actions">The actions to spawn in separate threads.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
        public static Task RunAndWaitAsync(params Func<Task>[] actions)
        {
            Argument.IsNotNull("actions", actions);

            if (actions.Length == 0)
            {
                return TaskHelper.Completed;
            }

            var tasks = (from action in actions
                         select action()).ToArray();

            return TaskShim.WhenAll(tasks);
        }
    }
}
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
    public static class TaskHelper
    {
        /// <summary>
        /// Runs the specified action using Task.Run if available. If <c>Task.Run</c> is not available on the target platform,
        /// it will use the right <c>Task.Factory.StartNew</c> usage.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Task.</returns>
        public static Task Run(Action action)
        {
            return Run(action, CancellationToken.None);
        }

        /// <summary>
        /// Runs the specified action using Task.Run if available. If <c>Task.Run</c> is not available on the target platform,
        /// it will use the right <c>Task.Factory.StartNew</c> usage.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
#if NET40 || SL5
            return Task.Factory.StartNew(action, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
#else
            return Task.Run(action, cancellationToken);
#endif
        }

        /// <summary>
        /// Runs the specified function using Task.Run if available. If <c>Task.Run</c> is not available on the target platform,
        /// it will use the right <c>Task.Factory.StartNew</c> usage.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func">The function.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> func)
        {
            return Run(func, CancellationToken.None);
        }

        /// <summary>
        /// Runs the specified function using Task.Run if available. If <c>Task.Run</c> is not available on the target platform,
        /// it will use the right <c>Task.Factory.StartNew</c> usage.
        /// </summary>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<TResult> Run<TResult>(Func<TResult> func, CancellationToken cancellationToken)
        {
#if NET40 || SL5
            return Task.Factory.StartNew(func, cancellationToken, TaskCreationOptions.None, TaskScheduler.Default);
#else
            return Task.Run(func, cancellationToken);
#endif
        }

        /// <summary>
        /// Runs all the specified actions in separate threads and waits for the to complete.
        /// </summary>
        /// <param name="actions">The actions to spawn in separate threads.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
        public static void RunAndWait(params Action[] actions)
        {
            Argument.IsNotNull("actions", actions);

            var list = actions.ToList();

#if !SILVERLIGHT && !PCL
            Parallel.Invoke(actions);
#elif PCL
            var tasks = new List<Task>();
            for (int i = 0; i < list.Count; i++)
            {
                var task = new Task(list[i]);
                tasks.Add(task);
                task.Start();
            }

            Task.WaitAll(tasks.ToArray());
#else
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
            return TaskHelper.Run(() => TaskHelper.RunAndWait(actions));
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

            var finalActions = new List<Action>();

            foreach (var action in actions)
            {
                var innerAction = action;
                finalActions.Add(() => innerAction().Wait());
            }

            return RunAndWaitAsync(finalActions.ToArray());
        }
    }
}
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

#if SILVERLIGHT
    using System.Threading;
#endif

    /// <summary>
    /// Helper class for tasks.
    /// </summary>
    public static class TaskHelper
    {
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
            for (int i = 0; i < list.Count; i++)
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
        public static async Task RunAndWaitAsync(params Action[] actions)
        {
            await Task.Factory.StartNew(() => TaskHelper.RunAndWait(actions));
        }

        /// <summary>
        /// Runs all the specified actions in separate threads and waits for the to complete.
        /// <para />
        /// The waiting for all threads is also done in a separate thread which makes this method asynchronous.
        /// </summary>
        /// <param name="actions">The actions to spawn in separate threads.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="actions"/> is <c>null</c>.</exception>
        public static async Task RunAndWaitAsync(params Func<Task>[] actions)
        {
            Argument.IsNotNull("actions", actions);

            var finalActions = new List<Action>();

            foreach (var action in actions)
            {
                var innerAction = action;
                finalActions.Add(() => innerAction().Wait());
            }

            await Task.Factory.StartNew(() => TaskHelper.RunAndWait(finalActions.ToArray()));
        }
    }
}
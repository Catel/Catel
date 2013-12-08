// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

#if !SILVERLIGHT
    using System.Threading.Tasks;
#else
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

#if !SILVERLIGHT
            Parallel.Invoke(actions);

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
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParallelHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using Catel.Threading;

    /// <summary>
    /// Helper class to execute groups of methods in parallel.
    /// </summary>
    public static class ParallelHelper
    {
        /// <summary>
        /// The log
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Executes all the items in the collection in parallel batches.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items.</param>
        /// <param name="actionToInvoke">The action to invoke per item.</param>
        /// <param name="itemsPerBatch">The items per batch.</param>
        /// <param name="taskName">Name of the task, can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="items" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="actionToInvoke" /> is <c>null</c>.</exception>
        public static void ExecuteInParallel<T>(List<T> items, Action<T> actionToInvoke, int itemsPerBatch = 1000, string taskName = null)
        {
            Argument.IsNotNull("items", items);
            Argument.IsNotNull("actionToInvoke", actionToInvoke);

            taskName = ObjectToStringHelper.ToString(taskName);

            Log.Debug("[{0}] Executing '{1}' actions in parallel in batches of '{2}' items per batch", taskName, items.Count, itemsPerBatch);

            var batches = new List<List<T>>();
            if (itemsPerBatch > 0)
            {
                var typeCount = items.Count;
                for (int i = 0; i < typeCount; i = i + itemsPerBatch)
                {
                    int itemsToSkip = i;
                    int itemsToTake = itemsPerBatch;
                    if (itemsToTake >= typeCount)
                    {
                        itemsToTake = typeCount - i;
                    }

                    batches.Add(items.Skip(itemsToSkip).Take(itemsToTake).ToList());
                }
            }
            else
            {
                batches.Add(new List<T>(items));
            }

            if (batches.Count == 1)
            {
                ExecuteBatch(taskName, "single", batches[0], actionToInvoke);
            }
            else
            {
                var actions = new List<Action>();
                for (int i = 0; i < batches.Count; i++)
                {
                    var batch = batches[i];
                    ExecuteBatch(taskName, i.ToString(), batch, actionToInvoke);
                }

                TaskHelper.RunAndWait(actions.ToArray());
            }

            Log.Debug("[{0}] Executed '{1}' actions in parallel in '{2}' batches of '{3}' items per batch", taskName, items.Count, batches.Count, itemsPerBatch);
        }

        /// <summary>
        /// Executes the batch for the specific set of items.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="taskName">Name of the task.</param>
        /// <param name="batchName">Name of the type group.</param>
        /// <param name="items">The items.</param>
        /// <param name="actionToInvoke">The action to invoke.</param>
        private static void ExecuteBatch<T>(string taskName, string batchName, List<T> items, Action<T> actionToInvoke)
        {
            string finalName = string.Format("[{0} | {1}]", taskName, batchName);

            Log.Debug("{0} Starting batch for '{1}' items", finalName, items.Count);

            foreach (var item in items)
            {
                try
                {
                    actionToInvoke(item);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "{0} An error occurred while executing a batch action", finalName);
                }
            }

            Log.Debug("{0} Finished batch for '{1}' items", finalName, items.Count);
        }
    }
}
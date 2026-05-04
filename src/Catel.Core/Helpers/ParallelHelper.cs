namespace Catel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Catel.Logging;
using Microsoft.Extensions.Logging;

/// <summary>
/// Helper class to execute groups of methods in parallel.
/// </summary>
public static class ParallelHelper
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ParallelHelper));

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
    public static void ExecuteInParallel<T>(List<T> items, Action<T> actionToInvoke, int itemsPerBatch = 1000, string? taskName = null)
    {
        taskName = ObjectToStringHelper.ToString(taskName);

        Logger.LogDebug($"[{taskName}] Executing '{items.Count}' actions in parallel in batches of '{itemsPerBatch}' items per batch");

        var batches = new List<List<T>>();
        if (itemsPerBatch > 0)
        {
            var typeCount = items.Count;
            for (var i = 0; i < typeCount; i = i + itemsPerBatch)
            {
                var itemsToSkip = i;
                var itemsToTake = itemsPerBatch;
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
            for (var i = 0; i < batches.Count; i++)
            {
                var innerI = i;
                var batch = batches[i];

                actions.Add(() => ExecuteBatch(taskName, innerI.ToString(), batch, actionToInvoke));
            }

            Parallel.Invoke(actions.ToArray());
        }

        Logger.LogDebug($"[{taskName}] Executed '{items.Count}' actions in parallel in '{batches.Count}' batches of '{itemsPerBatch}' items per batch");
    }

    /// <summary>
    /// Executes the batch for the specific set of items.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="taskName">Name of the task.</param>
    /// <param name="batchName">Name of the type group.</param>
    /// <param name="items">The items.</param>
    /// <param name="actionToInvoke">The action to invoke.</param>
    private static void ExecuteBatch<T>(string? taskName, string batchName, List<T> items, Action<T> actionToInvoke)
    {
        var finalName = string.Format("[{0} | {1}]", taskName, batchName);

        Logger.LogDebug($"{finalName} Starting batch for '{items.Count}' items");

        foreach (var item in items)
        {
            try
            {
                actionToInvoke(item);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "{BatchName} An error occurred while executing a batch action", finalName);
            }
        }

        Logger.LogDebug($"{finalName} Finished batch for '{items.Count}' items");
    }

    /// <summary>
    /// Runs async tasks in parallel in pre-defined batches.
    /// </summary>
    /// <param name="tasks">The task list.</param>
    /// <param name="taskName">The task name.</param>
    /// <param name="batchSize">The batch size.</param>
    public static async Task ExecuteInParallelAsync(List<Func<Task>> tasks, int batchSize = 1000, string? taskName = null)
    {
        taskName = ObjectToStringHelper.ToString(taskName);

        Logger.LogDebug($"[{taskName}] Executing '{tasks.Count}' async tasks in parallel in batches of size '{batchSize}'");

        for (var i = 0; i < tasks.Count; i = i + batchSize)
        {
            var batchTasks = tasks.Skip(i).Take(Math.Min(batchSize, tasks.Count - i)).ToArray();

            var runningBatchTasks = (from batchTask in batchTasks
                                     select batchTask()).ToArray();

            await Task.WhenAll(runningBatchTasks);
        }
    }
}

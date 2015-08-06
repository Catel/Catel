// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskShim.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET40 || SL5
#define USE_TASKEX
#endif

namespace Catel.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

#if USE_TASKEX
    using Microsoft.Runtime.CompilerServices;
#else
    using System.Runtime.CompilerServices;
#endif

    /// <summary>
    /// Task wrapper so it works on all platforms.
    /// </summary>
    /// <remarks>This code originally comes from https://github.com/StephenCleary/AsyncEx/blob/77b9711c2c5fd4ca28b220ce4c93d209eeca2b4a/Source/Unit%20Tests/Tests%20(NET40)/Internal/TaskShim.cs.</remarks>
    public static class TaskShim
    {
        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task</param>
        /// <returns>A task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="millisecondsDelay" /> is less than -1.</exception>
        public static Task Delay(int millisecondsDelay)
        {
#if USE_TASKEX
            return TaskEx.Delay(millisecondsDelay);
#else
            return Task.Delay(millisecondsDelay);
#endif
        }

        /// <summary>
        /// Creates a task that will complete after a time delay.
        /// </summary>
        /// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned task</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the time delay</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="millisecondsDelay" /> is less than -1.</exception>
        public static Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Delay(millisecondsDelay, cancellationToken);
#else
            return Task.Delay(millisecondsDelay, cancellationToken);
#endif
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <returns>The timed Task.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.</exception>
        public static Task Delay(TimeSpan dueTime)
        {
#if USE_TASKEX
            return TaskEx.Delay(dueTime);
#else
            return Task.Delay(dueTime);
#endif
        }

        /// <summary>
        /// Starts a Task that will complete after the specified due time.
        /// </summary>
        /// <param name="dueTime">The delay before the returned task completes.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The timed Task.</returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">The <paramref name="dueTime" /> argument must be non-negative or -1 and less than or equal to Int32.MaxValue.</exception>
        public static Task Delay(TimeSpan dueTime, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Delay(dueTime, cancellationToken);
#else
            return Task.Delay(dueTime, cancellationToken);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> parameter was null.</exception>
        public static Task Run(Action action)
        {
#if USE_TASKEX
            return TaskEx.Run(action);
#else
            return Task.Run(action);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a task handle for that work.
        /// </summary>
        /// <param name="action">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents the work queued to execute in the ThreadPool.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="action" /> parameter was null.</exception>
        public static Task Run(Action action, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Run(action, cancellationToken);
#else
            return Task.Run(action);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a proxy for the  task returned by <paramref name="function" />.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task Run(Func<Task> function)
        {
#if USE_TASKEX
            return TaskEx.Run(function);
#else
            return Task.Run(function);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a proxy for the  task returned by <paramref name="function" />.
        /// </summary>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A task that represents a proxy for the task returned by <paramref name="function" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task Run(Func<Task> function, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Run(function, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a Task(TResult) handle for that work.
        /// </summary>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <returns>A Task(TResult) that represents the work queued to execute in the ThreadPool.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function)
        {
#if USE_TASKEX
            return TaskEx.Run(function);
#else
            return Task.Run(function);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a Task(TResult) handle for that work.
        /// </summary>
        /// <typeparam name="TResult">The result type of the task.</typeparam>
        /// <param name="function">The work to execute asynchronously.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task(TResult) that represents the work queued to execute in the ThreadPool.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<TResult> function, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Run(function, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a proxy for the  Task(TResult) returned by <paramref name="function" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <param name="function">The work to execute asynchronously</param>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by <paramref name="function" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function)
        {
#if USE_TASKEX
            return TaskEx.Run(function);
#else
            return Task.Run(function);
#endif
        }

        /// <summary>
        /// Queues the specified work to run on the ThreadPool and returns a proxy for the  Task(TResult) returned by <paramref name="function" />.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the proxy task.</typeparam>
        /// <param name="function">The work to execute asynchronously</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task(TResult) that represents a proxy for the Task(TResult) returned by <paramref name="function" />.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="function" /> parameter was null.</exception>
        public static Task<TResult> Run<TResult>(Func<Task<TResult>> function, CancellationToken cancellationToken)
        {
#if USE_TASKEX
            return TaskEx.Run(function, cancellationToken);
#else
            return Task.Run(function, cancellationToken);
#endif
        }

        /// <summary>
        /// Creates a <see cref="T:System.Threading.Tasks.Task`1" /> that's completed successfully with the specified result.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the task.</typeparam>
        /// <param name="result">The result to store into the completed task.</param>
        /// <returns>The successfully completed task.</returns>
        public static Task<TResult> FromResult<TResult>(TResult result)
        {
#if USE_TASKEX
            return TaskEx.FromResult(result);
#else
            return Task.FromResult(result);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult">The type of the completed task.</typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> collection contained a null task.</exception>
        public static Task<TResult[]> WhenAll<TResult>(IEnumerable<Task<TResult>> tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult">The type of the completed task.</typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task.</exception>
        public static Task<TResult[]> WhenAll<TResult>(params Task<TResult>[] tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> collection contained a null task.</exception>
        public static Task WhenAll(IEnumerable<Task> tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when all of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of all of the supplied tasks.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task.</exception>
        public static Task WhenAll(params Task[] tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAll(tasks);
#else
            return Task.WhenAll(tasks);
#endif
        }

        /// <summary>
        /// Creates an awaitable task that asynchronously yields back to the current context when awaited.
        /// </summary>
        /// <returns>A context that, when awaited, will asynchronously transition back into the current context at the time of the await. If the current <see cref="T:System.Threading.SynchronizationContext" /> is non-null, it is treated as the current context. Otherwise, the task scheduler that is associated with the currently executing task is treated as the current context.</returns>
        public static YieldAwaitable Yield()
        {
#if USE_TASKEX
            return TaskEx.Yield();
#else
            return Task.Yield();
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return task's Result is the task that completed.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task, or was empty.</exception>
        public static Task<Task> WhenAny(params Task[] tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return task's Result is the task that completed.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task, or was empty.</exception>
        public static Task<Task> WhenAny(IEnumerable<Task> tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult">The type of the completed task.</typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return task's Result is the task that completed.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task, or was empty.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(params Task<TResult>[] tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }

        /// <summary>
        /// Creates a task that will complete when any of the supplied tasks have completed.
        /// </summary>
        /// <typeparam name="TResult">The type of the completed task.</typeparam>
        /// <param name="tasks">The tasks to wait on for completion.</param>
        /// <returns>A task that represents the completion of one of the supplied tasks.  The return task's Result is the task that completed.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="tasks" /> argument was null.</exception>
        /// <exception cref="T:System.ArgumentException">The <paramref name="tasks" /> array contained a null task, or was empty.</exception>
        public static Task<Task<TResult>> WhenAny<TResult>(IEnumerable<Task<TResult>> tasks)
        {
#if USE_TASKEX
            return TaskEx.WhenAny(tasks);
#else
            return Task.WhenAny(tasks);
#endif
        }
    }
}
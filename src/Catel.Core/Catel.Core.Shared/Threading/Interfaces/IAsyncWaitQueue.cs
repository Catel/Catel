// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAsyncWaitQueue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Threading
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// A collection of cancelable <see cref="TaskCompletionSource{TResult}"/> instances. Implementations must be threadsafe <b>and</b> must work correctly if the caller is holding a lock.
    /// </summary>
    /// <typeparam name="T">The type of the results. If this isn't needed, use <see cref="object"/>.</typeparam>
    /// <remarks>
    /// This code originally comes from AsyncEx: https://github.com/StephenCleary/AsyncEx
    /// </remarks>
    public interface IAsyncWaitQueue<T>
    {
        /// <summary>
        /// Gets whether the queue is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Creates a new entry and queues it to this wait queue. The returned task must support both synchronous and asynchronous waits.
        /// </summary>
        /// <returns>The queued task.</returns>
        Task<T> Enqueue();

        /// <summary>
        /// Removes a single entry in the wait queue. Returns a disposable that completes that entry.
        /// </summary>
        /// <param name="result">The result used to complete the wait queue entry. If this isn't needed, use <c>default(T)</c>.</param>
        IDisposable Dequeue(T result = default(T));

        /// <summary>
        /// Removes all entries in the wait queue. Returns a disposable that completes all entries.
        /// </summary>
        /// <param name="result">The result used to complete the wait queue entries. If this isn't needed, use <c>default(T)</c>.</param>
        IDisposable DequeueAll(T result = default(T));

        /// <summary>
        /// Attempts to remove an entry from the wait queue. Returns a disposable that cancels the entry.
        /// </summary>
        /// <param name="task">The task to cancel.</param>
        /// <returns>A value indicating whether the entry was found and canceled.</returns>
        IDisposable TryCancel(Task task);

        /// <summary>
        /// Removes all entries from the wait queue. Returns a disposable that cancels all entries.
        /// </summary>
        IDisposable CancelAll();
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AwaitableDisposable.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Threading
{
    using System;
    using System.Threading.Tasks;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An awaitable wrapper around a task whose result is disposable. The wrapper is not disposable, so this prevents usage errors like "using (MyAsync())" when the appropriate usage should be "using (await MyAsync())".
    /// </summary>
    /// <typeparam name="T">The type of the result of the underlying task.</typeparam>
    /// <remarks>
    /// This code originally comes from AsyncEx: https://github.com/StephenCleary/AsyncEx
    /// </remarks>
    public struct AwaitableDisposable<T>
        where T : IDisposable
    {
        /// <summary>
        /// The underlying task.
        /// </summary>
        private readonly Task<T> _task;

        /// <summary>
        /// Initializes a new awaitable wrapper around the specified task.
        /// </summary>
        /// <param name="task">The underlying task to wrap.</param>
        public AwaitableDisposable(Task<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// Returns the underlying task.
        /// </summary>
#pragma warning disable UseAsyncSuffix // Use Async suffix
#pragma warning disable CL0002
        public Task<T> AsTask()
#pragma warning restore CL0002
#pragma warning restore UseAsyncSuffix // Use Async suffix
        {
            return _task;
        }

        /// <summary>
        /// Implicit conversion to the underlying task.
        /// </summary>
        /// <param name="source">The awaitable wrapper.</param>
#pragma warning disable UseAsyncSuffix // Use Async suffix
#pragma warning disable CL0002
        public static implicit operator Task<T>(AwaitableDisposable<T> source)
#pragma warning restore CL0002
#pragma warning restore UseAsyncSuffix // Use Async suffix
        {
            return source.AsTask();
        }

        /// <summary>
        /// Infrastructure. Returns the task awaiter for the underlying task.
        /// </summary>
        public TaskAwaiter<T> GetAwaiter()
        {
            return _task.GetAwaiter();
        }

        /// <summary>
        /// Infrastructure. Returns a configured task awaiter for the underlying task.
        /// </summary>
        /// <param name="continueOnCapturedContext">Whether to attempt to marshal the continuation back to the captured context.</param>
        public ConfiguredTaskAwaitable<T> ConfigureAwait(bool continueOnCapturedContext)
        {
            return _task.ConfigureAwait(continueOnCapturedContext);
        }
    }
}

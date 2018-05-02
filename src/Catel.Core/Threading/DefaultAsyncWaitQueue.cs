// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultAsyncWaitQueue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Threading
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The default wait queue implementation, which uses a double-ended queue.
    /// </summary>
    /// <typeparam name="T">The type of the results. If this isn't needed, use <see cref="object"/>.</typeparam>
    /// <remarks>
    /// This code originally comes from AsyncEx: https://github.com/StephenCleary/AsyncEx
    /// </remarks>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof (DefaultAsyncWaitQueue<>.DebugView))]
    public sealed class DefaultAsyncWaitQueue<T> : IAsyncWaitQueue<T>
    {
        private readonly Dequeue<TaskCompletionSource<T>> _queue = new Dequeue<TaskCompletionSource<T>>();

        private int Count
        {
            get
            {
                lock (_queue)
                {
                    return _queue.Count;
                }
            }
        }

        bool IAsyncWaitQueue<T>.IsEmpty
        {
            get { return Count == 0; }
        }

        Task<T> IAsyncWaitQueue<T>.EnqueueAsync()
        {
            var tcs = new TaskCompletionSource<T>();

            lock (_queue)
            {
                _queue.AddToBack(tcs);
            }

            return tcs.Task;
        }

        IDisposable IAsyncWaitQueue<T>.Dequeue(T result)
        {
            TaskCompletionSource<T> tcs;

            lock (_queue)
            {
                tcs = _queue.RemoveFromFront();
            }

            return new CompleteDisposable(result, tcs);
        }

        IDisposable IAsyncWaitQueue<T>.DequeueAll(T result)
        {
            TaskCompletionSource<T>[] taskCompletionSources;

            lock (_queue)
            {
                taskCompletionSources = _queue.ToArray();
                _queue.Clear();
            }

            return new CompleteDisposable(result, taskCompletionSources);
        }

        IDisposable IAsyncWaitQueue<T>.CancelAll()
        {
            TaskCompletionSource<T>[] taskCompletionSources;

            lock (_queue)
            {
                taskCompletionSources = _queue.ToArray();
                _queue.Clear();
            }

            return new CancelDisposable(taskCompletionSources);
        }

        IDisposable IAsyncWaitQueue<T>.TryCancel(Task task)
        {
            TaskCompletionSource<T> tcs = null;

            lock (_queue)
            {
                for (int i = 0; i != _queue.Count; ++i)
                {
                    if (_queue[i].Task == task)
                    {
                        tcs = _queue[i];
                        _queue.RemoveAt(i);
                        break;
                    }
                }
            }

            if (tcs == null)
            {
                return new CancelDisposable();
            }

            return new CancelDisposable(tcs);
        }

        private sealed class CancelDisposable : IDisposable
        {
            private readonly TaskCompletionSource<T>[] _taskCompletionSources;

            public CancelDisposable(params TaskCompletionSource<T>[] taskCompletionSources)
            {
                _taskCompletionSources = taskCompletionSources;
            }

            public void Dispose()
            {
                foreach (var cts in _taskCompletionSources)
                {
                    cts.TrySetCanceled();
                }
            }
        }

        private sealed class CompleteDisposable : IDisposable
        {
            private readonly T _result;
            private readonly TaskCompletionSource<T>[] _taskCompletionSources;

            public CompleteDisposable(T result, params TaskCompletionSource<T>[] taskCompletionSources)
            {
                _result = result;
                _taskCompletionSources = taskCompletionSources;
            }

            public void Dispose()
            {
                foreach (var cts in _taskCompletionSources)
                {
                    cts.TrySetResult(_result);
                }
            }
        }

        [DebuggerNonUserCode]
        internal sealed class DebugView
        {
            private readonly DefaultAsyncWaitQueue<T> _queue;

            public DebugView(DefaultAsyncWaitQueue<T> queue)
            {
                _queue = queue;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Task<T>[] Tasks
            {
                get { return _queue._queue.Select(x => x.Task).ToArray(); }
            }
        }
    }
}
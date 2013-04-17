// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizationContext.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Threading
{
    using System;
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// Provides a syncronization context to acquire or release exclusive lock of an object and execute thread safe code.
    /// </summary>
    /// <remarks>
    /// Be aware when you use this class. If it is improperly used could provoke dead locks.
    /// </remarks>
    public class SynchronizationContext
    {
        #region Fields

        /// <summary>
        /// The sync obj.
        /// </summary>
        private readonly object _syncObj = new object();

        private int _calls = 0;
        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether is the lock is acquired.
        /// </summary>
        public bool IsLockAcquired { get; private set; }
        #endregion

        #region Methods

        /// <summary>
        /// This method execute the <paramref name="code"/> into a exclusive lock.
        /// </summary>
        /// <remarks>
        /// If the lock is acquired before call this method, then the <paramref name="code"/> execution is delayed until the lock would released. 
        /// </remarks>
        /// <param name="code">
        /// The code to be executed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="code"/> is <c>null</c>.
        /// </exception>
        public void Execute(Action code)
        {
            Argument.IsNotNull("code", code);

            Acquire();
            _calls++;
            try
            {
                code.Invoke();
            }
            finally
            {
                if (--_calls == 0)
                {
                    Release();
                }
            }
        }

        /// <summary>
        /// This method enqueue the execution of the <paramref name="code"/> into a exclusive lock.
        /// </summary>
        /// <remarks>
        /// If the lock is acquired before call this method, then the <paramref name="code"/> execution is delayed until the lock would released, without blocking the current thread.
        /// This method relay the execution of the code into an instance of <see cref="BackgroundWorker"/>.
        /// </remarks>
        /// <param name="code">
        /// The code to be executed.
        /// </param>
        /// <param name="runWorkerCompletedEventHandler">
        /// Method that will handle the RunWorkerCompleted event of a <see cref="BackgroundWorker"/> class. 
        ///  It occurs when the background operation has completed, has been canceled, or has raised an exception. 
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="code"/> is <c>null</c>.
        /// </exception>
        public void Enqueue(Action code, RunWorkerCompletedEventHandler runWorkerCompletedEventHandler = null)
        {
            Argument.IsNotNull("code", code);

            var worker = new BackgroundWorker();
            worker.RunWorkerCompleted += (sender, args) =>
                {
                    var handler = runWorkerCompletedEventHandler;
                    if (handler != null)
                    {
                        handler.Invoke(this, args);
                    }
                };

            worker.DoWork += (sender, args) => Execute(code);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// This method execute the <paramref name="code"/> into a exclusive lock and returns a value.
        /// </summary>
        /// <remarks>
        /// If the lock is acquired before call this method, then the <paramref name="code"/> execution is delayed until the lock would released, blocking the current thread. 
        /// This method relay the execution of the code into an instance of <see cref="BackgroundWorker"/>.
        /// </remarks>
        /// <param name="code">
        /// The code to be executed.
        /// </param>
        /// <param name="runWorkerCompletedEventHandler">
        /// Method that will handle the RunWorkerCompleted event of a <see cref="BackgroundWorker"/> class. 
        ///  It occurs when the background operation has completed, has been canceled, or has raised an exception. 
        /// </param>
        /// <typeparam name="T">
        /// The result type.
        /// </typeparam>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="code"/> is <c>null</c>.
        /// </exception>
        public void Enqueue<T>(Func<T> code, RunWorkerCompletedEventHandler runWorkerCompletedEventHandler = null)
        {
            Argument.IsNotNull("code", code);

            var worker = new BackgroundWorker();
            worker.RunWorkerCompleted += (sender, args) =>
                {
                    var handler = runWorkerCompletedEventHandler;
                    if (handler != null)
                    {
                        handler.Invoke(this, args);
                    }
                };

            worker.DoWork += (sender, args) => args.Result = Execute(code);
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// This method execute the <paramref name="code"/> into a exclusive lock and returns a value.
        /// </summary>
        /// <remarks>
        /// If the lock is acquired before call this method, then the <paramref name="code"/> execution is delayed until the lock would released, blocking the current thread. 
        /// </remarks>
        /// <param name="code">
        /// The code to be executed.
        /// </param>
        /// <typeparam name="T">
        /// The result type.
        /// </typeparam>
        /// <returns>
        /// The result of execute the code.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="code"/> is <c>null</c>.
        /// </exception>
        public T Execute<T>(Func<T> code)
        {
            Argument.IsNotNull("code", code);

            Acquire();
            _calls++;
            try
            {
                return code.Invoke();
            }
            finally
            {
                if (--_calls == 0)
                {
                    Release();
                }
            }
        }

        /// <summary>
        /// Acquires an exclusive lock.
        /// </summary>
        public void Acquire()
        {
            Monitor.Enter(_syncObj);
            IsLockAcquired = true;
        }

        /// <summary>
        /// Releases an exclusive lock.
        /// </summary>
        public void Release()
        {
            if (IsLockAcquired)
            {
                Monitor.Exit(_syncObj);
                IsLockAcquired = false;
            }
        }

        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Reflection;

#if NET45
    using System.Threading;
#endif

    /// <summary>
    /// The exception service allows the usage of the Try/Catch mechanics. This means that this service provides possibilities
    /// to handle all exception types previously registered.
    /// </summary>
    public class ExceptionService : IExceptionService
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The static instance of the exception service.
        /// </summary>
        private static readonly IExceptionService _default = new ExceptionService();
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an action is retrying.
        /// </summary>
        public event EventHandler<RetryingEventArgs> RetryingAction;

        /// <summary>
        /// Occurs when an exception is buffered. 
        /// </summary>
        public event EventHandler<BufferedEventArgs> ExceptionBuffered;
        #endregion

        #region Fields
        /// <summary>
        /// The _exception counts
        /// </summary>
        private readonly Dictionary<Type, Queue<DateTime>> _exceptionCounter = new Dictionary<Type, Queue<DateTime>>();

        /// <summary>
        /// The _exception handlers
        /// </summary>
        private readonly Dictionary<Type, IExceptionHandler> _exceptionHandlers = new Dictionary<Type, IExceptionHandler>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance of the exception service.
        /// </summary>
        /// <value>The default instance.</value>
        public static IExceptionService Default
        {
            get { return _default; }
        }
        #endregion

        #region IExceptionService Members
        /// <summary>
        /// Gets the exception handlers.
        /// </summary>
        public IEnumerable<IExceptionHandler> ExceptionHandlers
        {
            get
            {
                lock (_exceptionHandlers)
                {
                    return _exceptionHandlers.Values.ToArray();
                }
            }
        }

        /// <summary>
        /// Determines whether the specified exception type is registered.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns>
        ///   <c>true</c> if the exception type is registered; otherwise, <c>false</c>.
        /// </returns>
        public bool IsExceptionRegistered<TException>() where TException : Exception
        {
            var exceptionType = typeof(TException);

            return IsExceptionRegistered(exceptionType);
        }

        /// <summary>
        /// Determines whether the specified exception type is registered.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <returns>
        ///   <c>true</c> if the specified exception type is registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref ref="exceptionType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="exceptionType" /> is not of type <see cref="Exception"/>.</exception>
        public bool IsExceptionRegistered(Type exceptionType)
        {
            Argument.IsOfType("exceptionType", exceptionType, typeof(Exception));

            lock (_exceptionHandlers)
            {
                return _exceptionHandlers.ContainsKey(exceptionType);
            }
        }

        /// <summary>
        /// Gets the exception handler for the specified exception type.
        /// </summary>
        /// <param name="exceptionType">Type of the exception.</param>
        /// <returns>
        /// The exception handler.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref ref="exceptionType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="exceptionType" /> is not of type <see cref="Exception"/>.</exception>
        public IExceptionHandler GetHandler(Type exceptionType)
        {
            Argument.IsOfType("exceptionType", exceptionType, typeof(Exception));

            lock (_exceptionHandlers)
            {
                if (IsExceptionRegistered(exceptionType))
                {
                    return _exceptionHandlers[exceptionType];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the exception handler for the specified exception type.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns>
        /// The exception handler.
        /// </returns>
        public IExceptionHandler GetHandler<TException>() where TException : Exception
        {
            var exceptionType = typeof(TException);

            return GetHandler(exceptionType);
        }

        /// <summary>
        /// Registers a specific exception including the handler.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="handler">The action to execute when the exception occurs.</param>
        /// <returns>The handler to use.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="handler"/> is <c>null</c>.</exception>
        public IExceptionHandler Register<TException>(Action<TException> handler)
            where TException : Exception
        {
            Argument.IsNotNull("handler", handler);

            var exceptionType = typeof(TException);

            lock (_exceptionHandlers)
            {
                if (!_exceptionHandlers.ContainsKey(exceptionType))
                {
                    var exceptionAction = new Action<Exception>(exception => handler((TException)exception));

                    var exceptionHandler = new ExceptionHandler(exceptionType, exceptionAction);
                    _exceptionHandlers.Add(exceptionType, exceptionHandler);

                    Log.Debug("Added exception handler for type '{0}'", exceptionType.Name);
                }

                return _exceptionHandlers[exceptionType];
            }
        }

        /// <summary>
        /// Unregisters a specific exception for handling.
        /// </summary>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <returns><c>true</c> if the exception is unsubscribed; otherwise <c>false</c>.</returns>
        public bool Unregister<TException>()
            where TException : Exception
        {
            var exceptionType = typeof(TException);

            lock (_exceptionHandlers)
            {
                if (_exceptionHandlers.ContainsKey(exceptionType))
                {
                    _exceptionHandlers.Remove(exceptionType);
                    Log.Debug("Removed exception handler for type '{0}'", exceptionType.Name);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Handles the specified exception if possible.
        /// </summary>
        /// <param name="exception">The exception to handle.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <returns><c>true</c> if the exception is handled; otherwise <c>false</c>.</returns>
        public bool HandleException(Exception exception)
        {
            Argument.IsNotNull("exception", exception);

            lock (_exceptionHandlers)
            {
                if (_exceptionHandlers == null)
                {
                    return false;
                }

                var exceptionType = exception.GetType();

                IExceptionHandler handler = null;
                foreach (var exceptionHandler in _exceptionHandlers)
                {
                    if (exceptionHandler.Key.IsAssignableFromEx(exceptionType))
                    {
                        if (exceptionHandler.Value.BufferPolicy != null)
                        {
                            if (!_exceptionCounter.ContainsKey(exceptionHandler.Key))
                            {
                                _exceptionCounter.Add(exceptionHandler.Key, new Queue<DateTime>());
                            }

                            _exceptionCounter[exceptionHandler.Key].Enqueue(DateTime.Now);

                            if (_exceptionCounter[exceptionHandler.Key].Count <= exceptionHandler.Value.BufferPolicy.NumberOfTimes)
                            {
                                OnExceptionBuffered(exception, DateTime.Now);
                                Log.Debug("[{0}] '{1}' buffered", DateTime.Now, exceptionType.Name);
                                continue;
                            }

                            var dateTime = _exceptionCounter[exceptionHandler.Key].Dequeue();

                            var duration = (DateTime.Now - dateTime);

                            if (duration >= exceptionHandler.Value.BufferPolicy.Interval && exceptionHandler.Value.BufferPolicy.Interval != TimeSpan.Zero)
                            {
                                OnExceptionBuffered(exception, DateTime.Now);
                                Log.Debug("[{0}] '{1}' buffered", DateTime.Now, exceptionType.Name);
                                continue;
                            }
                            _exceptionCounter[exceptionHandler.Key].Clear();
                        }

                        handler = exceptionHandler.Value;

                        break;
                    }
                }

                if (handler == null)
                {
                    return false;
                }

                handler.Handle(exception);
                return true;
            }
        }

        /// <summary>
        /// Processes the specified action.
        /// <para />
        /// If the exception could not be handled safely by this service, it will throw the exception.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void Process(Action action)
        {
            Argument.IsNotNull("action", action);

            try
            {
                action();
            }
            catch (Exception exception)
            {
                if (!HandleException(exception))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Processes the specified action.
        /// <para />
        /// If the exception could not be handled safely by this service, it will throw the exception.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public TResult Process<TResult>(Func<TResult> action)
        {
            Argument.IsNotNull("action", action);

            try
            {
                return action();
            }
            catch (Exception exception)
            {
                if (!HandleException(exception))
                {
                    throw;
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// Processes the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public void ProcessWithRetry(Action action)
        {
            Argument.IsNotNull("action", action);

            ProcessWithRetry(() => { action(); return default(object); });
        }

        /// <summary>
        /// Processes the specified action with possibilty to retry on error.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public TResult ProcessWithRetry<TResult>(Func<TResult> action)
        {
            Argument.IsNotNull("action", action);

            var retryCount = 1;

            while (true)
            {
                Exception lastError;
                TimeSpan interval;
                try
                {
                    return action();
                }
                catch (Exception exception)
                {
                    lock (_exceptionHandlers)
                    {
                        lastError = exception;

                        var exceptionHandler = ExceptionHandlers.FirstOrDefault(handler => handler.ExceptionType.IsAssignableFromEx(lastError.GetType()));

                        if (exceptionHandler != null && exceptionHandler.RetryPolicy != null)
                        {
                            var retryPolicy = exceptionHandler.RetryPolicy;

                            retryCount++;

                            if (retryCount <= retryPolicy.NumberOfTimes)
                            {
                                interval = retryPolicy.Interval;
                            }
                            else
                            {
                                if (!HandleException(lastError))
                                {
                                    throw;
                                }

                                return default(TResult);
                            }
                        }
                        else
                        {
                            if (!HandleException(lastError))
                            {
                                throw;
                            }

                            return default(TResult);
                        }
                    }
                }

                if (interval.TotalMilliseconds < 0)
                {
                    interval = TimeSpan.FromMilliseconds(1);
                }

                OnRetryingAction(retryCount, lastError, interval);

                Log.Debug("Retrying action for the '{0}' times", retryCount);

#if NET40
                Delay(interval.TotalMilliseconds).Wait();
#endif

#if NET45
                Task.Delay(interval).Wait();
#endif
                
            }
        }

#if NET45
        /// <summary>
        /// Processes the specified action. The action will be executed asynchrounously.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public async Task ProcessAsync(Action action, CancellationToken cancellationToken = default(CancellationToken))
        {
            Argument.IsNotNull("action", action);

            try
            {
                await Task.Run(action, cancellationToken);
            }
            catch (Exception exception)
            {
                if (!HandleException(exception))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Processes the specified action. The action will be executed asynchrounously.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public async Task<TResult> ProcessAsync<TResult>(Func<Task<TResult>> action, CancellationToken cancellationToken = default(CancellationToken))
        {
            Argument.IsNotNull("action", action);

            try
            {
                return await Task.Run(action, cancellationToken);
            }
            catch (Exception exception)
            {
                if (!HandleException(exception))
                {
                    throw;
                }
            }

            return default(TResult);
        }
#endif
        #endregion

        #region Methods

#if NET40
        /// <summary>
        /// Delays the specified milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">The <paramref name="milliseconds"/> is larger than <c>1</c>.</exception>
        private static Task Delay(double milliseconds)
        {
            Argument.IsMinimal("milliseconds", milliseconds, 1);

            var taskCompletionSource = new TaskCompletionSource<bool>();
            var timer = new System.Timers.Timer();
            timer.Elapsed += (obj, args) => taskCompletionSource.TrySetResult(true);
            timer.Interval = milliseconds;
            timer.AutoReset = false;
            timer.Start();

            return taskCompletionSource.Task;
        }
#endif

        /// <summary>
        /// Notifies the subscribers whenever a retry event occurs.
        /// </summary>
        /// <param name="retryCount">The current retry attempt count.</param>
        /// <param name="lastError">The exception that caused the retry conditions to occur.</param>
        /// <param name="delay">The delay that indicates how long the current thread will be suspended before the next iteration is invoked.</param>
        protected virtual void OnRetryingAction(int retryCount, Exception lastError, TimeSpan delay)
        {
            RetryingAction.SafeInvoke(this, new RetryingEventArgs(retryCount, delay, lastError));
        }

        /// <summary>
        /// Notifies the subscribers whenever a exception buffered event occurs.
        /// </summary>
        /// <param name="bufferedException">The buffered exception</param>
        /// <param name="dateTime">The date and time when the event occurs.</param>
        protected virtual void OnExceptionBuffered(Exception bufferedException, DateTime dateTime)
        {
            ExceptionBuffered.SafeInvoke(this, new BufferedEventArgs(bufferedException, dateTime));
        }
        #endregion
    }
}
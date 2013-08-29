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
    using Logging;
    using Reflection;

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
        private static readonly IExceptionService Instance = new ExceptionService();
        #endregion

        #region Fields
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
            get { return Instance; }
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
            var exceptionType = typeof (TException);

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
        public bool IsExceptionRegistered(Type exceptionType)
        {
            Argument.IsOfType("exceptionType", exceptionType, typeof(Exception));

            lock (_exceptionHandlers)
            {
                return _exceptionHandlers.ContainsKey(exceptionType);
            }
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

            var exceptionType = typeof (TException);

            lock (_exceptionHandlers)
            {
                if (!_exceptionHandlers.ContainsKey(exceptionType))
                {
                    var exceptionAction = new Action<Exception>(exception => handler((TException) exception));

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
            var exceptionType = typeof (TException);

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

                var exceptionInstanceType = exception.GetType();

                IExceptionHandler handler = null;
                foreach (var exceptionHandler in _exceptionHandlers)
                {
                    if (exceptionHandler.Key.IsAssignableFromEx(exceptionInstanceType))
                    {
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
        #endregion
    }
}
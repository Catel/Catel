// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using Reflection;

    /// <summary>
    /// Log manager that allows external libraries to subscribe to logging of Catel.
    /// <para />
    /// The manager automatically adds an instance of the <see cref="DebugLogListener"/> to
    /// the list of listeners so the tracing is available in debug mode. To remove this listener,
    /// call <see cref="ClearListeners"/> before doing any initialization.
    /// </summary>
    public static class LogManager
    {
        #region Classes
        /// <summary>
        /// Class containing log info.
        /// </summary>
        public static class LogInfo
        {
            #region Properties
            /// <summary>
            /// Gets a value indicating whether any logging is enabled, which means there is at least one listener.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsEnabled
            {
                get { return IsDebugEnabled || IsInfoEnabled || IsWarningEnabled || IsErrorEnabled; }
            }

            /// <summary>
            /// Gets a value indicating whether debug logging is enabled. This means that there is at least one listener 
            /// that is interested in debug logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if debug logging is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsDebugEnabled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether info logging is enabled. This means that there is at least one listener 
            /// that is interested in info logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if info logging is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsInfoEnabled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether warning logging is enabled. This means that there is at least one listener 
            /// that is interested in warning logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if warning logging is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsWarningEnabled { get; private set; }

            /// <summary>
            /// Gets a value indicating whether error logging is enabled. This means that there is at least one listener 
            /// that is interested in error logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if error logging is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsErrorEnabled { get; private set; }
            #endregion

            #region Methods
            /// <summary>
            /// Updates the log info.
            /// </summary>
            internal static void UpdateLogInfo()
            {
                lock (_logListeners)
                {
                    IsDebugEnabled = false;
                    IsInfoEnabled = false;
                    IsWarningEnabled = false;
                    IsErrorEnabled = false;

                    foreach (var listener in _logListeners)
                    {
                        if (listener.IsDebugEnabled)
                        {
                            IsDebugEnabled = true;
                        }

                        if (listener.IsInfoEnabled)
                        {
                            IsInfoEnabled = true;
                        }

                        if (listener.IsWarningEnabled)
                        {
                            IsWarningEnabled = true;
                        }

                        if (listener.IsErrorEnabled)
                        {
                            IsErrorEnabled = true;
                        }
                    }
                }
            }

            /// <summary>
            /// Determines whether the specified log event is enabled.
            /// </summary>
            /// <param name="logEvent">The log event.</param>
            /// <returns><c>true</c> if the specified log event is enabled; otherwise, <c>false</c>.</returns>
            public static bool IsLogEventEnabled(LogEvent logEvent)
            {
                switch (logEvent)
                {
                    case LogEvent.Debug:
                        return IsDebugEnabled;

                    case LogEvent.Info:
                        return IsInfoEnabled;

                    case LogEvent.Warning:
                        return IsWarningEnabled;

                    case LogEvent.Error:
                        return IsErrorEnabled;

                    default:
                        throw new ArgumentOutOfRangeException("logEvent");
                }
            }
            #endregion
        }
        #endregion

        #region Constants
        /// <summary>
        /// List of all registered <see cref="ILogListener"/> instances.
        /// </summary>
        private static readonly List<ILogListener> _logListeners = new List<ILogListener>();

        /// <summary>
        /// Dictionary containing the logs per type.
        /// </summary>
        private static readonly Dictionary<Type, ILog> _loggers = new Dictionary<Type, ILog>();

        /// <summary>
        /// Logging of the class. Must be declared after the log listeners and loggers.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="LogManager" /> class.
        /// </summary>
        static LogManager()
        {
#if NET
            AppDomain.CurrentDomain.DomainUnload += (sender, e) => FlushAll();
#endif

#if NET
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => FlushAll();
#endif
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when a log message is written to one of the logs.
        /// </summary>
        public static event EventHandler<LogMessageEventArgs> LogMessage;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the current class logger.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ILog GetCurrentClassLogger()
        {
            var callingType = StaticHelper.GetCallingType();

            return GetLogger(callingType);
        }

        /// <summary>
        /// Registers the default debug listener. Starting with Catel 2.4, the debug listener is no longer
        /// attached for performance reasons. To register the debug listener, call this method.
        /// <para />
        /// When an instance of the <see cref="DebugLogListener"/> is already registered, the existing instance
        /// is returned.
        /// <para />
        /// This method does not ignore Catel logging.
        /// </summary>
        /// <returns>The newly created or existing <see cref="DebugLogListener"/>.</returns>
        public static ILogListener RegisterDebugListener()
        {
            return RegisterDebugListener(false);
        }

        /// <summary>
        /// Registers the default debug listener. Starting with Catel 2.4, the debug listener is no longer
        /// attached for performance reasons. To register the debug listener, call this method.
        /// <para />
        /// When an instance of the <see cref="DebugLogListener"/> is already registered, the existing instance
        /// is returned.
        /// </summary>
        /// <returns>The newly created or existing <see cref="DebugLogListener"/>.</returns>
        public static ILogListener RegisterDebugListener(bool ignoreCatelLogging)
        {
            var debugLogListener = (from logListener in _logListeners
                                    where logListener is DebugLogListener
                                    select logListener).FirstOrDefault();

            if (debugLogListener == null)
            {
                debugLogListener = new DebugLogListener { IgnoreCatelLogging = ignoreCatelLogging };
                AddListener(debugLogListener);
            }

            return debugLogListener;
        }

        /// <summary>
        /// Gets the logger for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static ILog GetLogger(Type type)
        {
            Argument.IsNotNull("type", type);

            lock (_loggers)
            {
                if (!_loggers.ContainsKey(type))
                {
                    var log = new Log(type);
                    log.LogMessage += OnLogMessage;

                    _loggers.Add(type, log);
                }

                return _loggers[type];
            }
        }

        /// <summary>
        /// Flushes all listeners that implement the <see cref="IBatchLogListener"/> by calling <see cref="IBatchLogListener.Flush"/>.
        /// </summary>
        public static void FlushAll()
        {
            lock (_logListeners)
            {
                foreach (var listener in _logListeners)
                {
                    var batchListener = listener as IBatchLogListener;
                    if (batchListener != null)
                    {
                        batchListener.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the currently registered log listeners.
        /// </summary>
        /// <returns>An enumerable of all listeners.</returns>
        public static IEnumerable<ILogListener> GetListeners()
        {
            lock (_logListeners)
            {
                return _logListeners;
            }
        }

        /// <summary>
        /// Adds a log listener which will receive all log events.
        /// <para />
        /// This method does not check whether the <paramref name="listener"/> is already added to the list
        /// of registered listeners.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static void AddListener(ILogListener listener)
        {
            Argument.IsNotNull("listener", listener);

            lock (_logListeners)
            {
                _logListeners.Add(listener);

                LogInfo.UpdateLogInfo();

                Log.Debug("Added listener '{0}' to log manager", listener.GetType().FullName);
            }
        }

        /// <summary>
        /// Removes the a log listener which will stop receiving all log events.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static void RemoveListener(ILogListener listener)
        {
            Argument.IsNotNull("listener", listener);

            lock (_logListeners)
            {
                _logListeners.Remove(listener);

                LogInfo.UpdateLogInfo();

                Log.Debug("Removed listener '{0}' from log manager", listener.GetType().FullName);
            }
        }

        /// <summary>
        /// Determines whether the specified listener is already registered or not.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <returns>
        /// 	<c>true</c> if the specified listener is already registered; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        public static bool IsListenerRegistered(ILogListener listener)
        {
            Argument.IsNotNull("listener", listener);

            lock (_logListeners)
            {
                return _logListeners.Contains(listener);
            }
        }

        /// <summary>
        /// Clears all the current listeners.
        /// </summary>
        public static void ClearListeners()
        {
            lock (_logListeners)
            {
                _logListeners.Clear();
            }

            Log.Debug("Cleared all listeners");
        }

        /// <summary>
        /// Called when one of the logs has written a log message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Logging.LogMessageEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="LogEvent"/> is not supported.</exception>
        private static void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            LogMessage.SafeInvoke(sender, e);

            lock (_logListeners)
            {
                foreach (var listener in _logListeners)
                {
                    if (IsListenerInterested(listener, e.LogEvent))
                    {
                        listener.Write(e.Log, e.Message, e.LogEvent, e.ExtraData);

                        switch (e.LogEvent)
                        {
                            case LogEvent.Debug:
                                listener.Debug(e.Log, e.Message, e.ExtraData);
                                break;

                            case LogEvent.Info:
                                listener.Info(e.Log, e.Message, e.ExtraData);
                                break;

                            case LogEvent.Warning:
                                listener.Warning(e.Log, e.Message, e.ExtraData);
                                break;

                            case LogEvent.Error:
                                listener.Error(e.Log, e.Message, e.ExtraData);
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the specified listener is interested in the log event.
        /// </summary>
        /// <param name="listener">The listener.</param>
        /// <param name="logEvent">The log event.</param>
        /// <returns>
        /// <c>true</c> if the specified listener is interested in the log event; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="listener"/> is <c>null</c>.</exception>
        private static bool IsListenerInterested(ILogListener listener, LogEvent logEvent)
        {
            Argument.IsNotNull("listener", listener);

            switch (logEvent)
            {
                case LogEvent.Debug:
                    return listener.IsDebugEnabled;

                case LogEvent.Info:
                    return listener.IsInfoEnabled;

                case LogEvent.Warning:
                    return listener.IsWarningEnabled;

                case LogEvent.Error:
                    return listener.IsErrorEnabled;

                default:
                    throw new ArgumentOutOfRangeException("logEvent");
            }
        }

        #endregion
    }
}
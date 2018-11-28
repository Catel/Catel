// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManager.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Reflection;

#if NET || NETCORE
    using System.Configuration;
    using Catel.Configuration;
#endif

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
                get { return IsDebugEnabled || IsInfoEnabled || IsWarningEnabled || IsErrorEnabled || IsStatusEnabled; }
            }

            /// <summary>
            /// Gets a value indicating whether Catel logging should be ignored. This means that all log listeners are ignoring
            /// Catel logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if Catel logging is ignored; otherwise, <c>false</c>.
            /// </value>
            public static bool IgnoreCatelLogging { get; private set; }

            /// <summary>
            /// Gets a value indicating whether duplicate exception logging is ignored.
            /// </summary>
            /// <value>
            /// <c>true</c> if duplicate exception logging is ignored; otherwise, <c>false</c>.
            /// </value>
            public static bool IgnoreDuplicateExceptionLogging { get; private set; }

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

            /// <summary>
            /// Gets a value indicating whether status logging is enabled. This means that there is at least one listener 
            /// that is interested in status logging.
            /// </summary>
            /// <value>
            /// <c>true</c> if status logging is enabled; otherwise, <c>false</c>.
            /// </value>
            public static bool IsStatusEnabled { get; private set; }


            #endregion

            #region Methods
            /// <summary>
            /// Updates the log info.
            /// </summary>
            internal static void UpdateLogInfo()
            {
                lock (_logListeners)
                {
                    IgnoreCatelLogging = true;
                    IgnoreDuplicateExceptionLogging = true;
                    IsDebugEnabled = false;
                    IsInfoEnabled = false;
                    IsWarningEnabled = false;
                    IsErrorEnabled = false;
                    IsStatusEnabled = false;

                    foreach (var listener in _logListeners)
                    {
                        if (!listener.IgnoreCatelLogging)
                        {
                            IgnoreCatelLogging = false;
                        }

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

                        if (listener.IsStatusEnabled)
                        {
                            IsStatusEnabled = true;
                        }
                    }

                    // Allow overriding via LogManager
                    if (LogManager.IgnoreCatelLogging.HasValue)
                    {
                        IgnoreCatelLogging = LogManager.IgnoreCatelLogging.Value;
                    }

                    if (LogManager.IsDebugEnabled.HasValue)
                    {
                        IsDebugEnabled = LogManager.IsDebugEnabled.Value;
                    }

                    if (LogManager.IsInfoEnabled.HasValue)
                    {
                        IsInfoEnabled = LogManager.IsInfoEnabled.Value;
                    }

                    if (LogManager.IsWarningEnabled.HasValue)
                    {
                        IsWarningEnabled = LogManager.IsWarningEnabled.Value;
                    }

                    if (LogManager.IsErrorEnabled.HasValue)
                    {
                        IsErrorEnabled = LogManager.IsErrorEnabled.Value;
                    }

                    if (LogManager.IsStatusEnabled.HasValue)
                    {
                        IsStatusEnabled = LogManager.IsStatusEnabled.Value;
                    }

                    if (LogManager.IgnoreDuplicateExceptionLogging.HasValue)
                    {
                        IgnoreDuplicateExceptionLogging = LogManager.IgnoreDuplicateExceptionLogging.Value;
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

                    case LogEvent.Status:
                        return IsStatusEnabled;
                }

                return false;
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
        private static readonly Dictionary<string, ILog> _loggers = new Dictionary<string, ILog>();

        /// <summary>
        /// Logging of the class. Must be declared after the log listeners and loggers.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static bool? _ignoreCatelLogging;
        private static bool? _ignoreDuplicateExceptionLogging;
        private static bool? _isDebugEnabled;
        private static bool? _isInfoEnabled;
        private static bool? _isWarningEnabled;
        private static bool? _isErrorEnabled;
        private static bool? _isStatusEnabled;

        #endregion

        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="LogManager" /> class.
        /// </summary>
        static LogManager()
        {
#if NET || NETCORE
            AppDomain.CurrentDomain.DomainUnload += (sender, e) => FlushAll();
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => FlushAll();
#endif
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the global IgnoreCatelLogging should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if Catel logging must be ignored for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IgnoreCatelLogging
        {
            get { return _ignoreCatelLogging; }
            set
            {
                _ignoreCatelLogging = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the global IsDebugEnabled should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if debug logging must be enabled for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IsDebugEnabled
        {
            get { return _isDebugEnabled; }
            set
            {
                _isDebugEnabled = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the global IsInfoEnabled should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if info logging must be enabled for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IsInfoEnabled
        {
            get { return _isInfoEnabled; }
            set
            {
                _isInfoEnabled = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the global IsWarningEnabled should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if warning logging must be enabled for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IsWarningEnabled
        {
            get { return _isWarningEnabled; }
            set
            {
                _isWarningEnabled = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the global IsErrorEnabled should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if error logging must be enabled for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IsErrorEnabled
        {
            get { return _isErrorEnabled; }
            set
            {
                _isErrorEnabled = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the global IsStatusEnabled should be overriden.
        /// <para />
        /// Note that this value will override all settings of each listener globally. If this value is
        /// set to <c>null</c>, nothing will be overriden.
        /// </summary>
        /// <value><c>true</c> if status logging must be enabled for all log listeners; otherwise, <c>false</c>.</value>
        public static bool? IsStatusEnabled
        {
            get { return _isStatusEnabled; }
            set
            {
                _isStatusEnabled = value;
                LogInfo.UpdateLogInfo();
            }
        }

        /// <summary>
        /// Gets a value indicating whether duplicate exception loggin is ignored.
        /// </summary>
        /// <value>
        /// <c>true</c> if duplicate exception logging is ignored; otherwise, <c>false</c>.
        /// </value>
        public static bool? IgnoreDuplicateExceptionLogging {
            get { return _ignoreDuplicateExceptionLogging; }
            set
            {
                _ignoreDuplicateExceptionLogging = value;
                LogInfo.UpdateLogInfo();
            }
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

#if NET || NETCORE
        /// <summary>
        /// Loads the listeners from the specified configuration file.
        /// </summary>
        /// <param name="configurationFilePath">The configuration file path.</param>
        /// <param name="assembly">The assembly to determine product info. If <c>null</c>, the entry assembly will be used.</param>
        public static void LoadListenersFromConfigurationFile(string configurationFilePath, Assembly assembly = null)
        {
            if (string.IsNullOrWhiteSpace(configurationFilePath))
            {
                return;
            }

            try
            {
                var configFile = configurationFilePath;
                var map = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = configFile
                };

                var configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                LoadListenersFromConfiguration(configuration, assembly);
            }
            catch (Exception)
            {
                // Swallow
            }
        }

        /// <summary>
        /// Loads the listeners from the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="assembly">The assembly to determine product info. If <c>null</c>, the entry assembly will be used.</param>
        public static void LoadListenersFromConfiguration(Configuration configuration, Assembly assembly = null)
        {
            if (configuration == null)
            {
                return;
            }

            try
            {
                var configurationSection = configuration.GetSection<LoggingConfigurationSection>("logging", "catel");
                if (configurationSection != null)
                {
                    var logListeners = configurationSection.GetLogListeners(assembly);
                    foreach (var logListener in logListeners)
                    {
                        AddListener(logListener);
                    }
                }
            }
            catch (Exception)
            {
                // Swallow
            }
        }
#endif

        /// <summary>
        /// Registers the default debug listener. Starting with Catel 2.4, the debug listener is no longer
        /// attached for performance reasons. To register the debug listener, call this method.
        /// <para />
        /// When an instance of the <see cref="DebugLogListener"/> is already registered, the existing instance
        /// is returned.
        /// </summary>
        /// <returns>The newly created or existing <see cref="DebugLogListener"/>.</returns>
        public static ILogListener AddDebugListener(bool ignoreCatelLogging = false)
        {
            var debugLogListener = (from logListener in _logListeners
                                    where logListener is DebugLogListener
                                    select logListener).FirstOrDefault();

            if (debugLogListener == null)
            {
                debugLogListener = new DebugLogListener
                {
                    IgnoreCatelLogging = ignoreCatelLogging
                };

                AddListener(debugLogListener);
            }

            return debugLogListener;
        }

        /// <summary>
        /// Gets the logger for the specified generic type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The <see cref="ILog"/> object for the specified type.</returns>
        public static ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        /// <summary>
        /// Gets the logger for the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="ILog"/> object for the specified type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static ILog GetLogger(Type type)
        {
            Argument.IsNotNull("type", type);

            return GetLogger(type.FullName, type);
        }

        /// <summary>
        /// Gets the logger with the specified name.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>The <see cref="ILog"/> object with the specified name.</returns>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        public static ILog GetLogger(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(name, out var log))
                {
                    log = new Log(name);
                    log.LogMessage += OnLogMessage;

                    _loggers.Add(name, log);
                }

                return log;
            }
        }

        /// <summary>
        /// Gets the logger with the specified name and type.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="ILog"/> object with the specified name.</returns>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static ILog GetLogger(string name, Type type)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("type", type);

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(name, out var log))
                {
                    log = new Log(name, type);
                    log.LogMessage += OnLogMessage;

                    _loggers.Add(name, log);
                }

                return log;
            }
        }

        /// <summary>
        /// Gets the catel logger with the specified name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="alwaysLog">The flag indicating wether or not the logger should always write logging statements regardless of log filter settings.</param>
        /// <returns>The <see cref="ICatelLog"/> object for the specified type.</returns>
        /// <exception cref="ArgumentException">An element with the same key already exists in the <see cref="LogManager"/> and does not implement <see cref="ICatelLog"/>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        internal static ICatelLog GetCatelLogger(Type type, bool alwaysLog = false)
        {
            Argument.IsNotNull("type", type);

            var name = type.FullName;

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(name, out var log))
                {
                    log = new CatelLog(name, alwaysLog);
                    log.LogMessage += OnLogMessage;

                    _loggers.Add(name, log);
                }

                var catelLog = log as ICatelLog;
                if (catelLog == null)
                {
                    // Handle the unlikely event where a logger with the same name is initialized before the catel logger.
                    throw new ArgumentException(string.Format("An element with the same key already exists in the {0} and does not implement {1}.", typeof(LogManager).Name, typeof(ICatelLog).Name));
                }

                return catelLog;
            }
        }

        /// <summary>
        /// Removes the logger with the specified name from the <see cref="LogManager"/>.
        /// </summary>
        /// <param name="name">The name of the logger.</param>
        /// <returns>true if the logger is successfully found and removed; otherwise, false. This method returns false if <paramref name="name"/> is not found in the <see cref="LogManager"/>.</returns>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        internal static bool RemoveLogger(string name)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            lock (_loggers)
            {
                if (!_loggers.TryGetValue(name, out var log))
                {
                    return false;
                }

                log.LogMessage -= OnLogMessage;

                return _loggers.Remove(name);
            }
        }

        /// <summary>
        /// Flushes all listeners that implement the <see cref="IBatchLogListener" /> by calling <see cref="IBatchLogListener.FlushAsync" />.
        /// </summary>
        public static async Task FlushAllAsync()
        {
            var logListenersToFlush = new List<IBatchLogListener>();

            var logListeners = GetThreadSafeLogListeners();
            foreach (var listener in logListeners)
            {
                var batchListener = listener as IBatchLogListener;
                if (batchListener != null)
                {
                    logListenersToFlush.Add(batchListener);
                }
            }

            foreach (var logListenerToFlush in logListenersToFlush)
            {
                await logListenerToFlush.FlushAsync();
            }
        }

        /// <summary>
        /// Flushes all listeners that implement the <see cref="IBatchLogListener" /> by calling <see cref="IBatchLogListener.FlushAsync" />.
        /// </summary>
        [ObsoleteEx(ReplacementTypeOrMember = nameof(FlushAllAsync), Message = "Since listeners only have FlushAsync, a non-async flush doesn't make sense", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
#pragma warning disable AvoidAsyncVoid // Avoid async void
        public static async void FlushAll()
#pragma warning restore AvoidAsyncVoid // Avoid async void
        {
            await FlushAllAsync();
        }

        /// <summary>
        /// Gets all the currently registered log listeners.
        /// </summary>
        /// <returns>An enumerable of all listeners.</returns>
        public static IEnumerable<ILogListener> GetListeners()
        {
            return GetThreadSafeLogListeners();
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
            }

            LogInfo.UpdateLogInfo();

            Log.Debug("Added listener '{0}' to log manager", listener.GetType().FullName);
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
            }

            LogInfo.UpdateLogInfo();

            Log.Debug("Removed listener '{0}' from log manager", listener.GetType().FullName);
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
        /// Gets the current log listeners in a thread-safe manner.
        /// </summary>
        /// <returns>List&lt;ILogListener&gt;.</returns>
        private static List<ILogListener> GetThreadSafeLogListeners()
        {
            var logListeners = new List<ILogListener>();

            lock (_logListeners)
            {
                logListeners.AddRange(_logListeners);
            }

            return logListeners;
        }

        /// <summary>
        /// Called when one of the logs has written a log message.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Catel.Logging.LogMessageEventArgs"/> instance containing the event data.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <see cref="LogEvent"/> is not supported.</exception>
        private static void OnLogMessage(object sender, LogMessageEventArgs e)
        {
            if (LogInfo.IgnoreCatelLogging && e.Log.IsCatelLoggingAndCanBeIgnored())
            {
                return;
            }

            if (!LogInfo.IsLogEventEnabled(e.LogEvent))
            {
                return;
            }

            LogMessage.SafeInvoke(sender, e);

            var logListeners = GetThreadSafeLogListeners();
            if (logListeners.Count == 0)
            {
                return;
            }

            foreach (var listener in logListeners)
            {
                if (IsListenerInterested(listener, e.LogEvent))
                {
                    listener.Write(e.Log, e.Message, e.LogEvent, e.ExtraData, e.LogData, e.Time);

                    switch (e.LogEvent)
                    {
                        case LogEvent.Debug:
                            listener.Debug(e.Log, e.Message, e.ExtraData, e.LogData, e.Time);
                            break;

                        case LogEvent.Info:
                            listener.Info(e.Log, e.Message, e.ExtraData, e.LogData, e.Time);
                            break;

                        case LogEvent.Warning:
                            listener.Warning(e.Log, e.Message, e.ExtraData, e.LogData, e.Time);
                            break;

                        case LogEvent.Error:
                            listener.Error(e.Log, e.Message, e.ExtraData, e.LogData, e.Time);
                            break;

                        case LogEvent.Status:
                            listener.Status(e.Log, e.Message, e.ExtraData, e.LogData, e.Time);
                            break;
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

                case LogEvent.Status:
                    return listener.IsStatusEnabled;
            }

            return false;
        }

        #endregion
    }
}

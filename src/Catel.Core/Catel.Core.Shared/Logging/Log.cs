// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Logging
{
    using System;
    using Reflection;

    /// <summary>
    ///   Default logging class that writes to the console or output window.
    /// </summary>
    public class Log : ILog
    {
        #region Fields
        private int _indentSize = 2;
        private int _indentLevel = 0;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="targetType">The type for which this log is intented.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> is <c>null</c>.</exception>
        public Log(Type targetType)
        {
            Argument.IsNotNull("targetType", targetType);

            TargetType = targetType;
            Tag = targetType.FullName;

            IsCatelLogging = IsCatelType(targetType);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the target type of the log. This is the type where the log is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; private set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; set; }

        /// <summary>
        /// Gets a value indicating whether this logger is a Catel logger.
        /// <para />
        /// This value can be useful to exclude Catel logging for external listeners.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is a Catel logger; otherwise, <c>false</c>.
        /// </value>
        public bool IsCatelLogging { get; private set; }

        /// <summary>
        /// Gets or sets the size of the indent.
        /// <para />
        /// The default value is <c>2</c>.
        /// </summary>
        /// <value>
        /// The size of the indent.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        public int IndentSize
        {
            get
            {
                return _indentSize;
            }
            set
            {
                Argument.IsMinimal("value", value, 0);

                _indentSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the indent level.
        /// <para />
        /// The default value is <c>0</c>.
        /// </summary>
        /// <value>
        /// The size of the indent.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The <c>value</c> is negative.</exception>
        public int IndentLevel
        {
            get { return _indentLevel; }
            set
            {
                Argument.IsMinimal("value", value, 0);
                
                _indentLevel = value;
            }
        }
        #endregion

        #region Events
        /// <summary>
        ///   Occurs when a message is written to the log.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified type is a Catel type.
        /// </summary>
        /// <param name="type">Type of the target.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is a Catel type; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static bool IsCatelType(Type type)
        {
            Argument.IsNotNull("type", type);

            var assemblyName = type.GetAssemblyFullNameEx();

            // TODO: can we get better performance with a hashset on types?

            return assemblyName.StartsWith("Catel.Core") ||
                assemblyName.StartsWith("Catel.Mvc") || 
                assemblyName.StartsWith("Catel.MVVM") ||
                assemblyName.StartsWith("Catel.Extensions");
        }

        /// <summary>
        /// Writes the specified message as debug message with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public void DebugWithData(string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsDebugEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, extraData, LogEvent.Debug);
        }

        /// <summary>
        /// Writes the specified message as debug message with extra data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        [ObsoleteEx(Replacement = "Use extension methods instead", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void DebugWithData(Exception exception, string message, object extraData = null)
        {
            LogExtensions.DebugWithData(this, exception, message, extraData);
        }

        /// <summary>
        /// Writes the specified message as info message with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public void InfoWithData(string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsInfoEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, extraData, LogEvent.Info);
        }

        /// <summary>
        /// Writes the specified message as info message with extra data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        [ObsoleteEx(Replacement = "Use extension methods instead", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void InfoWithData(Exception exception, string message, object extraData = null)
        {
            LogExtensions.InfoWithData(this, exception, message, extraData);
        }

        /// <summary>
        /// Writes the specified message as warning message with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public void WarningWithData(string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsWarningEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, extraData, LogEvent.Warning);
        }

        /// <summary>
        /// Writes the specified message as warning message with extra data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        [ObsoleteEx(Replacement = "Use extension methods instead", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void WarningWithData(Exception exception, string message, object extraData = null)
        {
            LogExtensions.WarningWithData(this, exception, message, extraData);
        }

        /// <summary>
        /// Writes the specified message as error message with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        public void ErrorWithData(string message, object extraData = null)
        {
            if (!LogManager.LogInfo.IsErrorEnabled)
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, extraData, LogEvent.Error);
        }

        /// <summary>
        /// Writes the specified message as error message with extra data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        [ObsoleteEx(Replacement = "Use extension methods instead", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void ErrorWithData(Exception exception, string message, object extraData = null)
        {
            LogExtensions.ErrorWithData(this, exception, message, extraData);
        }

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        public void WriteWithData(string message, object extraData, LogEvent logEvent)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, extraData, logEvent);
        }

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception" /> is <c>null</c>.</exception>
        [ObsoleteEx(Replacement = "Use extension methods instead", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "6.0")]
        public void WriteWithData(Exception exception, string message, object extraData, LogEvent logEvent)
        {
            LogExtensions.WriteWithData(this, exception, message, extraData, logEvent);
        }

        /// <summary>
        /// Raises the <see cref="LogMessage" /> event.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        private void WriteMessage(string message, object extraData, LogEvent logEvent)
        {
            var logMessage = LogMessage;
            if (logMessage != null)
            {
                var eventArgs = new LogMessageEventArgs(this, string.Format("{0}{1}", new string(' ', IndentLevel * IndentSize), message ?? string.Empty), extraData, logEvent, DateTime.Now);
                logMessage(this, eventArgs);
            }
        }

        /// <summary>
        /// Increases the <see cref="IndentLevel"/> by <c>1</c>.
        /// </summary>
        public void Indent()
        {
            IndentLevel++;
        }

        /// <summary>
        /// Decreases the <see cref="IndentLevel"/> by <c>1</c>.
        /// </summary>
        public void Unindent()
        {
            if (IndentLevel > 0)
            {
                IndentLevel--;
            }
        }
        #endregion
    }
}
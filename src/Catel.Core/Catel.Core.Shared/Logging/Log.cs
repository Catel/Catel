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
        /// <param name="targetType">The type for which this logger is intended.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="targetType"/> is <c>null</c>.</exception>
        public Log(Type targetType) : this(targetType?.FullName, targetType)
        {
            Argument.IsNotNull("targetType", targetType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="name">The name of this logger.</param>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        public Log(string name) : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="name">The name of this logger.</param>
        /// <param name="targetType">The type for which this logger is intended.</param>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        public Log(string name, Type targetType)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            Name = name;
            TargetType = targetType;

            IsCatelLogging = targetType.IsCatelType();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        /// <value>The name of the logger.</value>
        public string Name { get; }

        /// <summary>
        /// Gets the target type of the log. This is the type where the log is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        public Type TargetType { get; }

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
        public bool IsCatelLogging { get; }

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

            WriteMessage(message, extraData, null, logEvent);
        }

        /// <summary>
        /// Writes the specified message as error message with log data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="logEvent">The log event.</param>
        public void WriteWithData(string message, LogData logData, LogEvent logEvent)
        {
            if (!LogManager.LogInfo.IsLogEventEnabled(logEvent))
            {
                return;
            }

            if (LogManager.LogInfo.IgnoreCatelLogging && IsCatelLogging)
            {
                return;
            }

            WriteMessage(message, null, logData, logEvent);
        }

        /// <summary>
        /// Raises the <see cref="LogMessage" /> event.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="logEvent">The log event.</param>
        private void WriteMessage(string message, object extraData, LogData logData, LogEvent logEvent)
        {
            var logMessage = LogMessage;
            if (logMessage != null)
            {
                var eventArgs = new LogMessageEventArgs(this, string.Format("{0}{1}", new string(' ', IndentLevel * IndentSize), message ?? string.Empty), extraData, logData, logEvent, DateTime.Now);
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
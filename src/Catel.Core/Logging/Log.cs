namespace Catel.Logging
{
    using System;
    using Reflection;

    /// <summary>
    ///   Default logging class that writes to the console or output window.
    /// </summary>
    public class Log : ILog
    {
        private int _indentSize = 2;
        private int _indentLevel = 0;
        private readonly Lazy<bool> _shouldIgnoreIfCatelLoggingIsDisabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="targetType">The type for which this logger is intended.</param>
        /// <exception cref="ArgumentException">If <paramref name="targetType"/> is <c>null</c>.</exception>
        public Log(Type targetType)
            : this(targetType?.FullName, targetType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class.
        /// </summary>
        /// <param name="name">The name of this logger.</param>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or a whitespace.</exception>
        public Log(string name)
            : this(name, null)
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

            IsCatelLogging = targetType?.IsCatelType() ?? false;
            _shouldIgnoreIfCatelLoggingIsDisabled = new Lazy<bool>(ShouldIgnoreIfCatelLoggingIsDisabled);
        }

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
        public virtual bool IsCatelLogging { get; }

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

        /// <summary>
        ///   Occurs when a message is written to the log.
        /// </summary>
        public event EventHandler<LogMessageEventArgs> LogMessage;

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

            if (LogManager.LogInfo.IgnoreCatelLogging && _shouldIgnoreIfCatelLoggingIsDisabled.Value)
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

            if (LogManager.LogInfo.IgnoreCatelLogging && _shouldIgnoreIfCatelLoggingIsDisabled.Value)
            {
                return;
            }

            WriteMessage(message, null, logData, logEvent);
        }

        /// <summary>
        /// Retuns a value whether this should be ignored if it as Catel logging.
        /// </summary>
        /// <returns><c>true</c> if this log should be ignored if Catel logging is </returns>
        protected virtual bool ShouldIgnoreIfCatelLoggingIsDisabled()
        {
            return this.IsCatelLoggingAndCanBeIgnored();
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
            if (logMessage is not null)
            {
                var now = FastDateTime.Now;
                var eventArgs = new LogMessageEventArgs(this, string.Format("{0}{1}", new string(' ', IndentLevel * IndentSize), message ?? string.Empty), extraData, logData, logEvent, now);
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
    }
}

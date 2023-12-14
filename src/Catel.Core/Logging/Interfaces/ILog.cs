namespace Catel.Logging
{
    using System;

    /// <summary>
    /// Log interface.
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the target type of the log. This is the type where the log is created for.
        /// </summary>
        /// <value>The type of the target.</value>
        Type TargetType { get; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        object? Tag { get; set; }

        /// <summary>
        /// Gets a value indicating whether this logger is a Catel logger.
        /// <para />
        /// This value can be useful to exclude Catel logging for external listeners.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a Catel logger; otherwise, <c>false</c>.
        /// </value>
        bool IsCatelLogging { get; }

        /// <summary>
        /// Gets or sets the size of the indent.
        /// <para />
        /// The default value is <c>2</c>.
        /// </summary>
        /// <value>
        /// The size of the indent.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The value is negative.</exception>
        int IndentSize { get; set; }

        /// <summary>
        /// Gets or sets the indent level.
        /// </summary>
        /// <value>
        /// The size of the indent. The default value is <c>0</c>.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The <c>value</c> is negative.</exception>
        int IndentLevel { get; set; }

        /// <summary>
        ///   Occurs when a message is written to the log.
        /// </summary>
        event EventHandler<LogMessageEventArgs>? LogMessage;

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="extraData">The extra data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message" /> is <c>null</c>.</exception>
        void WriteWithData(string message, object? extraData, LogEvent logEvent);

        /// <summary>
        /// Writes the specified message as specified log event with extra data.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logData">The log data.</param>
        /// <param name="logEvent">The log event.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="message" /> is <c>null</c>.</exception>
        void WriteWithData(string message, LogData? logData, LogEvent logEvent);

        /// <summary>
        /// Increases the <see cref="IndentLevel"/> by <c>1</c>.
        /// </summary>
        void Indent();

        /// <summary>
        /// Decreases the <see cref="IndentLevel"/> by <c>1</c>.
        /// </summary>
        void Unindent();
    }
}

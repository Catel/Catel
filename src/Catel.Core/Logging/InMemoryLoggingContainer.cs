namespace Catel.Logging;

using System;
using System.Collections.Generic;

public class InMemoryLoggingContainer : IInMemoryLoggingContainer
{
    private readonly List<LogEntry> _logEntries = new List<LogEntry>();

    public int MaxCount { get; set; } = 1000;

    public IReadOnlyList<LogEntry> LogEntries => _logEntries.AsReadOnly();

    public event EventHandler<LogEntryEventArgs>? LogEntryAdded;

    public void Add(LogEntry logEntry)
    {
        lock (_logEntries)
        {
            _logEntries.Add(logEntry);

            if (_logEntries.Count > MaxCount)
            {
                _logEntries.RemoveAt(0);
            }

            var logEntryAdded = LogEntryAdded;
            if (logEntryAdded is not null)
            {
                logEntryAdded.Invoke(this, new LogEntryEventArgs(logEntry));
            }
        }
    }
}

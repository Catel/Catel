namespace Catel.Logging;

using System;
using System.Collections.Generic;

public interface IInMemoryLoggingContainer
{
    int MaxCount { get; set; }

    IReadOnlyList<LogEntry> LogEntries { get; }

    event EventHandler<LogEntryEventArgs>? LogEntryAdded;

    void Add(LogEntry logEntry);
}

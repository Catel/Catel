namespace Catel.Logging;

using System;

public class LogEntryEventArgs : EventArgs
{
    public LogEntryEventArgs(LogEntry logEntry)
    {
        LogEntry = logEntry;
    }
        
    public LogEntry LogEntry { get; }
}

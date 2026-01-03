namespace Catel.Logging;

using System;
using Microsoft.Extensions.Logging;

public class LogEntry
{
    public required DateTimeOffset DateTime { get; init; }

    public required string Category { get; init; }

    public required LogLevel LogLevel { get; init; }

    public required string Message { get; init; }

    public override string ToString()
    {
        return Message;
    }
}

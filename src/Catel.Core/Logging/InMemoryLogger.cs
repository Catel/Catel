namespace Catel.Logging;

using System;
using Catel;
using Microsoft.Extensions.Logging;

public class InMemoryLogger : IInMemoryLogger
{
    private readonly IInMemoryLoggingContainer _inMemoryLoggingContainer;
    private readonly ITimeProvider _timeProvider;

    public InMemoryLogger(IInMemoryLoggingContainer inMemoryLoggingContainer, ITimeProvider timeProvider)
    {
        _inMemoryLoggingContainer = inMemoryLoggingContainer;
        _timeProvider = timeProvider;
    }

    public required string Category { get; set; }

    public IDisposable? BeginScope<TState>(TState state) 
        where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _inMemoryLoggingContainer.Add(new LogEntry
        {
            DateTime = _timeProvider.GetUtcNow(),
            Category = Category,
            LogLevel = logLevel,
            Message = formatter(state, exception)
        });
    }
}

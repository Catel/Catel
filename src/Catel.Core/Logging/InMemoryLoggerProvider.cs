namespace Catel.Logging;

using Catel;
using Microsoft.Extensions.Logging;

public sealed class InMemoryLoggerProvider : IInMemoryLoggerProvider
{
    private readonly IInMemoryLoggingContainer _inMemoryLoggingContainer;
    private readonly ITimeProvider _timeProvider;

    public InMemoryLoggerProvider(IInMemoryLoggingContainer inMemoryLoggingContainer, ITimeProvider timeProvider)
    {
        _inMemoryLoggingContainer = inMemoryLoggingContainer;
        _timeProvider = timeProvider;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new InMemoryLogger(_inMemoryLoggingContainer, _timeProvider)
        { 
            Category = categoryName
        };
    }

    public void Dispose()
    {
        
    }
}

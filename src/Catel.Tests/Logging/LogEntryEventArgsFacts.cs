namespace Catel.Tests.Logging;

using System;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

public class LogEntryEventArgsFacts
{
    [TestFixture]
    public class The_Constructor
    {
        [Test]
        public void Stores_LogEntry()
        {
            var logEntry = new LogEntry
            {
                Category = "category",
                DateTime = DateTimeOffset.UtcNow,
                LogLevel = LogLevel.Information,
                Message = "message"
            };

            var eventArgs = new LogEntryEventArgs(logEntry);

            Assert.That(eventArgs.LogEntry, Is.EqualTo(logEntry));
        }
    }
}

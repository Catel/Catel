namespace Catel.Tests.Logging
{
    using System;

    using Catel.Logging;
    using NUnit.Framework;

    [TestFixture]
    public class LogMessageEventArgsTest
    {
        [TestCase]
        public void Constructor_AutomaticDateTime()
        {
            var log = new Log(GetType());
            var eventArgs = new LogMessageEventArgs(log, "log message", 42, null, LogEvent.Error);

            Assert.That(eventArgs.Log, Is.EqualTo(log));
            Assert.That(eventArgs.Message, Is.EqualTo("log message"));
            Assert.That(eventArgs.ExtraData, Is.EqualTo(42));
            Assert.That(eventArgs.LogEvent, Is.EqualTo(LogEvent.Error));
            Assert.That((int)DateTime.Now.Subtract(eventArgs.Time).TotalSeconds, Is.EqualTo(0));
        }

        [TestCase]
        public void Constructor_ManualDateTime()
        {
            var log = new Log(GetType());
            var eventArgs = new LogMessageEventArgs(log, "log message", 42, null, LogEvent.Warning, DateTime.Today);

            Assert.That(eventArgs.Log, Is.EqualTo(log));
            Assert.That(eventArgs.Message, Is.EqualTo("log message"));
            Assert.That(eventArgs.ExtraData, Is.EqualTo(42));
            Assert.That(eventArgs.LogEvent, Is.EqualTo(LogEvent.Warning));
            Assert.That(eventArgs.Time, Is.EqualTo(DateTime.Today));
        }
    }
}
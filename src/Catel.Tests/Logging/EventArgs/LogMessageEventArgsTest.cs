// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessageEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Logging
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

            Assert.AreEqual(log, eventArgs.Log);
            Assert.AreEqual("log message", eventArgs.Message);
            Assert.AreEqual(42, eventArgs.ExtraData);
            Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
            Assert.AreEqual(0, (int)DateTime.Now.Subtract(eventArgs.Time).TotalSeconds);
        }

        [TestCase]
        public void Constructor_ManualDateTime()
        {
            var log = new Log(GetType());
            var eventArgs = new LogMessageEventArgs(log, "log message", 42, null, LogEvent.Warning, DateTime.Today);

            Assert.AreEqual(log, eventArgs.Log);
            Assert.AreEqual("log message", eventArgs.Message);
            Assert.AreEqual(42, eventArgs.ExtraData);
            Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
            Assert.AreEqual(DateTime.Today, eventArgs.Time);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessageEventArgsTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Logging
{
    using System;

    using Catel.Logging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class LogMessageEventArgsTest
    {
        [TestMethod]
        public void Constructor_AutomaticDateTime()
        {
            var log = new Log(GetType());
            var eventArgs = new LogMessageEventArgs(log, "log message", 42, LogEvent.Error);

            Assert.AreEqual(log, eventArgs.Log);
            Assert.AreEqual("log message", eventArgs.Message);
            Assert.AreEqual(42, eventArgs.ExtraData);
            Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
            Assert.AreEqual(0, (int)DateTime.Now.Subtract(eventArgs.Time).TotalSeconds);
        }

        [TestMethod]
        public void Constructor_ManualDateTime()
        {
            var log = new Log(GetType());
            var eventArgs = new LogMessageEventArgs(log, "log message", 42, LogEvent.Warning, DateTime.Today);

            Assert.AreEqual(log, eventArgs.Log);
            Assert.AreEqual("log message", eventArgs.Message);
            Assert.AreEqual(42, eventArgs.ExtraData);
            Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
            Assert.AreEqual(DateTime.Today, eventArgs.Time);
        }
    }
}
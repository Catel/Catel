// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatelLogFacts.cs" company="Catel development team">
//   Copyright (c) 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Logging
{
    using Catel.Logging;
    using NUnit.Framework;

    [TestFixture]
    public class CatelLogFacts
    {
        private CatelLog _logger;
        private CatelLog Logger
        {
            get { return _logger ?? (_logger = (CatelLog) LogManager.GetCatelLogger(typeof(CatelLogFacts), AlwaysLog)); }
            set { _logger = value; }
        }

        private bool AlwaysLog { get; set; }
        private ILogListener FakeListener { get; set; }

        /// <summary>
        /// Use Test_Initialize to run code before running each test.
        /// </summary>
        [SetUp]
        public void Test_Initialize()
        {
            AlwaysLog = true;
            FakeListener = LogManager.AddDebugListener();
            FakeListener.IgnoreCatelLogging = true;
        }

        /// <summary>
        /// Use Test_Cleanup to run code after each test has run.
        /// </summary>
        [TearDown]
        public void Test_Cleanup()
        {
            LogManager.RemoveLogger(typeof(CatelLogFacts).FullName);
            LogManager.RemoveListener(FakeListener);
            Logger = null;
        }

        [Test]
        public void Debug_AlwaysLogIsTrue_IgnoreCatelLogsIsTrue_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Debug;
            const string expectedMessage = "A message";

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }
        
        [Test]
        public void Debug_AlwaysLogIsFalse_IgnoreCatelLogsIsTrue_DoesNotWriteLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Debug;
            const string expectedMessage = "A message";

            AlwaysLog = false;

            FakeListener.LogMessage += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            // did not log to listener
        }

        [Test]
        public void Debug_AlwaysLogIsFalse_IgnoreCatelLogsIsFalse_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Debug;
            const string expectedMessage = "A message";

            AlwaysLog = false;
            FakeListener.IgnoreCatelLogging = false;

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsFalse(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Error_AlwaysLogIsTrue_IgnoreCatelLogsIsTrue_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Error;
            const string expectedMessage = "A message";

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Error_AlwaysLogIsFalse_IgnoreCatelLogsIsTrue_DoesNotWriteLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Error;
            const string expectedMessage = "A message";

            AlwaysLog = false;

            FakeListener.LogMessage += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            // did not log to listener
        }

        [Test]
        public void Error_AlwaysLogIsFalse_IgnoreCatelLogsIsFalse_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Error;
            const string expectedMessage = "A message";

            AlwaysLog = false;
            FakeListener.IgnoreCatelLogging = false;

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsFalse(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Info_AlwaysLogIsTrue_IgnoreCatelLogsIsTrue_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Info;
            const string expectedMessage = "A message";

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Info_AlwaysLogIsFalse_IgnoreCatelLogsIsTrue_DoesNotWriteLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Info;
            const string expectedMessage = "A message";

            AlwaysLog = false;

            FakeListener.LogMessage += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            // did not log to listener
        }

        [Test]
        public void Info_AlwaysLogIsFalse_IgnoreCatelLogsIsFalse_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Info;
            const string expectedMessage = "A message";

            AlwaysLog = false;
            FakeListener.IgnoreCatelLogging = false;

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsFalse(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Status_AlwaysLogIsTrue_IgnoreCatelLogsIsTrue_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Status;
            const string expectedMessage = "A message";

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Status_AlwaysLogIsFalse_IgnoreCatelLogsIsTrue_DoesNotWriteWriteLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Status;
            const string expectedMessage = "A message";

            AlwaysLog = false;

            FakeListener.LogMessage += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            // did not log to listener
        }

        [Test]
        public void Status_AlwaysLogIsFalse_IgnoreCatelLogsIsFalse_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Status;
            const string expectedMessage = "A message";

            AlwaysLog = false;
            FakeListener.IgnoreCatelLogging = false;

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsFalse(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Warning_AlwaysLogIsTrue_IgnoreCatelLogsIsTrue_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Warning;
            const string expectedMessage = "A message";

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }

        [Test]
        public void Warning_AlwaysLogIsFalse_IgnoreCatelLogsIsTrue_DoesNotWriteLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Warning;
            const string expectedMessage = "A message";

            AlwaysLog = false;

            FakeListener.LogMessage += (sender, e) =>
                {
                    Assert.Fail();
                };

            Assert.IsTrue(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            // did not log to listener
        }

        [Test]
        public void Warning_AlwaysLogIsFalse_IgnoreCatelLogsIsFalse_WritesLog()
        {
            // ARRANGE
            const LogEvent expectedLogEvent = LogEvent.Warning;
            const string expectedMessage = "A message";

            AlwaysLog = false;
            FakeListener.IgnoreCatelLogging = false;

            var receivedLog = false;
            FakeListener.LogMessage += (sender, e) =>
                {
                    receivedLog = true;
                    Assert.NotNull(e);
                    Assert.AreEqual(expectedLogEvent, e.LogEvent);
                    Assert.AreEqual(expectedMessage, e.Message);
                };

            Assert.IsFalse(FakeListener.IgnoreCatelLogging);

            // ACT
            Logger.Write(expectedLogEvent, expectedMessage);


            // ASSERT
            Assert.IsTrue(receivedLog);
        }
    }
}
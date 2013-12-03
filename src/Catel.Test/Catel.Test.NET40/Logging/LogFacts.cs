// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogTest.cs" company="Catel development team">
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

    public class LogFacts
    {
        #region Test classes
        public class ExceptionWithoutStringConstructor : Exception
        {
        }
        #endregion

#if WINDOWS_PHONE
        private const string ArgumentNullExceptionText = "[ArgumentNullException] System.ArgumentNullException: Value can not be null.";
#else
        private const string ArgumentNullExceptionText = "[ArgumentNullException] System.ArgumentNullException: Value cannot be null.";
#endif

        [TestClass]
        public class TheIndentMethod
        {
            [TestMethod]
            public void IncreasesIndentLevel()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                Assert.AreEqual(0, log.IndentLevel);

                log.Indent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [TestMethod]
            public void WritesMessagesWithIndent()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Indent();
                log.Info("Indented message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32]   Indented message", eventArgs.Message);
            }
        }

        [TestClass]
        public class TheUnindentMethod
        {
            [TestMethod]
            public void DecreasesIndentLevel()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int)) {IndentLevel = 2};

                Assert.AreEqual(2, log.IndentLevel);

                log.Unindent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [TestMethod]
            public void WriteMessagesWithUnIndent()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Indent();
                log.Info("Indented message");
                log.Unindent();
                log.Info("Unindented message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] Unindented message", eventArgs.Message);
            }
        }

        [TestClass]
        public class TheIndentLevelProperty
        {
            [TestMethod]
            public void DefaultsToZero()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                Assert.AreEqual(0, log.IndentLevel);
            }

            [TestMethod]
            public void SetsPositiveValue()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int)) {IndentSize = 5};

                Assert.AreEqual(5, log.IndentSize);
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                // TODO: IndentLevel should be settable
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => log.IndentLevel = -1);
            }
        }

        [TestClass]
        public class TheIndentSizeProperty
        {
            [TestMethod]
            public void DefaultsToTwo()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                Assert.AreEqual(2, log.IndentSize);
            }

            [TestMethod]
            public void SetsPositiveValue()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int)) {IndentSize = 5};

                Assert.AreEqual(5, log.IndentSize);
            }

            [TestMethod]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                // TODO: IndentSize should be settable
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => log.IndentSize = -1);
            }
        }

        [TestClass]
        public class TheConstructor
        {
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new Log(null));
            }

            [TestMethod]
            public void CreatesLogForType()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                Assert.AreEqual(typeof (int), log.TargetType);
            }
        }

        [TestClass]
        public class TheDebugMethod
        {
            [TestMethod]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof(int));

                log.Debug("This is a string with { and sometimes and ending }");
            }

            [TestMethod]
            public void Debug_Message_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Debug((string) null);
            }

            [TestMethod]
            public void Debug_Message()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Debug("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message", eventArgs.Message);
            }

            [TestMethod]
            public void Debug_MessageFormat_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Debug((string) null, null);
            }

            [TestMethod]
            public void Debug_MessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Debug("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message 1", eventArgs.Message);
            }

            [TestMethod]
            public void Debug_Exception_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug((Exception) null));
            }

            [TestMethod]
            public void Debug_Exception()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Debug_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug(null, string.Empty));
            }

            [TestMethod]
            public void Debug_ExceptionWithMessage_MessageNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null);
            }

            [TestMethod]
            public void Debug_ExceptionWithMessage()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Debug_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug(null, "additional message", 1));
            }

            [TestMethod]
            public void Debug_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null, 1);
            }

            [TestMethod]
            public void Debug_ExceptionWithMessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestClass]
        public class TheInfoMethod
        {
            [TestMethod]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof(int));

                log.Info("This is a string with { and sometimes and ending }");
            }

            [TestMethod]
            public void Info_Message_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Info((string) null);
            }

            [TestMethod]
            public void Info_Message()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Info("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message", eventArgs.Message);
            }

            [TestMethod]
            public void Info_MessageFormat_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Info((string) null, null);
            }

            [TestMethod]
            public void Info_MessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Info("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message 1", eventArgs.Message);
            }

            [TestMethod]
            public void Info_Exception_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info((Exception) null));
            }

            [TestMethod]
            public void Info_Exception()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Info_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info(null, string.Empty));
            }

            [TestMethod]
            public void Info_ExceptionWithMessage_MessageNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null);
            }

            [TestMethod]
            public void Info_ExceptionWithMessage()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Info_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info(null, "additional message", 1));
            }

            [TestMethod]
            public void Info_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null, 1);
            }

            [TestMethod]
            public void Info_ExceptionWithMessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestClass]
        public class TheWarningMethod
        {
            [TestMethod]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof(int));

                log.Warning("This is a string with { and sometimes and ending }");
            }

            [TestMethod]
            public void Warning_Message_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Warning((string) null);
            }

            [TestMethod]
            public void Warning_Message()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Warning("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message", eventArgs.Message);
            }

            [TestMethod]
            public void Warning_MessageFormat_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Warning((string) null, null);
            }

            [TestMethod]
            public void Warning_MessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Warning("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message 1", eventArgs.Message);
            }

            [TestMethod]
            public void Warning_Exception_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning((Exception) null));
            }

            [TestMethod]
            public void Warning_Exception()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Warning_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning(null, string.Empty));
            }

            [TestMethod]
            public void Warning_ExceptionWithMessage_MessageNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null);
            }

            [TestMethod]
            public void Warning_ExceptionWithMessage()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Warning_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning(null, "additional message", 1));
            }

            [TestMethod]
            public void Warning_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null, 1);
            }

            [TestMethod]
            public void Warning_ExceptionWithMessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestClass]
        public class TheErrorMethod
        {
            [TestMethod]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof(int));

                log.Error("This is a string with { and sometimes and ending }");
            }

            [TestMethod]
            public void Error_Message_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Error((string) null);
            }

            [TestMethod]
            public void Error_Message()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Error("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message", eventArgs.Message);
            }

            [TestMethod]
            public void Error_MessageFormat_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.Error((string) null, null);
            }

            [TestMethod]
            public void Error_MessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Error("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual("[System.Int32] log message 1", eventArgs.Message);
            }

            [TestMethod]
            public void Error_Exception_Null()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error((Exception) null));
            }

            [TestMethod]
            public void Error_Exception()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Error_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error(null, string.Empty));
            }

            [TestMethod]
            public void Error_ExceptionWithMessage_MessageNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null);
            }

            [TestMethod]
            public void Error_ExceptionWithMessage()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void Error_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error(null, "additional message", 1));
            }

            [TestMethod]
            public void Error_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null, 1);
            }

            [TestMethod]
            public void Error_ExceptionWithMessageFormat()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("[System.Int32] additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestMethod]
            public void ErrorAndThrowException_NullInput()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                log.ErrorAndThrowException<InvalidOperationException>(null);
            }

            [TestMethod]
            public void ErrorAndThrowException_ExceptionWithoutMessageConstructor()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => log.ErrorAndThrowException<ExceptionWithoutStringConstructor>("exception test"));
            }

            [TestMethod]
            public void ErrorAndThrowException_ExceptionWithMessageConstructor()
            {
                LogManager.RegisterDebugListener();
                var log = new Log(typeof (int));

                // Several tests to make sure we are not testing the NotSupportedException of the class itself
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => log.ErrorAndThrowException<InvalidOperationException>("exception test"));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.ErrorAndThrowException<ArgumentNullException>("exception test"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => log.ErrorAndThrowException<ArgumentException>("exception test"));
            }
        }
    }
}
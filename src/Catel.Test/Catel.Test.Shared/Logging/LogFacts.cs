// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Logging
{
    using System;
    using System.Collections.Generic;
    using Catel.Logging;
    using NUnit.Framework;

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

        [TestFixture]
        public class ThePerformance
        {
            [TestCase]
            public void LoggingPerformance()
            {
                var log = new Log(typeof (ThePerformance));

                var averageDuration = TimeMeasureHelper.MeasureAction(5000, "Log.Write", () => log.WriteWithData("this is a test", null, LogEvent.Error));

                Assert.IsTrue(averageDuration < 1d);
            }
        }

        [TestFixture]
        public class TheIndentMethod
        {
            [TestCase]
            public void IncreasesIndentLevel()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(0, log.IndentLevel);

                log.Indent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [TestCase]
            public void WritesMessagesWithIndent()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Indent();
                log.Info("Indented message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("  Indented message", eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheUnindentMethod
        {
            [TestCase]
            public void DecreasesIndentLevel()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentLevel = 2 };

                Assert.AreEqual(2, log.IndentLevel);

                log.Unindent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [TestCase]
            public void WriteMessagesWithUnIndent()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Indent();
                log.Info("Indented message");
                log.Unindent();
                log.Info("Unindented message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("Unindented message", eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheIndentLevelProperty
        {
            [TestCase]
            public void DefaultsToZero()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(0, log.IndentLevel);
            }

            [TestCase]
            public void SetsPositiveValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentSize = 5 };

                Assert.AreEqual(5, log.IndentSize);
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // TODO: IndentLevel should be settable
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => log.IndentLevel = -1);
            }
        }

        [TestFixture]
        public class TheIndentSizeProperty
        {
            [TestCase]
            public void DefaultsToTwo()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(2, log.IndentSize);
            }

            [TestCase]
            public void SetsPositiveValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentSize = 5 };

                Assert.AreEqual(5, log.IndentSize);
            }

            [TestCase]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // TODO: IndentSize should be settable
                ExceptionTester.CallMethodAndExpectException<ArgumentOutOfRangeException>(() => log.IndentSize = -1);
            }
        }

        [TestFixture]
        public class TheConstructor
        {
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new Log(null));
            }

            [TestCase]
            public void CreatesLogForType()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(typeof(int), log.TargetType);
            }
        }

        [TestFixture]
        public class TheDebugMethod
        {
            [TestCase]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug("This is a string with { and sometimes and ending }");
            }

            [TestCase]
            public void Debug_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug((string)null);
            }

            [TestCase]
            public void Debug_Message()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Debug("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);
            }

            [TestCase]
            public void Debug_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug((string)null, null);
            }

            [TestCase]
            public void Debug_MessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Debug("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual("log message 1", eventArgs.Message);
            }

            [TestCase]
            public void Debug_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug((Exception)null));
            }

            [TestCase]
            public void Debug_Exception()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("{0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Debug_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug(null, string.Empty));
            }

            [TestCase]
            public void Debug_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null);
            }

            [TestCase]
            public void Debug_ExceptionWithMessage()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Debug_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Debug(null, "additional message", 1));
            }

            [TestCase]
            public void Debug_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null, 1);
            }

            [TestCase]
            public void Debug_ExceptionWithMessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Debug(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheInfoMethod
        {
            [TestCase]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info("This is a string with { and sometimes and ending }");
            }

            [TestCase]
            public void Info_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info((string)null);
            }

            [TestCase]
            public void Info_Message()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Info("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);
            }

            [TestCase]
            public void Info_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info((string)null, null);
            }

            [TestCase]
            public void Info_MessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Info("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("log message 1", eventArgs.Message);
            }

            [TestCase]
            public void Info_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info((Exception)null));
            }

            [TestCase]
            public void Info_Exception()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("{0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Info_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info(null, string.Empty));
            }

            [TestCase]
            public void Info_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null);
            }

            [TestCase]
            public void Info_ExceptionWithMessage()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Info_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Info(null, "additional message", 1));
            }

            [TestCase]
            public void Info_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null, 1);
            }

            [TestCase]
            public void Info_ExceptionWithMessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Info(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheWarningMethod
        {
            [TestCase]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning("This is a string with { and sometimes and ending }");
            }

            [TestCase]
            public void Warning_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning((string)null);
            }

            [TestCase]
            public void Warning_Message()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Warning("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);
            }

            [TestCase]
            public void Warning_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning((string)null, null);
            }

            [TestCase]
            public void Warning_MessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Warning("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual("log message 1", eventArgs.Message);
            }

            [TestCase]
            public void Warning_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning((Exception)null));
            }

            [TestCase]
            public void Warning_Exception()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("{0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Warning_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning(null, string.Empty));
            }

            [TestCase]
            public void Warning_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null);
            }

            [TestCase]
            public void Warning_ExceptionWithMessage()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Warning_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Warning(null, "additional message", 1));
            }

            [TestCase]
            public void Warning_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null, 1);
            }

            [TestCase]
            public void Warning_ExceptionWithMessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Warning(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheErrorMethod
        {
            [TestCase]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error("This is a string with { and sometimes and ending }");
            }

            [TestCase]
            public void Error_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error((string)null);
            }

            [TestCase]
            public void Error_Message()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Error("log message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);
            }

            [TestCase]
            public void Error_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error((string)null, null);
            }

            [TestCase]
            public void Error_MessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.Error("log message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual("log message 1", eventArgs.Message);
            }

            [TestCase]
            public void Error_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error((Exception)null));
            }

            [TestCase]
            public void Error_Exception()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("{0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Error_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error(null, string.Empty));
            }

            [TestCase]
            public void Error_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null);
            }

            [TestCase]
            public void Error_ExceptionWithMessage()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception, "additional message");

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void Error_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.Error(null, "additional message", 1));
            }

            [TestCase]
            public void Error_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null, 1);
            }

            [TestCase]
            public void Error_ExceptionWithMessageFormat()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new ArgumentNullException("log test");
                log.Error(exception, "additional message {0}", 1);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual(string.Format("additional message 1 | {0}\r\nParameter name: log test", ArgumentNullExceptionText), eventArgs.Message);
            }

            [TestCase]
            public void ErrorAndThrowException_NullInput()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => log.ErrorAndThrowException<InvalidOperationException>(null));
            }

            [TestCase]
            public void ErrorAndThrowException_ExceptionWithoutMessageConstructor()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                ExceptionTester.CallMethodAndExpectException<NotSupportedException>(() => log.ErrorAndThrowException<ExceptionWithoutStringConstructor>("exception test"));
            }

            [TestCase]
            public void ErrorAndThrowException_ExceptionWithMessageConstructor()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // Several tests to make sure we are not testing the NotSupportedException of the class itself
                ExceptionTester.CallMethodAndExpectException<InvalidOperationException>(() => log.ErrorAndThrowException<InvalidOperationException>("exception test"));
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => log.ErrorAndThrowException<ArgumentNullException>("exception test"));
                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => log.ErrorAndThrowException<ArgumentException>("exception test"));
            }
        }

        [TestFixture]
        public class TheLogDataFunctionality
        {
            [TestCase]
            public void WorksWithoutData()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                log.InfoWithData("log message", null);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);

                var logData = eventArgs.LogData;

                Assert.IsNull(logData);
            }

            [TestCase]
            public void WorksWithData()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var threadId = ThreadHelper.GetCurrentThreadId();

                log.InfoWithData("log message", new LogData
                {
                    { "ThreadId", threadId }
                });

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("log message", eventArgs.Message);

                var logData = eventArgs.LogData;

                Assert.IsNotNull(logData);
                Assert.IsTrue(ObjectHelper.AreEqual(logData["ThreadId"], threadId));
            }
        }
    }
}
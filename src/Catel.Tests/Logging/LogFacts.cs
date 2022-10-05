// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Logging
{
    using System;
    using Catel.Logging;
    using NUnit.Framework;

    public class LogFacts
    {
        #region Test classes
        public class ExceptionWithoutStringConstructor : Exception
        {
        }
        #endregion

        private const string ArgumentNullExceptionText = "[ArgumentNullException] System.ArgumentNullException: Value cannot be null.";

        [TestFixture]
        public class ThePerformance
        {
            [Test]
            public void LoggingPerformance()
            {
                var log = new Log(typeof(ThePerformance));

                var averageDuration = TimeMeasureHelper.MeasureAction(5000, "Log.Write", () => log.WriteWithData("this is a test", null, LogEvent.Error));

                Assert.IsTrue(averageDuration < 1d);
            }
        }

        [TestFixture]
        public class TheIndentMethod
        {
            [Test]
            public void IncreasesIndentLevel()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(0, log.IndentLevel);

                log.Indent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [Test]
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
            [Test]
            public void DecreasesIndentLevel()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentLevel = 2 };

                Assert.AreEqual(2, log.IndentLevel);

                log.Unindent();

                Assert.AreEqual(1, log.IndentLevel);
            }

            [Test]
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
            [Test]
            public void DefaultsToZero()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(0, log.IndentLevel);
            }

            [Test]
            public void SetsPositiveValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentSize = 5 };

                Assert.AreEqual(5, log.IndentSize);
            }

            [Test]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // TODO: IndentLevel should be settable
                Assert.Throws<ArgumentOutOfRangeException>(() => log.IndentLevel = -1);
            }
        }

        [TestFixture]
        public class TheIndentSizeProperty
        {
            [Test]
            public void DefaultsToTwo()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(2, log.IndentSize);
            }

            [Test]
            public void SetsPositiveValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int)) { IndentSize = 5 };

                Assert.AreEqual(5, log.IndentSize);
            }

            [Test]
            public void ThrowsArgumentOutOfRangeForNegativeValue()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // TODO: IndentSize should be settable
                Assert.Throws<ArgumentOutOfRangeException>(() => log.IndentSize = -1);
            }
        }

        [TestFixture]
        public class TheConstructor
        {
            [Test]
            public void ThrowsArgumentNullExceptionForNullType()
            {
                Assert.Throws<ArgumentNullException>(() => new Log((Type) null));
            }

            [Test]
            public void ThrowsArgumentExceptionForNullString()
            {
                Assert.Throws<ArgumentException>(() => new Log((string)null));
            }

            [Test]
            public void ThrowsArgumentExceptionForWhitespaceString()
            {
                Assert.Throws<ArgumentException>(() => new Log(String.Empty));
            }

            [Test]
            public void ThrowsArgumentExceptionForNullString_WithStringAndType()
            {
                Assert.Throws<ArgumentException>(() => new Log(null, typeof(object)));
            }

            [Test]
            public void ThrowsArgumentExceptionForWhitespaceString_WithStringAndType()
            {
                Assert.Throws<ArgumentException>(() => new Log(String.Empty, typeof(object)));
            }

            [Test]
            public void CreatesLogForType()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.AreEqual(typeof(int), log.TargetType);
            }

            [Test]
            public void CreatesLogForString()
            {
                LogManager.AddDebugListener();
                var log = new Log("log");

                Assert.AreEqual("log", log.Name);
                Assert.AreEqual(typeof(object), log.TargetType);
            }

            [Test]
            public void CreatesLogForStringAndType()
            {
                LogManager.AddDebugListener();
                var log = new Log("log", typeof(int));

                Assert.AreEqual("log", log.Name);
                Assert.AreEqual(typeof(int), log.TargetType);
            }
        }

        [TestFixture]
        public class TheDebugMethod
        {
            [Test]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug("This is a string with { and sometimes and ending }");
            }

            [Test]
            public void Debug_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug((string)null);
            }

            [Test]
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

            [Test]
            public void Debug_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Debug((string)null, null);
            }

            [Test]
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

            [Test]
            public void Debug_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Debug((Exception)null));
            }

            [Test]
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
                Assert.AreEqual(string.Format("{0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Debug_Exception_Aggregated()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new AggregateException("log test", new []
                {
                    new ArgumentNullException("arg1"),
                    new ArgumentNullException("arg2"),
                });

                log.Debug(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Debug, eventArgs.LogEvent);
                Assert.AreEqual("[AggregateException] System.AggregateException: log test (Value cannot be null. (Parameter 'arg1')) (Value cannot be null. (Parameter 'arg2'))\r\n ---> System.ArgumentNullException: Value cannot be null. (Parameter 'arg1')\r\n   --- End of inner exception stack trace ---\r\n ---> (Inner Exception #1) System.ArgumentNullException: Value cannot be null. (Parameter 'arg2')<---\r\n", eventArgs.Message);
            }

            [Test]
            public void Debug_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Debug(null, string.Empty));
            }

            [Test]
            public void Debug_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Debug_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Debug(null, "additional message", 1));
            }

            [Test]
            public void Debug_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Debug(exception, null, 1);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message 1 | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheInfoMethod
        {
            [Test]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info("This is a string with { and sometimes and ending }");
            }

            [Test]
            public void Info_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info((string)null);
            }

            [Test]
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

            [Test]
            public void Info_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Info((string)null, null);
            }

            [Test]
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

            [Test]
            public void Info_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Info((Exception)null));
            }

            [Test]
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
                Assert.AreEqual(string.Format("{0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }


            [Test]
            public void Info_Exception_Aggregated()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new AggregateException("log test", new[]
                {
                    new ArgumentNullException("arg1"),
                    new ArgumentNullException("arg2"),
                });

                log.Info(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
                Assert.AreEqual("[AggregateException] System.AggregateException: log test (Value cannot be null. (Parameter 'arg1')) (Value cannot be null. (Parameter 'arg2'))\r\n ---> System.ArgumentNullException: Value cannot be null. (Parameter 'arg1')\r\n   --- End of inner exception stack trace ---\r\n ---> (Inner Exception #1) System.ArgumentNullException: Value cannot be null. (Parameter 'arg2')<---\r\n", eventArgs.Message);
            }

            [Test]
            public void Info_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Info(null, string.Empty));
            }

            [Test]
            public void Info_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Info_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Info(null, "additional message", 1));
            }

            [Test]
            public void Info_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Info(exception, null, 1);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message 1 | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheWarningMethod
        {
            [Test]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning("This is a string with { and sometimes and ending }");
            }

            [Test]
            public void Warning_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning((string)null);
            }

            [Test]
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

            [Test]
            public void Warning_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Warning((string)null, null);
            }

            [Test]
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

            [Test]
            public void Warning_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Warning((Exception)null));
            }

            [Test]
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
                Assert.AreEqual(string.Format("{0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Warning_Exception_Aggregated()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new AggregateException("log test", new[]
                {
                    new ArgumentNullException("arg1"),
                    new ArgumentNullException("arg2"),
                });

                log.Warning(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Warning, eventArgs.LogEvent);
                Assert.AreEqual("[AggregateException] System.AggregateException: log test (Value cannot be null. (Parameter 'arg1')) (Value cannot be null. (Parameter 'arg2'))\r\n ---> System.ArgumentNullException: Value cannot be null. (Parameter 'arg1')\r\n   --- End of inner exception stack trace ---\r\n ---> (Inner Exception #1) System.ArgumentNullException: Value cannot be null. (Parameter 'arg2')<---\r\n", eventArgs.Message);
            }

            [Test]
            public void Warning_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Warning(null, string.Empty));
            }

            [Test]
            public void Warning_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Warning_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Warning(null, "additional message", 1));
            }

            [Test]
            public void Warning_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Warning(exception, null, 1);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message 1 | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }
        }

        [TestFixture]
        public class TheErrorMethod
        {
            [Test]
            public void CorrectlyLogsMessageWithBraces()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error("This is a string with { and sometimes and ending }");
            }

            [Test]
            public void Error_Message_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error((string)null);
            }

            [Test]
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

            [Test]
            public void Error_MessageFormat_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                log.Error((string)null, null);
            }

            [Test]
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

            [Test]
            public void Error_Exception_Null()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Error((Exception)null));
            }

            [Test]
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
                Assert.AreEqual(string.Format("{0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Error_Exception_Aggregated()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                LogMessageEventArgs eventArgs = null;
                log.LogMessage += (sender, e) => eventArgs = e;

                var exception = new AggregateException("log test", new[]
                {
                    new ArgumentNullException("arg1"),
                    new ArgumentNullException("arg2"),
                });

                log.Error(exception);

                Assert.IsNotNull(eventArgs);
                Assert.AreEqual(log, eventArgs.Log);
                Assert.AreEqual(LogEvent.Error, eventArgs.LogEvent);
                Assert.AreEqual("[AggregateException] System.AggregateException: log test (Value cannot be null. (Parameter 'arg1')) (Value cannot be null. (Parameter 'arg2'))\r\n ---> System.ArgumentNullException: Value cannot be null. (Parameter 'arg1')\r\n   --- End of inner exception stack trace ---\r\n ---> (Inner Exception #1) System.ArgumentNullException: Value cannot be null. (Parameter 'arg2')<---\r\n", eventArgs.Message);
            }

            [Test]
            public void Error_ExceptionWithMessage_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Error(null, string.Empty));
            }

            [Test]
            public void Error_ExceptionWithMessage_MessageNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void Error_ExceptionWithMessageFormat_ExceptionNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ArgumentNullException>(() => log.Error(null, "additional message", 1));
            }

            [Test]
            public void Error_ExceptionWithMessageFormat_MessageFormatNull()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                var exception = new ArgumentNullException("log test");

                log.Error(exception, null, 1);
            }

            [Test]
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
                Assert.AreEqual(string.Format("additional message 1 | {0} (Parameter 'log test')", ArgumentNullExceptionText), eventArgs.Message);
            }

            [Test]
            public void ErrorAndCreateException_NullInput()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<InvalidOperationException>(() => { throw log.ErrorAndCreateException<InvalidOperationException>(null); });
            }

            [Test]
            public void ErrorAndCreateException_ExceptionWithoutMessageConstructor()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                Assert.Throws<ExceptionWithoutStringConstructor>(() => { throw log.ErrorAndCreateException<ExceptionWithoutStringConstructor>("exception test"); });
            }

            [Test]
            public void ErrorAndCreateException_ExceptionWithMessageConstructor()
            {
                LogManager.AddDebugListener();
                var log = new Log(typeof(int));

                // Several tests to make sure we are not testing the NotSupportedException of the class itself
                Assert.Throws<InvalidOperationException>(() => { throw log.ErrorAndCreateException<InvalidOperationException>("exception test"); });
                Assert.Throws<ArgumentNullException>(() => { throw log.ErrorAndCreateException<ArgumentNullException>("exception test"); });
                Assert.Throws<ArgumentException>(() => { throw log.ErrorAndCreateException<ArgumentException>("exception test"); });
            }
        }

        [TestFixture]
        public class TheLogDataFunctionality
        {
            [Test]
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

            [Test]
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

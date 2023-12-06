namespace Catel.Tests.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Logging;
    using NUnit.Framework;

    ///<summary>
    ///  This is a test class for LogManagerTest and is intended
    ///  to contain all LogManagerTest Unit Tests
    ///</summary>
    [TestFixture]
    public class LogManagerTest
    {
        #region Test classes
        public class TestLogListener : LogListenerBase
        {
            public static readonly ILog Log = LogManager.GetCurrentClassLogger();

            public int DebugCount { get; private set; }

            public int InfoCount { get; private set; }

            public int WarningCount { get; private set; }

            public int ErrorCount { get; private set; }

            public int StatusCount { get; private set; }

            protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, LogData logData, DateTime time)
            {
                if (log.TargetType != typeof(TestLogListener))
                {
                    return;
                }

                switch (logEvent)
                {
                    case LogEvent.Debug:
                        DebugCount++;
                        break;

                    case LogEvent.Info:
                        InfoCount++;
                        break;

                    case LogEvent.Warning:
                        WarningCount++;
                        break;

                    case LogEvent.Error:
                        ErrorCount++;
                        break;

                    case LogEvent.Status:
                        StatusCount++;
                        break;
                }
            }
        }
        #endregion

        [TestCase]
        public void LogMessageEvent()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);

            LogMessageEventArgs eventArgs = null;
            LogManager.LogMessage += (sender, e) => eventArgs = e;

            var log = LogManager.GetLogger(typeof(LogManager));
            log.Info("hello there");

            Assert.IsNotNull(eventArgs);
            Assert.That(eventArgs.Log, Is.EqualTo(log));
            Assert.That(eventArgs.LogEvent, Is.EqualTo(LogEvent.Info));
            Assert.That(eventArgs.Message, Is.EqualTo("hello there"));
        }

        [TestCase]
        public void GetListeners()
        {
            LogManager.ClearListeners();

            var listener1 = new DebugLogListener();
            var listener2 = new DebugLogListener();

            LogManager.AddListener(listener1);
            LogManager.AddListener(listener2);

            var listeners = LogManager.GetListeners();

            Assert.That(((List<ILogListener>)listeners)[0], Is.EqualTo(listener1));
            Assert.That(((List<ILogListener>)listeners)[1], Is.EqualTo(listener2));
        }

        [TestCase]
        public void IsListenerRegistered_Null()
        {
            Assert.Throws<ArgumentNullException>(() => LogManager.IsListenerRegistered(null));
        }

        [TestCase]
        public void IsListenerRegistered_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            Assert.That(LogManager.IsListenerRegistered(listener), Is.False);
        }

        [TestCase]
        public void IsListenerRegistered_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.True);
        }

        [TestCase]
        public void AddListener_Null()
        {
            Assert.Throws<ArgumentNullException>(() => LogManager.AddListener(null));
        }

        [TestCase]
        public void AddListener_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.True);
            Assert.That(LogManager.GetListeners().Count(), Is.EqualTo(1));
        }

        [TestCase]
        public void AddListener_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.True);
            Assert.That(LogManager.GetListeners().Count(), Is.EqualTo(1));

            LogManager.AddListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.True);
            Assert.That(LogManager.GetListeners().Count(), Is.EqualTo(2));
        }

        [TestCase]
        public void RemoveListener_Null()
        {
            Assert.Throws<ArgumentNullException>(() => LogManager.RemoveListener(null));
        }

        [TestCase]
        public void RemoveListener_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void RemoveListener_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.True);

            LogManager.RemoveListener(listener);
            Assert.That(LogManager.IsListenerRegistered(listener), Is.False);
        }

        [TestCase]
        public void ClearListeners()
        {
            var listener = new DebugLogListener();
            LogManager.AddListener(listener);

            Assert.That(LogManager.GetListeners().Count(), Is.Not.EqualTo(0));

            LogManager.ClearListeners();

            Assert.That(LogManager.GetListeners().Count(), Is.EqualTo(0));
        }

        [TestCase]
        public void GetCurrentClassLogger()
        {
            var logger = LogManager.GetCurrentClassLogger();

            Assert.That(logger.TargetType, Is.EqualTo(GetType()));
        }

        [TestCase]
        public void GetLogger_NullType()
        {
            Assert.Throws<ArgumentNullException>(() => LogManager.GetLogger((Type)null));
        }

        [TestCase]
        public void GetLogger_NullString()
        {
            Assert.Throws<ArgumentException>(() => LogManager.GetLogger((string)null));
        }

        [TestCase]
        public void GetLogger_EmptyString()
        {
            Assert.Throws<ArgumentException>(() => LogManager.GetLogger(String.Empty));
        }

        [TestCase]
        public void GetLogger_Type_CheckIfSameLogIsReturned()
        {
            var log1 = LogManager.GetLogger(typeof(int));
            var log2 = LogManager.GetLogger(typeof(int));

            Assert.That(ReferenceEquals(log1, log2), Is.True);
        }

        [TestCase]
        public void GetLogger_String_CheckIfSameLogIsReturned()
        {
            var log1 = LogManager.GetLogger("log");
            var log2 = LogManager.GetLogger("log");

            Assert.That(ReferenceEquals(log1, log2), Is.True);
        }

        [TestCase]
        public void Log_DefaultIsEnabledValues()
        {
            var listener = new TestLogListener();

            Assert.That(listener.IsDebugEnabled, Is.True);
            Assert.That(listener.IsInfoEnabled, Is.True);
            Assert.That(listener.IsWarningEnabled, Is.True);
            Assert.That(listener.IsErrorEnabled, Is.True);
            Assert.That(listener.IsStatusEnabled, Is.True);

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(1));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(1));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(1));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(1));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(1));

            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void Log_DebugNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsDebugEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(0));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(1));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(1));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(1));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(1));

            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void Log_InfoNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsInfoEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(1));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(0));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(1));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(1));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(1));

            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void Log_WarningNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsWarningEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(1));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(1));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(0));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(1));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(1));

            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void Log_ErrorNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsErrorEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(1));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(1));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(1));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(0));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(1));

            LogManager.RemoveListener(listener);
        }

        [TestCase]
        public void Log_StatusNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsStatusEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.That(listener.DebugCount, Is.EqualTo(1));

            TestLogListener.Log.Info("info");
            Assert.That(listener.InfoCount, Is.EqualTo(1));

            TestLogListener.Log.Warning("warning");
            Assert.That(listener.WarningCount, Is.EqualTo(1));

            TestLogListener.Log.Error("error");
            Assert.That(listener.ErrorCount, Is.EqualTo(1));

            TestLogListener.Log.Status("status");
            Assert.That(listener.StatusCount, Is.EqualTo(0));

            LogManager.RemoveListener(listener);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerTest.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Logging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    ///<summary>
    ///  This is a test class for LogManagerTest and is intended
    ///  to contain all LogManagerTest Unit Tests
    ///</summary>
    [TestClass]
    public class LogManagerTest
    {
        #region Test classes
        public class TestLogListener : LogListenerBase
        {
            public static ILog Log = LogManager.GetCurrentClassLogger();

            public int DebugCount { get; private set; }

            public int InfoCount { get; private set; }

            public int WarningCount { get; private set; }

            public int ErrorCount { get; private set; }

            protected override void Write(ILog log, string message, LogEvent logEvent, object extraData, DateTime time)
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
                }
            }
        }
        #endregion

        [TestMethod]
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
            Assert.AreEqual(log, eventArgs.Log);
            Assert.AreEqual(LogEvent.Info, eventArgs.LogEvent);
            Assert.AreEqual("hello there", eventArgs.Message);
        }

        [TestMethod]
        public void GetListeners()
        {
            LogManager.ClearListeners();

            var listener1 = new DebugLogListener();
            var listener2 = new DebugLogListener();

            LogManager.AddListener(listener1);
            LogManager.AddListener(listener2);

            var listeners = LogManager.GetListeners();

            Assert.AreEqual(listener1, ((List<ILogListener>)listeners)[0]);
            Assert.AreEqual(listener2, ((List<ILogListener>)listeners)[1]);
        }

        [TestMethod]
        public void IsListenerRegistered_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => LogManager.IsListenerRegistered(null));
        }

        [TestMethod]
        public void IsListenerRegistered_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            Assert.IsFalse(LogManager.IsListenerRegistered(listener));
        }

        [TestMethod]
        public void IsListenerRegistered_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.IsTrue(LogManager.IsListenerRegistered(listener));
        }

        [TestMethod]
        public void AddListener_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => LogManager.AddListener(null));
        }

        [TestMethod]
        public void AddListener_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.IsTrue(LogManager.IsListenerRegistered(listener));
            Assert.AreEqual(1, LogManager.GetListeners().Count());
        }

        [TestMethod]
        public void AddListener_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.IsTrue(LogManager.IsListenerRegistered(listener));
            Assert.AreEqual(1, LogManager.GetListeners().Count());

            LogManager.AddListener(listener);
            Assert.IsTrue(LogManager.IsListenerRegistered(listener));
            Assert.AreEqual(2, LogManager.GetListeners().Count());
        }

        [TestMethod]
        public void RemoveListener_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => LogManager.RemoveListener(null));
        }

        [TestMethod]
        public void RemoveListener_UnregisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.RemoveListener(listener);
        }

        [TestMethod]
        public void RemoveListener_RegisteredListener()
        {
            LogManager.ClearListeners();

            var listener = new DebugLogListener();
            LogManager.AddListener(listener);
            Assert.IsTrue(LogManager.IsListenerRegistered(listener));

            LogManager.RemoveListener(listener);
            Assert.IsFalse(LogManager.IsListenerRegistered(listener));
        }

        [TestMethod]
        public void ClearListeners()
        {
            var listener = new DebugLogListener();
            LogManager.AddListener(listener);

            Assert.AreNotEqual(0, LogManager.GetListeners().Count());

            LogManager.ClearListeners();

            Assert.AreEqual(0, LogManager.GetListeners().Count());
        }

        [TestMethod]
        public void GetCurrentClassLogger()
        {
            var logger = LogManager.GetCurrentClassLogger();

            Assert.AreEqual(GetType(), logger.TargetType);
        }

        [TestMethod]
        public void GetLogger_Null()
        {
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => LogManager.GetLogger(null));
        }

        [TestMethod]
        public void GetLogger_CheckIfSameLogIsReturned()
        {
            var log1 = LogManager.GetLogger(typeof(int));
            var log2 = LogManager.GetLogger(typeof(int));

            Assert.IsTrue(ReferenceEquals(log1, log2));
        }

        [TestMethod]
        public void Log_DefaultIsEnabledValues()
        {
            var listener = new TestLogListener();

            Assert.IsTrue(listener.IsDebugEnabled);
            Assert.IsTrue(listener.IsInfoEnabled);
            Assert.IsTrue(listener.IsWarningEnabled);
            Assert.IsTrue(listener.IsErrorEnabled);
            
            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.AreEqual(1, listener.DebugCount);

            TestLogListener.Log.Info("info");
            Assert.AreEqual(1, listener.InfoCount);

            TestLogListener.Log.Warning("warning");
            Assert.AreEqual(1, listener.WarningCount);

            TestLogListener.Log.Error("error");
            Assert.AreEqual(1, listener.ErrorCount);

            LogManager.RemoveListener(listener);
        }

        [TestMethod]
        public void Log_DebugNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsDebugEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.AreEqual(0, listener.DebugCount);

            TestLogListener.Log.Info("info");
            Assert.AreEqual(1, listener.InfoCount);

            TestLogListener.Log.Warning("warning");
            Assert.AreEqual(1, listener.WarningCount);

            TestLogListener.Log.Error("error");
            Assert.AreEqual(1, listener.ErrorCount);

            LogManager.RemoveListener(listener);
        }

        [TestMethod]
        public void Log_InfoNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsInfoEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.AreEqual(1, listener.DebugCount);

            TestLogListener.Log.Info("info");
            Assert.AreEqual(0, listener.InfoCount);

            TestLogListener.Log.Warning("warning");
            Assert.AreEqual(1, listener.WarningCount);

            TestLogListener.Log.Error("error");
            Assert.AreEqual(1, listener.ErrorCount);

            LogManager.RemoveListener(listener);
        }

        [TestMethod]
        public void Log_WarningNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsWarningEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.AreEqual(1, listener.DebugCount);

            TestLogListener.Log.Info("info");
            Assert.AreEqual(1, listener.InfoCount);

            TestLogListener.Log.Warning("warning");
            Assert.AreEqual(0, listener.WarningCount);

            TestLogListener.Log.Error("error");
            Assert.AreEqual(1, listener.ErrorCount);

            LogManager.RemoveListener(listener);
        }

        [TestMethod]
        public void Log_ErrorNotEnabled()
        {
            var listener = new TestLogListener();

            listener.IsErrorEnabled = false;

            LogManager.AddListener(listener);

            TestLogListener.Log.Debug("debug");
            Assert.AreEqual(1, listener.DebugCount);

            TestLogListener.Log.Info("info");
            Assert.AreEqual(1, listener.InfoCount);

            TestLogListener.Log.Warning("warning");
            Assert.AreEqual(1, listener.WarningCount);

            TestLogListener.Log.Error("error");
            Assert.AreEqual(0, listener.ErrorCount);

            LogManager.RemoveListener(listener);
        }
    }
}
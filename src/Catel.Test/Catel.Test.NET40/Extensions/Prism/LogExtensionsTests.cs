// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensionsTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Test.Extensions.Prism
{
    using System;

    using Catel.Logging;

    using Microsoft.Practices.Prism.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    /// <summary>
    /// The log extensions test.
    /// </summary>
    public class LogExtensionsTests
    {
        #region Nested type: TheDebugMethod

        /// <summary>
        /// The the debug method.
        /// </summary>
        [TestClass]
        public class TheDebugMethod
        {
            #region Methods

            /// <summary>
            /// Throws argument null exception when message format is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenMessageFormatIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Debug(Priority.High, null));
            }

            /// <summary>
            /// Throws argument null exception when exception is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Debug((Exception)null, Priority.High, null));
            }

            ///// <summary>
            ///// Puts the priority value into the message format.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat()
            //{
            //    var logMock = new Mock<ILog>();
            //    logMock.Object.Debug(Priority.High, string.Empty);
            //    logMock.Verify(log => log.Debug(It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            ///// <summary>
            ///// Puts the priority value into the message format 2.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat2()
            //{
            //    var logMock = new Mock<ILog>();
            //    var exception = new Exception();
            //    logMock.Object.Debug(exception, Priority.High, string.Empty);
            //    logMock.Verify(log => log.Debug(It.Is<Exception>(e => ReferenceEquals(e, exception)), It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            #endregion
        }
        #endregion

        #region Nested type: TheErrorMethod

        /// <summary>
        /// The the error method.
        /// </summary>
        [TestClass]
        public class TheErrorMethod
        {
            #region Methods

            /// <summary>
            /// Throws argument null exception when message format is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenMessageFormatIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Error(Priority.High, null));
            }

            /// <summary>
            /// Throws argument null exception when exception is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Error((Exception)null, Priority.High, null));
            }

            ///// <summary>
            ///// Put the priority value into the message format.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat()
            //{
            //    var logMock = new Mock<ILog>();
            //    logMock.Object.Error(Priority.High, string.Empty);
            //    logMock.Verify(log => log.Error(It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            ///// <summary>
            ///// Puts the priority value into the message format 2.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat2()
            //{
            //    var logMock = new Mock<ILog>();
            //    var exception = new Exception();
            //    logMock.Object.Error(exception, Priority.High, string.Empty);
            //    logMock.Verify(log => log.Error(It.Is<Exception>(e => ReferenceEquals(e, exception)), It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            #endregion
        }
        #endregion

        #region Nested type: TheInfoMethod

        /// <summary>
        /// The the info method.
        /// </summary>
        [TestClass]
        public class TheInfoMethod
        {
            #region Methods

            /// <summary>
            /// Throws argument null exception when message format is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenMessageFormatIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Info(Priority.High, null));
            }

            /// <summary>
            /// Throws argument null exception when exception is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Info((Exception)null, Priority.High, null));
            }

            ///// <summary>
            ///// Puts the priority value into the message format.
            ///// </summary>
            //[TestMethod]
            //public void PutThePriorityValueIntoTheMessageFormat()
            //{
            //    var logMock = new Mock<ILog>();
            //    logMock.Object.Info(Priority.High, string.Empty);
            //    logMock.Verify(log => log.Info(It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            ///// <summary>
            ///// Puts the priority value into the message format 2.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat2()
            //{
            //    var logMock = new Mock<ILog>();
            //    var exception = new Exception();
            //    logMock.Object.Info(exception, Priority.High, string.Empty);
            //    logMock.Verify(log => log.Info(It.Is<Exception>(e => ReferenceEquals(e, exception)), It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            #endregion
        }
        #endregion

        #region Nested type: TheWarningMethod

        /// <summary>
        /// The warning method.
        /// </summary>
        [TestClass]
        public class TheWarningMethod
        {
            #region Methods

            /// <summary>
            /// Throws argument null exception when message format is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenMessageFormatIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Warning(Priority.High, null));
            }

            /// <summary>
            /// Throws argument null when exception exception is null.
            /// </summary>
            [TestMethod]
            public void ThrowsArgumentNullExceptionWhenExceptionIsNull()
            {
                var logMock = new Mock<ILog>();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => logMock.Object.Warning((Exception)null, Priority.High, null));
            }

            ///// <summary>
            ///// Put the priority value into the message format.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat()
            //{
            //    var logMock = new Mock<ILog>();
            //    logMock.Object.Warning(Priority.High, string.Empty);
            //    logMock.Verify(log => log.Warning(It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            ///// <summary>
            ///// Put the priority value into the message format 2.
            ///// </summary>
            //[TestMethod]
            //public void PutsThePriorityValueIntoTheMessageFormat2()
            //{
            //    var logMock = new Mock<ILog>();
            //    var exception = new Exception();
            //    logMock.Object.Warning(exception, Priority.High, string.Empty);
            //    logMock.Verify(log => log.Warning(It.Is<Exception>(e => ReferenceEquals(e, exception)), It.Is<string>(s => s.Contains("High"))), Times.Once());
            //}

            #endregion
        }
        #endregion
    }
}
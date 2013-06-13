// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.ExceptionHandling
{
    using System;
    using Catel.ExceptionHandling;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ExceptionHandlerFacts
    {
        #region Nested type: TheConstructor
        [TestClass]
        public class TheConstructor
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForExceptionTypeNullParameter()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ExceptionHandler(null, exception => { }));
            }

            [TestMethod]
            public void ThrowsArgumentNullExceptionForActionNullParameter()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ExceptionHandler(typeof (Exception), null));
            }

            [TestMethod]
            public void SetTypeValueInExceptionProperty()
            {
                var type = typeof (Exception);
                var handler = new ExceptionHandler(type, exception => { });
                Assert.AreEqual(type, handler.Exception);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheHandleMethod
        [TestClass]
        public class TheHandleMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForHandleNullParameter()
            {
                var handler = new ExceptionHandler(typeof (Exception), exception => { });
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => handler.Handle(null));
            }

            [TestMethod]
            public void PerformsHandle()
            {
                var originalException = new ArgumentException("achieved");
                var message = string.Empty;
                var handler = new ExceptionHandler(typeof (ArgumentException), exception => { message = exception.Message; });
                handler.Handle(originalException);

                Assert.AreEqual("achieved", message);
            }
            #endregion
        }
        #endregion
    }
}
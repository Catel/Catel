// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionHandlerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.ExceptionHandling
{
    using System;
    using Catel.ExceptionHandling;

    using NUnit.Framework;

    public class ExceptionHandlerFacts
    {
        #region Nested type: TheConstructor
        [TestFixture]
        public class TheConstructor
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForExceptionTypeNullParameter()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ExceptionHandler(null, exception => { }));
            }

            [TestCase]
            public void ThrowsArgumentNullExceptionForActionNullParameter()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ExceptionHandler(typeof (Exception), null));
            }

            [TestCase]
            public void SetTypeValueInExceptionProperty()
            {
                var type = typeof (Exception);
                var handler = new ExceptionHandler(type, exception => { });
                Assert.AreEqual(type, handler.ExceptionType);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheHandleMethod
        [TestFixture]
        public class TheHandleMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForHandleNullParameter()
            {
                var handler = new ExceptionHandler(typeof (Exception), exception => { });
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => handler.Handle(null));
            }

            [TestCase]
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.ExceptionHandling
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Catel.ExceptionHandling;
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;

#endif

    public class ExceptionServiceFacts
    {
        #region Nested type: TheConstructor
        [TestClass]
        public class TheConstructor
        {
            #region Methods
            [TestMethod]
            public void ChecksIfTheExceptionHandlersHasAnyItems()
            {
                var exceptionService = new ExceptionService();
                Assert.IsNotNull(exceptionService.ExceptionHandlers);
                Assert.IsFalse(exceptionService.ExceptionHandlers.Any());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGenericProcessMethod
        [TestClass]
        public class TheGenericProcessMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullParameterInGeneric()
            {
                var exceptionService = new ExceptionService();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => exceptionService.Process<int>(null));
            }

            [TestMethod]
            public void ProceedToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });
                value = exceptionService.Process(() => (1 + 1).ToString(CultureInfo.InvariantCulture));

                Assert.AreEqual("2", value);

                exceptionService.Process<string>(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestMethod]
            public void ProceedToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });
                exceptionService.Process<string>(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheHandleExceptionMethod
        [TestClass]
        public class TheHandleExceptionMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullParameter()
            {
                var exceptionService = new ExceptionService();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => exceptionService.HandleException(null));
            }

            [TestMethod]
            public void PerformsHandleForNotRegisteredTypeViaInheritance()
            {
                var exceptionService = new ExceptionService();
                var originalException = new DivideByZeroException("achieved");
                var value = string.Empty;
                exceptionService.Register<Exception>(exception => { value = exception.Message; });

                Assert.IsTrue(exceptionService.HandleException(originalException));

                Assert.AreEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsExceptionRegisteredMethod
        [TestClass]
        public class TheIsExceptionRegisteredMethod
        {
            #region Methods
            [TestMethod]
            public void ReturnsArgumentNullException()
            {
                var exceptionService = new ExceptionService();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => exceptionService.IsExceptionRegistered(null));
            }

            [TestMethod]
            public void ReturnsFalseWhenNotRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { });

                Assert.IsFalse(exceptionService.IsExceptionRegistered(typeof (Exception)));
            }

            [TestMethod]
            public void ReturnsFalseWhenNotRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { });

                Assert.IsFalse(exceptionService.IsExceptionRegistered<Exception>());
            }

            [TestMethod]
            public void ReturnsTrueWhenRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { });

                exceptionService.Register<ArgumentNullException>(exception => { });

                Assert.IsTrue(exceptionService.IsExceptionRegistered(typeof (ArgumentNullException)));
            }

            [TestMethod]
            public void ReturnsTrueWhenRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { });

                exceptionService.Register<ArgumentNullException>(exception => { });

                Assert.IsTrue(exceptionService.IsExceptionRegistered<Exception>());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheNonGenericProcessMethod
        [TestClass]
        public class TheNonGenericProcessMethod
        {
            #region Methods
            [TestMethod]
            public void ThrowsArgumentNullExceptionForNullParameterInNonGeneric()
            {
                var exceptionService = new ExceptionService();
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => exceptionService.Process(null));
            }

            [TestMethod]
            public void ProceedToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });
                exceptionService.Process(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestMethod]
            public void ProceedToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });
                exceptionService.Process(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheOnErrorRetryImmediatelyMethod
        [TestClass]
        public class TheOnErrorRetryImmediatelyMethod
        {
            #region Methods
            [TestMethod]
            public void ShouldCallBackOnRetrying()
            {
                var exceptionService = new ExceptionService();

                var index = 0;

                exceptionService.RetryingAction += (sender, args) => index++;

                exceptionService
                    .Register<DivideByZeroException>(exception => { })
                    .OnErrorRetryImmediately(2);

                exceptionService.ProcessWithRetry(() => { throw new DivideByZeroException(); });

                Assert.AreEqual(2, index);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheOnErrorRetryMethod
        [TestClass]
        public class TheOnErrorRetryMethod
        {
        }
        #endregion

        #region Nested type: TheRegisterMethod
        [TestClass]
        public class TheRegisterMethod
        {
            #region Methods
            [TestMethod]
            public void RegistersException()
            {
                var exceptionService = new ExceptionService();
                Assert.IsNotNull(exceptionService.ExceptionHandlers);
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);

                exceptionService.Register<ArgumentException>(exception => { });

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);
            }

            [TestMethod]
            public void RegistersExceptionForDoubleRegistration()
            {
                var exceptionService = new ExceptionService();
                Assert.IsNotNull(exceptionService.ExceptionHandlers);
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);

                exceptionService.Register<ArgumentException>(exception => { });
                exceptionService.Register<ArgumentException>(exception => { });

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheUnregisterMethod
        [TestClass]
        public class TheUnregisterMethod
        {
            #region Methods
            [TestMethod]
            public void UnregistersException()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentException>(exception => { });

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);

                Assert.IsTrue(exceptionService.Unregister<ArgumentException>());

                Assert.IsFalse(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);
            }

            [TestMethod]
            public void UnregistersExceptionForDoubleUnregistration()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentException>(exception => { });

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);

                Assert.IsTrue(exceptionService.Unregister<ArgumentException>());
                Assert.IsFalse(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);

                Assert.IsFalse(exceptionService.Unregister<ArgumentException>());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheUsingToleranceMethod
        [TestClass]
        public class TheUsingToleranceMethod
        {
            #region Methods
            [TestMethod]
            public void MultipleExceptionsOfSameTypeThrownTooManyTimesProducesOnlyOneException()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<DivideByZeroException>(exception => { })
                    .UsingTolerance(9, TimeSpan.FromSeconds(10.0));

                var index = 0;
                var exceptionHandledAt10Th = false;

                for (; index < 10; index++)
                {
                    ThreadHelper.Sleep(100);
                    exceptionHandledAt10Th = exceptionService.HandleException(new DivideByZeroException());
                }

                Assert.IsTrue(exceptionHandledAt10Th);
                Assert.AreEqual(10, index);
            }
            #endregion
        }
        #endregion
    }
}
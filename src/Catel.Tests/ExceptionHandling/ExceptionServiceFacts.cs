// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.ExceptionHandling
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Catel.ExceptionHandling;
    using System.Threading.Tasks;
    using NUnit.Framework;
    
    public class ExceptionServiceFacts
    {
        #region Nested type: TheConstructor
        [TestFixture]
        public class TheConstructor
        {
            #region Methods
            [TestCase]
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
        [TestFixture]
        public class TheGenericProcessMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParameterInGeneric()
            {
                var exceptionService = new ExceptionService();
                Assert.Throws<ArgumentNullException>(() => exceptionService.Process<int>(null));
            }

            [TestCase]
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

            [TestCase]
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

        #region Nested type: TheRegisterWithPredicateMethod
        [TestFixture]
        public class TheRegisterWithPredicateMethod
        {
            #region Methods
            [TestCase]
            public void ShouldHandleTheGoodExceptionTheFluentWay()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<CodeException>(
                    exception => Assert.AreEqual(3, exception.Code)
                    , exception => exception.Code == 3);

                exceptionService.Register<DivideByZeroException>(
                    exception => Assert.AreEqual("trying to divide by zero", exception.Message));

                Assert.Throws<CodeException>(() => exceptionService.Process(() => { throw new CodeException(2); }));
                exceptionService.Process(() => { throw new DivideByZeroException("trying to divide by zero"); });
                exceptionService.Process(() => { throw new CodeException(3); });
            }

            [TestCase]
            public void ShouldHandleTheGoodExceptionUsingCustomHandler()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<CodeExceptionHandler>();

                exceptionService.Register<DivideByZeroException>(
                    exception => Assert.AreEqual("trying to divide by zero", exception.Message));

                Assert.Throws<CodeException>(() => exceptionService.Process(() => { throw new CodeException(2); }));
                exceptionService.Process(() => { throw new DivideByZeroException("trying to divide by zero"); });
                exceptionService.Process(() => { throw new CodeException(3); });
            }
            #endregion

            private class CodeException : Exception
            {
                public int Code { get; private set; }

                public CodeException(int code)
                {
                    Code = code;
                }
            }

            private class CodeExceptionHandler : ExceptionHandler<CodeException>
            {
                public override void OnException(CodeException exception)
                {
                    
                }

                public override Func<CodeException, bool> GetFilter()
                {
                    return exception => exception.Code == 3;
                }
            }
        }
        #endregion

        #region Nested type: TheGenericProcessAsyncMethod
        [TestFixture]
        public class TheGenericProcessAsyncMethod
        {
            #region Methods

            [TestCase]
            public async Task ProceedActionToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                value = await exceptionService.ProcessAsync(() => (1 + 1).ToString(CultureInfo.InvariantCulture)).ConfigureAwait(false);
                Assert.AreEqual("2", value);

                await exceptionService.ProcessAsync<string>(() => { throw new ArgumentException("achieved"); }).ConfigureAwait(false);

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedTaskToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });
                value = await exceptionService.ProcessAsync(async () => (1 + 1).ToString(CultureInfo.InvariantCulture));
                Assert.AreEqual("2", value);

                await exceptionService.ProcessAsync<string>(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedActionToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });

                await exceptionService.ProcessAsync<string>(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedTaskToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; });

                await exceptionService.ProcessAsync<string>( async () => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }
            #endregion
        }

        #endregion

        #region Nested type: TheRegisterHandlerMethod
        [TestFixture]
        public class TheRegisterHandlerMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParameter()
            {
                var exceptionService = new ExceptionService();
                Assert.Throws<ArgumentNullException>(() => exceptionService.Register((IExceptionHandler)null));
            }

            [TestCase]
            public void ProceedToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                var argumentExceptionHandler = new ExceptionHandler(typeof(ArgumentException), exception => { value = exception.Message; });

                exceptionService.Register(argumentExceptionHandler);
                value = exceptionService.Process(() => (1 + 1).ToString(CultureInfo.InvariantCulture));

                Assert.AreEqual("2", value);

                exceptionService.Process<string>(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public void ProceedToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                var argumentExceptionHandler = new ExceptionHandler(typeof(ArgumentException), exception => { value = exception.Message; });

                exceptionService.Register(argumentExceptionHandler);
                exceptionService.Process<string>(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }

            [TestCase]
            public void ShouldSucceedToHandleUsingRegisteredHandler()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<DivideByZeroExceptionHandler>();

                exceptionService.Process(() => { throw new DivideByZeroException("trying to divide by zero"); });
            }
            #endregion

            private class DivideByZeroExceptionHandler : ExceptionHandler<DivideByZeroException>
            {
                public override void OnException(DivideByZeroException exception)
                {
                    Assert.AreEqual("trying to divide by zero", exception.Message);
                }
            }
        }
        #endregion

        #region Nested type: TheGetHandlerMethod
        [TestFixture]
        public class TheGetHandlerMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsArgumentNullException()
            {
                var exceptionService = new ExceptionService();

                Assert.Throws<ArgumentNullException>(() => exceptionService.GetHandler(null));
            }

            [TestCase]
            public void ReturnsNullWhenNotRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsNull(exceptionService.GetHandler(typeof (Exception)));
            }

            [TestCase]
            public void ReturnsNullWhenNotRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsNull(exceptionService.GetHandler<Exception>());
            }

            [TestCase]
            public void ReturnsHandlerWhenRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { }, null);
                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                var handler = exceptionService.GetHandler(typeof (ArgumentNullException));

                Assert.IsNotNull(handler);
                Assert.AreEqual(typeof (ArgumentNullException), handler.ExceptionType);
            }

            [TestCase]
            public void ReturnsHandlerWhenRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { }, null);
                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                var handler = exceptionService.GetHandler<Exception>();

                Assert.IsNotNull(handler);
                Assert.AreEqual(typeof (Exception), handler.ExceptionType);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheHandleExceptionMethod
        [TestFixture]
        public class TheHandleExceptionMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParameter()
            {
                var exceptionService = new ExceptionService();
                Assert.Throws<ArgumentNullException>(() => exceptionService.HandleException(null));
            }

            [TestCase]
            public void PerformsHandleForNotRegisteredTypeViaInheritance()
            {
                var exceptionService = new ExceptionService();
                var originalException = new DivideByZeroException("achieved");
                var value = string.Empty;
                exceptionService.Register<Exception>(exception => { value = exception.Message; }, null);

                Assert.IsTrue(exceptionService.HandleException(originalException));

                Assert.AreEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheIsExceptionRegisteredMethod
        [TestFixture]
        public class TheIsExceptionRegisteredMethod
        {
            #region Methods
            [TestCase]
            public void ReturnsArgumentNullException()
            {
                var exceptionService = new ExceptionService();

                Assert.Throws<ArgumentNullException>(() => exceptionService.IsExceptionRegistered(null));
            }

            [TestCase]
            public void ReturnsFalseWhenNotRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsFalse(exceptionService.IsExceptionRegistered(typeof (Exception)));
            }

            [TestCase]
            public void ReturnsFalseWhenNotRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsFalse(exceptionService.IsExceptionRegistered<Exception>());
            }

            [TestCase]
            public void ReturnsTrueWhenRegistered()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { }, null);
                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsTrue(exceptionService.IsExceptionRegistered(typeof (ArgumentNullException)));
            }

            [TestCase]
            public void ReturnsTrueWhenRegisteredUsingGeneric()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<Exception>(exception => { }, null);
                exceptionService.Register<ArgumentNullException>(exception => { }, null);

                Assert.IsTrue(exceptionService.IsExceptionRegistered<Exception>());
            }
            #endregion
        }
        #endregion

        #region Nested type: TheNonGenericProcessMethod
        [TestFixture]
        public class TheNonGenericProcessMethod
        {
            #region Methods
            [TestCase]
            public void ThrowsArgumentNullExceptionForNullParameterInNonGeneric()
            {
                var exceptionService = new ExceptionService();
                Assert.Throws<ArgumentNullException>(() => exceptionService.Process(null));
            }

            [TestCase]
            public void ProceedToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                exceptionService.Process(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public void ProceedToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                exceptionService.Process(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheNonGenericProcessAsyncMethod
        [TestFixture]
        public class TheNonGenericProcessAsyncMethod
        {
            #region Methods

            [TestCase]
            public async Task ProceedActionToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                await exceptionService.ProcessAsync(() => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedTaskToSucceed()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                await exceptionService.ProcessAsync(async () => { throw new ArgumentException("achieved"); });

                Assert.AreEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedActionToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                await exceptionService.ProcessAsync(() => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }

            [TestCase]
            public async Task ProceedTaskToFail()
            {
                var exceptionService = new ExceptionService();
                var value = string.Empty;

                exceptionService.Register<ArgumentException>(exception => { value = exception.Message; }, null);
                await exceptionService.ProcessAsync(async () => { throw new ArgumentOutOfRangeException("achieved"); });

                Assert.AreNotEqual("achieved", value);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheOnErrorRetryImmediatelyMethod
        [TestFixture]
        public class TheOnErrorRetryImmediatelyMethod
        {
            #region Methods
            [TestCase]
            public void ChecksIfRetryActionEventRegistrationWorks()
            {
                const int attemptsCount = 1;

                var exceptionService = new ExceptionService();

                exceptionService.RetryingAction += (sender, args) => Assert.AreEqual(attemptsCount, args.CurrentRetryCount);

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetryImmediately(attemptsCount);

                exceptionService.ProcessWithRetry(() => { throw new DivideByZeroException(); });
            }

            [TestCase]
            public void ShouldRetryTwice()
            {
                var attemptsCount = 0;

                var exceptionService = new ExceptionService();

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetryImmediately(2);

                exceptionService.ProcessWithRetry(() =>
                {
                    attemptsCount++;
                    throw new DivideByZeroException();
                });

                Assert.AreEqual(2, attemptsCount);
            }

            [TestCase]
            public void ShouldNotRetryWhenAnotherExceptionTypeIsThrown()
            {
                var exceptionService = new ExceptionService();

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetryImmediately(3);

                var attemptsCount = 0;

                Assert.Throws<ArgumentException>(() => exceptionService.ProcessWithRetry(() =>
                {
                    attemptsCount++;
                    throw new ArgumentException();
                }));
            }

            [TestCase]
            public void ShouldNotRetryWhenNotAnyExceptionIsThrown()
            {
                var attemptsCount = 0;

                var exceptionService = new ExceptionService();

                exceptionService.RetryingAction += (sender, args) => attemptsCount++;

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetryImmediately(3);

                exceptionService.ProcessWithRetry(() => { });

                Assert.AreEqual(0, attemptsCount);
            }

            [TestCase]
            public void ShouldNotRetryAndReturnsResult()
            {
                var attemptsCount = 0;

                var exceptionService = new ExceptionService();

                exceptionService.RetryingAction += (sender, args) => attemptsCount++;

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetryImmediately(3);

                var result = exceptionService.ProcessWithRetry(() => 1 + 1);

                Assert.AreEqual(0, attemptsCount);
                Assert.AreEqual(2, result);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheOnErrorRetryMethod
        [TestFixture]
        public class TheOnErrorRetryMethod
        {
            #region Methods
            [TestCase]
            public void ShouldRetryWithDelay()
            {
                var interval = TimeSpan.FromMilliseconds(10);

                var exceptionService = new ExceptionService();

                exceptionService.RetryingAction += (sender, args) => Assert.AreEqual(interval, args.Delay);

                exceptionService
                    .Register<DivideByZeroException>(exception => { }, null)
                    .OnErrorRetry(2, interval);

                exceptionService.ProcessWithRetry(() => { throw new DivideByZeroException(); });
            }
            #endregion
        }
        #endregion

        #region Nested type: TheRegisterMethod
        [TestFixture]
        public class TheRegisterMethod
        {
            #region Methods
            [TestCase]
            public void RegistersException()
            {
                var exceptionService = new ExceptionService();
                Assert.IsNotNull(exceptionService.ExceptionHandlers);
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);

                exceptionService.Register<ArgumentException>(exception => { }, null);

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);
            }

            [TestCase]
            public void RegistersExceptionForDoubleRegistration()
            {
                var exceptionService = new ExceptionService();
                Assert.IsNotNull(exceptionService.ExceptionHandlers);
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);

                exceptionService.Register<ArgumentException>(exception => { }, null);
                exceptionService.Register<ArgumentException>(exception => { }, null);

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);
            }
            #endregion
        }
        #endregion

        #region Nested type: TheUnregisterMethod
        [TestFixture]
        public class TheUnregisterMethod
        {
            #region Methods
            [TestCase]
            public void UnregistersException()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentException>(exception => { }, null);

                Assert.IsTrue(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 1);

                Assert.IsTrue(exceptionService.Unregister<ArgumentException>());

                Assert.IsFalse(exceptionService.ExceptionHandlers.ToList().Any(row => row.ExceptionType == typeof (ArgumentException)));
                Assert.AreEqual(exceptionService.ExceptionHandlers.Count(), 0);
            }

            [TestCase]
            public void UnregistersExceptionForDoubleUnregistration()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<ArgumentException>(exception => { }, null);

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
        [TestFixture]
        public class TheUsingToleranceMethod
        {
            #region Methods
            [TestCase]
            public void MultipleExceptionsOfSameTypeThrownTooManyTimesProducesOnlyOneException()
            {
                var exceptionService = new ExceptionService();

                exceptionService.Register<DivideByZeroException>(exception => { }, null)
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

            [TestCase]
            public void ChecksIfTheBufferedEventRegistrationWorks()
            {
                var buffercount = 0;

                var exceptionService = new ExceptionService();

                exceptionService.ExceptionBuffered += (sender, args) =>
                {
                    Assert.IsInstanceOf(typeof(DivideByZeroException), args.BufferedException);
                    buffercount++;
                };

                exceptionService.Register<DivideByZeroException>(exception => { }, null)
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
                Assert.AreEqual(9, buffercount);
            }
            #endregion
        }
        #endregion

        #region Nested type: CheckSorting
        [TestFixture]
        public class CheckSorting
        {
            private ExceptionService _exceptionService;
            private bool _exLevel0;
            private bool _exLevel21;
            private bool _exLevel11;
            private bool _exLevel31;
            private bool _exLevel32;
            private bool _exLevel22;

            [SetUp]
            public void Setup()
            {
                _exceptionService = new ExceptionService();

                _exceptionService.Register<Exception>(exception => { _exLevel0 = true; }, null);
                _exceptionService.Register<Level21Exception>(exception => { _exLevel21 = true; }, null);
                _exceptionService.Register<Level11Exception>(exception => { _exLevel11 = true; }, null);
                _exceptionService.Register<Level31Exception>(exception => { _exLevel31 = true; }, null);
                _exceptionService.Register<Level32Exception>(exception => { _exLevel32 = true; }, null);
                _exceptionService.Register<Level22Exception>(exception => { _exLevel22 = true; }, null);
                _exLevel0 = false;
                _exLevel21 = false;
                _exLevel11 = false;
                _exLevel31 = false;
                _exLevel32 = false;
                _exLevel22 = false;

            }

            [TestCase]
            public void PerformHandleExceptionLevel0()
            {
                var originalException = new Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsTrue(_exLevel0);
                Assert.IsFalse(_exLevel11);
                Assert.IsFalse(_exLevel21);
                Assert.IsFalse(_exLevel31);
                Assert.IsFalse(_exLevel32);
                Assert.IsFalse(_exLevel22);
            }

            [TestCase]
            public void PerformHandleExceptionLevel11()
            {
                var originalException = new Level11Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsFalse(_exLevel0);
                Assert.IsTrue(_exLevel11);
                Assert.IsFalse(_exLevel21);
                Assert.IsFalse(_exLevel31);
                Assert.IsFalse(_exLevel32);
                Assert.IsFalse(_exLevel22);
            }

            [TestCase]
            public void PerformHandleExceptionLevel21()
            {
                var originalException = new Level21Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsFalse(_exLevel0);
                Assert.IsFalse(_exLevel11);
                Assert.IsTrue(_exLevel21);
                Assert.IsFalse(_exLevel31);
                Assert.IsFalse(_exLevel32);
                Assert.IsFalse(_exLevel22);
            }

            [TestCase]
            public void PerformHandleExceptionLevel31()
            {
                var originalException = new Level31Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsFalse(_exLevel0);
                Assert.IsFalse(_exLevel11);
                Assert.IsFalse(_exLevel21);
                Assert.IsTrue(_exLevel31);
                Assert.IsFalse(_exLevel32);
                Assert.IsFalse(_exLevel22);
            }

            [TestCase]
            public void PerformHandleExceptionLevel32()
            {
                var originalException = new Level32Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsFalse(_exLevel0);
                Assert.IsFalse(_exLevel11);
                Assert.IsFalse(_exLevel21);
                Assert.IsFalse(_exLevel31);
                Assert.IsTrue(_exLevel32);
                Assert.IsFalse(_exLevel22);
            }

            [TestCase]
            public void PerformHandleExceptionLevel22()
            {
                var originalException = new Level22Exception();
                Assert.IsTrue(_exceptionService.HandleException(originalException));

                Assert.IsFalse(_exLevel0);
                Assert.IsFalse(_exLevel11);
                Assert.IsFalse(_exLevel21);
                Assert.IsFalse(_exLevel31);
                Assert.IsFalse(_exLevel32);
                Assert.IsTrue(_exLevel22);
            }

            private class Level11Exception : Exception { }
            private class Level21Exception : Level11Exception { }
            private class Level31Exception : Level21Exception { }
            private class Level22Exception : Level11Exception { }
            private class Level32Exception : Level22Exception { }
        }
        #endregion
    }
}
namespace Catel.Tests.Factories
{
    using System;
    using NUnit.Framework;

    public class ExceptionFactoryFacts
    {
        [TestFixture]
        public class TheCreateExceptionMethod
        {
            public class TestExceptionWithInnerExceptionSupport : Exception
            {
                public TestExceptionWithInnerExceptionSupport(string message)
                    : base(message)
                {
                }

                public TestExceptionWithInnerExceptionSupport(string message, Exception innerException)
                    : base(message, innerException)
                {
                }
            }

            [TestCase]
            public void ReturnsNullWhenExceptionCannotBeConstructed()
            {
                var exception = ExceptionFactory.CreateException<TestExceptionWithInnerExceptionSupport>(null);

                Assert.IsNull(exception);
            }

            [TestCase]
            public void ConstructsExceptionWithMessageWhenSpecifiedOnlyMessage()
            {
                var exception = ExceptionFactory.CreateException<TestExceptionWithInnerExceptionSupport>("msg", null);

                Assert.AreEqual("msg", exception.Message);
                Assert.IsNull(exception.InnerException);
            }

            [TestCase]
            public void ConstructsExceptionWithMessageAndInnerWhenSpecifiedBoth()
            {
                var innerException = new Exception();
                var exception = ExceptionFactory.CreateException<TestExceptionWithInnerExceptionSupport>("msg", innerException);

                Assert.AreEqual("msg", exception.Message);
                Assert.AreEqual(innerException, exception.InnerException);
            }
        }
    }
}
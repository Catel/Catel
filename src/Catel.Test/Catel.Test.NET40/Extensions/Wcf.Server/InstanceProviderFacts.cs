namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using Catel.IoC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServiceModel.Dispatching;
    using Services;

    public class InstanceProviderFacts
    {
        [TestClass]
        public class TheConstructorMethod
        {
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(null, typeof(ITestService), typeof(TestService)));
            }

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsContractType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, null, typeof(TestService)));
            }

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, typeof(ITestService), null));
            }
        }

        [TestClass]
        public class TheGetInstanceMethod
        {
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsInstanceContext()
            {
                var instanceProvider = new InstanceProvider(ServiceLocator.Default, typeof (ITestService), typeof (TestService));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => instanceProvider.GetInstance(null));
            }
        }

        [TestClass]
        public class TheReleaseInstanceMethod
        {
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsInstanceContext()
            {
                var instanceProvider = new InstanceProvider(ServiceLocator.Default, typeof(ITestService), typeof(TestService));

                var instance = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => instanceProvider.ReleaseInstance(null, instance));
            }
        }
    }
}
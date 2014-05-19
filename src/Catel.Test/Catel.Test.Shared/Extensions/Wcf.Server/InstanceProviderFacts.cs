// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceProviderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServiceModel.Dispatching;
    using Services;
    using Catel.IoC;

    public class InstanceProviderFacts
    {
        #region Nested type: TheConstructorMethod
        [TestClass]
        public class TheConstructorMethod
        {
            #region Methods
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(null, typeof (ITestService), typeof (TestService)));
            }

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsContractType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, null, typeof (TestService)));
            }

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, typeof (ITestService), null));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetInstanceMethod
        [TestClass]
        public class TheGetInstanceMethod
        {
            #region Methods
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsInstanceContext()
            {
                var instanceProvider = new InstanceProvider(ServiceLocator.Default, typeof (ITestService), typeof (TestService));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => instanceProvider.GetInstance(null));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheReleaseInstanceMethod
        [TestClass]
        public class TheReleaseInstanceMethod
        {
            #region Methods
            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsInstanceContext()
            {
                var instanceProvider = new InstanceProvider(ServiceLocator.Default, typeof (ITestService), typeof (TestService));

                var instance = new object();

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => instanceProvider.ReleaseInstance(null, instance));
            }
            #endregion
        }
        #endregion
    }
}

#endif
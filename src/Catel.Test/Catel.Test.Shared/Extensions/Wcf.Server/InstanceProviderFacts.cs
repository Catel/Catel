// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceProviderFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using NUnit.Framework;
    using ServiceModel.Dispatching;
    using Services;
    using Catel.IoC;

    public class InstanceProviderFacts
    {
        #region Nested type: TheConstructorMethod
        [TestFixture]
        public class TheConstructorMethod
        {
            #region Methods
            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(null, typeof (ITestService), typeof (TestService)));
            }

            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsContractType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, null, typeof (TestService)));
            }

            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new InstanceProvider(ServiceLocator.Default, typeof (ITestService), null));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheGetInstanceMethod
        [TestFixture]
        public class TheGetInstanceMethod
        {
            #region Methods
            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsInstanceContext()
            {
                var instanceProvider = new InstanceProvider(ServiceLocator.Default, typeof (ITestService), typeof (TestService));

                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => instanceProvider.GetInstance(null));
            }
            #endregion
        }
        #endregion

        #region Nested type: TheReleaseInstanceMethod
        [TestFixture]
        public class TheReleaseInstanceMethod
        {
            #region Methods
            [TestCase]
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
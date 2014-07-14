// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using System.Linq;
    using Catel.IoC;
    using NUnit.Framework;
    using ServiceModel;
    using Services;

    public class ServiceHostFacts
    {
        [TestFixture]
        public class TheConstructorMethod
        {
            private readonly Uri[] _endpoints = { new Uri("http://localhost") };

            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ServiceHost(null, typeof(TestService), _endpoints));
            }

            [TestCase]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ServiceHost(ServiceLocator.Default, null, _endpoints));
            }

            [TestCase]
            public void ShouldSucceedToCreateInstance()
            {
                var serviceType = typeof (TestService);

                var serviceHost = new ServiceHost(ServiceLocator.Default, serviceType, _endpoints);

                Assert.AreEqual(serviceHost.ServiceType, serviceType);

                Assert.IsTrue(serviceHost.Description.Behaviors.Any());
            }
        }
    }
}

#endif
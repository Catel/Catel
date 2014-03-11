// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using System.Linq;
    using Catel.IoC;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ServiceModel;
    using Services;

    public class ServiceHostFacts
    {
        [TestClass]
        public class TheConstructorMethod
        {
            private readonly Uri[] _endpoints = { new Uri("http://localhost") };

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceLocator()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ServiceHost(null, typeof(TestService), _endpoints));
            }

            [TestMethod]
            public void ShouldThrowArgumentNullExceptionForNullAsServiceType()
            {
                ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => new ServiceHost(ServiceLocator.Default, null, _endpoints));
            }

            [TestMethod]
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
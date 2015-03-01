// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHostFactoryFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Extensions.Wcf.Server
{
    using System;
    using System.ServiceModel;
    using System.Web.Hosting;
    using NUnit.Framework;
    using ServiceModel;
    using Services;
    using ServiceHost = System.ServiceModel.ServiceHost;

    public class ServiceHostFactoryFacts
    {
        [TestFixture]
        public class TheCreateServiceHostMethod
        {
#if NET45
            private readonly Uri[] _endpoints = { new Uri("http://localhost") };

            [SetUp]
            public void SetUp()
            {
                if (HostingEnvironment.IsHosted)
                {
                    return;
                }

                new HostingEnvironment();


                ServiceHostingEnvironment.EnsureInitialized();

            }

            [TestCase]
            public void ShouldTrowArgumentExceptionForNullValueAsConstructorString()
            {
                var serviceHostFactory = new ServiceHostFactory();

                var exception = ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceHostFactory.CreateServiceHost(null, _endpoints));

                Assert.AreEqual(exception.ParamName, "constructorString");
            }

            [TestCase]
            public void ShouldTrowArgumentExceptionForNullValueAsEndPoints()
            {
                var serviceHostFactory = new ServiceHostFactory();

                var exception = ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceHostFactory.CreateServiceHost("Service", null));

                Assert.AreEqual(exception.ParamName, "baseAddresses");
            }

            [TestCase]
            public void ShouldTrowArgumentExceptionForEmptyValueAsConstructorString()
            {
                var serviceHostFactory = new ServiceHostFactory();

                var exception = ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => serviceHostFactory.CreateServiceHost(string.Empty, _endpoints));

                Assert.AreEqual(exception.ParamName, "constructorString");
            }

            [TestCase]
            public void ShouldSucceedToCreateServiceHostInstanceWithAssemblyQualifiedNameOfATypeAsConstructorString()
            {
                var serviceHostFactory = new ServiceHostFactory();

                var sertviceType = typeof (TestService);

                var constructorString = sertviceType.AssemblyQualifiedName;

                var serviceHost = serviceHostFactory.CreateServiceHost(constructorString, _endpoints);

                Assert.IsInstanceOf(typeof(ServiceHost), serviceHost);
                Assert.AreEqual(sertviceType, serviceHost.Description.ServiceType);
            }

            [TestCase]
            public void ShoulFailToCreateServiceHostInstanceWithFullNameOfATypeAsConstructorString()
            {
                var serviceHostFactory = new ServiceHostFactory();

                var sertviceType = typeof(TestService);

                var constructorString = sertviceType.FullName;

                var serviceHost = serviceHostFactory.CreateServiceHost(constructorString, _endpoints);

                Assert.IsInstanceOf(typeof(ServiceHost), serviceHost);
                Assert.AreEqual(sertviceType, serviceHost.Description.ServiceType);
            }
#endif
        }
    }
}

#endif
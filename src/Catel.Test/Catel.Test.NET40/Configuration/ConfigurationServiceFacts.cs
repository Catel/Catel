// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Configuration
{
    using System;
    using Catel.Configuration;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class ConfigurationServiceFacts
    {
        [TestClass]
        public class TheGetValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(null));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(string.Empty));
            }

            [TestMethod]
            public void ReturnsExistingValue()
            {
                var configurationService = new ConfigurationService();

                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }

            [TestMethod]
            public void ReturnsDefaultValueForNonExistingValue()
            {
                var configurationService = new ConfigurationService();

                Assert.AreEqual("nonExistingValue", configurationService.GetValue("nonExistingKey", "nonExistingValue"));
            }
        }

        [TestClass]
        public class TheSetValueMethod
        {
            [TestMethod]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(null, "value"));
            }

            [TestMethod]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(string.Empty, "value"));
            }

            [TestMethod]
            public void SetsValueCorrectly()
            {
                var configurationService = new ConfigurationService();
 
                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }
        }

        [TestClass]
        public class TheConfigurationChangedEvent
        {
            [TestMethod]
            public void IsInvokedDuringSetValueMethod()
            {
                var configurationService = new ConfigurationService();

                bool invoked = false;
                configurationService.ConfigurationChanged += (sender, e) => invoked = true;

                configurationService.SetValue("key", "value");

                Assert.IsTrue(invoked);
            }
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Configuration
{
    using System;
    using Catel.Configuration;

    using NUnit.Framework;

    public class ConfigurationServiceFacts
    {
        [TestFixture]
        public class TheGetValueMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(string.Empty));
            }

            [TestCase]
            public void ReturnsExistingValue()
            {
                var configurationService = new ConfigurationService();

                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }

            [TestCase]
            public void ReturnsDefaultValueForNonExistingValue()
            {
                var configurationService = new ConfigurationService();

                Assert.AreEqual("nonExistingValue", configurationService.GetValue("nonExistingKey", "nonExistingValue"));
            }
        }

        [TestFixture]
        public class TheSetValueMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(null, "value"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = new ConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(string.Empty, "value"));
            }

            [TestCase]
            public void SetsValueCorrectly()
            {
                var configurationService = new ConfigurationService();
 
                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }
        }

        [TestFixture]
        public class TheConfigurationChangedEvent
        {
            [TestCase]
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
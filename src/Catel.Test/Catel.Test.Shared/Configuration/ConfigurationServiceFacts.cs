// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Configuration
{
    using System;
    using Catel.Configuration;

    using NUnit.Framework;
    using Catel.Runtime.Serialization;
    using Catel.Services;

    public class ConfigurationServiceFacts
    {
        private static ConfigurationService GetConfigurationService()
        {
            return new ConfigurationService(new SerializationManager(), new ObjectConverterService());
        }

        [TestFixture]
        public class TheGetValueMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(null));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(string.Empty));
            }

            [TestCase]
            public void ReturnsExistingValue()
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }

            [TestCase]
            public void ReturnsDefaultValueForNonExistingValue()
            {
                var configurationService = GetConfigurationService();

                Assert.AreEqual("nonExistingValue", configurationService.GetValue("nonExistingKey", "nonExistingValue"));
            }

            [TestCase]
            public void ReturnsValueForKeyWithSpecialCharacters()
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue("key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue("key with special chars", "nonExistingValue"));
            }
        }

        [TestFixture]
        public class TheSetValueMethod
        {
            [TestCase]
            public void ThrowsArgumentExceptionForNullKey()
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(null, "value"));
            }

            [TestCase]
            public void ThrowsArgumentExceptionForEmptyKey()
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(string.Empty, "value"));
            }

            [TestCase]
            public void SetsValueCorrectly()
            {
                var configurationService = GetConfigurationService();
 
                configurationService.SetValue("myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("myKey"));
            }

            [TestCase]
            public void SetsValueCorrectlyForKeyWithSpecialCharacters()
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue("key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>("key with special chars"));
            }
        }

        [TestFixture]
        public class TheConfigurationChangedEvent
        {
            [TestCase]
            public void IsInvokedDuringSetValueMethod()
            {
                var configurationService = GetConfigurationService();

                bool invoked = false;
                configurationService.ConfigurationChanged += (sender, e) => invoked = true;

                configurationService.SetValue("key", "value");

                Assert.IsTrue(invoked);
            }
        }
    }
}
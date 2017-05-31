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
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForNullKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(container, null));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForEmptyKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.GetValue<string>(container, string.Empty));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ReturnsExistingValue(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue(container, "myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ReturnsDefaultValueForNonExistingValue(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.AreEqual("nonExistingValue", configurationService.GetValue(container, "nonExistingKey", "nonExistingValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ReturnsValueForKeyWithSpecialCharacters(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue(container, "key with special chars", "nonExistingValue"));
            }
        }

        [TestFixture]
        public class TheSetValueMethod
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForNullKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(container, null, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForEmptyKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => configurationService.SetValue(container, string.Empty, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void SetsValueCorrectly(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();
 
                configurationService.SetValue(container, "myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void SetsValueCorrectlyForKeyWithSpecialCharacters(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "key with special chars"));
            }
        }

        [TestFixture]
        public class TheConfigurationChangedEvent
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void IsInvokedDuringSetValueMethod(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                var invoked = false;
                var receivedContainer = ConfigurationContainer.Roaming;
                string receivedKey = null;
                object receivedValue = null;

                configurationService.ConfigurationChanged += (sender, e) =>
                {
                    invoked = true;
                    receivedContainer = e.Container;
                    receivedKey = e.Key;
                    receivedValue = e.NewValue;
                };

                var guid = Guid.NewGuid();

                configurationService.SetValue(container, "key", guid.ToString());

                Assert.IsTrue(invoked);
                Assert.AreEqual(container, receivedContainer);
                Assert.AreEqual("key", receivedKey);
                Assert.AreEqual(guid.ToString(), (string)receivedValue);
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void IsNotInvokedDuringSetValueMethodForEqualValues(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();
                var invoked = false;

                configurationService.SetValue(container, "key", "value");

                configurationService.ConfigurationChanged += (sender, e) =>
                {
                    invoked = true;
                };

                configurationService.SetValue(container, "key", "value");

                Assert.IsFalse(invoked);
            }
        }
    }
}
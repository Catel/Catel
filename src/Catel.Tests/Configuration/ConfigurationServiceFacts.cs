// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationServiceFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Configuration
{
    using System;
    using Catel.Configuration;

    using NUnit.Framework;
    using Catel.Runtime.Serialization;
    using Catel.Services;
    using Catel.Runtime.Serialization.Xml;
    using Catel.IO;
    using System.IO;
    using System.Threading.Tasks;

    public partial class ConfigurationServiceFacts
    {
        private class TestConfigurationService : ConfigurationService
        {
            private readonly string _name;

            public TestConfigurationService(string name, ISerializationManager serializationManager, IObjectConverterService objectConverterService, IXmlSerializer serializer, IAppDataService appDataService) 
                : base(serializationManager, objectConverterService, serializer, appDataService)
            {
                _name = name;
            }

            public bool CreateDelayDuringSave { get; set; }

            protected override string GetConfigurationFileName(ApplicationDataTarget applicationDataTarget)
            {
                var configFileName = base.GetConfigurationFileName(applicationDataTarget);

                if (!string.IsNullOrWhiteSpace(_name))
                {
                    configFileName = $"{System.IO.Path.GetFileNameWithoutExtension(configFileName)}.{_name}.xml";
                }

                return configFileName;
            }

            protected override async void SaveConfiguration(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
            {
                base.SaveConfiguration(container, configuration, fileName);

                if (CreateDelayDuringSave)
                {
                    using (File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        await Task.Delay(1000);
                    }
                }
            }
        }

        private static TestConfigurationService GetConfigurationService(string name = null)
        {
            return new TestConfigurationService(name, new SerializationManager(), new ObjectConverterService(), SerializationFactory.GetXmlSerializer(), new AppDataService());
        }

        [TestFixture]
        public class TheGetValueMethod
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForNullKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, null));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForEmptyKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, string.Empty));
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

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, null, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public void ThrowsArgumentExceptionForEmptyKey(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, string.Empty, "value"));
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

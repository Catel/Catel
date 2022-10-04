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

            public TestConfigurationService(string name, IObjectConverterService objectConverterService, IXmlSerializer serializer, IAppDataService appDataService) 
                : base(objectConverterService, serializer, appDataService)
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

            protected override async Task SaveConfigurationAsync(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
            {
                await base.SaveConfigurationAsync(container, configuration, fileName);

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
            return new TestConfigurationService(name, new ObjectConverterService(), SerializationFactory.GetXmlSerializer(), new AppDataService());
        }

        [TestFixture]
        public class The_GetValueAsync_Method
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForNullKeyAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.ThrowsAsync<ArgumentException>(async () => await configurationService.GetValueAsync<string>(container, null));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForEmptyKeyAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.ThrowsAsync<ArgumentException>(async () => await configurationService.GetValueAsync<string>(container, string.Empty));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsExistingValueAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                await configurationService.SetValueAsync(container, "myKey", "myValue");

                Assert.AreEqual("myValue", await configurationService.GetValueAsync<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsDefaultValueForNonExistingValueAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.AreEqual("nonExistingValue", await configurationService.GetValueAsync(container, "nonExistingKey", "nonExistingValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsValueForKeyWithSpecialCharactersAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                await configurationService.SetValueAsync(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", await configurationService.GetValueAsync(container, "key with special chars", "nonExistingValue"));
            }
        }

        [TestFixture]
        public class The_SetValueAsync_Method
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForNullKeyAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.ThrowsAsync<ArgumentException>(async () => await configurationService.SetValueAsync(container, null, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForEmptyKeyAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                Assert.ThrowsAsync<ArgumentException>(async () => await configurationService.SetValueAsync(container, string.Empty, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task SetsValueCorrectlyAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();
 
                await configurationService.SetValueAsync(container, "myKey", "myValue");

                Assert.AreEqual("myValue", await configurationService.GetValueAsync<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task SetsValueCorrectlyForKeyWithSpecialCharactersAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();

                await configurationService.SetValueAsync(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", await configurationService.GetValueAsync<string>(container, "key with special chars"));
            }
        }

        [TestFixture]
        public class The_ConfigurationChanged_Event
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task IsInvokedDuringSetValueMethodAsync(ConfigurationContainer container)
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

                await configurationService.SetValueAsync(container, "key", guid.ToString());

                Assert.IsTrue(invoked);
                Assert.AreEqual(container, receivedContainer);
                Assert.AreEqual("key", receivedKey);
                Assert.AreEqual(guid.ToString(), (string)receivedValue);
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task IsNotInvokedDuringSetValueMethodForEqualValuesAsync(ConfigurationContainer container)
            {
                var configurationService = GetConfigurationService();
                var invoked = false;

                await configurationService.SetValueAsync(container, "key", "value");

                configurationService.ConfigurationChanged += (sender, e) =>
                {
                    invoked = true;
                };

                await configurationService.SetValueAsync(container, "key", "value");

                Assert.IsFalse(invoked);
            }
        }
    }
}

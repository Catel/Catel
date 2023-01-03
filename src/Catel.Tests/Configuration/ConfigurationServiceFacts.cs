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
    using System.ComponentModel;

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

            public async Task<DynamicConfiguration> PublicLoadConfigurationAsync(string fileName)
            {
                return await base.LoadConfigurationAsync(fileName);
            }
        }

        private static async Task<TestConfigurationService> GetConfigurationServiceAsync(string name = null)
        {
            var configurationService = new TestConfigurationService(name, new ObjectConverterService(), SerializationFactory.GetXmlSerializer(), new AppDataService());

            await configurationService.LoadAsync();

            return configurationService;
        }

        [TestFixture]
        public class The_LoadConfigurationAsync_Method
        {
            [Test]
            public async Task IgnoresEmptyFilesAsync()
            {
                var configurationService = await GetConfigurationServiceAsync();
                var fileName = System.IO.Path.GetTempFileName();

                try
                {
                    using (File.Create(fileName))
                    {
                    }

                    var config = await configurationService.PublicLoadConfigurationAsync(fileName);

                    Assert.IsNotNull(config);
                }
                finally
                {
                    File.Delete(fileName);
                }
            }
        }

        [TestFixture]
        public class The_GetValue_Method
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForNullKeyAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, null));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForEmptyKeyAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, string.Empty));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsExistingValueAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsDefaultValueForNonExistingValueAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.AreEqual("nonExistingValue", configurationService.GetValue(container, "nonExistingKey", "nonExistingValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ReturnsValueForKeyWithSpecialCharactersAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue(container, "key with special chars", "nonExistingValue"));
            }
        }

        [TestFixture]
        public class The_SetValue_Method
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForNullKeyAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, null, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task ThrowsArgumentExceptionForEmptyKeyAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, string.Empty, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task SetsValueCorrectlyAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "myKey", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "myKey"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task SetsValueCorrectlyForKeyWithSpecialCharactersAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.AreEqual("myValue", configurationService.GetValue<string>(container, "key with special chars"));
            }
        }

        [TestFixture]
        public class The_ConfigurationChanged_Event
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task IsInvokedDuringSetValueMethodAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

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
            public async Task IsNotInvokedDuringSetValueMethodForEqualValuesAsync(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();
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

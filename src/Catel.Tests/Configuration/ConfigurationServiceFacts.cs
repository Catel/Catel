namespace Catel.Tests.Configuration
{
    using System;
    using Catel.Configuration;

    using NUnit.Framework;
    using Catel.Services;
    using Catel.IO;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging.Abstractions;

    public partial class ConfigurationServiceFacts
    {
        private class TestConfigurationService : ConfigurationService
        {
            private readonly string _name;

            public TestConfigurationService(string name, IObjectConverterService objectConverterService, IAppDataService appDataService)
                : base(new NullLogger<ConfigurationService>(), objectConverterService, appDataService, new ConfigurationBuilder())
            {
                _name = name;
            }

            public bool CreateDelayDuringSave { get; set; }

            public int SaveConfigurationCallCount { get; private set; }

            protected override string GetConfigurationFileName(ApplicationDataTarget applicationDataTarget)
            {
                var configFileName = base.GetConfigurationFileName(applicationDataTarget);

                if (!string.IsNullOrWhiteSpace(_name))
                {
                    var directory = System.IO.Path.GetDirectoryName(configFileName);
                    var fileName = $"{System.IO.Path.GetFileNameWithoutExtension(configFileName)}.{_name}.json";

                    configFileName = System.IO.Path.Combine(directory, fileName);
                }

                return configFileName;
            }

            protected override async Task SaveConfigurationAsync(ConfigurationContainer container, IConfiguration configuration, string fileName)
            {
                SaveConfigurationCallCount++;

                await base.SaveConfigurationAsync(container, configuration, fileName);

                if (CreateDelayDuringSave)
                {
                    using (File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        await Task.Delay(1000);
                    }
                }
            }

            public async Task<IConfiguration> PublicLoadConfigurationAsync(ConfigurationContainer container, string fileName)
            {
                return await base.LoadConfigurationAsync(container, fileName);
            }
        }

        private static async Task<TestConfigurationService> GetConfigurationServiceAsync(string name = null)
        {
            var configurationService = new TestConfigurationService(name, new ObjectConverterService(), new AppDataService());

            await configurationService.LoadAsync();

            return configurationService;
        }

        [TestFixture]
        public class The_LoadConfigurationAsync_Method
        {
            [Test]
            public async Task Ignores_Empty_Files()
            {
                var configurationService = await GetConfigurationServiceAsync();
                var fileName = System.IO.Path.GetTempFileName();

                try
                {
                    using (File.Create(fileName))
                    {
                    }

                    var config = await configurationService.PublicLoadConfigurationAsync(ConfigurationContainer.Roaming, fileName);

                    Assert.That(config, Is.Not.Null);
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
            public async Task Throws_ArgumentException_For_Null_Key(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, null));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Throws_ArgumentException_For_Empty_Key(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.GetValue<string>(container, string.Empty));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Returns_Existing_Value(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "myKey", "myValue");

                Assert.That(configurationService.GetValue<string>(container, "myKey"), Is.EqualTo("myValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Returns_Default_Value_For_Non_Existing_Value(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.That(configurationService.GetValue(container, "nonExistingKey", "nonExistingValue"), Is.EqualTo("nonExistingValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Returns_Value_For_Key_With_Special_Characters(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.That(configurationService.GetValue(container, "key with special chars", "nonExistingValue"), Is.EqualTo("myValue"));
            }
        }

        [TestFixture]
        public class The_SetValue_Method
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Throws_ArgumentException_For_Null_Key(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, null, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Throws_ArgumentException_For_Empty_Key(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                Assert.Throws<ArgumentException>(() => configurationService.SetValue(container, string.Empty, "value"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Sets_Value_Correctly(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "myKey", "myValue");

                Assert.That(configurationService.GetValue<string>(container, "myKey"), Is.EqualTo("myValue"));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Sets_Value_Correctly_For_Key_With_Special_Characters(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "key with special chars", "myValue");

                Assert.That(configurationService.GetValue<string>(container, "key with special chars"), Is.EqualTo("myValue"));
            }
            
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Does_Not_Schedule_Save_When_Values_Are_Equal(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();

                configurationService.SetValue(container, "test_key", "myValue");

                await Task.Delay(1000);

                var currentValue = configurationService.SaveConfigurationCallCount;

                configurationService.SetValue(container, "test_key", "myValue");

                await Task.Delay(1000);

                Assert.That(configurationService.SaveConfigurationCallCount, Is.EqualTo(currentValue));
            }
        }

        [TestFixture]
        public class The_ConfigurationChanged_Event
        {
            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Is_Invoked_During_SetValue_Method(ConfigurationContainer container)
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

                Assert.That(invoked, Is.True);
                Assert.That(receivedContainer, Is.EqualTo(container));
                Assert.That(receivedKey, Is.EqualTo("key"));
                Assert.That((string)receivedValue, Is.EqualTo(guid.ToString()));
            }

            [TestCase(ConfigurationContainer.Local)]
            [TestCase(ConfigurationContainer.Roaming)]
            public async Task Is_Not_Invoked_During_SetValue_Method_For_Equal_Values(ConfigurationContainer container)
            {
                var configurationService = await GetConfigurationServiceAsync();
                var invoked = false;

                configurationService.SetValue(container, "key", "value");

                configurationService.ConfigurationChanged += (sender, e) =>
                {
                    invoked = true;
                };

                configurationService.SetValue(container, "key", "value");

                Assert.That(invoked, Is.False);
            }
        }
    }
}

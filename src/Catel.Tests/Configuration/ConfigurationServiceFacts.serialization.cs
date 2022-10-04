namespace Catel.Tests.Configuration
{
    using System.Threading.Tasks;
    using Catel.Configuration;
    using Catel.Runtime.Serialization;
    using Catel.Services;
    using NUnit.Framework;

    public partial class ConfigurationServiceFacts
    {
        [TestFixture]
        public class Serialization
        {
            private class SerializationConfigurationService : ConfigurationService
            {
                public SerializationConfigurationService()
                    : base(new ObjectConverterService(), SerializationFactory.GetXmlSerializer(), new AppDataService())
                {
                }

                protected override async Task SetValueToStoreAsync(ConfigurationContainer container, string key, string value)
                {
                    await base.SetValueToStoreAsync(container, key, value);

                    switch (container)
                    {
                        case ConfigurationContainer.Local:
                            LocalChangeCount++;
                            break;

                        case ConfigurationContainer.Roaming:
                            RoamingChangeCount++;
                            break;
                    }
                }

                protected override async Task SaveConfigurationAsync(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
                {
                    switch (container)
                    {
                        case ConfigurationContainer.Local:
                            LocalSaveCount++;
                            break;

                        case ConfigurationContainer.Roaming:
                            RoamingSaveCount++;
                            break;
                    }
                }

                public int LocalChangeCount { get; private set; }

                public int LocalSaveCount { get; private set; }

                public int RoamingChangeCount { get; private set; }

                public int RoamingSaveCount { get; private set; }
            }

            [Test]
            public async Task DuplicateProcessesDoNotResetConfigurationAsync()
            {
                // See https://github.com/Catel/Catel/issues/1840 for details:
                // 
                // 1. Process A and B are launched at the same time, process A is allowed to run and loads the correct config, but process B resets the config and writes to disk
                // 2. If process A makes no changes, it will happily close
                // 3. Process C is launched, but B reset the configuration and configuration has been reset to default values
                var configServiceA = GetConfigurationService("GH1840");
                configServiceA.CreateDelayDuringSave = true;
                await configServiceA.SetRoamingValueAsync("NAME", "A");

                // The save should be ready, but the additional delay will lock the file and mimic process A from locking the file
                // and thus not allowing process B to correctly load the config
                await Task.Delay(150);

                // This code must be called *while service A is writing* so we added a delay. It should have waited until
                // the config value of A was released, then set value and overwrite the file instead of resetting it
                var configServiceB = GetConfigurationService("GH1840");
                await configServiceB.SetRoamingValueAsync("ANOTHER VALUE", "B");

                // Close both files, wait long enough (longer than 5 seconds)
                await Task.Delay(7000);

                var configServiceC = GetConfigurationService("GH1840");
                var value = await configServiceC.GetRoamingValueAsync<string>("NAME", string.Empty);

                Assert.AreEqual("A", value);
            }

            [Test]
            public async Task DoesNotCallSaveMultipleTimesAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int j = 0; j < 50; j++)
                {
                    await configurationService.SetRoamingValueAsync($"Key_{j:D2}", j);

                    await Task.Delay(10);
                }

                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(200);
                }

                // Note: we expect +1 because an initial write is done when initializing the configuration
                Assert.AreEqual(2, configurationService.RoamingSaveCount);
            }

            [Test]
            public async Task CorrectlySchedulesLocalSerializationAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        await configurationService.SetLocalValueAsync($"Key_{i:D2}_{j:D2}", i + j);

                        await Task.Delay(25);
                    }

                    await Task.Delay(100);
                }

                Assert.AreEqual(0, configurationService.RoamingChangeCount);
                Assert.AreEqual(0, configurationService.RoamingSaveCount);

                // Note: we expect +1 because an initial write is done when initializing the configuration
                Assert.AreEqual(5 * 50, configurationService.LocalChangeCount);
                Assert.AreEqual(6, configurationService.LocalSaveCount);
            }

            [Test]
            public async Task CorrectlySchedulesRoamingSerializationAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        await configurationService.SetRoamingValueAsync($"Key_{i:D2}_{j:D2}", i + j);

                        await Task.Delay(25);
                    }

                    await Task.Delay(100);
                }

                Assert.AreEqual(0, configurationService.LocalChangeCount);
                Assert.AreEqual(0, configurationService.LocalSaveCount);

                // Note: we expect +1 because an initial write is done when initializing the configuration
                Assert.AreEqual(5 * 50, configurationService.RoamingChangeCount);
                Assert.AreEqual(6, configurationService.RoamingSaveCount);
            }
        }
    }
}

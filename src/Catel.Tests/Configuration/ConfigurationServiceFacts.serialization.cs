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
                    : base(new SerializationManager(), new ObjectConverterService(), SerializationFactory.GetXmlSerializer(), new AppDataService())
                {
                }

                protected override void SetValueToStore(ConfigurationContainer container, string key, string value)
                {
                    base.SetValueToStore(container, key, value);

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

                protected override void SaveSettings(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
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
            public async Task DoesNotCallSaveMultipleTimesAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int j = 0; j < 50; j++)
                {
                    configurationService.SetRoamingValue($"{j:D2}", j);

                    await Task.Delay(10);
                }

                for (int i = 0; i < 10; i++)
                {
                    await Task.Delay(200);

                    Assert.AreEqual(1, configurationService.RoamingSaveCount);
                }

                Assert.AreEqual(1, configurationService.RoamingSaveCount);
            }

            [Test]
            public async Task CorrectlySchedulesLocalSerializationAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        configurationService.SetLocalValue($"{i:D2}_{j:D2}", i + j);

                        await Task.Delay(25);
                    }

                    await Task.Delay(100);
                }

                Assert.AreEqual(0, configurationService.RoamingChangeCount);
                Assert.AreEqual(0, configurationService.RoamingSaveCount);

                Assert.AreEqual(5 * 50, configurationService.LocalChangeCount);
                Assert.AreEqual(5, configurationService.LocalSaveCount);
            }

            [Test]
            public async Task CorrectlySchedulesRoamingSerializationAsync()
            {
                var configurationService = new SerializationConfigurationService();

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 50; j++)
                    {
                        configurationService.SetRoamingValue($"{i:D2}_{j:D2}", i + j);

                        await Task.Delay(25);
                    }

                    await Task.Delay(100);
                }

                Assert.AreEqual(0, configurationService.LocalChangeCount);
                Assert.AreEqual(0, configurationService.LocalSaveCount);

                Assert.AreEqual(5 * 50, configurationService.RoamingChangeCount);
                Assert.AreEqual(5, configurationService.RoamingSaveCount);
            }
        }
    }
}

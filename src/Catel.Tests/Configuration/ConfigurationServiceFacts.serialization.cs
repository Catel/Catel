﻿namespace Catel.Tests.Configuration
{
    using System.IO;
    using System.Threading.Tasks;
    using Catel.Configuration;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Xml;
    using Catel.Services;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    public partial class ConfigurationServiceFacts
    {
        [TestFixture]
        public class Serialization
        {
            [SetUp()]
            public void Setup()
            {
                var appDataService = new AppDataService();

                var localConfigurationFile = Path.Combine(appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserLocal), "configuration.xml");
                File.Delete(localConfigurationFile);

                var roamingConfigurationFile = Path.Combine(appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming), "configuration.xml");
                File.Delete(roamingConfigurationFile);
            }

            private class SerializationConfigurationService : ConfigurationService
            {
                public SerializationConfigurationService(IXmlSerializer xmlSerializer)
                    : base(new ObjectConverterService(), xmlSerializer, 
                        new AppDataService(), new DispatcherService(new DispatcherProviderService()))
                {
                }

                protected override void SetValueToStore(ConfigurationContainer container, string key, object? value)
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
                var configServiceA = await GetConfigurationServiceAsync("GH1840");
                configServiceA.CreateDelayDuringSave = true;
                configServiceA.SetRoamingValue("NAME", "A");

                // The save should be ready, but the additional delay will lock the file and mimic process A from locking the file
                // and thus not allowing process B to correctly load the config
                await Task.Delay(150);

                // This code must be called *while service A is writing* so we added a delay. It should have waited until
                // the config value of A was released, then set value and overwrite the file instead of resetting it
                var configServiceB = await GetConfigurationServiceAsync("GH1840");
                configServiceB.SetRoamingValue("ANOTHER VALUE", "B");

                // Close both files, wait long enough (longer than 5 seconds)
                await Task.Delay(7000);

                var configServiceC = await GetConfigurationServiceAsync("GH1840");
                var value = configServiceC.GetRoamingValue<string>("NAME", string.Empty);

                Assert.That(value, Is.EqualTo("A"));
            }

            [Test]
            public async Task DoesNotCallSaveMultipleTimesAsync()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var xmlSerializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var configurationService = new SerializationConfigurationService(xmlSerializer);

                    await configurationService.LoadAsync();

                    for (int j = 0; j < 50; j++)
                    {
                        configurationService.SetRoamingValue($"Key_{j:D2}", j);

                        await Task.Delay(10);
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(200);
                    }

                    Assert.That(configurationService.RoamingSaveCount, Is.EqualTo(1));
                }
            }

            [Test]
            public async Task CorrectlySchedulesLocalSerializationAsync()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var xmlSerializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var configurationService = new SerializationConfigurationService(xmlSerializer);

                    await configurationService.LoadAsync();

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            configurationService.SetLocalValue($"Key_{i:D2}_{j:D2}", i + j);

                            await Task.Delay(25);
                        }

                        await Task.Delay(100);
                    }

                    Assert.That(configurationService.RoamingChangeCount, Is.EqualTo(0));
                    Assert.That(configurationService.RoamingSaveCount, Is.EqualTo(0));

                    Assert.That(configurationService.LocalChangeCount, Is.EqualTo(5 * 50));
                    Assert.That(configurationService.LocalSaveCount, Is.EqualTo(5));
                }
            }

            [Test]
            public async Task CorrectlySchedulesRoamingSerializationAsync()
            {
                var serviceCollection = new ServiceCollection();

                serviceCollection.AddCatelCoreServices();

                using (var serviceProvider = serviceCollection.BuildServiceProvider())
                {
                    var xmlSerializer = serviceProvider.GetRequiredService<IXmlSerializer>();

                    var configurationService = new SerializationConfigurationService(xmlSerializer);

                    await configurationService.LoadAsync();

                    for (int i = 0; i < 5; i++)
                    {
                        for (int j = 0; j < 50; j++)
                        {
                            configurationService.SetRoamingValue($"Key_{i:D2}_{j:D2}", i + j);

                            await Task.Delay(25);
                        }

                        await Task.Delay(100);
                    }

                    Assert.That(configurationService.LocalChangeCount, Is.EqualTo(0));
                    Assert.That(configurationService.LocalSaveCount, Is.EqualTo(0));

                    Assert.That(configurationService.RoamingChangeCount, Is.EqualTo(5 * 50));
                    Assert.That(configurationService.RoamingSaveCount, Is.EqualTo(5));
                }
            }
        }
    }
}

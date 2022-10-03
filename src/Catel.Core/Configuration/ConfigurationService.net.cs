namespace Catel.Configuration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Timers;
    using Catel.Data;
    using Catel.Logging;
    using Catel.Runtime.Serialization;

    public partial class ConfigurationService
    {
        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected virtual async Task<DynamicConfiguration> GetSettingsContainerAsync(ConfigurationContainer container)
        {
            DynamicConfiguration settings = null;

            switch (container)
            {
                case ConfigurationContainer.Local:
                    settings = _localConfiguration;
                    break;

                case ConfigurationContainer.Roaming:
                    settings = _roamingConfiguration;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("container");
            }

            if (settings is null)
            {
                switch (container)
                {
                    case ConfigurationContainer.Local:
                        var defaultLocalConfigFilePath = GetConfigurationFileName(IO.ApplicationDataTarget.UserLocal);
                        await SetLocalConfigFilePathAsync(defaultLocalConfigFilePath);
                        break;

                    case ConfigurationContainer.Roaming:
                        var defaultRoamingConfigFilePath = GetConfigurationFileName(IO.ApplicationDataTarget.UserRoaming);
                        await SetRoamingConfigFilePathAsync(defaultRoamingConfigFilePath);
                        break;
                }

                // Let's try again
                settings = await GetSettingsContainerAsync(container);

                if (settings is not null)
                {
                    // As soon as we initialized the config, make sure we do a 1-time write so we have the serializer and all required objects
                    // to prevent any deadlocks when resolving required services when doing a delayed save of the settings
                    _xmlSerializer ??= SerializationFactory.GetXmlSerializer();

                    switch (container)
                    {
                        case ConfigurationContainer.Local:
                            await SaveLocalConfigurationAsync();
                            break;

                        case ConfigurationContainer.Roaming:
                            await SaveRoamingConfigurationAsync();
                            break;
                    }
                }
            }

            return settings;
        }

        private async void OnLocalSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _localSaveConfigurationTimer.Stop();

            await SaveLocalConfigurationAsync();
        }

        private async void OnRoamingSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _roamingSaveConfigurationTimer.Stop();

            await SaveRoamingConfigurationAsync();
        }

        protected virtual void ScheduleSaveConfiguration(ConfigurationContainer container)
        {
            switch (container)
            {
                case ConfigurationContainer.Local:
                    ScheduleLocalConfigurationSave();
                    break;

                case ConfigurationContainer.Roaming:
                    ScheduleRoamingConfigurationSave();
                    break;
            }
        }

        protected async void ScheduleLocalConfigurationSave()
        {
            _localSaveConfigurationTimer.Stop();

            if (_localSaveConfigurationTimer.Interval > 0)
            {
                _localSaveConfigurationTimer.Start();
            }
            else
            {
                await SaveLocalConfigurationAsync();
            }
        }

        protected async void ScheduleRoamingConfigurationSave()
        {
            _roamingSaveConfigurationTimer.Stop();

            if (_roamingSaveConfigurationTimer.Interval > 0)
            {
                _roamingSaveConfigurationTimer.Start();
            }
            else
            {
                await SaveRoamingConfigurationAsync();
            }
        }

        private async Task SaveLocalConfigurationAsync()
        {
            var container = ConfigurationContainer.Local;

            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var settings = await GetSettingsContainerAsync(container);
                var fileName = _localConfigFilePath;

                try
                {
                    await SaveConfigurationAsync(container, settings, fileName);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to save local configuration");
                }
            }
        }

        private async Task SaveRoamingConfigurationAsync()
        {
            var container = ConfigurationContainer.Roaming;

            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var settings = await GetSettingsContainerAsync(container);
                var fileName = _roamingConfigFilePath;

                try
                {
                    await SaveConfigurationAsync(container, settings, fileName);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Failed to save roaming configuration");
                }
            }
        }

        protected virtual async Task SaveConfigurationAsync(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                configuration.Save(fileStream, _xmlSerializer);
            }
        }
    }
}

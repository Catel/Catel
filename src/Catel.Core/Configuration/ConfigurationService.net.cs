﻿namespace Catel.Configuration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Timers;
    using Catel.Data;
    using Catel.Logging;

    public partial class ConfigurationService
    {
        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected virtual DynamicConfiguration? GetSettingsContainer(ConfigurationContainer container)
        {
            DynamicConfiguration? settings = null;

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
                        SetLocalConfigFilePath(defaultLocalConfigFilePath);
                        break;

                    case ConfigurationContainer.Roaming:
                        var defaultRoamingConfigFilePath = GetConfigurationFileName(IO.ApplicationDataTarget.UserRoaming);
                        SetRoamingConfigFilePath(defaultRoamingConfigFilePath);
                        break;
                }

                // Let's try again
                settings = GetSettingsContainer(container);

                if (settings is not null)
                {
                    ScheduleSaveConfiguration(container);
                }
            }

            return settings;
        }

        private async void OnLocalSaveConfigurationTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            _localSaveConfigurationTimer.Stop();

            await SaveLocalConfigurationAsync();
        }

        private async void OnRoamingSaveConfigurationTimerElapsed(object? sender, ElapsedEventArgs e)
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
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return;
                }

                var fileName = _localConfigFilePath;
                if (fileName is null)
                {
                    throw Log.ErrorAndCreateException<CatelException>("Cannot save local configuration without a file name");
                }

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
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return;
                }

                var fileName = _roamingConfigFilePath;
                if (fileName is null)
                {
                    throw Log.ErrorAndCreateException<CatelException>("Cannot save roaming configuration without a file name");
                }

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
                configuration.Save(fileStream, _serializer);
            }
        }
    }
}

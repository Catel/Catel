namespace Catel.Configuration
{
    using System;
    using System.IO;
    using System.Text.Json.Nodes;
    using System.Threading.Tasks;
    using System.Timers;
    using Catel.Logging;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public partial class ConfigurationService
    {
        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected virtual IConfiguration GetSettingsContainer(ConfigurationContainer container)
        {
            IConfiguration? settings = null;

            switch (container)
            {
                case ConfigurationContainer.Local:
                    settings = _localConfiguration;
                    break;

                case ConfigurationContainer.Roaming:
                    settings = _roamingConfiguration;
                    break;

                default:
                    throw _logger.LogErrorAndCreateException<ArgumentOutOfRangeException>("container");
            }

            if (settings is null)
            {
                throw _logger.LogErrorAndCreateException<InvalidOperationException>($"Configuration is not yet initialized for '{container}' container, make sure to call LoadAsync first");
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

            if (_localSaveConfigurationTimer.Interval > IgnoreTimerThresholdInMilliseconds)
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

            if (_roamingSaveConfigurationTimer.Interval > IgnoreTimerThresholdInMilliseconds)
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
            _localSaveConfigurationTimer.Stop();

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
                    throw _logger.LogErrorAndCreateException<CatelException>("Cannot save local configuration without a file name");
                }

                try
                {
                    await SaveConfigurationAsync(ConfigurationContainer.Local, settings, fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save local configuration");
                }
            }
        }

        private async Task SaveRoamingConfigurationAsync()
        {
            _roamingSaveConfigurationTimer.Stop();

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
                    throw _logger.LogErrorAndCreateException<CatelException>("Cannot save roaming configuration without a file name");
                }

                try
                {
                    await SaveConfigurationAsync(ConfigurationContainer.Roaming, settings, fileName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to save roaming configuration");
                }
            }
        }

        protected virtual async Task SaveConfigurationAsync(ConfigurationContainer container, 
            IConfiguration configuration, string fileName)
        {
            var jsonNode = SerializeConfiguration(configuration)!;

            await File.WriteAllTextAsync(fileName, jsonNode.ToString());
        }

        protected virtual JsonNode? SerializeConfiguration(IConfiguration configuration)
        {
            var jsonObject = new JsonObject
            {
            };

            foreach (var child in configuration.GetChildren())
            {
                if (child.Path.EndsWith(":0"))
                {
                    var array = new JsonArray();

                    foreach (var arrayChild in configuration.GetChildren())
                    {
                        array.Add(SerializeConfiguration(arrayChild));
                    }

                    return array;
                }

                jsonObject.Add(child.Key, SerializeConfiguration(child));
            }

            if (jsonObject.Count > 0 ||
                configuration is not IConfigurationSection section)
            {
                return jsonObject;
            }

            var jsonValue = JsonValue.Create(section.Value);
            return jsonValue ?? null;
        }
    }
}

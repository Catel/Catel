// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.netfxcore.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using System;
    using System.IO;
    using System.Timers;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    public partial class ConfigurationService
    {
        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected virtual DynamicConfiguration GetSettingsContainer(ConfigurationContainer container)
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
                    // As soon as we initialized the config, make sure we do a 1-time write so we have the serializer and all required objects
                    // to prevent any deadlocks when resolving required services when doing a delayed save of the settings
                    _xmlSerializer ??= SerializationFactory.GetXmlSerializer();

                    switch (container)
                    {
                        case ConfigurationContainer.Local:
                            SaveLocalConfiguration();
                            break;

                        case ConfigurationContainer.Roaming:
                            SaveRoamingConfiguration();
                            break;
                    }
                }
            }

            return settings;
        }

        private void OnLocalSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _localSaveConfigurationTimer.Stop();

            SaveLocalConfiguration();
        }

        private void OnRoamingSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _roamingSaveConfigurationTimer.Stop();

            SaveRoamingConfiguration();
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

        protected void ScheduleLocalConfigurationSave()
        {
            _localSaveConfigurationTimer.Stop();

            if (_localSaveConfigurationTimer.Interval > 0)
            {
                _localSaveConfigurationTimer.Start();
            }
            else
            {
                SaveLocalConfiguration();
            }
        }

        protected void ScheduleRoamingConfigurationSave()
        {
            _roamingSaveConfigurationTimer.Stop();

            if (_roamingSaveConfigurationTimer.Interval > 0)
            {
                _roamingSaveConfigurationTimer.Start();
            }
            else
            {
                SaveRoamingConfiguration();
            }
        }

        private void SaveLocalConfiguration()
        {
            var container = ConfigurationContainer.Local;

            lock (GetLockObject(container))
            {
                var settings = GetSettingsContainer(container);
                var fileName = _localConfigFilePath;

                SaveConfiguration(container, settings, fileName);
            }
        }

        private void SaveRoamingConfiguration()
        {
            var container = ConfigurationContainer.Roaming;

            lock (GetLockObject(container))
            {
                var settings = GetSettingsContainer(container);
                var fileName = _roamingConfigFilePath;

                SaveConfiguration(container, settings, fileName);
            }
        }

        protected virtual void SaveConfiguration(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                configuration.Save(fileStream, _xmlSerializer);
            }
        }
    }
}

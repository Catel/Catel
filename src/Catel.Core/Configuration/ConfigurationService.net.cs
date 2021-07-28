// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.netfxcore.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE || NETSTANDARD

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

                // As soon as we initialized the config, make sure we have access to the serializer (instantiate it at least once)
                _xmlSerializer ??= SerializationFactory.GetXmlSerializer();

                // Let's try again
                settings = GetSettingsContainer(container);
            }

            return settings;
        }

        private void OnLocalSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _localSaveConfigurationTimer.Stop();

            lock (GetLockObject(ConfigurationContainer.Local))
            {
                var settings = GetSettingsContainer(ConfigurationContainer.Local);
                var fileName = _localConfigFilePath;

                SaveConfiguration(ConfigurationContainer.Local, settings, fileName);
            }
        }

        private void OnRoamingSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _roamingSaveConfigurationTimer.Stop();

            lock (GetLockObject(ConfigurationContainer.Roaming))
            {
                var settings = GetSettingsContainer(ConfigurationContainer.Roaming);
                var fileName = _roamingConfigFilePath;

                SaveConfiguration(ConfigurationContainer.Roaming, settings, fileName);
            }
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
            _localSaveConfigurationTimer.Start();
        }

        protected void ScheduleRoamingConfigurationSave()
        {
            _roamingSaveConfigurationTimer.Stop();
            _roamingSaveConfigurationTimer.Start();
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

#endif

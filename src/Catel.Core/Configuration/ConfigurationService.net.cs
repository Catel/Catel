// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.netfxcore.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE || NETSTANDARD

namespace Catel.Configuration
{
    using System;
    using System.Timers;
    using Catel.Data;

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

            return settings;
        }

        private void OnLocalSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _localSaveConfigurationTimer.Stop();

            lock (GetLockObject(ConfigurationContainer.Local))
            {
                var settings = GetSettingsContainer(ConfigurationContainer.Local);
                var fileName = _localConfigFilePath;

                SaveSettings(ConfigurationContainer.Local, settings, fileName);
            }
        }

        private void OnRoamingSaveConfigurationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (GetLockObject(ConfigurationContainer.Roaming))
            {
                _roamingSaveConfigurationTimer.Stop();

                var settings = GetSettingsContainer(ConfigurationContainer.Roaming);
                var fileName = _roamingConfigFilePath;

                SaveSettings(ConfigurationContainer.Roaming, settings, fileName);
            }
        }

        protected virtual void ScheduleSaveSettings(ConfigurationContainer container)
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

        protected virtual void SaveSettings(ConfigurationContainer container, DynamicConfiguration configuration, string fileName)
        {
            configuration.SaveAsXml(fileName);
        }
    }
}

#endif

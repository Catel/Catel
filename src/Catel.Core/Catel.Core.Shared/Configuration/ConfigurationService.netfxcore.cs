// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.netfxcore.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Configuration
{
    using System;
    using Windows.Storage;

    public partial class ConfigurationService
    {
        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected ApplicationDataContainer GetSettingsContainer(ConfigurationContainer container)
        {
            ApplicationDataContainer settings = null;

            switch (container)
            {
                case ConfigurationContainer.Local:
                    settings = ApplicationData.Current.LocalSettings;
                    break;

                case ConfigurationContainer.Roaming:
                    settings = ApplicationData.Current.RoamingSettings;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("container");
            }

            return settings;
        }
    }
}

#endif
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.netfxcore.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.Configuration
{
    using System;
    using System.Collections.Generic;
    using Windows.Storage;

    public partial class ConfigurationService
    {
        private readonly Dictionary<ConfigurationContainer, ApplicationDataContainer> _containerCache = new Dictionary<ConfigurationContainer, ApplicationDataContainer>();

        /// <summary>
        /// Gets the settings container for this platform
        /// </summary>
        /// <param name="container">The settings container.</param>
        /// <returns>The settings container.</returns>
        protected ApplicationDataContainer GetSettingsContainer(ConfigurationContainer container)
        {
            lock (_containerCache)
            {
                if (!_containerCache.TryGetValue(container, out var settings))
                {
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

                    _containerCache[container] = settings;
                }

                return settings;
            }
        }
    }
}

#endif

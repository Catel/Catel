// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2016 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// Extensions for the <see cref="IConfigurationService"/>.
    /// </summary>
    public static class IConfigurationServiceExtensions
    {
        /// <summary>
        /// Determines whether the specified value is available using <see cref="ConfigurationContainer.Local" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static bool IsLocalValueAvailable(this IConfigurationService configurationService, string key)
        {
            Argument.IsNotNull("configurationService", configurationService);

            return configurationService.IsValueAvailable(ConfigurationContainer.Local, key);
        }

        /// <summary>
        /// Determines whether the specified value is available using <see cref="ConfigurationContainer.Roaming" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static bool IsRoamingValueAvailable(this IConfigurationService configurationService, string key)
        {
            Argument.IsNotNull("configurationService", configurationService);

            return configurationService.IsValueAvailable(ConfigurationContainer.Roaming, key);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist using <see cref="ConfigurationContainer.Local"/>.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static void InitializeLocalValue(this IConfigurationService configurationService, string key, object defaultValue)
        {
            Argument.IsNotNull("configurationService", configurationService);

            configurationService.InitializeValue(ConfigurationContainer.Local, key, defaultValue);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist using <see cref="ConfigurationContainer.Roaming"/>.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static void InitializeRoamingValue(this IConfigurationService configurationService, string key, object defaultValue)
        {
            Argument.IsNotNull("configurationService", configurationService);

            configurationService.InitializeValue(ConfigurationContainer.Roaming, key, defaultValue);
        }

        /// <summary>
        /// Gets the configuration value using <see cref="ConfigurationContainer.Local" />.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static T GetLocalValue<T>(this IConfigurationService configurationService, string key, T defaultValue = default(T))
        {
            Argument.IsNotNull("configurationService", configurationService);

            return configurationService.GetValue(ConfigurationContainer.Local, key, defaultValue);
        }

        /// <summary>
        /// Gets the configuration value using <see cref="ConfigurationContainer.Roaming" />.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static T GetRoamingValue<T>(this IConfigurationService configurationService, string key, T defaultValue = default(T))
        {
            Argument.IsNotNull("configurationService", configurationService);

            return configurationService.GetValue(ConfigurationContainer.Roaming, key, defaultValue);
        }
         
        /// <summary>
        /// Sets the configuration value using <see cref="ConfigurationContainer.Local" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static void SetLocalValue(this IConfigurationService configurationService, string key, object value)
        {
            Argument.IsNotNull("configurationService", configurationService);

            configurationService.SetValue(ConfigurationContainer.Local, key, value);
        }

        /// <summary>
        /// Sets the configuration value using <see cref="ConfigurationContainer.Roaming" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static void SetRoamingValue(this IConfigurationService configurationService, string key, object value)
        {
            Argument.IsNotNull("configurationService", configurationService);

            configurationService.SetValue(ConfigurationContainer.Roaming, key, value);
        }
    }
}
namespace Catel.Configuration
{
    using System;
    using System.Threading.Tasks;

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
        public static Task<bool> IsLocalValueAvailableAsync(this IConfigurationService configurationService, string key)
        {
            return configurationService.IsValueAvailableAsync(ConfigurationContainer.Local, key);
        }

        /// <summary>
        /// Determines whether the specified value is available using <see cref="ConfigurationContainer.Roaming" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static Task<bool> IsRoamingValueAvailableAsync(this IConfigurationService configurationService, string key)
        {
            return configurationService.IsValueAvailableAsync(ConfigurationContainer.Roaming, key);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist using <see cref="ConfigurationContainer.Local"/>.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static Task InitializeLocalValueAsync(this IConfigurationService configurationService, string key, object? defaultValue)
        {
            return configurationService.InitializeValueAsync(ConfigurationContainer.Local, key, defaultValue);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist using <see cref="ConfigurationContainer.Roaming"/>.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static Task InitializeRoamingValueAsync(this IConfigurationService configurationService, string key, object? defaultValue)
        {
            return configurationService.InitializeValueAsync(ConfigurationContainer.Roaming, key, defaultValue);
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
        public static Task<T> GetLocalValueAsync<T>(this IConfigurationService configurationService, string key, T defaultValue = default!)
        {
            return configurationService.GetValueAsync(ConfigurationContainer.Local, key, defaultValue);
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
        public static Task<T> GetRoamingValueAsync<T>(this IConfigurationService configurationService, string key, T defaultValue = default!)
        {
            return configurationService.GetValueAsync(ConfigurationContainer.Roaming, key, defaultValue);
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
        public static Task SetLocalValueAsync(this IConfigurationService configurationService, string key, object? value)
        {
            return configurationService.SetValueAsync(ConfigurationContainer.Local, key, value);
        }

        /// <summary>
        /// Sets the configuration value using <see cref="ConfigurationContainer.Roaming" />.
        /// </summary>
        /// <param name="configurationService">The configuration service.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="configurationService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public static Task SetRoamingValueAsync(this IConfigurationService configurationService, string key, object? value)
        {
            return configurationService.SetValueAsync(ConfigurationContainer.Roaming, key, value);
        }
    }
}

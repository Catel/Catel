namespace Catel.Configuration
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

        /// <summary>
        /// Suspends the notifications of this service until the returned object is disposed.
        /// </summary>
        /// <returns>IDisposable.</returns>
        IDisposable SuspendNotifications();

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        Task<T> GetValueAsync<T>(ConfigurationContainer container, string key, T defaultValue = default!);

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        Task SetValueAsync(ConfigurationContainer container, string key, object? value);

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        Task<bool> IsValueAvailableAsync(ConfigurationContainer container, string key);

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        Task InitializeValueAsync(ConfigurationContainer container, string key, object? defaultValue);

        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        Task SetRoamingConfigFilePathAsync(string filePath);

        /// <summary>
        /// Sets the local config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        Task SetLocalConfigFilePathAsync(string filePath);
    }
}

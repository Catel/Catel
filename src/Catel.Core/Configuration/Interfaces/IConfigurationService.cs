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
        T GetValue<T>(ConfigurationContainer container, string key, T defaultValue = default!);

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        void SetValue(ConfigurationContainer container, string key, object? value);

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        bool IsValueAvailable(ConfigurationContainer container, string key);

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        void InitializeValue(ConfigurationContainer container, string key, object? defaultValue);

        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        void SetRoamingConfigFilePath(string filePath);

        /// <summary>
        /// Sets the local config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        void SetLocalConfigFilePath(string filePath);

        /// <summary>
        /// Explicitly saves the specific configuration container.
        /// <para />
        /// This call can be useful when the saving of the configuration should not be done in the background.
        /// </summary>
        /// <param name="container">The configuration container to save.</param>
        /// <returns>The task that can be awaited.</returns>
        Task SaveAsync(ConfigurationContainer container);

        /// <summary>
        /// Explicitly loads the specified configuration container.
        /// <para />
        /// This call can be useful when the saving of the configuration should not be done in the background.
        /// </summary>
        /// <param name="container">The configuration container to load.</param>
        /// <returns>The task that can be awaited.</returns>
        Task LoadAsync(ConfigurationContainer container);
    }
}

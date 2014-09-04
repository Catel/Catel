// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        T GetValue<T>(string key, T defaultValue = default(T));

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        void SetValue(string key, object value);

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        bool IsValueAvailable(string key);

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        void InitializeValue(string key, object defaultValue);
    }
}
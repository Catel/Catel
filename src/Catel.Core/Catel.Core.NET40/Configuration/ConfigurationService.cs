// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// <para />
    /// This default implementation writes to the 
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        #region Events
        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        public T GetValue<T>(string key)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var value = GetValueFromStore(key);
            return (T)StringToObjectHelper.ToRightType(typeof (T), value);
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        public void SetValue(string key, object value)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var stringValue = ObjectToStringHelper.ToString(value);
            SetValueToStore(key, stringValue);

            var handler = ConfigurationChanged;
            if (handler != null)
            {
                handler.Invoke(this, new ConfigurationChangedEventArgs(key, value));
            }
        }

        /// <summary>
        /// Gets the value from the store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected virtual string GetValueFromStore(string key)
        {
            // TODO: Implement
            return null;
        }

        /// <summary>
        /// Sets the value to the store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetValueToStore(string key, string value)
        {
            // TODO: Implement
        }
        #endregion
    }
}
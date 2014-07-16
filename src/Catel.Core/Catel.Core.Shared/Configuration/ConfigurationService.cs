// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;

#if PCL
    // Not supported
#elif NETFX_CORE
    using Windows.Storage;
#elif WINDOWS_PHONE || SILVERLIGHT
    using System.IO.IsolatedStorage;
#else
    using System.Configuration;
    using System.Linq;
#endif

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
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            Argument.IsNotNullOrWhitespace("key", key);

            if (!ValueExists(key))
            {
                return defaultValue;
            }

            var value = GetValueFromStore(key);
            return (T)StringToObjectHelper.ToRightType(typeof(T), value);
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
        /// Determines whether the specified key value exists in the configuration.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
        protected virtual bool ValueExists(string key)
        {
#if PCL || XAMARIN
            throw new NotSupportedInPlatformException();
#elif NETFX_CORE
            var settings = ApplicationData.Current.RoamingSettings;
            return settings.Values.ContainsKey(key);
#elif WINDOWS_PHONE || SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return settings.Contains(key);
#else
            return ConfigurationManager.AppSettings.AllKeys.Contains(key);
#endif
        }

        /// <summary>
        /// Gets the value from the store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected virtual string GetValueFromStore(string key)
        {
#if PCL || XAMARIN
            throw new NotSupportedInPlatformException();
#elif NETFX_CORE
            var settings = ApplicationData.Current.RoamingSettings;
            return (string)settings.Values[key];
#elif WINDOWS_PHONE || SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            return (string)settings[key];
#else
            return ConfigurationManager.AppSettings[key];
#endif
        }

        /// <summary>
        /// Sets the value to the store.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetValueToStore(string key, string value)
        {
#if PCL || XAMARIN
            throw new NotSupportedInPlatformException();
#elif NETFX_CORE
            var settings = ApplicationData.Current.RoamingSettings;
            settings.Values[key] = value;
#elif WINDOWS_PHONE || SILVERLIGHT
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings[key] = value;
            settings.Save();
#else
            ConfigurationManager.AppSettings[key] = value;
#endif
        }
        #endregion
    }
}
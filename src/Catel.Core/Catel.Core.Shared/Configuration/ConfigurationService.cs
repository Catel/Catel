// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Configuration
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using Data;
    using Catel.Logging;

#if PCL
    // Not supported
#elif NETFX_CORE
    using Windows.Storage;
#elif WINDOWS_PHONE || SILVERLIGHT
    using System.IO.IsolatedStorage;
#else
    using System.Configuration;
    using System.Linq;
    using Runtime.Serialization;
    using Path = IO.Path;
#endif

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// <para />
    /// This default implementation writes to the 
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET
        private readonly ISerializationManager _serializationManager;

        private readonly DynamicConfiguration _configuration;
        private readonly string _configFilePath;
#endif

        private bool _suspendNotifications = false;
        private bool _hasPendingNotifications = false;

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        public ConfigurationService(ISerializationManager serializationManager)
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            _serializationManager = serializationManager;
            _configFilePath = Path.Combine(Path.GetApplicationDataDirectory(), "configuration.xml");

            try
            {
                if (File.Exists(_configFilePath))
                {
                    using (var fileStream = new FileStream(_configFilePath, FileMode.Open))
                    {
                        _configuration = ModelBase.Load<DynamicConfiguration>(fileStream, SerializationMode.Xml);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load configuration, using default settings");
            }

            if (_configuration == null)
            {
                _configuration = new DynamicConfiguration();
            }
        }
#endif

        #region Events
        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;
        #endregion

        #region Methods
        /// <summary>
        /// Suspends the notifications of this service until the returned object is disposed.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public IDisposable SuspendNotifications()
        {
            return new SuspendNotificationsToken(this);
        }

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

            try
            {
                var value = GetValueFromStore(key);
                if (value == null)
                {
                    return defaultValue;
                }

                return (T) StringToObjectHelper.ToRightType(typeof (T), value);
            }
            catch (Exception)
            {
                return defaultValue;
            }
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

            RaiseConfigurationChanged(key, value);
        }

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        public bool IsValueAvailable(string key)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            return ValueExists(key);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key"/> is <c>null</c> or whitespace.</exception>
        public void InitializeValue(string key, object defaultValue)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            if (!IsValueAvailable(key))
            {
                SetValue(key, defaultValue);
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
            return _configuration.IsConfigurationKeyAvailable(key);
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
            return _configuration.GetConfigurationValue<string>(key);
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
            if (!_configuration.IsConfigurationKeyAvailable(key))
            {
                _configuration.RegisterConfigurationKey(key);
            }

            _configuration.SetConfigurationValue(key, value);
            _configuration.SaveAsXml(_configFilePath);
#endif
        }

        private void RaiseConfigurationChanged(string key, object value)
        {
            if (_suspendNotifications)
            {
                _hasPendingNotifications = true;
                return;
            }

            var handler = ConfigurationChanged;
            if (handler != null)
            {
                handler.Invoke(this, new ConfigurationChangedEventArgs(key, value));
            }
        }
        #endregion

        private class SuspendNotificationsToken : IDisposable
        {
            #region Fields
            private ConfigurationService _configurationService;
            #endregion

            #region Constructors
            public SuspendNotificationsToken(ConfigurationService configurationService)
            {
                Argument.IsNotNull("configurationService", configurationService);

                _configurationService = configurationService;
                configurationService._suspendNotifications = true;
            }
            #endregion

            #region IDisposable Members
            public void Dispose()
            {
                if (_configurationService != null)
                {
                    _configurationService._suspendNotifications = false;
                    if (_configurationService._hasPendingNotifications)
                    {
                        _configurationService.RaiseConfigurationChanged(string.Empty, string.Empty);
                        _configurationService._hasPendingNotifications = false;
                    }

                    _configurationService = null;
                }
            }
            #endregion
        }
    }
}
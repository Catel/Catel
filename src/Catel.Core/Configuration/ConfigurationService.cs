namespace Catel.Configuration
{
    using Runtime.Serialization;
    using Services;
    using System;
    using System.Globalization;
    using System.IO;
    using Data;
    using Catel.Logging;
    using Runtime.Serialization.Xml;
    using System.Timers;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Catel.Threading;

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// <para />
    /// This default implementation writes to the
    /// </summary>
    public partial class ConfigurationService : IConfigurationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ISerializationManager _serializationManager;
        private readonly IObjectConverterService _objectConverterService;
        private readonly ISerializer _serializer;
        private readonly IAppDataService _appDataService;

        private DynamicConfiguration _localConfiguration;
        private DynamicConfiguration _roamingConfiguration;

        private readonly AsyncLock _localConfigurationLock = new();
        private readonly AsyncLock _roamingConfigurationLock = new();

        private readonly Timer _localSaveConfigurationTimer = new();
        private readonly Timer _roamingSaveConfigurationTimer = new();

        private IXmlSerializer _xmlSerializer;

        private string _localConfigFilePath;
        private string _roamingConfigFilePath;

        private bool _suspendNotifications = false;
        private bool _hasPendingNotifications = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="appDataService">The application data service.</param>
        public ConfigurationService(ISerializationManager serializationManager,
            IObjectConverterService objectConverterService, IXmlSerializer serializer, IAppDataService appDataService)
            : this(serializationManager, objectConverterService, (ISerializer)serializer, appDataService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="appDataService">The application data service.</param>
        public ConfigurationService(ISerializationManager serializationManager,
            IObjectConverterService objectConverterService, ISerializer serializer,
            IAppDataService appDataService)
        {
            Argument.IsNotNull("serializationManager", serializationManager);
            Argument.IsNotNull("objectConverterService", objectConverterService);
            Argument.IsNotNull("serializer", serializer);
            Argument.IsNotNull("appDataService", appDataService);

            _serializationManager = serializationManager;
            _objectConverterService = objectConverterService;
            _serializer = serializer;
            _appDataService = appDataService;

            _localSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _localSaveConfigurationTimer.Elapsed += OnLocalSaveConfigurationTimerElapsed;

            _roamingSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _roamingSaveConfigurationTimer.Elapsed += OnRoamingSaveConfigurationTimerElapsed;
        }

        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs> ConfigurationChanged;

        /// <summary>
        /// Gets the configuration file name for the specified application data target.
        /// </summary>
        /// <param name="applicationDataTarget">The application data target.</param>
        /// <returns>Returns the full configuration filename for the specified application data target.</returns>
        protected virtual string GetConfigurationFileName(Catel.IO.ApplicationDataTarget applicationDataTarget)
        {
            var filename = System.IO.Path.Combine(_appDataService.GetApplicationDataDirectory(applicationDataTarget), "configuration.xml");
            return filename;
        }

        protected virtual double GetSaveSettingsSchedulerIntervalInMilliseconds()
        {
            return 100d;
        }

        /// <summary>
        /// Suspends the notifications of this service until the returned object is disposed.
        /// </summary>
        /// <returns>IDisposable.</returns>
        public IDisposable SuspendNotifications()
        {
            return new DisposableToken<ConfigurationService>(this,
                x =>
                {
                    x.Instance._suspendNotifications = true;
                },
                x =>
                {
                    x.Instance._suspendNotifications = false;
                    if (x.Instance._hasPendingNotifications)
                    {
                        x.Instance.RaiseConfigurationChanged(ConfigurationContainer.Roaming, string.Empty, string.Empty);
                        x.Instance._hasPendingNotifications = false;
                    }
                });
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value. Will be returned if the value cannot be found.</param>
        /// <returns>The configuration value.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public virtual async Task<T> GetValueAsync<T>(ConfigurationContainer container, string key, T defaultValue = default(T))
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                try
                {
                    if (!await ValueExistsAsync(container, key))
                    {
                        return defaultValue;
                    }

                    var value = await GetValueFromStoreAsync(container, key);
                    if (value is null)
                    {
                        return defaultValue;
                    }

                    // ObjectConverterService doesn't support object, but just return the value as is
                    if (typeof(T) == typeof(object))
                    {
                        return (T)(object)value;
                    }

                    return (T)_objectConverterService.ConvertFromStringToObject(value, typeof(T), CultureInfo.InvariantCulture);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, $"Failed to retrieve configuration value '{Enum<ConfigurationContainer>.ToString(container)}.{key}', returning default value");

                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public virtual async Task SetValueAsync(ConfigurationContainer container, string key, object value)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var originalKey = key;
            key = GetFinalKey(key);
            var raiseEvent = false;

            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var stringValue = _objectConverterService.ConvertFromObjectToString(value, CultureInfo.InvariantCulture);
                var existingValue = await GetValueFromStoreAsync(container, key);

                await SetValueToStoreAsync(container, key, stringValue);

                if (!string.Equals(stringValue, existingValue))
                {
                    raiseEvent = true;
                }
            }

            if (raiseEvent)
            {
                RaiseConfigurationChanged(container, originalKey, value);
            }
        }

        /// <summary>
        /// Determines whether the specified value is available.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the specified value is available; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public virtual async Task<bool> IsValueAvailableAsync(ConfigurationContainer container, string key)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            return await ValueExistsAsync(container, key);
        }

        /// <summary>
        /// Initializes the value by setting the value to the <paramref name="defaultValue" /> if the value does not yet exist.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <exception cref="ArgumentException">The <paramref name="key" /> is <c>null</c> or whitespace.</exception>
        public virtual async Task InitializeValueAsync(ConfigurationContainer container, string key, object defaultValue)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                if (!await IsValueAvailableAsync(container, key))
                {
                    await SetValueAsync(container, key, defaultValue);
                }
            }
        }

        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        public virtual async Task SetRoamingConfigFilePathAsync(string filePath)
        {
            Argument.IsNotNullOrEmpty(nameof(filePath), filePath);

            Log.Debug($"Setting roaming config file path to '{filePath}'");

            var lockObject = GetLockObject(ConfigurationContainer.Roaming);
            using (await lockObject.LockAsync())
            {
                _roamingConfigFilePath = filePath;
                _roamingConfiguration = await LoadConfigurationAsync(filePath);
            }
        }

        /// <summary>
        /// Sets the roaming config file path.
        /// </summary>
        /// <param name="filePath">The file path. </param>
        public virtual async Task SetLocalConfigFilePathAsync(string filePath)
        {
            Argument.IsNotNullOrEmpty(nameof(filePath), filePath);

            Log.Debug($"Setting local config file path to '{filePath}'");

            var lockObject = GetLockObject(ConfigurationContainer.Local);
            using (await lockObject.LockAsync())
            {
                _localConfigFilePath = filePath;
                _localConfiguration = await LoadConfigurationAsync(filePath);
            }
        }

        protected virtual async Task<DynamicConfiguration> LoadConfigurationAsync(string fileName)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!File.Exists(fileName))
            {
                // No file, we can really start from scratch
                return new DynamicConfiguration();
            }

            // Try for 5 seconds
            while (stopwatch.ElapsedMilliseconds < 5000)
            {
                try
                {
                    using (var fileStream = File.Open(fileName, FileMode.Open))
                    {
                        if (!fileStream.CanRead)
                        {
                            continue;
                        }

                        var configuration = SavableModelBase<DynamicConfiguration>.Load(fileStream, _serializer);
                        return configuration;
                    }
                }
                catch (IOException)
                {
                    // allow
                }
            }

            throw Log.ErrorAndCreateException<InvalidOperationException>($"File '{fileName}' could not be used to load the configuration, it was locked for too long");
        }

        /// <summary>
        /// Determines whether the specified key value exists in the configuration.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
        protected virtual async Task<bool> ValueExistsAsync(ConfigurationContainer container, string key)
        {
            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var settings = await GetSettingsContainerAsync(container);
                return settings.IsConfigurationValueSet(key);
            }
        }

        /// <summary>
        /// Gets the value from the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected virtual async Task<string> GetValueFromStoreAsync(ConfigurationContainer container, string key)
        {
            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var settings = await GetSettingsContainerAsync(container);
                return settings.GetConfigurationValue<string>(key, string.Empty);
            }
        }

        /// <summary>
        /// Sets the value to the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual async Task SetValueToStoreAsync(ConfigurationContainer container, string key, string value)
        {
            var lockObject = GetLockObject(container);
            using (await lockObject.LockAsync())
            {
                var settings = await GetSettingsContainerAsync(container);

                if (!settings.IsConfigurationValueSet(key))
                {
                    settings.RegisterConfigurationKey(key);
                }

                settings.SetConfigurationValue(key, value);

                ScheduleSaveConfiguration(container);
            }
        }

        /// <summary>
        /// Gets the final key. This method allows customization of the key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetFinalKey(string key)
        {
            key = key.Replace(" ", "_");

            return key;
        }

        protected AsyncLock GetLockObject(ConfigurationContainer container)
        {
            switch (container)
            {
                case ConfigurationContainer.Local:
                    return _localConfigurationLock;

                case ConfigurationContainer.Roaming:
                    return _roamingConfigurationLock;
            }

            throw Log.ErrorAndCreateException<InvalidOperationException>($"Container type '{container}' has no lock object");
        }

        protected void RaiseConfigurationChanged(ConfigurationContainer container, string key, object value)
        {
            if (_suspendNotifications)
            {
                _hasPendingNotifications = true;
                return;
            }

            ConfigurationChanged?.Invoke(this, new ConfigurationChangedEventArgs(container, key, value));
        }
    }
}

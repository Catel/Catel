﻿namespace Catel.Configuration
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

        /// <summary>
        /// If the timer duration is smaller than this threshold, the
        /// timer will not be used.
        /// </summary>
        private const int IgnoreTimerThresholdInMilliseconds = 10;

        private readonly IObjectConverterService _objectConverterService;
        private readonly ISerializer _serializer;
        private readonly IAppDataService _appDataService;
        private readonly IDispatcherService _dispatcherService;

        private DynamicConfiguration? _localConfiguration;
        private DynamicConfiguration? _roamingConfiguration;

        private readonly AsyncLock _localConfigurationLock = new()
        {
            Name = "ConfigurationService.Local"
        };

        private readonly AsyncLock _roamingConfigurationLock = new()
        {
            Name = "ConfigurationService.Roaming"
        };

        private readonly Timer _localSaveConfigurationTimer = new();
        private readonly Timer _roamingSaveConfigurationTimer = new();

        private string? _localConfigFilePath;
        private string? _roamingConfigFilePath;

        private bool _suspendNotifications = false;
        private bool _hasPendingNotifications = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="appDataService">The application data service.</param>
        /// <param name="dispatcherService">Dispatcher service.</param>
        public ConfigurationService(IObjectConverterService objectConverterService, IXmlSerializer serializer, IAppDataService appDataService,
            IDispatcherService dispatcherService)
            : this(objectConverterService, (ISerializer)serializer, appDataService, dispatcherService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationService" /> class.
        /// </summary>
        /// <param name="objectConverterService">The object converter service.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="appDataService">The application data service.</param>
        /// <param name="dispatcherService">The application data service.</param>
        public ConfigurationService(IObjectConverterService objectConverterService, ISerializer serializer,
            IAppDataService appDataService, IDispatcherService dispatcherService)
        {
            _objectConverterService = objectConverterService;
            _serializer = serializer;
            _appDataService = appDataService;
            _dispatcherService = dispatcherService;

            _localSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _localSaveConfigurationTimer.Elapsed += OnLocalSaveConfigurationTimerElapsed;

            _roamingSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _roamingSaveConfigurationTimer.Elapsed += OnRoamingSaveConfigurationTimerElapsed;

#if DEBUG
            _localConfigurationLock.EnableExtremeLogging = true;
            _roamingConfigurationLock.EnableExtremeLogging = true;
#endif

            // To prevent any deadlocks (when saving from timer tick), make sure to
            // dispatch on timer ticks.
            // 
            // 1. Create a type in the type factory, that tries to read a value inside INeedCustomInitialization
            // 2. At the *same* time, because other values were stored before, the timer in this class ticks
            //    to save for the very first time, causing the modifiers to be constructed in a timer thread
            //
            // Since the TypeFactory is still creating the type, it cannot construct the newly required type thus
            // causing a deadlock.
            //
            // For this reason, we have decided to dispatcher the timer tick events back to the 
            // "main" thread
        }

        /// <summary>
        /// Occurs when the configuration has changed.
        /// </summary>
        public event EventHandler<ConfigurationChangedEventArgs>? ConfigurationChanged;

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

        /// <inheritdoc />
        public virtual T GetValue<T>(ConfigurationContainer container, string key, T defaultValue = default!)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            try
            {
                object? value;

                var lockObject = GetLockObject(container);
                using (lockObject.Lock())
                {
                    if (!ValueExists(container, key))
                    {
                        return defaultValue;
                    }

                    value = GetValueFromStore(container, key);
                }

                return value switch
                {
                    null => defaultValue,
                    string s => (T)_objectConverterService.ConvertFromStringToObject(s, typeof(T), CultureInfo.InvariantCulture)!,
                    _ => (T)value
                };
            }
            catch (Exception ex)
            {
                Log.Warning(ex, $"Failed to retrieve configuration value '{Enum<ConfigurationContainer>.ToString(container)}.{key}', returning default value");

                return defaultValue;
            }
        }

        /// <inheritdoc />
        public virtual void SetValue(ConfigurationContainer container, string key, object? value)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var originalKey = key;
            key = GetFinalKey(key);

            object? existingValue;

            var areEqual = false;

            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                existingValue = GetValueFromStore(container, key);

                areEqual = ObjectHelper.AreEqual(value, existingValue);
                if (!areEqual)
                {
                    SetValueToStore(container, key, value);
                }
            }

            if (!areEqual)
            {
                RaiseConfigurationChanged(container, originalKey, value);
            }
        }

        /// <inheritdoc />
        public virtual bool IsValueAvailable(ConfigurationContainer container, string key)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            key = GetFinalKey(key);

            return ValueExists(container, key);
        }

        /// <inheritdoc />
        public virtual void InitializeValue(ConfigurationContainer container, string key, object? defaultValue)
        {
            Argument.IsNotNullOrWhitespace("key", key);

            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                if (!IsValueAvailable(container, key))
                {
                    SetValue(container, key, defaultValue);
                }
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual async Task LoadAsync(ConfigurationContainer configuration)
        {
            switch (configuration)
            {
                case ConfigurationContainer.Local:
                    if (_localConfiguration is null)
                    {
                        var defaultLocalConfigFilePath = GetConfigurationFileName(IO.ApplicationDataTarget.UserLocal);
                        await SetLocalConfigFilePathAsync(defaultLocalConfigFilePath);
                    }
                    break;

                case ConfigurationContainer.Roaming:
                    if (_roamingConfiguration is null)
                    {
                        var defaultRoamingConfigFilePath = GetConfigurationFileName(IO.ApplicationDataTarget.UserRoaming);
                        await SetRoamingConfigFilePathAsync(defaultRoamingConfigFilePath);
                    }
                    break;
            }
        }

        /// <inheritdoc />
        public virtual async Task SaveAsync(ConfigurationContainer configuration)
        {
            switch (configuration)
            {
                case ConfigurationContainer.Local:
                    await SaveLocalConfigurationAsync();
                    break;

                case ConfigurationContainer.Roaming:
                    await SaveRoamingConfigurationAsync();
                    break;
            }
        }

        protected virtual async Task<DynamicConfiguration> LoadConfigurationAsync(string source)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!File.Exists(source))
            {
                // No file, we can really start from scratch
                return new DynamicConfiguration();
            }

            // Try for 5 seconds
            while (stopwatch.ElapsedMilliseconds < 5000)
            {
                try
                {
                    using (var fileStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        if (!fileStream.CanRead)
                        {
                            continue;
                        }

                        if (fileStream.Length == 0)
                        {
                            return new DynamicConfiguration();
                        }

                        var configuration = SavableModelBase<DynamicConfiguration>.Load(fileStream, _serializer);
                        if (configuration is null)
                        {
                            return new DynamicConfiguration();
                        }

                        return configuration;
                    }
                }
                catch (IOException)
                {
                    // allow
                }
            }

            throw Log.ErrorAndCreateException<InvalidOperationException>($"File '{source}' could not be used to load the configuration, it was locked for too long");
        }

        /// <summary>
        /// Determines whether the specified key value exists in the configuration.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
        protected virtual bool ValueExists(ConfigurationContainer container, string key)
        {
            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return false;
                }

                return settings.IsConfigurationValueSet(key);
            }
        }

        /// <summary>
        /// Gets the value from the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        protected virtual object? GetValueFromStore(ConfigurationContainer container, string key)
        {
            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return null;
                }

                return settings.GetConfigurationValue(key);
            }
        }

        /// <summary>
        /// Sets the value to the store.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetValueToStore(ConfigurationContainer container, string key, object? value)
        {
            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return;
                }

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

        protected void RaiseConfigurationChanged(ConfigurationContainer container, string key, object? value)
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

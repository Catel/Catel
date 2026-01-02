namespace Catel.Configuration
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Xml.Linq;
    using Catel.Logging;
    using Catel.Threading;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Services;

    /// <summary>
    /// Configuration service implementation that allows customization how configuration values
    /// are being used inside an application.
    /// </summary>
    public partial class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// If the timer duration is smaller than this threshold, the
        /// timer will not be used.
        /// </summary>
        private const int IgnoreTimerThresholdInMilliseconds = 10;

        private const string ConfigurationFileName = "configuration.json";

        private readonly ILogger<IConfigurationService> _logger;
        private readonly IObjectConverterService _objectConverterService;
        private readonly IAppDataService _appDataService;
        private readonly IConfigurationBuilder _configurationBuilder;

        private IConfiguration? _localConfiguration;
        private IConfiguration? _roamingConfiguration;

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

        public ConfigurationService(ILogger<ConfigurationService> logger,
            IObjectConverterService objectConverterService, IAppDataService appDataService,
            [FromKeyedServices("CatelConfiguration")] IConfigurationBuilder configurationBuilder)
        {
            _logger = logger;
            _objectConverterService = objectConverterService;
            _appDataService = appDataService;
            _configurationBuilder = configurationBuilder;
            _localSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _localSaveConfigurationTimer.Elapsed += OnLocalSaveConfigurationTimerElapsed;

            _roamingSaveConfigurationTimer.Interval = GetSaveSettingsSchedulerIntervalInMilliseconds();
            _roamingSaveConfigurationTimer.Elapsed += OnRoamingSaveConfigurationTimerElapsed;

#if DEBUG
            _localConfigurationLock.EnableExtremeLogging = true;
            _roamingConfigurationLock.EnableExtremeLogging = true;
#endif
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
            var filename = System.IO.Path.Combine(_appDataService.GetApplicationDataDirectory(applicationDataTarget), ConfigurationFileName);
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
                _logger.LogWarning(ex, $"Failed to retrieve configuration value '{Enum<ConfigurationContainer>.ToString(container)}.{key}', returning default value");

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

            _logger.LogDebug($"Setting roaming config file path to '{filePath}'");

            var lockObject = GetLockObject(ConfigurationContainer.Roaming);
            using (await lockObject.LockAsync())
            {
                _roamingConfigFilePath = filePath;
                _roamingConfiguration = await LoadConfigurationAsync(ConfigurationContainer.Roaming, filePath);
            }
        }

        /// <inheritdoc />
        public virtual async Task SetLocalConfigFilePathAsync(string filePath)
        {
            Argument.IsNotNullOrEmpty(nameof(filePath), filePath);

            _logger.LogDebug($"Setting local config file path to '{filePath}'");

            var lockObject = GetLockObject(ConfigurationContainer.Local);
            using (await lockObject.LockAsync())
            {
                _localConfigFilePath = filePath;
                _localConfiguration = await LoadConfigurationAsync(ConfigurationContainer.Local, filePath);
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

        protected virtual async Task<IConfiguration> LoadConfigurationAsync(ConfigurationContainer configurationContainer, string source)
        {
            var builder = _configurationBuilder;

            // At least 1 provider is required so always add 1
            builder = builder.AddInMemoryCollection();

            if (File.Exists(source) &&
                new FileInfo(source).Length > 0)
            {
                builder = builder.AddJsonFile(source, true, false);
            }

            var configuration = (IConfiguration)builder.Build();

            // For backwards compatibility, we will only replace the extension
            var oldConfigurationFile = Path.ChangeExtension(source, ".xml");
            if (File.Exists(oldConfigurationFile))
            {
                var stopwatch = Stopwatch.StartNew();

                // Try for 5 seconds
                while (stopwatch.ElapsedMilliseconds < 5000)
                {
                    try
                    {
                        _logger.LogInformation("Starting migration of xml configuration");

                        using (var fileStream = File.Open(oldConfigurationFile, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            if (!fileStream.CanRead)
                            {
                                continue;
                            }

                            if (fileStream.Length == 0)
                            {
                                continue;
                            }

                            using var streamReader = new StreamReader(fileStream);

                            var fileContents = await streamReader.ReadToEndAsync();

                            var xmlDocument = XDocument.Parse(fileContents);

                            var rootElement = xmlDocument.Root;
                            if (rootElement is not null)
                            {
                                foreach (var childElement in rootElement.Elements())
                                {
                                    var key = childElement.Name.LocalName;
                                    var value = childElement.Value;

                                    var finalKey = GetFinalKey(key);

                                    configuration[finalKey] = value;
                                }
                            }

                            _logger.LogInformation("Storing migrated configuration as json");

                            await SaveConfigurationAsync(configurationContainer, configuration, source);
                        }

                        _logger.LogInformation("Changing extension of migrated configuration to 'xml.bak'");

                        File.Move(oldConfigurationFile, $"{oldConfigurationFile}.bak");

                        break;
                    }
                    catch (IOException)
                    {
                        // allow
                    }
                }
            }

            return configuration;
        }

        /// <summary>
        /// Determines whether the specified key value exists in the configuration.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if the value exists, <c>false</c> otherwise.</returns>
        protected virtual bool ValueExists(ConfigurationContainer container, string key)
        {
            var finalKey = GetFinalKey(key);

            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var configuration = GetSettingsContainer(container);
                if (configuration is null)
                {
                    return false;
                }

                return configuration[finalKey] is not null;
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
            var finalKey = GetFinalKey(key);

            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return null;
                }

                return settings[finalKey];
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
            var finalKey = GetFinalKey(key);

            var lockObject = GetLockObject(container);
            using (lockObject.Lock())
            {
                var settings = GetSettingsContainer(container);
                if (settings is null)
                {
                    return;
                }

                settings[finalKey] = ObjectToStringHelper.ToString(value);

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

            // Convert . to : for section support
            key = key.Replace(".", ":");

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

            throw _logger.LogErrorAndCreateException<InvalidOperationException>($"Container type '{container}' has no lock object");
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

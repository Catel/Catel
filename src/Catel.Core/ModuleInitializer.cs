namespace Catel.Core
{
    using System;
    using Reflection;
    using IoC;
    using Logging;
    using System.Configuration;
    using System.Reflection;
    using System.Collections.Generic;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            try
            {
                var configurations = new List<Configuration>();

                var exeConfig = GetExeConfiguration();
                if (exeConfig is not null)
                {
                    configurations.Add(exeConfig);
                }

                var dllConfig = GetDllConfiguration();
                if (dllConfig is not null)
                {
                    configurations.Add(dllConfig);
                }

                var configFilesHandled = new List<string>();
                foreach (var configuration in configurations)
                {
                    var configPath = configuration.FilePath.ToLower();
                    if (configFilesHandled.Contains(configPath))
                    {
                        continue;
                    }

                    configFilesHandled.Add(configPath);

                    LogManager.LoadListenersFromConfiguration(configuration);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load log listeners from the configuration file");
            }

            var serviceLocator = ServiceLocator.Default;
            var module = new CoreModule();
            module.Initialize(serviceLocator);
        }

        private static Configuration? GetExeConfiguration()
        {
            Configuration? config = null;

            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (ContainsVsHost(config.FilePath))
            {
                return null;
            }

            return config;
        }

        private static Configuration? GetDllConfiguration()
        {
            var entryAssembly = AssemblyHelper.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            if (entryAssembly is null)
            {
                return null;
            }

            var entryAssemblyLocation = entryAssembly.Location;
            if (string.IsNullOrWhiteSpace(entryAssemblyLocation))
            {
                return null;
            }

            if (ContainsVsHost(entryAssemblyLocation))
            {
                return null;
            }

            var configFile = entryAssemblyLocation + ".config";
            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };

            return ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
        }

        private static bool ContainsVsHost(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return value.ContainsIgnoreCase(".vshost.");
        }
    }
}

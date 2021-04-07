// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Core
{
    using System;
    using Reflection;
    using IoC;
    using Logging;

#if NET || NETCORE
    using System.Configuration;
    using System.Reflection;
    using System.Collections.Generic;
#endif

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
#if NET || NETCORE
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
#endif

            var serviceLocator = ServiceLocator.Default;
            var module = new CoreModule();
            module.Initialize(serviceLocator);
        }

#if NET || NETCORE
        private static Configuration GetExeConfiguration()
        {
            Configuration config = null;
            if (HttpContextHelper.HasHttpContext())
            {
                Log.Debug("Application is living in a web context, loading web configuration");

                // All via reflection because we are support .NET 4.0 client profile, reflection equals this call:
                //   config = Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                var webConfigurationManagerType = TypeCache.GetTypeWithoutAssembly("System.Web.Configuration.WebConfigurationManager", allowInitialization: false);
                var openWebConfigurationMethodInfo = webConfigurationManagerType.GetMethodEx("OpenWebConfiguration", new[] { typeof(string) }, allowStaticMembers: true);
                config = (Configuration)openWebConfigurationMethodInfo.Invoke(null, new[] { "~" });
            }
            else
            {
                Log.Debug("Application is living in an application context, loading application configuration");

                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (ContainsVsHost(config.FilePath))
                {
                    return null;
                }
            }

            return config;
        }

        private static Configuration GetDllConfiguration()
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
#endif
    }
}

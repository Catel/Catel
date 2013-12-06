// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Core
{
    using System;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using ExceptionHandling;
    using IoC;
    using Messaging;
    using Catel.Logging;

#if NET
    using System.Configuration;
    using System.Web;
    using Catel.Configuration;
    using System.Reflection;
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
#if NET
            try
            {
                bool isWebContext = false;

                var httpContextType = TypeCache.GetTypeWithoutAssembly("System.Web.HttpContext");
                if (httpContextType != null)
                {
                    var currentPropertyInfo = httpContextType.GetProperty("Current", BindingFlags.Public | BindingFlags.Static);
                    if (currentPropertyInfo != null)
                    {
                        isWebContext = (currentPropertyInfo.GetValue(null, null) != null);
                    }
                }

                Configuration config = null;
                if (isWebContext)
                {
                    Log.Debug("Application is living in a web context, loading web configuration");

                    // All via reflection because we are support .NET 4.0 client profile, reflection equals this call:
                    //   config = Configuration.WebConfigurationManager.OpenWebConfiguration("~");
                    var webConfigurationManagerType = TypeCache.GetTypeWithoutAssembly("System.Web.Configuration.WebConfigurationManager");
                    var openWebConfigurationMethodInfo = webConfigurationManagerType.GetMethodEx("OpenWebConfiguration", new [] { typeof(string) }, allowStaticMembers: true);
                    config = (Configuration) openWebConfigurationMethodInfo.Invoke(null, new []{ "~" });
                }
                else
                {
                    Log.Debug("Application is living in an application context, loading application configuration");

                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                }

                if (config != null)
                {
                    var configurationSection = config.GetSection<LoggingConfigurationSection>("logging", "catel");
                    if (configurationSection != null)
                    {
                        var logListeners = configurationSection.GetLogListeners();
                        foreach (var logListener in logListeners)
                        {
                            LogManager.AddListener(logListener);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load log listeners from the configuration file");
            }
#endif

            var serviceLocator = ServiceLocator.Default;
            CoreModule.RegisterServices(serviceLocator);
        }
    }
}
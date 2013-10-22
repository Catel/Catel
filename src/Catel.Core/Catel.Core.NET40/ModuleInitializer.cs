// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Core
{
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using ExceptionHandling;
    using IoC;
    using Messaging;

#if NET
    using System.Configuration;
    using Catel.Configuration;
    using Catel.Logging;
#endif

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
#if NET
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (openExeConfiguration != null)
            {
                var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");
                if (configurationSection != null)
                {
                    var logListeners = configurationSection.GetLogListeners();
                    foreach (var logListener in logListeners)
                    {
                        LogManager.AddListener(logListener);
                    }
                }
            }
#endif

            var serviceLocator = ServiceLocator.Default;
            CoreModule.RegisterServices(serviceLocator);
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingConfigurationSection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Logging
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;

    /// <summary>
    /// Logging configuration section.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <configuration>
    ///     <configSections>
    ///         <sectionGroup name="catel">
    ///             <section name="logging" type="Catel.Logging.LoggingConfigurationSection, Catel.Core" />
    ///         </sectionGroup>
    ///     </configSections>
    ///     <catel>
    ///         <logging>
    ///             <listeners>
    ///                 <listener type="Catel.Logging.FileLogListener"
    ///                     [IsDebugEnabled="true"]
    ///                     [IsInfoEnabled="true"]
    ///                     [IsWarningEnabled="true"]
    ///                     [IsErrorEnabled="true"]
    ///                     [IgnoreCatelLogging="true"] />
    ///             </listeners>
    ///         </logging>
    ///     </catel>
    /// </configuration>
    ///  ]]>
    /// </code>
    /// </example>
    public sealed class LoggingConfigurationSection : ConfigurationSection
    {
        #region Constants
        /// <summary>
        /// The logging configuration collection property name.
        /// </summary>
        private const string LoggingConfigurationCollectionPropertyName = "listeners";
        #endregion

        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the logging configuration collection.
        /// </summary>
        [ConfigurationProperty(LoggingConfigurationCollectionPropertyName, IsDefaultCollection = false)]
        public LogListenerConfigurationCollection LogListenerConfigurationCollection
        {
            get { return (LogListenerConfigurationCollection)base[LoggingConfigurationCollectionPropertyName]; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the log listeners.
        /// </summary>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        /// <returns>IEnumerable{ILogListener}.</returns>
        public IEnumerable<ILogListener> GetLogListeners(Assembly assembly = null)
        {
            var logListeners = new List<ILogListener>();

            Log.Debug("Instantiating {0} log listener(s) from configuration", LogListenerConfigurationCollection.Count.ToString());

            foreach (LogListenerConfiguration logListenerConfiguration in LogListenerConfigurationCollection)
            {
                logListeners.Add(logListenerConfiguration.GetLogListener(assembly));
            }

            Log.Debug("Instantiated {0} log listener(s) from configuration", logListeners.Count.ToString());

            return logListeners;
        }
        #endregion
    }
}

#endif

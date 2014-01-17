// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggingConfigurationSection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Logging
{
    using System.Collections.Generic;
    using System.Configuration;

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
        /// <returns>IEnumerable{ILogListener}.</returns>
        public IEnumerable<ILogListener> GetLogListeners()
        {
            var logListeners = new List<ILogListener>();

            Log.Debug("Instantiating {0} log listener(s) from configuration", LogListenerConfigurationCollection.Count);

            foreach (LogListenerConfiguration logListenerConfiguration in LogListenerConfigurationCollection)
            {
                logListeners.Add(logListenerConfiguration.GetLogListener());
            }

            Log.Debug("Instantiated {0} log listener(s) from configuration", logListeners.Count);

            return logListeners;
        }
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfigurationSectionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Tests.Logging.Configuration
{
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class IoLoggingConfigurationSectionFacts
    {
        #region Methods
        [TestCase]
        public void LoadSectionFromConfigurationFileTest()
        {
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");

            Assert.IsNotNull(configurationSection.LogListenerConfigurationCollection);
            Assert.AreNotEqual(0, configurationSection.LogListenerConfigurationCollection.Count);
        }

        [TestCase]
        public void InitializeLogListenersFromConfiguration()
        {
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");

            var logListeners = configurationSection.GetLogListeners();

            Assert.AreEqual(logListeners.Count(), 1);

            var fileLogListener = (FileLogListener)logListeners.First();
            
            Assert.IsTrue(fileLogListener.IgnoreCatelLogging);
            Assert.IsFalse(fileLogListener.IsDebugEnabled);
            Assert.IsTrue(fileLogListener.IsInfoEnabled);
            Assert.IsTrue(fileLogListener.IsWarningEnabled);
            Assert.IsTrue(fileLogListener.IsErrorEnabled);

            var assembly = typeof(FileLogListener).Assembly;
            var appDataDirectory = Catel.IO.Path.GetApplicationDataDirectory(assembly.Company(), assembly.Product());
            //Assert.AreEqual(fileLogListener.FilePath, Path.Combine(appDataDirectory, "CatelLogging.txt.log"));
        }
        #endregion
    }
}

#endif

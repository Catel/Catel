// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfigurationSectionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Logging.Configuration
{
    using System.Configuration;
    using System.Linq;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class IoLoggingConfigurationSectionFacts
    {
        #region Methods
        [TestMethod]
        public void LoadSectionFromConfigurationFileTest()
        {
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");

            Assert.IsNotNull(configurationSection.LogListenerConfigurationCollection);
            Assert.AreNotEqual(0, configurationSection.LogListenerConfigurationCollection.Count);
        }

        [TestMethod]
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
            Assert.AreEqual(fileLogListener.FilePath, "CatelLogging.txt");
        }
        #endregion
    }
}

#endif
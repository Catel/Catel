namespace Catel.Tests.Logging.Configuration
{
    using System.Configuration;
    using System.Linq;
    using Catel.Configuration;
    using Catel.Logging;
    using Catel.Reflection;
    using NUnit.Framework;

    [TestFixture, Explicit]
    public class IoLoggingConfigurationSectionFacts
    {
        [TestCase]
        public void LoadSectionFromConfigurationFileTest()
        {
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");

            Assert.That(configurationSection.LogListenerConfigurationCollection, Is.Not.Null);
            Assert.That(configurationSection.LogListenerConfigurationCollection.Count, Is.Not.EqualTo(0));
        }

        [TestCase]
        public void InitializeLogListenersFromConfiguration()
        {
            var openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<LoggingConfigurationSection>("logging", "catel");

            var logListeners = configurationSection.GetLogListeners();

            Assert.That(logListeners.Count(), Is.EqualTo(1));

            var fileLogListener = (FileLogListener)logListeners.First();

            Assert.That(fileLogListener.IgnoreCatelLogging, Is.True);
            Assert.That(fileLogListener.IsDebugEnabled, Is.False);
            Assert.That(fileLogListener.IsInfoEnabled, Is.True);
            Assert.That(fileLogListener.IsWarningEnabled, Is.True);
            Assert.That(fileLogListener.IsErrorEnabled, Is.True);

            var assembly = typeof(FileLogListener).Assembly;
            var appDataDirectory = Catel.IO.Path.GetApplicationDataDirectory(assembly.Company(), assembly.Product());
            //Assert.AreEqual(fileLogListener.FilePath, Path.Combine(appDataDirectory, "CatelLogging.txt.log"));
        }
    }
}

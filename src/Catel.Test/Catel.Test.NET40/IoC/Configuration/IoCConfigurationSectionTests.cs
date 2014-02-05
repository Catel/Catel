// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfigurationSectionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.IoC.Configuration
{
    using System.Configuration;
    using System.Linq;
    using Catel.Configuration;
    using Catel.IoC;
    
#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class IoCConfigurationSectionTests
    {
        #region Methods
        [TestMethod]
        public void LoadSectionFromConfigurationFileTest()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            Assert.IsNotNull(configurationSection.ServiceLocatorConfigurationCollection);
            Assert.AreNotEqual(0, configurationSection.ServiceLocatorConfigurationCollection.Count);
        }

        [TestMethod]
        public void InitializeServiceLocatorFromDefaultConfiguration()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            var serviceLocatorConfiguration = configurationSection.DefaultServiceLocatorConfiguration;
            Assert.IsNotNull(serviceLocatorConfiguration);

            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocatorConfiguration.Configure(serviceLocator);

            Assert.AreEqual(serviceLocatorConfiguration.SupportDependencyInjection, serviceLocator.SupportDependencyInjection);
            foreach (Registration registration in serviceLocatorConfiguration)
            {
                serviceLocator.IsTypeRegistered(registration.InterfaceType);
                if (registration.RegistrationType == RegistrationType.Singleton)
                {
                    serviceLocator.IsTypeRegisteredAsSingleton(registration.InterfaceType);
                }
            }
        }

        [TestMethod]
        public void InitializeServiceLocatorFromNonDefaultConfiguration()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            var serviceLocatorConfiguration = configurationSection.GetServiceLocatorConfiguration("test");
            Assert.IsNotNull(serviceLocatorConfiguration);

            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocatorConfiguration.Configure(serviceLocator);

            Assert.AreEqual(serviceLocatorConfiguration.SupportDependencyInjection, serviceLocator.SupportDependencyInjection);
            foreach (Registration registration in serviceLocatorConfiguration)
            {
                serviceLocator.IsTypeRegistered(registration.InterfaceType);
                if (registration.RegistrationType == RegistrationType.Singleton)
                {
                    serviceLocator.IsTypeRegisteredAsSingleton(registration.InterfaceType);
                }
            }
        }
        #endregion
    }
}
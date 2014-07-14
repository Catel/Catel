// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfigurationSectionTests.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.IoC.Configuration
{
    using System.Configuration;
    using Catel.Configuration;
    using Catel.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class IoCConfigurationSectionTests
    {
        #region Methods
        [TestCase]
        public void LoadSectionFromConfigurationFileTest()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            Assert.IsNotNull(configurationSection.ServiceLocatorConfigurationCollection);
            Assert.AreNotEqual(0, configurationSection.ServiceLocatorConfigurationCollection.Count);
        }

        [TestCase]
        public void InitializeServiceLocatorFromDefaultConfiguration()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            var serviceLocatorConfiguration = configurationSection.DefaultServiceLocatorConfiguration;
            Assert.IsNotNull(serviceLocatorConfiguration);

            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocatorConfiguration.Configure(serviceLocator);

            foreach (Registration registration in serviceLocatorConfiguration)
            {
                serviceLocator.IsTypeRegistered(registration.InterfaceType);
                if (registration.RegistrationType == RegistrationType.Singleton)
                {
                    serviceLocator.IsTypeRegisteredAsSingleton(registration.InterfaceType);
                }
            }
        }

        [TestCase]
        public void InitializeServiceLocatorFromNonDefaultConfiguration()
        {
            Configuration openExeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var configurationSection = openExeConfiguration.GetSection<IoCConfigurationSection>("ioc", "catel");
            var serviceLocatorConfiguration = configurationSection.GetServiceLocatorConfiguration("test");
            Assert.IsNotNull(serviceLocatorConfiguration);

            var serviceLocator = IoCFactory.CreateServiceLocator();
            serviceLocatorConfiguration.Configure(serviceLocator);

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

#endif
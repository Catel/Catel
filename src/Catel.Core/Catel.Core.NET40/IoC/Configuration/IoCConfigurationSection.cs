// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoCConfigurationSection.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System.Configuration;
    using System.Linq;

    /// <summary>
    /// IoC configuration section.
    /// </summary>
    /// <example>
    /// <code>
    /// <![CDATA[
    /// <configuration>
    ///     <configSections>
    ///         <sectionGroup name="catel">
    ///             <section name="ioc" type="Catel.IoC.IoCConfigurationSection, Catel.Core" />
    ///         </sectionGroup>
    ///     </configSections>
    ///     <catel>
    ///         <ioc>
    ///             <serviceLocatorConfigurations>
    ///                 <serviceLocatorConfiguration [name="default"] [supportDependencyInjection="true"]>
    ///                     <register interfaceType="Catel.MVVM.Services.IUIVisualizerService" implementationType="Catel.MVVM.Services.UIVisualizerService" />
    ///                     <register interfaceType="Catel.MVVM.Services.IProcessService" implementationType="Catel.MVVM.Services.ProcessService" />
    ///                     <!-- Add more registrations here if is requiered -->
    ///                 </serviceLocatorConfiguration>
    ///                 <serviceLocatorConfiguration name="test" supportDependencyInjection="false">
    ///                     <register interfaceType="Catel.MVVM.Services.IUIVisualizerService" implementationType="Catel.MVVM.Services.Test.UIVisualizerService" type="Transient"/>
    ///                     <register interfaceType="Catel.MVVM.Services.IProcessService" implementationType="Catel.MVVM.Services.Test.ProcessService" />
    ///                     <!-- Add more registrations here if is requiered -->
    ///                 </serviceLocatorConfiguration>
    ///             </serviceLocatorConfigurations>
    ///         </ioc>
    ///     </catel>
    /// </configuration>
    ///  ]]>
    /// </code>
    /// </example>
    public sealed class IoCConfigurationSection : ConfigurationSection
    {
        #region Constants
        
        /// <summary>
        /// The service locator configuration collection property name.
        /// </summary>
        private const string ServiceLocatorConfigurationCollectionPropertyName = "serviceLocatorConfigurations";
        
        #endregion

        #region Properties
        /// <summary>
        /// Gets the IoC configuration collection.
        /// </summary>
        [ConfigurationProperty(ServiceLocatorConfigurationCollectionPropertyName, IsDefaultCollection = false)]
        public ServiceLocatorConfigurationCollection ServiceLocatorConfigurationCollection
        {
            get
            {
				return (ServiceLocatorConfigurationCollection)base[ServiceLocatorConfigurationCollectionPropertyName];
            }
        }
        
        /// <summary>
        /// Gets the Default service locator configuration.
        /// </summary>
        public ServiceLocatorConfiguration DefaultServiceLocatorConfiguration
        {
        	get 
        	{
				return ServiceLocatorConfigurationCollection.Cast<ServiceLocatorConfiguration>().FirstOrDefault(element => element.Name == "default");		 
        	}
        }
        
        /// <summary>
        /// Gets the service locator configuration from a given name.
        /// </summary>
        /// <param name="name">The name of the service locator configuration.</param>
        public ServiceLocatorConfiguration GetServiceLocatorConfiguration(string name = "default")
        {
			return ServiceLocatorConfigurationCollection.Cast<ServiceLocatorConfiguration>().FirstOrDefault(element => element.Name == name);		 
        } 
      
        #endregion
    }
}
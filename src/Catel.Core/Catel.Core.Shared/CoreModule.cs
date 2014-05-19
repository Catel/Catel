// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Catel.Configuration;
    using Catel.Data;
    using Catel.ExceptionHandling;
    using Catel.IoC;
    using Catel.Messaging;
    using Catel.Runtime.Serialization;
    using Catel.Services;

#if NET
    using Catel.Runtime.Serialization.Binary;
#endif

    using Catel.Runtime.Serialization.Xml;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public class CoreModule : IServiceLocatorInitializer
    {
        /// <summary>
        /// Initializes the specified service locator.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        public void Initialize(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            serviceLocator.RegisterType<ILanguageService, LanguageService>();
            serviceLocator.RegisterInstance<IExceptionService>(ExceptionService.Default);
            serviceLocator.RegisterInstance<IMessageMediator>(MessageMediator.Default);

            serviceLocator.RegisterType<IValidatorProvider, AttributeValidatorProvider>();
            serviceLocator.RegisterType<IRegistrationConventionHandler, RegistrationConventionHandler>();

#if NET
            serviceLocator.RegisterType<IBinarySerializer, BinarySerializer>();
#endif
            serviceLocator.RegisterType<IDataContractSerializerFactory, DataContractSerializerFactory>();
            serviceLocator.RegisterType<IXmlSerializer, XmlSerializer>();
            serviceLocator.RegisterType<IXmlNamespaceManager, XmlNamespaceManager>();
            serviceLocator.RegisterType<ISerializationManager, SerializationManager>();

            serviceLocator.RegisterType<IModelEqualityComparer, ModelEqualityComparer>();
            serviceLocator.RegisterType<IConfigurationService, ConfigurationService>();
        }
    }
}
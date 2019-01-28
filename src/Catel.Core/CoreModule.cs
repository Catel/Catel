// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using Configuration;
    using Data;
    using ExceptionHandling;
    using IoC;
    using Messaging;
    using Runtime.Serialization;
    using Services;

#if NET || NETCORE
    using Runtime.Serialization.Binary;
#endif

    using Runtime.Serialization.Xml;

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
            Argument.IsNotNull("serviceLocator", serviceLocator);

            serviceLocator.RegisterType<ILanguageService, LanguageService>();
            serviceLocator.RegisterInstance<IExceptionService>(ExceptionService.Default);
            serviceLocator.RegisterInstance<IMessageMediator>(MessageMediator.Default);

            serviceLocator.RegisterType<IValidatorProvider, AttributeValidatorProvider>();
            serviceLocator.RegisterType<IRegistrationConventionHandler, RegistrationConventionHandler>();

#if NET || NETCORE
            serviceLocator.RegisterType<IBinarySerializer, BinarySerializer>();
            serviceLocator.RegisterTypeWithTag<ISerializationContextInfoFactory, BinarySerializationContextInfoFactory>(typeof(BinarySerializer));
#endif
            serviceLocator.RegisterType<IDataContractSerializerFactory, DataContractSerializerFactory>();
            serviceLocator.RegisterType<IXmlSerializer, XmlSerializer>();
            serviceLocator.RegisterType<IXmlNamespaceManager, XmlNamespaceManager>();
            serviceLocator.RegisterType<ISerializationManager, SerializationManager>();
            serviceLocator.RegisterType<IObjectAdapter, ObjectAdapter>();

            serviceLocator.RegisterType<ISerializer, XmlSerializer>();
            serviceLocator.RegisterTypeWithTag<ISerializationContextInfoFactory, XmlSerializationContextInfoFactory>(typeof(XmlSerializer));

            serviceLocator.RegisterType<IModelEqualityComparer, ModelEqualityComparer>();
            serviceLocator.RegisterType<IConfigurationService, ConfigurationService>();
            serviceLocator.RegisterType<IObjectConverterService, ObjectConverterService>();
            serviceLocator.RegisterType<IRollingInMemoryLogService, RollingInMemoryLogService>();
        }
    }
}

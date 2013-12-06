// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CoreModule.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using Catel.Data;
    using Catel.ExceptionHandling;
    using Catel.IoC;
    using Catel.Messaging;
    using Catel.Runtime.Serialization;

#if NET
    using Catel.Runtime.Serialization.Binary;
#endif

    using Catel.Runtime.Serialization.Xml;

    /// <summary>
    /// Core module which allows the registration of default services in the service locator.
    /// </summary>
    public static class CoreModule
    {
        /// <summary>
        /// Registers the services in the specified <see cref="IServiceLocator" />.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        public static void RegisterServices(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => serviceLocator);

            serviceLocator.RegisterInstance<IExceptionService>(ExceptionService.Default);
            serviceLocator.RegisterInstance<IMessageMediator>(MessageMediator.Default);

            serviceLocator.RegisterTypeIfNotYetRegistered<IValidatorProvider, AttributeValidatorProvider>();

#if NET
            serviceLocator.RegisterType<IBinarySerializer, BinarySerializer>();
#endif

            serviceLocator.RegisterType<IDataContractSerializerFactory, DataContractSerializerFactory>();
            serviceLocator.RegisterType<IXmlSerializer, XmlSerializer>();
            serviceLocator.RegisterType<IXmlNamespaceManager, XmlNamespaceManager>();
            serviceLocator.RegisterType<ISerializationManager, SerializationManager>();

            serviceLocator.RegisterType<IModelEqualityComparer, ModelEqualityComparer>();
        }
    }
}
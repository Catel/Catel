// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleInitializer.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using ExceptionHandling;
    using IoC;
    using Messaging;

    /// <summary>
    /// Class that gets called as soon as the module is loaded.
    /// </summary>
    /// <remarks>
    /// This is made possible thanks to Fody.
    /// </remarks>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initializes the module.
        /// </summary>
        public static void Initialize()
        {
            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterInstance<IExceptionService>(ExceptionService.Default);
            serviceLocator.RegisterInstance<IMessageMediator>(MessageMediator.Default);

            serviceLocator.RegisterTypeIfNotYetRegistered<IValidatorProvider, AttributeValidatorProvider>();

#if NET
            serviceLocator.RegisterType<IBinarySerializer, BinarySerializer>();
#endif

            serviceLocator.RegisterType<IDataContractNameResolver, DataContractNameResolver>();
            serviceLocator.RegisterType<IDataContractSerializerFactory, DataContractSerializerFactory>();
            serviceLocator.RegisterType<IXmlSerializer, XmlSerializer>();
        }
    }
}
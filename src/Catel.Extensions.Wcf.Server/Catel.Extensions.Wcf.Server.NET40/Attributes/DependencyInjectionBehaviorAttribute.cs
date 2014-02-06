// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DependencyInjectionBehaviorAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Dispatching;
    using IoC;
    using Reflection;

    /// <summary>
    /// Attribute which allow dependency injection in the service implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DependencyInjectionBehaviorAttribute : Attribute, IServiceBehavior
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyInjectionBehaviorAttribute" /> class.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="tag">The tag.</param>
        public DependencyInjectionBehaviorAttribute(Type contractType,
            RegistrationType registrationType = RegistrationType.Singleton, object tag = null)
        {
            Argument.IsNotNull("registrationType", registrationType);

            ContractType = contractType;
            RegistrationType = registrationType;
            Tag = tag;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyInjectionBehaviorAttribute" /> class.
        /// </summary>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="tag">The tag.</param>
        public DependencyInjectionBehaviorAttribute(RegistrationType registrationType = RegistrationType.Singleton,
            object tag = null)
            : this(null, registrationType, tag)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the type of the contract.
        /// </summary>
        /// <value>
        ///     The type of the contract.
        /// </value>
        public Type ContractType { get; private set; }

        /// <summary>
        ///     Gets the type of the service.
        /// </summary>
        /// <value>
        ///     The type of the service.
        /// </value>
        public Type ServiceType { get; private set; }

        /// <summary>
        ///     Gets the registration type.
        /// </summary>
        public RegistrationType RegistrationType { get; private set; }

        /// <summary>
        ///     Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }
        #endregion

        #region IServiceBehavior Members
        /// <summary>
        ///     Provides the ability to pass custom data to binding elements to support the contract implementation.
        /// </summary>
        /// <param name="serviceDescription">The service description of the service.</param>
        /// <param name="serviceHostBase">The host of the service.</param>
        /// <param name="endpoints">The service endpoints.</param>
        /// <param name="bindingParameters">Custom objects to which binding elements have access.</param>
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        ///     Provides the ability to change run-time property values or insert custom extension objects such as error handlers,
        ///     message or parameter interceptors, security extensions, and other custom extension objects.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The host that is currently being built.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceDescription" /> is <c>null</c>.</exception>
        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            Argument.IsNotNull("serviceDescription", serviceDescription);

            ServiceType = serviceDescription.ServiceType;

            if (ObjectHelper.IsNull(ContractType))
            {
                var serviceTypeInterfaces = ServiceType.GetInterfacesEx();
                if (serviceTypeInterfaces != null && serviceTypeInterfaces.Any())
                {
                    ContractType = serviceTypeInterfaces.FirstOrDefault();
                }
            }

            IInstanceProvider instanceProvider = new InstanceProvider(ServiceLocator.Default, ContractType, ServiceType,
                Tag, RegistrationType);

            var dispatchRuntimes =
                serviceHostBase.ChannelDispatchers.Cast<ChannelDispatcher>()
                    .SelectMany(
                        dispatcher =>
                            dispatcher.Endpoints.Select(endpointDispatcher => endpointDispatcher.DispatchRuntime));

            foreach (var dispatchRuntime in dispatchRuntimes)
            {
                dispatchRuntime.InstanceProvider = instanceProvider;
                dispatchRuntime.InstanceContextInitializers.Add(new InstanceContextInitializer());
            }
        }

        /// <summary>
        ///     Provides the ability to inspect the service host and the service description to confirm that the service can run
        ///     successfully.
        /// </summary>
        /// <param name="serviceDescription">The service description.</param>
        /// <param name="serviceHostBase">The service host that is currently being constructed.</param>
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
        #endregion
    }

    /// <summary>
    /// Attribute which allow dependency injection in the service implementation.
    /// </summary>
    [ObsoleteEx(Replacement = "DependencyInjectionBehaviorAttribute", TreatAsErrorFromVersion = "4.1", RemoveInVersion = "5.0")]
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceLocatorRegistrationBehaviorAttribute : DependencyInjectionBehaviorAttribute
    {
    }
}
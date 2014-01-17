// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHost.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Hosting
{
    using System;
    using System.ServiceModel.Description;
    using Behaviors;
    using Dispatching;
    using IoC;

    /// <summary>
    /// </summary>
    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceHost" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceType"/> is <c>null</c>.</exception>
        public ServiceHost(IServiceLocator serviceLocator, Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);
            Argument.IsNotNull("serviceType", serviceType);

            ServiceLocator = serviceLocator;
            ServiceType = serviceType;

            ApplyServiceBehaviors(ServiceLocator);

            ApplyContractBehaviors(ServiceLocator);

            foreach (var contractDescription in ImplementedContracts.Values)
            {
                var contractBehavior =
                    new ContractBehavior(new InstanceProvider(ServiceLocator, contractDescription.ContractType,
                        ServiceType));

                contractDescription.Behaviors.Add(contractBehavior);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the service locator.
        /// </summary>
        /// <value>
        ///     The service locator.
        /// </value>
        public IServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        ///     Gets the type of the service.
        /// </summary>
        /// <value>
        ///     The type of the service.
        /// </value>
        public Type ServiceType { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        ///     Applies the contract behaviors.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        private void ApplyContractBehaviors(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            var registeredContractBehaviors = serviceLocator.ResolveTypes<IContractBehavior>();

            if (registeredContractBehaviors == null)
            {
                return;
            }

            foreach (var contractBehavior in registeredContractBehaviors)
            {
                foreach (var contractDescription in ImplementedContracts.Values)
                {
                    contractDescription.Behaviors.Add(contractBehavior);
                }
            }
        }

        /// <summary>
        ///     Applies the service behaviors.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="serviceLocator"/> is <c>null</c>.</exception>
        private void ApplyServiceBehaviors(IServiceLocator serviceLocator)
        {
            Argument.IsNotNull("serviceLocator", serviceLocator);

            var registeredServiceBehaviors = serviceLocator.ResolveTypes<IServiceBehavior>();

            if (registeredServiceBehaviors == null)
            {
                return;
            }

            foreach (var serviceBehavior in registeredServiceBehaviors)
            {
                Description.Behaviors.Add(serviceBehavior);
            }
        }
        #endregion
    }
}
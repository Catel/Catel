// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceHost.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.ServiceModel.Description;
    using Behaviors;
    using Dispatching;
    using IoC;
    using Logging;

    /// <summary>
    /// </summary>
    [ObsoleteEx(Message = "We are considering to remove Wcf.Server support. See https://catelproject.atlassian.net/browse/CTL-672",
        TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class ServiceHost : System.ServiceModel.ServiceHost
    {
        #region Fields
        private readonly ILog _log;
        #endregion

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

            _log = LogManager.GetCurrentClassLogger();

            ServiceType = serviceType;

            ApplyServiceBehaviors(serviceLocator);

            ApplyContractBehaviors(serviceLocator);

            foreach (var contractDescription in ImplementedContracts.Values)
            {
                var contractBehavior =
                    new ContractBehavior(new InstanceProvider(serviceLocator, contractDescription.ContractType,
                        ServiceType));

                contractDescription.Behaviors.Add(contractBehavior);
            }
        }
        #endregion

        #region Properties

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
        private void ApplyContractBehaviors(IServiceLocator serviceLocator)
        {
            var registeredContractBehaviors = serviceLocator.ResolveTypes<IContractBehavior>();

            if (registeredContractBehaviors == null)
            {
                return;
            }

            foreach (var contractBehavior in registeredContractBehaviors)
            {
                foreach (var contractDescription in ImplementedContracts.Values)
                {
                    _log.Debug("Applying the contract behavior '{0}' on contract description '{1}'", contractBehavior, contractDescription);
                    contractDescription.Behaviors.Add(contractBehavior);
                }
            }
        }

        /// <summary>
        ///     Applies the service behaviors.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        private void ApplyServiceBehaviors(IServiceLocator serviceLocator)
        {
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContractBehavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel.Behaviors
{
    using System;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    /// <summary>
    /// The contract behavior.
    /// </summary>
    public class ContractBehavior : IContractBehavior
    {
        #region Fields
        /// <summary>
        /// The _instance provider
        /// </summary>
        private readonly IInstanceProvider _instanceProvider;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ContractBehavior"/> class.
        /// </summary>
        /// <param name="instanceProvider">The instance provider.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceProvider"/> is <c>null</c>.</exception>
        public ContractBehavior(IInstanceProvider instanceProvider)
        {
            Argument.IsNotNull("instanceProvider", instanceProvider);

            _instanceProvider = instanceProvider;
        }
        #endregion

        #region IContractBehavior Members
        /// <summary>
        /// Configures any binding elements to support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract description to modify.</param>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint,
            BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description for which the extension is intended.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientRuntime">The client runtime.</param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            ClientRuntime clientRuntime)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description to be modified.</param>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="dispatchRuntime">The dispatch runtime that controls service execution.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatchRuntime"/> is <c>null</c>.</exception>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint,
            DispatchRuntime dispatchRuntime)
        {
            Argument.IsNotNull("dispatchRuntime", dispatchRuntime);

            dispatchRuntime.InstanceProvider = _instanceProvider;
            dispatchRuntime.InstanceContextInitializers.Add(new InstanceContextInitializer());
        }

        /// <summary>
        /// Implement to confirm that the contract and endpoint can support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract to validate.</param>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }
        #endregion
    }
}
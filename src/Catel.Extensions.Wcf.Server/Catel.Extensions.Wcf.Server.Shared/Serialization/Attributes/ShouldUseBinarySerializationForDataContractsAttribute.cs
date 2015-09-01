// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShouldUseBinarySerializationForDataContractsAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.Linq;
    using System.ServiceModel.Description;
    using Behaviors;
    using Reflection;

    /// <summary>
    /// Decore your service contract with this attribute to use binary serialization mechanism on datacontracts serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    [ObsoleteEx(Message = "We are considering to remove Wcf.Server support. See https://catelproject.atlassian.net/browse/CTL-672",
        TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
    public class ShouldUseBinarySerializationForDataContractsAttribute : Attribute, IContractBehavior
    {
        #region IContractBehavior Members
        /// <summary>
        /// Configures any binding elements to support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract description to modify.</param>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description for which the extension is intended.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientRuntime">The client runtime.</param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            ReplaceSerializerOperationBehavior(contractDescription);
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description to be modified.</param>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="dispatchRuntime">The dispatch runtime that controls service execution.</param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
            ReplaceSerializerOperationBehavior(contractDescription);
        }

        /// <summary>
        /// Implement to confirm that the contract and endpoint can support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract to validate.</param>
        /// <param name="endpoint">The endpoint to validate.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="contractDescription"/> is <c>null</c>.</exception>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            Argument.IsNotNull("contractDescription", contractDescription);

            var messageDescriptions = contractDescription.Operations.SelectMany(operationDescription => operationDescription.Messages);

            foreach (var messageDescription in messageDescriptions)
            {
                ValidateMessagePartDescription(messageDescription.Body.ReturnValue);

                var messagePartDescriptions = messageDescription.Body.Parts;

                foreach (var messagePartDescription in messagePartDescriptions)
                {
                    ValidateMessagePartDescription(messagePartDescription);
                }

                var messageHeaderDescriptions = messageDescription.Headers;

                foreach (var messageHeaderDescription in messageHeaderDescriptions)
                {
                    ValidateBinarySerializableType(messageHeaderDescription.Type);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Replaces the serializer operation behavior.
        /// </summary>
        /// <param name="contract">The contract.</param>
        private static void ReplaceSerializerOperationBehavior(ContractDescription contract)
        {
            foreach (var operationDescription in contract.Operations)
            {
                for (var index = 0; index < operationDescription.Behaviors.Count; index++)
                {
                    var dataContractSerializerOperationBehavior = operationDescription.Behaviors[index] as DataContractSerializerOperationBehavior;
                    if (dataContractSerializerOperationBehavior != null)
                    {
                        operationDescription.Behaviors[index] = new BinarySerializerOperationBehavior(operationDescription);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the message part description.
        /// </summary>
        /// <param name="messagePartDescription">The message part description.</param>
        private void ValidateMessagePartDescription(MessagePartDescription messagePartDescription)
        {
            if (messagePartDescription != null)
            {
                ValidateBinarySerializableType(messagePartDescription.Type);
            }
        }

        /// <summary>
        /// Validates the type of the binary serializable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Binary optimized serialization is supported in public types only
        /// or
        /// Binary optimized serializable types must have a public, parameterless constructor
        /// </exception>
        private static void ValidateBinarySerializableType(Type type)
        {
            if (type == typeof (void))
            {
                return;
            }

            if (!type.IsPublicEx())
            {
                throw new InvalidOperationException("Binary optimized serialization is supported in public types only");
            }

            var defaultConstructor = type.GetConstructorEx(new Type[0]);
            if (defaultConstructor == null && !type.IsPrimitive)
            {
                throw new InvalidOperationException("Binary optimized serializable types must have a public, parameterless constructor");
            }
        }
        #endregion
    }
}
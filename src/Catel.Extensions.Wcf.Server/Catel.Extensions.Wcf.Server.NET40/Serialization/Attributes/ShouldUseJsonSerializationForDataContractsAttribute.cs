// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShouldUseJsonSerializationForDataContractsAttribute.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ServiceModel
{
    using System;
    using System.Linq;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using Behaviors;
    using Reflection;

    /// <summary>
    /// Decore your service contract with this attribute to use json serialization mechanism on datacontracts serialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class ShouldUseJsonSerializationForDataContractsAttribute : Attribute, IContractBehavior
    {
        #region IContractBehavior Members
        /// <summary>
        /// Configures any binding elements to support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract description to modify.</param>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description for which the extension is intended.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="clientRuntime">The client runtime.</param>
        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ReplaceSerializerOperationBehavior(contractDescription);
        }

        /// <summary>
        /// Implements a modification or extension of the client across a contract.
        /// </summary>
        /// <param name="contractDescription">The contract description to be modified.</param>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="dispatchRuntime">The dispatch runtime that controls service execution.</param>
        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime)
        {
            ReplaceSerializerOperationBehavior(contractDescription);
        }

        /// <summary>
        /// Implement to confirm that the contract and endpoint can support the contract behavior.
        /// </summary>
        /// <param name="contractDescription">The contract to validate.</param>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
            var messageDescriptions = contractDescription.Operations.SelectMany(operationDescription => operationDescription.Messages);

            foreach (var messageDescription in messageDescriptions)
            {
                ValidateMessagePartDescription(messageDescription.Body.ReturnValue);

                var messagePartDescriptions = messageDescription.Body.Parts;

                foreach (var messagePartDescription in messagePartDescriptions)
                {
                    ValidateMessagePartDescription(messagePartDescription);
                }

                foreach (var messageHeaderDescription in messageDescription.Headers)
                {
                    ValidateJsonSerializableType(messageHeaderDescription.Type);
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
                        operationDescription.Behaviors[index] = new JsonSerializerOperationBehavior(operationDescription);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the message part description.
        /// </summary>
        /// <param name="messagePartDescription">The message part description.</param>
        private static void ValidateMessagePartDescription(MessagePartDescription messagePartDescription)
        {
            if (messagePartDescription != null)
            {
                ValidateJsonSerializableType(messagePartDescription.Type);
            }
        }

        /// <summary>
        /// Validates the type of the json serializable.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="System.InvalidOperationException">
        /// Json serialization is supported in public types only
        /// or
        /// Json serializable types must have a public, parameterless constructor
        /// </exception>
        private static void ValidateJsonSerializableType(Type type)
        {
            if (type == typeof (void))
            {
                return;
            }

            if (!type.IsPublicEx())
            {
                throw new InvalidOperationException("Json serialization is supported in public types only");
            }

            var defaultConstructor = type.GetConstructorEx(new Type[0]);
            if (defaultConstructor == null && !type.IsPrimitive)
            {
                throw new InvalidOperationException("Json serializable types must have a public, parameterless constructor");
            }
        }
        #endregion
    }
}
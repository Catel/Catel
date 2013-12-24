// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MefHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Reflection;

    /// <summary>
    /// Helper class for MEF IoC containers.
    /// </summary>
    [ObsoleteEx(Message = "External container support will be removed in 4.0, see https://catelproject.atlassian.net/browse/CTL-273", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    internal class MefHelper : ExternalContainerHelperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MefHelper"/> class.
        /// </summary>
        public MefHelper()
            : base("MEF", false)
        {
        }

        /// <summary>
        /// Determines whether the specified <paramref name="container"/> is a MEF IoC container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="container"/> is a MEF IoC container; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        public override bool IsValidContainer(object container)
        {
            Argument.IsNotNull("container", container);

            Type containerType = container.GetType();
            if (string.Equals(containerType.Name, "CompositionContainer"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the registration info.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The registration info about the type or <c>null</c> if the type is not registered.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        public override RegistrationInfo GetRegistrationInfo(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only MEF containers are supported");
            }

            var key = GetKeyFromInterface(container, interfaceType);
            var exports = GetExportsFromKey(container, key);

            var firstExport = exports.FirstOrDefault();
            if (firstExport == null)
            {
                return null;
            }

            return new RegistrationInfo(interfaceType, firstExport != null ? firstExport.GetType() : typeof(object), RegistrationType.Singleton);
        }

        /// <summary>
        /// Registers the specified type for the specified interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementingType">Type of the implementing.</param>
        /// <param name="registrationType">The registration type.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="implementingType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a MEF IoC container.</exception>
        public override void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingType", implementingType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only MEF containers are supported");
            }

            throw new NotSupportedException("MEF doesn't support registrations of types without instantiating it");
        }

        /// <summary>
        /// Registers a specific instance for the specified interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="implementingInstance">The implementing instance.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="implementingInstance"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a MEF IoC container.</exception>
        public override void RegisterInstance(object container, Type interfaceType, object implementingInstance)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingInstance", implementingInstance);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only MEF containers are supported");
            }

            var attributedModelServicesType = GetContainerType("System.ComponentModel.Composition.AttributedModelServices", container);

            // Retrieve ComposeExportedValue<T>(this CompositionContainer container, T exportedValue)
            var methods = attributedModelServicesType.GetMethodsEx(BindingFlagsHelper.GetFinalBindingFlags(false, true));
            var composeExportedValueMethodInfo = methods.Where(method => method.Name == "ComposeExportedValue").
                Where(method => method.GetGenericArguments().Count() == 1).
                Where(method => method.GetParameters().Count() == 2).FirstOrDefault();
            var genericComposeExportedValue = composeExportedValueMethodInfo.MakeGenericMethod(new[] { interfaceType });

            genericComposeExportedValue.Invoke(null, new[] { container, implementingInstance });
        }

        /// <summary>
        /// Resolves an instance of the specified interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The resolved instance or <c>null</c> if the instance could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a MEF IoC container.</exception>
        /// <exception cref="NotSupportedException">If the type is not registered in the container.</exception>
        public override object ResolveType(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only MEF containers are supported");
            }

            if (!IsTypeRegistered(container, interfaceType))
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not registered, so cannot be resolved", interfaceType));
            }

            string key = GetKeyFromInterface(container, interfaceType);
            var exports = GetExportsFromKey(container, key);

            if (exports.Any())
            {
                var export = exports.First();

                return PropertyHelper.GetPropertyValue(export, "Value");
            }

            return null;
        }

        /// <summary>
        /// Gets the key from interface, which is required for MEF.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The key based on the interface.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="interfaceType"/> is <c>null</c>.</exception>
        private string GetKeyFromInterface(object container, Type interfaceType)
        {
            Argument.IsNotNull("interfaceType", interfaceType);

            var attributedModelServicesType = GetContainerType("System.ComponentModel.Composition.AttributedModelServices", container);
            var getContractNameMethodInfo = attributedModelServicesType.GetMethodEx("GetContractName", new[] { typeof(Type) }, BindingFlagsHelper.GetFinalBindingFlags(false, true));

            return (string)getContractNameMethodInfo.Invoke(null, new[] { interfaceType });
        }

        /// <summary>
        /// Gets the exports from key.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// An enumeration of exports.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="key"/> is <c>null</c>.</exception>
        private IEnumerable<object> GetExportsFromKey(object container, string key)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("key", key);

            // Retrieve GetExports<T>(string contractName)
            var methods = container.GetType().GetMethodsEx(BindingFlagsHelper.GetFinalBindingFlags(false, true));
            var getExportsMethodInfo = methods.Where(method => method.Name == "GetExports").
                Where(method => method.GetGenericArguments().Count() == 1).
                Where(method => method.GetParameters().Count() == 1).FirstOrDefault();
            var genericMethodInfo = getExportsMethodInfo.MakeGenericMethod(new[] { typeof (object) });

            return ((IEnumerable)genericMethodInfo.Invoke(container, new object[] { key })).Cast<object>();
        }
    }
}

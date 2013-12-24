// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NinjectHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections;
    using System.Linq;

    using Reflection;

    /// <summary>
    /// Helper class for Ninject IoC containers.
    /// </summary>
    [ObsoleteEx(Message = "External container support will be removed in 4.0, see https://catelproject.atlassian.net/browse/CTL-273", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    internal class NinjectHelper : ExternalContainerHelperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectHelper"/> class.
        /// </summary>
        public NinjectHelper()
            : base("Ninject", true)
        {
        }

        #region Methods
        /// <summary>
        /// Determines whether the specified <paramref name="container"/> is a valid container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="container"/> is a valid container; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        public override bool IsValidContainer(object container)
        {
            Argument.IsNotNull("container", container);

            Type containerType = container.GetType();
            var interfaces = containerType.GetInterfaces();

            return interfaces.Any(iface => iface.FullName.Contains("IKernel"));
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
        /// <remarks>
        /// Equals <c>kernel.GetBindings(interfaceType).Any()</c>.
        /// </remarks>
        public override RegistrationInfo GetRegistrationInfo(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Ninject containers are supported");
            }

            // Note that this code equals:
            //   var binding = kernel.GetBindings(interfaceType).FirstOrDefault();
            //   binding.ScopeCallback != StandardScopeCallbacks.Singleton

            var containerType = container.GetType();
            var getBindingsMethodInfo = containerType.GetMethodEx("GetBindings");
            var bindings = (IEnumerable)getBindingsMethodInfo.Invoke(container, new object[] { interfaceType });

            var firstBinding = bindings.Cast<object>().FirstOrDefault();
            if (firstBinding == null)
            {
                return null;
            }

            var standardScopeCallbacks = TypeCache.GetTypeWithoutAssembly("Ninject.Infrastructure.StandardScopeCallbacks");
            var singletonScopeCallbackFieldInfo = standardScopeCallbacks.GetFieldEx("Singleton", false, true);
            var singletonScopeCallback = singletonScopeCallbackFieldInfo.GetValue(null);
            var scopeCallback = PropertyHelper.GetPropertyValue(firstBinding, "ScopeCallback");

            var registrationType = ObjectHelper.AreEqualReferences(scopeCallback, singletonScopeCallback) ? RegistrationType.Singleton : RegistrationType.Transient;

            return new RegistrationInfo(interfaceType, typeof(object), registrationType);
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
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        /// <remarks>
        /// Equals <c>kernel.Bind{interfaceType}().To{implementingType}().InSingletonScope()</c>.
        /// </remarks>
        public override void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingType", implementingType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Ninject containers are supported");
            }

            var containerType = container.GetType();
            var bindMethodInfo = containerType.GetMethodEx("Bind", new[] { typeof(Type[]) });
            var binding = bindMethodInfo.Invoke(container, new object[] { new[] { interfaceType } });

            var bindingType = binding.GetType();
            var toMethodInfo = bindingType.GetMethodEx("To", new[] { typeof(Type) });
            var finalBinding = toMethodInfo.Invoke(binding, new object[] { implementingType });

            Type finalBindingType = null;

            switch (registrationType)
            {
                case RegistrationType.Singleton:
                    finalBindingType = finalBinding.GetType();
                    var inSingletonScopeMethodInfo = finalBindingType.GetMethodEx("InSingletonScope");
                    inSingletonScopeMethodInfo.Invoke(finalBinding, null);
                    break;

                case RegistrationType.Transient:
                    finalBindingType = finalBinding.GetType();
                    var inTransientScopeMethodInfo = finalBindingType.GetMethodEx("InTransientScope");
                    inTransientScopeMethodInfo.Invoke(finalBinding, null);
                    break;

                default:
                    throw new ArgumentOutOfRangeException("registrationType");
            }
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
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        /// <remarks>
        /// Equals <c>kernel.Bind{interfaceType}().ToConstant(implementingInstance)</c>.
        /// </remarks>
        public override void RegisterInstance(object container, Type interfaceType, object implementingInstance)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingInstance", implementingInstance);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Ninject containers are supported");
            }

            var containerType = container.GetType();
            var bindMethodInfo = containerType.GetMethodEx("Bind", new[] { typeof(Type[]) });
            var binding = bindMethodInfo.Invoke(container, new object[] { new[] { interfaceType } });

            var bindingType = binding.GetType();
            var toConstantMethodInfo = bindingType.GetMethodEx("ToConstant");
            var toConstantMethodInfoGeneric = toConstantMethodInfo.MakeGenericMethod(implementingInstance.GetType());
            toConstantMethodInfoGeneric.Invoke(binding, new[] { implementingInstance });
        }

        /// <summary>
        /// Resolves an instance of the specified interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The resolved instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        /// <exception cref="NotSupportedException">If the type is not registered in the container.</exception>
        /// <remarks>
        /// Equals <c>kernel.GetBindings(interfaceType).FirstOrDefault()</c>.
        /// </remarks>
        public override object ResolveType(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Ninject containers are supported");
            }

            if (!IsTypeRegistered(container, interfaceType))
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not registered, so cannot be resolved", interfaceType));
            }

            var resolutionExtensionsType = GetContainerType("Ninject.ResolutionExtensions", container);
            var resolutionRootType = GetContainerType("Ninject.Syntax.IResolutionRoot", container);
            var iparameterType = GetContainerType("Ninject.Parameters.IParameter", container);

            var parameters = Array.CreateInstance(iparameterType, 0);

            var getMethodInfo = resolutionExtensionsType.GetMethodEx("Get", new[] { resolutionRootType, typeof(Type), parameters.GetType() }, BindingFlagsHelper.GetFinalBindingFlags(true, true));
            return getMethodInfo.Invoke(null, new[] { container, interfaceType, parameters });
        }
        #endregion
    }
}
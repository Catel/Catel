// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnityHelper.cs" company="Catel development team">
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
    /// Helper class for Unity IoC containers.
    /// </summary>
    internal class UnityHelper : ExternalContainerHelperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnityHelper"/> class.
        /// </summary>
        public UnityHelper()
            : base("Unity", true)
        {
        }

        #region Methods
        /// <summary>
        /// Determines whether the specified <paramref name="container"/> is a Unity IoC container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="container"/> is a Unity IoC container; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        public override bool IsValidContainer(object container)
        {
            Argument.IsNotNull("container", container);

            Type containerType = container.GetType();
            var interfaces = containerType.GetInterfacesEx();

            return interfaces.Any(iface => iface.FullName.Contains("IUnityContainer"));
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
                throw new NotSupportedException("Only Unity containers are supported");
            }

            var containerType = container.GetType();
            var registrationsPropertyInfo = containerType.GetPropertyEx("Registrations");
            var registrationsPropertyValue = (IEnumerable)registrationsPropertyInfo.GetValue(container, null);
            foreach (var containerRegistration in registrationsPropertyValue)
            {
                var containerRegistrationType = containerRegistration.GetType();
                var registeredTypePropertyInfo = containerRegistrationType.GetPropertyEx("RegisteredType");
                var registeredTypePropertyValue = registeredTypePropertyInfo.GetValue(containerRegistration, null) as Type;
                if (registeredTypePropertyValue == interfaceType)
                {
                    var registrationType = RegistrationType.Transient;

                    var lifetimeManagerType = PropertyHelper.GetPropertyValue<Type>(containerRegistration, "LifetimeManagerType");
                    if ((lifetimeManagerType != null) && (lifetimeManagerType.FullName.Contains("ContainerControlled")))
                    {
                        registrationType = RegistrationType.Singleton;
                    }

                    return new RegistrationInfo(interfaceType, typeof(object), registrationType);
                }
            }

            return null;
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
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a Unity IoC container.</exception>
        public override void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingType", implementingType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Unity containers are supported");
            }

            var containerType = container.GetType();
            var registerTypeMethodInfo = containerType.GetMethodEx("RegisterType");

            switch (registrationType)
            {
                case RegistrationType.Singleton:
                    registerTypeMethodInfo.Invoke(container, new[] { interfaceType, implementingType, null, 
                        CreateDefaultLifetimeManager(container), CreateEmptyInjectionMemberArray(container)});
                    break;

                case RegistrationType.Transient:
                    registerTypeMethodInfo.Invoke(container, new[] { interfaceType, implementingType, null, 
                        null, CreateEmptyInjectionMemberArray(container)});
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
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a Unity IoC container.</exception>
        public override void RegisterInstance(object container, Type interfaceType, object implementingInstance)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingInstance", implementingInstance);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Unity containers are supported");
            }

            var containerType = container.GetType();
            var registerInstanceMethodInfo = containerType.GetMethodEx("RegisterInstance");
            registerInstanceMethodInfo.Invoke(container, new[] { interfaceType, null, implementingInstance, CreateDefaultLifetimeManager(container) });
        }

        /// <summary>
        /// Resolves an instance of the specified interface.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The resolved instance or <c>null</c> if the instance could not be resolved.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a Unity IoC container.</exception>
        /// <exception cref="NotSupportedException">If the type is not registered in the container.</exception>
        public override object ResolveType(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Unity containers are supported");
            }

            if (!IsTypeRegistered(container, interfaceType))
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not registered, so cannot be resolved", interfaceType));
            }

            var containerType = container.GetType();
            var resolveMethodInfo = containerType.GetMethodEx("Resolve");
            // ReSharper disable RedundantExplicitArrayCreation
            return resolveMethodInfo.Invoke(container, new object[] { interfaceType, null, CreateEmptyResolverOverrideArray(container) });
            // ReSharper restore RedundantExplicitArrayCreation
        }

        /// <summary>
        /// Creates the default lifetime manager required for Unity.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The instantiated lifetime manager.</returns>
        private object CreateDefaultLifetimeManager(object container)
        {
            return InstantiateContainerType("Microsoft.Practices.Unity.ContainerControlledLifetimeManager", container);
        }

        /// <summary>
        /// Creates an empty <c>InjectionMember</c> array. This is required for some methods of Unity.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>An empty array containing 0 members.</returns>
        private object CreateEmptyInjectionMemberArray(object container)
        {
            var type = GetContainerType("Microsoft.Practices.Unity.InjectionConstructor", container);
            return Array.CreateInstance(type, 0);
        }

        /// <summary>
        /// Creates an empty <c>ResolverOverride</c> array. This is required for some methods of Unity.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>An empty array containing 0 members.</returns>
        private object CreateEmptyResolverOverrideArray(object container)
        {
            var type = GetContainerType("Microsoft.Practices.Unity.ResolverOverride", container);
            return Array.CreateInstance(type, 0);
        }
        #endregion
    }
}

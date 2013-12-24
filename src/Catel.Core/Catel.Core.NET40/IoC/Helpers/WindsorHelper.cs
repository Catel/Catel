// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindsorHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Reflection;

    /// <summary>
    /// Helper class for Castle Windsor IoC containers.
    /// </summary>
    [ObsoleteEx(Message = "External container support will be removed in 4.0, see https://catelproject.atlassian.net/browse/CTL-273", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    internal class WindsorHelper : ExternalContainerHelperBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindsorHelper"/> class.
        /// </summary>
        public WindsorHelper()
            : base("Castle Windsor", true)
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
            var interfaces = containerType.GetInterfacesEx();

            return interfaces.Any(iface => iface.FullName.Contains("IWindsorContainer"));
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
        /// Equals <c>container.Resolve(interfaceType) != null</c>.
        /// </remarks>
        public override RegistrationInfo GetRegistrationInfo(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Castle Windsor containers are supported");
            }

            try
            {
                var containerType = container.GetType();
                var resolveMethodInfo = containerType.GetMethodEx("Resolve", new[] { typeof(Type) });
                var instance = resolveMethodInfo.Invoke(container, new object[] { interfaceType });
                if (instance != null)
                {
                    return new RegistrationInfo(interfaceType, instance.GetType(), RegistrationType.Transient);
                }

                return null;
            }
            catch (TargetInvocationException)
            {
                return null;
            }
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
        /// Equals <c>container.Register(new IRegistration[] {new ComponentRegistration{interfaceType}().ImplementedBy{implementingType}() });</c>.
        /// </remarks>
        public override void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingType", implementingType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Castle Windsor containers are supported");
            }

            var iregistrationType = GetContainerType("Castle.MicroKernel.Registration.IRegistration", container);
            var componentRegistrationType = GetContainerType("Castle.MicroKernel.Registration.ComponentRegistration`1", container).MakeGenericType(interfaceType);
            var componentRegistration = Activator.CreateInstance(componentRegistrationType, null);

            var implementedByMethodInfo = componentRegistrationType.GetMethodEx("ImplementedBy", new[] { typeof(Type) });
            componentRegistration = implementedByMethodInfo.Invoke(componentRegistration, new object[] { implementingType });

            string registrationLifestyle;
            switch (registrationType)
            {
                case RegistrationType.Singleton:
                    registrationLifestyle = "LifestyleSingleton";
                    break;

                case RegistrationType.Transient:
                    registrationLifestyle = "LifestyleTransient";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("registrationType");
            }

            MethodInfo lifestyleMethod = componentRegistrationType.GetMethodEx(registrationLifestyle, new Type[] { });
            componentRegistration = lifestyleMethod.Invoke(componentRegistration, new object[] { });

            var listType = typeof(List<>).MakeGenericType(iregistrationType);
            var list = Activator.CreateInstance(listType);
            var addMethodInfo = listType.GetMethodEx("Add");
            var toArrayMethodInfo = listType.GetMethodEx("ToArray");
            addMethodInfo.Invoke(list, new[] { componentRegistration });

            var containerType = container.GetType();
            var registerMethodInfo = containerType.GetMethodEx("Register");
            registerMethodInfo.Invoke(container, new[] { toArrayMethodInfo.Invoke(list, null) });
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
        /// Equals <c>container.Register(new IRegistration[] {new ComponentRegistration{interfaceType}().Instance{implementingInstance}() });</c>.
        /// </remarks>
        public override void RegisterInstance(object container, Type interfaceType, object implementingInstance)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);
            Argument.IsNotNull("implementingInstance", implementingInstance);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Castle Windsor containers are supported");
            }

            var iregistrationType = GetContainerType("Castle.MicroKernel.Registration.IRegistration", container);
            var componentRegistrationType = GetContainerType("Castle.MicroKernel.Registration.ComponentRegistration`1", container).MakeGenericType(interfaceType);
            var componentRegistration = Activator.CreateInstance(componentRegistrationType, null);

            var instanceMethodInfo = componentRegistrationType.GetMethodEx("Instance");
            componentRegistration = instanceMethodInfo.Invoke(componentRegistration, new[] { implementingInstance });

            var listType = typeof(List<>).MakeGenericType(iregistrationType);
            var list = Activator.CreateInstance(listType);
            var addMethodInfo = listType.GetMethodEx("Add");
            var toArrayMethodInfo = listType.GetMethodEx("ToArray");
            addMethodInfo.Invoke(list, new[] { componentRegistration });

            var containerType = container.GetType();
            var registerMethodInfo = containerType.GetMethodEx("Register");
            registerMethodInfo.Invoke(container, new[] { toArrayMethodInfo.Invoke(list, null) });
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
        /// Equals <c>container.Resolve(interfaceType)</c>.
        /// </remarks>
        public override object ResolveType(object container, Type interfaceType)
        {
            Argument.IsNotNull("container", container);
            Argument.IsNotNull("interfaceType", interfaceType);

            if (!IsValidContainer(container))
            {
                throw new NotSupportedException("Only Castle Windsor containers are supported");
            }

            if (!IsTypeRegistered(container, interfaceType))
            {
                throw new NotSupportedException(string.Format("Type '{0}' is not registered, so cannot be resolved", interfaceType));
            }

            var containerType = container.GetType();
            var resolveMethodInfo = containerType.GetMethodEx("Resolve", new[] { typeof(Type) });
            return resolveMethodInfo.Invoke(container, new object[] { interfaceType });
        }
        #endregion
    }
}

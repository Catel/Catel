// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExternalContainerHelperBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;
    using Reflection;

    /// <summary>
    /// Base class for all external container helpers that takes away the care to retrieve
    /// types from the right assemblies and implements caching at one place.
    /// </summary>
    public abstract class ExternalContainerHelperBase : IExternalContainerHelper
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalContainerHelperBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="canRegisterTypesWithoutInstantiating">If set to <c>true</c> this helper can register types without instantiating.</param>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        protected ExternalContainerHelperBase(string name, bool canRegisterTypesWithoutInstantiating)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            Name = name;
            CanRegisterTypesWithoutInstantiating = canRegisterTypesWithoutInstantiating;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the helper.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this helper can register types without instantiating.
        /// <para />
        /// If this value is <c>true</c>, the <see cref="IExternalContainerHelper.RegisterType"/> can be used. Otherwise, only
        /// <see cref="IExternalContainerHelper.RegisterInstance"/> can be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if this helper can register types without instantiating; otherwise, <c>false</c>.
        /// </value>
        public bool CanRegisterTypesWithoutInstantiating { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified <paramref name="container"/> is a valid container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="container"/> is a valid container; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        public abstract bool IsValidContainer(object container);

        /// <summary>
        /// Gets the registration info.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The registration info about the type or <c>null</c> if the type is not registered.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        public abstract RegistrationInfo GetRegistrationInfo(object container, Type interfaceType);

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
        public abstract void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType);

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
        public abstract void RegisterInstance(object container, Type interfaceType, object implementingInstance);

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
        public abstract object ResolveType(object container, Type interfaceType);

        /// <summary>
        /// Gets the type from the specified container.
        /// <para/>
        /// This keeps in mind the unity stuff.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="container">The container.</param>
        /// <returns>The type.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is <c>null</c>.</exception>
        protected Type GetContainerType(string typeName, object container)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);
            Argument.IsNotNull("container", container);

            return container.GetType().GetAssemblyEx().GetType(typeName);
        }

        /// <summary>
        /// Instantiates the type of the container by using <c>Activator.CreateInstance</c>.
        /// <para />
        /// If that doesn't work, use <see cref="GetContainerType"/> and instantiate the type manually.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="container">The container.</param>
        /// <returns>The instantiated type.</returns>
        /// <exception cref="ArgumentException">The <paramref name="typeName"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="container"/> is <c>null</c>.</exception>
        protected object InstantiateContainerType(string typeName, object container)
        {
            Argument.IsNotNullOrWhitespace("typeName", typeName);
            Argument.IsNotNull("container", container);

            var type = GetContainerType(typeName, container);
            return TypeFactory.Default.CreateInstance(type);
        }

        /// <summary>
        /// Determines whether the specified type is registered.
        /// <para />
        /// This is a wrapper for <see cref="GetRegistrationInfo"/>.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns><c>true</c> if the type is registered; otherwise, <c>false</c>.</returns>
        protected bool IsTypeRegistered(object container, Type interfaceType)
        {
            return GetRegistrationInfo(container, interfaceType) != null;
        }
        #endregion
    }
}
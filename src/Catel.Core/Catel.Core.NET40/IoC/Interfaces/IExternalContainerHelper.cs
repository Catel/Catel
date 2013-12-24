// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExternalContainerHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Interface defining the least functionality of an external IoC container.
    /// </summary>
    [ObsoleteEx(Message = "External container support will be removed in 4.0, see https://catelproject.atlassian.net/browse/CTL-273", TreatAsErrorFromVersion = "3.9", RemoveInVersion = "4.0")]
    public interface IExternalContainerHelper
    {
        /// <summary>
        /// Gets the name of the helper.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this helper can register types without instantiating.
        /// <para />
        /// If this value is <c>true</c>, the <see cref="RegisterType"/> can be used. Otherwise, only
        /// <see cref="RegisterInstance"/> can be used.
        /// </summary>
        /// <value>
        /// <c>true</c> if this helper can register types without instantiating; otherwise, <c>false</c>.
        /// </value>
        bool CanRegisterTypesWithoutInstantiating { get; }

        /// <summary>
        /// Determines whether the specified <paramref name="container"/> is a valid container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>
        /// <c>true</c> if the specified <paramref name="container"/> is a valid container; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        bool IsValidContainer(object container);

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
        void RegisterType(object container, Type interfaceType, Type implementingType, RegistrationType registrationType);

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
        void RegisterInstance(object container, Type interfaceType, object implementingInstance);

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
        object ResolveType(object container, Type interfaceType);

        /// <summary>
        /// Gets the registration info.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <returns>The registration info about the type or <c>null</c> if the type is not registered.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="container"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="interfaceType"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">If <paramref name="container"/> is not a valid container.</exception>
        RegistrationInfo GetRegistrationInfo(object container, Type interfaceType);
    }
}

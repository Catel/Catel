// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegistrationConventionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// The registration convention handler.
    /// </summary>
    public interface IRegistrationConventionHandler
    {
        #region Properties
        /// <summary>
        /// Gets the registration conventions.
        /// </summary>
        /// <value>
        /// The registration conventions.
        /// </value>
        IEnumerable<IRegistrationConvention> RegistrationConventions { get; }

        /// <summary>
        /// Gets the type filter.
        /// </summary>
        /// <value>
        /// The type filter.
        /// </value>
        ICompositeFilter<Type> TypeFilter { get; }

        /// <summary>
        /// Gets the assembly filter.
        /// </summary>
        /// <value>
        /// The assembly filter.
        /// </value>
        ICompositeFilter<Assembly> AssemblyFilter { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the convention.
        /// </summary>
        /// <typeparam name="TRegistrationConvention">The type of the registration convention.</typeparam>
        /// <param name="registrationType">Type of the registration.</param>
        void RegisterConvention<TRegistrationConvention>(RegistrationType registrationType = RegistrationType.Singleton) where TRegistrationConvention : IRegistrationConvention;

        /// <summary>
        /// Applies the registered conventions.
        /// </summary>
        void ApplyConventions();

        /// <summary>
        /// Adds the assembly to scan.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <exception cref="System.ArgumentNullException">The <paramref name="assembly"/> is <c>null</c>.</exception>
        void AddAssemblyToScan(Assembly assembly);
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegistrationConventionHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// 
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
        /// Gets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        CompositeFilter<Type> Filter { get; }
        #endregion

        #region Methods

        /// <summary>
        /// Registers the convention.
        /// </summary>
        /// <typeparam name="TRegistrationConvention">The type of the registration convention.</typeparam>
        /// <param name="registrationType">Type of the registration.</param>
        void RegisterConvention<TRegistrationConvention>(RegistrationType registrationType = RegistrationType.Singleton) where TRegistrationConvention : IRegistrationConvention;

        /// <summary>
        /// Applies the registe conventions.
        /// </summary>
        void ApplyConventions();
        #endregion
    }
}
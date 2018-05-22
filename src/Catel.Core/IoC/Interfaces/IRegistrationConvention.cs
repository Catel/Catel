// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegistrationConvention.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The registration convention interface.
    /// </summary>
    public interface IRegistrationConvention
    {
        #region Properties

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        /// <value>
        /// The type of the registration.
        /// </value>
        RegistrationType RegistrationType { get; }

        #endregion

        #region Methods
        /// <summary>
        /// Processes the specified types to register.
        /// </summary>
        /// <param name="typesToRegister">The types to register.</param>
        void Process(IEnumerable<Type> typesToRegister);
        #endregion
    }
}
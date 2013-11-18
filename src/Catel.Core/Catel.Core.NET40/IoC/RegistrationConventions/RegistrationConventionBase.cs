// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationConventionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public class RegistrationConventionBase : IRegistrationConvention
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationConventionBase" /> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator.</param>
        /// <param name="registrationType">Type of the registration.</param>
        public RegistrationConventionBase(IServiceLocator serviceLocator, RegistrationType registrationType = RegistrationType.Singleton)
        {
            Container = serviceLocator;
            RegistrationType = registrationType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public IServiceLocator Container { get; protected set; }
        #endregion

        #region IRegistrationConvention Members
        /// <summary>
        /// Gets or sets the type of the registration.
        /// </summary>
        /// <value>
        /// The type of the registration.
        /// </value>
        public RegistrationType RegistrationType { get; protected set; }

        /// <summary>
        /// Processes the specified types to register.
        /// </summary>
        /// <param name="typesToRegister">The types to register.</param>
        public virtual void Process(IEnumerable<Type> typesToRegister)
        {
        }
        #endregion
    }
}
﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorRegistrationGroup.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Represents a group of service locator registrations. This is needed to implement automatic
    /// resolving of related (generic) types on the same singleton instance of a registered service.
    /// </summary>
    public class ServiceLocatorRegistrationGroup
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorRegistrationGroup"/> class.
        /// </summary>
        /// <param name="entryRegistration">The entry registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="entryRegistration"/> is <c>null</c>.</exception>
        public ServiceLocatorRegistrationGroup(ServiceLocatorRegistration entryRegistration)
        {
            Argument.IsNotNull("entryRegistration", entryRegistration);

            EntryRegistration = entryRegistration;
        }
        #endregion

        /// <summary>
        /// Gets the entry registration.
        /// </summary>
        /// <value>The entry registration.</value>
        public ServiceLocatorRegistration EntryRegistration { get; private set; }

        
        
    }
}
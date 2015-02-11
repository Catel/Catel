﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceLocatorRegistration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Contains all information about the registration of an entry in the <see cref="ServiceLocator"/>.
    /// </summary>
    [DebuggerDisplay("{DeclaringType} => {ImplementingType} ({RegistrationType})")]
    public class ServiceLocatorRegistration
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocatorRegistration" /> class.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="implementingType">Type of the implementing.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="createServiceFunc">The create service function.</param>
        public ServiceLocatorRegistration(Type declaringType, Type implementingType, object tag, RegistrationType registrationType, Func<ServiceLocatorRegistration, object> createServiceFunc)
        {
            Argument.IsNotNull("createServiceFunc", createServiceFunc);

            CreateServiceFunc = createServiceFunc;
            DeclaringType = declaringType;
            DeclaringTypeName = declaringType.AssemblyQualifiedName;

            ImplementingType = implementingType;
            ImplementingTypeName = implementingType.AssemblyQualifiedName;

            Tag = tag;
            RegistrationType = registrationType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the create service function.
        /// </summary>
        /// <value>The create service function.</value>
        public Func<ServiceLocatorRegistration, object> CreateServiceFunc { get; private set; }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        /// <value>The declaring type.</value>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets the name of the declaring type.
        /// </summary>
        /// <value>The name of the declaring type.</value>
        public string DeclaringTypeName { get; private set; }

        /// <summary>
        /// Gets the implementing type.
        /// </summary>
        /// <value>The implementing type.</value>
        public Type ImplementingType { get; private set; }

        /// <summary>
        /// Gets the name of the implementing type.
        /// </summary>
        /// <value>The name of the implementing type.</value>
        public string ImplementingTypeName { get; private set; }

        /// <summary>
        /// Gets the type of the registration.
        /// </summary>
        /// <value>The type of the registration.</value>
        public RegistrationType RegistrationType { get; private set; }

        //public RegisteredTypeInfo RelatedTypeInfo { get; private set; }

        //public bool IsRelatedTypeInfo
        //{
        //    get { return RelatedTypeInfo != null; }
        //}

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public object Tag { get; private set; }
        #endregion
    }
}
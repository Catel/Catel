// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Class containing the registration info about a particular type registered in the <see cref="ServiceLocator"/>.
    /// </summary>
    public class RegistrationInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationInfo" /> class.
        /// </summary>
        /// <param name="declaringType">Type of the declaring.</param>
        /// <param name="implementingType">Type of the implementing.</param>
        /// <param name="registrationType">Type of the registration.</param>
        /// <param name="isTypeInstantiatedForSingleton">If set to <c>true</c> there already is an instance of this singleton registration.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="declaringType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="implementingType" /> is <c>null</c>.</exception>
        internal RegistrationInfo(Type declaringType, Type implementingType, RegistrationType registrationType, bool isTypeInstantiatedForSingleton = false)
        {
            Argument.IsNotNull("declaringType", declaringType);
            Argument.IsNotNull("implementingType", implementingType);

            DeclaringType = declaringType;
            ImplementingType = implementingType;
            RegistrationType = registrationType;
            IsTypeInstantiatedForSingleton = isTypeInstantiatedForSingleton;
        }

        /// <summary>
        /// Gets the declaring type, an interface in most cases.
        /// </summary>
        /// <value>The declaring type.</value>
        public Type DeclaringType { get; private set; }

        /// <summary>
        /// Gets the implementing type.
        /// </summary>
        /// <value>The implementing type.</value>
        public Type ImplementingType { get; private set; }

        /// <summary>
        /// Gets the registration type.
        /// </summary>
        /// <value>The registration type.</value>
        public RegistrationType RegistrationType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this is a late-bound registration, meaning that the type can only
        /// be determined after the type is created by a callback.
        /// </summary>
        /// <value><c>true</c> if this instance is late bound registration; otherwise, <c>false</c>.</value>
        public bool IsLateBoundRegistration { get { return ImplementingType == typeof (LateBoundImplementation); } }

        /// <summary>
        /// Gets or sets a value indicating whether there is already an instance of this type instantiated when registered as <see cref="IoC.RegistrationType.Singleton"/>.
        /// </summary>
        /// <remarks>
        /// Note that this value is always <c>false</c> for types that are not registered as <see cref="IoC.RegistrationType.Singleton"/>.
        /// </remarks>
        /// <value><c>true</c> if there is already an instance of this singleton registration; otherwise, <c>false</c>.</value>
        public bool IsTypeInstantiatedForSingleton { get; private set; }
    }
}
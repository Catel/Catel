// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;

    /// <summary>
    /// Exception class in case an requested type from <see cref="IServiceLocator"/> is not registered.
    /// </summary>
    internal class TypeNotRegisteredException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNotRegisteredException"/> class.
        /// </summary>
        /// <param name="requestedType">The requested type.</param>
        public TypeNotRegisteredException(Type requestedType)
            : base("The specified type is not registered. Please register type before using it.")
        {
            RequestedType = requestedType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the requested type.
        /// </summary>
        /// <value>The type.</value>
        public Type RequestedType { get; private set; }
        #endregion
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeNotRegisteredException.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.IoC
{
    using System;
    using Reflection;

    /// <summary>
    /// Exception class in case an requested type from <see cref="IServiceLocator"/> is not registered.
    /// </summary>
    public class TypeNotRegisteredException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeNotRegisteredException" /> class.
        /// </summary>
        /// <param name="requestedType">The requested type.</param>
        /// <param name="message">The message.</param>
        public TypeNotRegisteredException(Type requestedType, string message)
            : base(string.Format("The specified type '{0}' is not registered or could not be constructed. Please register type before using it. {1}", 
            requestedType.GetSafeFullName(), message ?? string.Empty))
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateInterceptor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using Callbacks;

    /// <summary>
    /// Interface that describes a single State interceptor.
    /// </summary>
    public interface IStateInterceptor
    {
        #region Properties
        /// <summary>
        /// Gets the callback collection.
        /// </summary>
        /// <value>
        /// The callback collection.
        /// </value>
        CallbackCollection CallbackCollection { get; }

        /// <summary>
        /// Gets the member definition.
        /// </summary>
        /// <value>
        /// The member definition.
        /// </value>
        IMemberDefinition MemberDefinition { get; }

        /// <summary>
        /// Gets a value indicating whether on invoke was registered.
        /// </summary>
        /// <value>
        /// <c>true</c> if on invoke was registered; otherwise, <c>false</c>.
        /// </value>
        bool OnInvokeWasRegistered { get; }
        #endregion
    }
}
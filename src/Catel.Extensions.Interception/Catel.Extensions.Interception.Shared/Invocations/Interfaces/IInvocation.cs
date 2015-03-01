// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInvocation.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Interface that describes a single invocation.
    /// </summary>
    public interface IInvocation
    {
        #region Properties
        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        object Proxy { get; }

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        object Target { get; }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        MethodInfo Method { get; }

        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        object[] Arguments { get; }

        /// <summary>
        /// Gets the generic arguments.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        Type[] GenericArguments { get; }
        #endregion
    }
}
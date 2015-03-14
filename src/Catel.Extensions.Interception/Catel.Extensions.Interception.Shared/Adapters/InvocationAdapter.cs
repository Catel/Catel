// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvocationAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Adapters
{
    using System;
    using System.Reflection;

    internal class InvocationAdapter : IInvocation
    {
        #region Fields
        /// <summary>
        ///     The invocation
        /// </summary>
        private readonly Castle.DynamicProxy.IInvocation _invocation;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InvocationAdapter"/> class.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public InvocationAdapter(Castle.DynamicProxy.IInvocation invocation, object target)
        {
            Argument.IsNotNull("invocation", invocation);
            Argument.IsNotNull("target", target);

            _invocation = invocation;
            Target = target;
        }
        #endregion

        #region IInvocation Members
        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public object[] Arguments
        {
            get { return _invocation.Arguments; }
        }

        /// <summary>
        /// Gets the generic arguments.
        /// </summary>
        /// <value>
        /// The generic arguments.
        /// </value>
        public Type[] GenericArguments
        {
            get { return _invocation.GenericArguments; }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public MethodInfo Method
        {
            get { return _invocation.Method; }
        }

        /// <summary>
        /// Gets the proxy.
        /// </summary>
        /// <value>
        /// The proxy.
        /// </value>
        public object Proxy
        {
            get { return _invocation.Proxy; }
        }

        /// <summary>
        ///     Gets the target.
        /// </summary>
        /// <value>
        ///     The target.
        /// </value>
        public object Target { get; private set; }
        #endregion
    }
}
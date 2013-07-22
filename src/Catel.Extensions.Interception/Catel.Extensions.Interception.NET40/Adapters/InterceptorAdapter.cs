// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptorAdapter.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Adapters
{
    using System;

    internal class InterceptorAdapter : Castle.DynamicProxy.IInterceptor
    {
        #region Fields
        /// <summary>
        /// The interceptor
        /// </summary>
        private readonly IInterceptor _interceptor;

        /// <summary>
        /// The target
        /// </summary>
        private readonly object _target;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InterceptorAdapter"/> class.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="target">The target.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> is <c>null</c>.</exception>
        public InterceptorAdapter(IInterceptor interceptor, object target)
        {
            Argument.IsNotNull("interceptor", interceptor);
            Argument.IsNotNull("target", target);

            _interceptor = interceptor;
            _target = target;
        }
        #endregion

        #region IInterceptor Members
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        public void Intercept(Castle.DynamicProxy.IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            invocation.ReturnValue = _interceptor.Intercept(new InvocationAdapter(invocation, _target));
        }
        #endregion
    }
}
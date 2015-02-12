// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionInterceptor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Interceptors
{
    using System;

    /// <summary>
    /// Represents the function interceptor implementation.
    /// </summary>
    public class FunctionInterceptor : IInterceptor
    {
        #region Fields
        /// <summary>
        /// The action
        /// </summary>
        private readonly Action<IInvocation> _action;

        /// <summary>
        /// The function
        /// </summary>
        private readonly Func<IInvocation, object> _function;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionInterceptor"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public FunctionInterceptor(Action<IInvocation> action)
        {
            Argument.IsNotNull("action", action);

            _action = action;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionInterceptor"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="function"/> is <c>null</c>.</exception>
        public FunctionInterceptor(Func<IInvocation, object> function)
        {
            Argument.IsNotNull("function", function);

            _function = function;
        }
        #endregion

        #region IInterceptor Members
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        public object Intercept(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            object result = null;

            if (_function != null)
            {
                result = _function(invocation);
            }
            else
            {
                _action(invocation);
            }
            return result;
        }
        #endregion
    }
}
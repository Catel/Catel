// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptorBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Interceptors
{
    using System;

    /// <summary>
    /// Represents the interceptor base implementation.
    /// </summary>
    public abstract class InterceptorBase : IInterceptor
    {
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

            OnBefore(invocation);

            try
            {
                result = OnInvoke(invocation);
            }
            catch (Exception exception)
            {
                OnCatch(invocation, exception);
            }
            finally
            {
                OnFinally(invocation);
            }

            OnAfter(invocation);
            return OnReturn(invocation, result);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        protected virtual void OnBefore(IInvocation invocation)
        {
        }

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected virtual object OnInvoke(IInvocation invocation)
        {
            Argument.IsNotNull(() => invocation);

            return invocation.Method.Invoke(invocation.Target, invocation.Arguments);
        }

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="exception">The exception.</param>
        protected virtual void OnCatch(IInvocation invocation, Exception exception)
        {
            throw exception;
        }

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        protected virtual void OnFinally(IInvocation invocation)
        {
        }

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        protected virtual void OnAfter(IInvocation invocation)
        {
        }

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected virtual object OnReturn(IInvocation invocation, object result)
        {
            return result;
        }
        #endregion
    }
}
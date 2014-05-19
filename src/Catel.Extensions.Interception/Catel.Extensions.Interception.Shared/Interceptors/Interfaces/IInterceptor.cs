// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterceptor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    /// <summary>
    /// Interface that describes a single interceptor.
    /// </summary>
    public interface IInterceptor
    {
        #region Methods
        /// <summary>
        /// Intercepts the specified invocation.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        object Intercept(IInvocation invocation);
        #endregion
    }
}
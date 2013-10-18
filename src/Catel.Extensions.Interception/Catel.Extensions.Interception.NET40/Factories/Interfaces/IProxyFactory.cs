// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProxyFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;

    /// <summary>
    /// The proxy factory interface.
    /// </summary>
    public interface IProxyFactory
    {
        #region Methods
        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified interceptor.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is <c>null</c>.</exception>
        TProxy Create<TProxy>(IInterceptor interceptor, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified action.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        TProxy Create<TProxy>(Action<IInvocation> action, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified action.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        TProxy Create<TProxy>(Func<IInvocation, object> action, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified target.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor" /> is <c>null</c>.</exception>
        TProxy Create<TProxy>(TProxy target, IInterceptor interceptor, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified target.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        TProxy Create<TProxy>(TProxy target, Action<IInvocation> action, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified target.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        TProxy Create<TProxy>(TProxy target, Func<IInvocation, object> action, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified proxy type.
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="proxyType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor" /> is <c>null</c>.</exception>
        object Create(Type proxyType, IInterceptor interceptor, params Type[] interfaces);

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified constructor arguments.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="parameters">The parameters.</param>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="parameters"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor"/> is <c>null</c>.</exception>
        TProxy Create<TProxy>(object[] parameters, IInterceptor interceptor, params Type[] interfaces)
            where TProxy : class;

        #endregion
    }
}
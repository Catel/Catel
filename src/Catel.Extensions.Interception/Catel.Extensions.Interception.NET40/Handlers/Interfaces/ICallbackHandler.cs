// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICallbackHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Handlers
{
    using System;

    /// <summary>
    /// Interface that describes a single Callback handler.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TServiceImplementation">The type of the service implementation.</typeparam>
    public interface ICallbackHandler<TService, TServiceImplementation> : IFluent
        where TServiceImplementation : TService
    {
        #region Methods

        /// <summary>
        /// Use it if you want to configure interception for another member.
        /// </summary>
        /// <returns></returns>
        IInterceptorHandler<TService, TServiceImplementation> And();

        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnBefore(Action action);

        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnBefore(Action<IInvocation> action);

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnInvoke(Func<IInvocation, object> action);

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnInvoke(Action<IInvocation> action);

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnCatch(Action<Exception> action);

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnCatch(Action<IInvocation, Exception> action);

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnFinally(Action action);

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnFinally(Action<IInvocation> action);

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnAfter(Action action);

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnAfter(Action<IInvocation> action);

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnReturn(Func<object> action);

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> OnReturn(Func<IInvocation, object, object> action);
        #endregion
    }
}
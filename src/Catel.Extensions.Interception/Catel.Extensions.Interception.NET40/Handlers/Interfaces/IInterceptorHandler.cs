// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IInterceptorHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Handlers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Interface that describes a single Interceptor handler.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TServiceImplementation">The type of the service implementation.</typeparam>
    public interface IInterceptorHandler<TService, TServiceImplementation> : IFluent
        where TServiceImplementation : TService
    {
        #region Methods
        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> Intercept(Expression<Action<TService>> method);

        /// <summary>
        /// Intercepts the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="member"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> Intercept(Expression<Func<TService, object>> member);

        /// <summary>
        /// Intercepts all.
        /// </summary>
        /// <returns></returns>
        ICallbackHandler<TService, TServiceImplementation> InterceptAll();

        /// <summary>
        /// Intercepts all properties getters.
        /// </summary>
        /// <returns></returns>
        ICallbackHandler<TService, TServiceImplementation> InterceptAllGetters();

        /// <summary>
        /// Intercepts all properties setters.
        /// </summary>
        /// <returns></returns>
        ICallbackHandler<TService, TServiceImplementation> InterceptAllSetters();

        /// <summary>
        /// Intercepts all members.
        /// </summary>
        /// <returns></returns>
        ICallbackHandler<TService, TServiceImplementation> InterceptAllMembers();

        /// <summary>
        /// Intercepts the specified methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methods"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptMethods(
            params Expression<Action<TService>>[] methods);

        /// <summary>
        /// Intercepts all members which respect the provided predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptWhere(Func<MethodInfo, bool> predicate);

        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptMethod(Expression<Action<TService>> method);

        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptMethod(
            Expression<Func<TService, object>> method);

        /// <summary>
        /// Intercepts the getter of the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptGetter(
            Expression<Func<TService, object>> property);

        /// <summary>
        /// Intercepts the setter of the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        ICallbackHandler<TService, TServiceImplementation> InterceptSetter(
            Expression<Func<TService, object>> property);
        #endregion
    }
}
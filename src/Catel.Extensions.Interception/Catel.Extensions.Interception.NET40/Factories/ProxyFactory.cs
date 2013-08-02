// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyFactory.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception
{
    using System;
    using Adapters;
    using Castle.DynamicProxy;
    using Interceptors;
    using IoC;
    using Reflection;

    /// <summary>
    /// Class that implements <see cref="IProxyFactory"/>.
    /// </summary>
    public class ProxyFactory : IProxyFactory
    {
        #region Constants
        /// <summary>
        /// The static instance of the proxy factory.
        /// </summary>
        private static readonly IProxyFactory _default = new ProxyFactory();
        #endregion

        #region Fields
        /// <summary>
        ///     The proxy generator
        /// </summary>
        private readonly ProxyGenerator _proxyGenerator;

        /// <summary>
        /// The service locator
        /// </summary>
        private readonly IServiceLocator _serviceLocator;

        /// <summary>
        /// The type factory
        /// </summary>
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyFactory"/> class.
        /// </summary>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, <see cref="ServiceLocator.Default" /> will be used.</param>
        /// <param name="typeFactory">The type factory. If <c>null</c>, <see cref="TypeFactory.Default" /> will be used.</param>
        public ProxyFactory(IServiceLocator serviceLocator = null, ITypeFactory typeFactory = null)
        {
            _serviceLocator = serviceLocator ?? ServiceLocator.Default;
            _typeFactory = typeFactory ?? TypeFactory.Default;
            _proxyGenerator = _typeFactory.CreateInstance<ProxyGenerator>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the default instance of the proxy factory..
        /// </summary>
        /// <value>
        /// The default instance.
        /// </value>
        public static IProxyFactory Default
        {
            get { return _default; }
        }
        #endregion

        #region IProxyFactory Members
        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified action.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public TProxy Create<TProxy>(Action<IInvocation> action, params Type[] interfaces)
        {
            Argument.IsNotNull("action", action);

            return Create<TProxy>(new FunctionInterceptor(action), interfaces);
        }

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified target.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public TProxy Create<TProxy>(TProxy target, Action<IInvocation> action, params Type[] interfaces)
        {
            Argument.IsNotNull("action", action);

            return Create(target, new FunctionInterceptor(action), interfaces);
        }

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified action.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="action">The action.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public TProxy Create<TProxy>(Func<IInvocation, object> action, params Type[] interfaces)
        {
            Argument.IsNotNull("action", action);

            return Create<TProxy>(new FunctionInterceptor(action), interfaces);
        }

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
        public TProxy Create<TProxy>(TProxy target, Func<IInvocation, object> action, params Type[] interfaces)
        {
            Argument.IsNotNull("target", target);
            Argument.IsNotNull("action", action);

            return Create(target, new FunctionInterceptor(action), interfaces);
        }

        /// <summary>
        /// Creates a proxy instance of the specified proxy type using the specified interceptor.
        /// </summary>
        /// <typeparam name="TProxy">The type of the proxy.</typeparam>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor" /> is <c>null</c>.</exception>
        public TProxy Create<TProxy>(IInterceptor interceptor, params Type[] interfaces)
        {
            return (TProxy) Create(typeof (TProxy), interceptor, interfaces);
        }

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
        public TProxy Create<TProxy>(TProxy target, IInterceptor interceptor, params Type[] interfaces)
        {
            Argument.IsNotNull("target", target);
            Argument.IsNotNull("interceptor", interceptor);

            var interceptorAdapter = new InterceptorAdapter(interceptor, target);

            var proxyType = typeof (TProxy);

            if (!proxyType.IsInterfaceEx())
            {
                return
                    (TProxy)
                    _proxyGenerator.CreateClassProxy(proxyType, interfaces, new Castle.DynamicProxy.IInterceptor[]
                        {
                            interceptorAdapter
                        });
            }
            return
                (TProxy)
                _proxyGenerator.CreateInterfaceProxyWithoutTarget(proxyType, interfaces,
                                                                  new Castle.DynamicProxy.IInterceptor[]
                                                                      {
                                                                          interceptorAdapter
                                                                      });
        }

        /// <summary>
        /// Creates a proxy instance of <paramref name="proxyType" /> type using the specified proxy type.
        /// </summary>
        /// <param name="proxyType">Type of the proxy.</param>
        /// <param name="interceptor">The interceptor.</param>
        /// <param name="interfaces">The interfaces.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="proxyType" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptor" /> is <c>null</c>.</exception>
        public object Create(Type proxyType, IInterceptor interceptor, params Type[] interfaces)
        {
            Argument.IsNotNull("proxyType", proxyType);
            Argument.IsNotNull("interceptor", interceptor);

            var interceptorAdapter = new InterceptorAdapter(interceptor, null);

            if (!proxyType.IsInterfaceEx())
            {
                return _proxyGenerator.CreateClassProxy(proxyType, interfaces, new Castle.DynamicProxy.IInterceptor[]
                    {
                        interceptorAdapter
                    });
            }

            return _proxyGenerator.CreateInterfaceProxyWithoutTarget(proxyType, interfaces,
                                                                     new Castle.DynamicProxy.IInterceptor[]
                                                                         {
                                                                             interceptorAdapter
                                                                         });
        }

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
        public TProxy Create<TProxy>(object[] parameters, IInterceptor interceptor, params Type[] interfaces)
            where TProxy : class
        {
            Argument.IsNotNullOrEmptyArray(() => parameters);
            Argument.IsNotNull(() => interceptor);

            var proxyType = typeof (TProxy);

            var proxy = (TProxy) _typeFactory.CreateInstanceWithParameters(proxyType, parameters);
            var interceptorAdapter = new InterceptorAdapter(interceptor, proxy);

            return
                (TProxy)
                _proxyGenerator.CreateClassProxy(proxyType, interfaces, ProxyGenerationOptions.Default,
                                                 parameters,
                                                 new Castle.DynamicProxy.IInterceptor[]
                                                     {
                                                         interceptorAdapter
                                                     });
        }
        #endregion
    }
}
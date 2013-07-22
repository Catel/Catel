// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CallbackHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caching;
    using Callbacks;
    using Exceptions;
    using Interceptors;
    using IoC;

    /// <summary>
    /// Implements the <see cref="ICallbackHandler{TService,TServiceImplementation}"/>.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TServiceImplementation">The type of the service implementation.</typeparam>
    public sealed class CallbackHandler<TService, TServiceImplementation> : HandlerBase,
                                                                            ICallbackHandler
                                                                                <TService, TServiceImplementation>
        where TServiceImplementation : TService
    {
        #region Fields
        /// <summary>
        /// The intercetor handler
        /// </summary>
        private readonly InterceptorHandler<TService, TServiceImplementation> _interceptorHandler;

        /// <summary>
        /// The proxy factory
        /// </summary>
        private readonly IProxyFactory _proxyFactory;

        /// <summary>
        ///     The target
        /// </summary>
        private readonly TService _target;

        /// <summary>
        /// The type factory
        /// </summary>
        private readonly ITypeFactory _typeFactory;

        /// <summary>
        ///     The context
        /// </summary>
        private ContextInterceptor _context;

        /// <summary>
        ///     The instance
        /// </summary>
        private TService _instance;

        /// <summary>
        ///     The _states
        /// </summary>
        private StateInterceptorCollection _states;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackHandler{TService, TServiceImplementation}"/> class.
        /// </summary>
        /// <param name="interceptorHandler">The intercetor handler.</param>
        /// <param name="proxyFactory">The proxy factory.</param>
        /// <param name="typeFactory">The type factory.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="interceptorHandler"/> is <c>null</c>.</exception>
        public CallbackHandler(InterceptorHandler<TService, TServiceImplementation> interceptorHandler, IProxyFactory proxyFactory = null, ITypeFactory typeFactory = null) :
            base(interceptorHandler.ServiceType, interceptorHandler.Tag)
        {
            Argument.IsNotNull(() => interceptorHandler);

            _interceptorHandler = interceptorHandler;
            _proxyFactory = proxyFactory ?? GetService<IProxyFactory>();
            _typeFactory = typeFactory ?? GetService<ITypeFactory>();

            _target = _typeFactory.CreateInstance<TServiceImplementation>();
        }
        #endregion

        #region Properties
        /// <summary>
        ///     Gets the callbacks.
        /// </summary>
        /// <value>
        ///     The callbacks.
        /// </value>
        protected internal override ICacheStorage<IMemberDefinition, CallbackCollection> Callbacks
        {
            get
            {
                lock (_interceptorHandler.Callbacks)
                {
                    return _interceptorHandler.Callbacks;
                }
            }
        }

        /// <summary>
        ///     Gets the intercepted members.
        /// </summary>
        /// <value>
        ///     The intercepted members.
        /// </value>
        protected internal override IList<IMemberDefinition> InterceptedMembers
        {
            get
            {
                lock (_interceptorHandler.InterceptedMembers)
                {
                    return _interceptorHandler.InterceptedMembers;
                }
            }
        }

        /// <summary>
        ///     Gets the implemented types.
        /// </summary>
        /// <value>
        ///     The implemented types.
        /// </value>
        protected internal override IList<Type> ImplementedTypes
        {
            get
            {
                lock (_interceptorHandler.ImplementedTypes)
                {
                    return _interceptorHandler.ImplementedTypes;
                }
            }
        }
        #endregion

        #region ICallbackHandler<TService,TServiceImplementation> Members
        /// <summary>
        /// Use it if you want to configure interception for another member.
        /// </summary>
        /// <returns></returns>
        public IInterceptorHandler<TService, TServiceImplementation> And()
        {
            lock (_interceptorHandler)
            {
                return _interceptorHandler;
            }
        }

        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnBefore(Action action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnBeforeCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnBefore(Action<IInvocation> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnBeforeCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnInvoke(Func<IInvocation, object> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnInvokeCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnInvoke(Action<IInvocation> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnInvokeCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnCatch(Action<Exception> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnCatchCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnCatch(Action<IInvocation, Exception> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnCatchCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnFinally(Action action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnFinallyCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnFinally(Action<IInvocation> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnFinallyCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnAfter(Action action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnAfterCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnAfter(Action<IInvocation> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnAfterCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnReturn(Func<object> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnReturnCallback(action);
            AddCallBack(callback);
            return this;
        }

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> OnReturn(Func<IInvocation, object, object> action)
        {
            Argument.IsNotNull("action", action);

            var callback = new OnReturnCallback(action);
            AddCallBack(callback);
            return this;
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Adds the specified call back.
        /// </summary>
        /// <param name="callback">The callback.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="callback"/> is <c>null</c>.</exception>
        private void AddCallBack(Callback callback)
        {
            Argument.IsNotNull("callback", callback);

            lock (InterceptedMembers)
            {
                InterceptedMembers.ForEach(member => Callbacks[member].Add(callback));

                Register();
            }
        }

        /// <summary>
        ///     Registers this instance.
        /// </summary>
        /// <exception cref="ProxyInitializationException"></exception>
        private void Register()
        {
            try
            {
                if (!ObjectHelper.IsNull(_instance))
                {
                    return;
                }

                _states = _typeFactory.CreateInstance<StateInterceptorCollection>();

                var stateIntercepors = Callbacks.Keys.Select(key => _typeFactory.CreateInstanceWithParameters<StateInterceptor>(key, Callbacks[key]));

                stateIntercepors.ForEach(stateInterceptor => _states.Add(stateInterceptor));

                _context = _typeFactory.CreateInstanceWithParameters<ContextInterceptor>(_states);

                _instance = _proxyFactory.Create(_target, _context, ImplementedTypes.ToArray());

                Container.RegisterInstance(ServiceType, _instance, Tag);
            }
            catch (Exception exception)
            {
                throw new ProxyInitializationException(
                    string.Format("Unable to create a proxy instance of a '{0}' type: {1}", ServiceType.Name,
                                  exception.Message),
                    exception);
            }
        }
        #endregion
    }
}
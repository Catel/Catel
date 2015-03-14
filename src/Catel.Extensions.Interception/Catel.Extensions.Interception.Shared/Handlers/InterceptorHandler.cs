// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InterceptorHandler.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Interception.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Caching;
    using Callbacks;
    using IoC;
    using Reflection;
    using Utilities;

    /// <summary>
    /// The <see cref="IInterceptorHandler{TService,TServiceImplementation}" /> implementation.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TServiceImplementation">The type of the service implementation.</typeparam>
    public sealed class InterceptorHandler<TService, TServiceImplementation> : HandlerBase,
                                                                               IInterceptorHandler<TService, TServiceImplementation>
        where TServiceImplementation : TService
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the
        /// <see>
        /// <cref>ProxyClass{TServiceImplementation}</cref>
        /// </see>
        /// class.
        /// </summary>
        /// <param name="serviceLocator">The service locator. If <c>null</c>, <see cref="ServiceLocator.Default" /> will be used.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="targetInstanceToUse">The target instance you want use in proxy instanciation.</param>
        /// <param name="typeFactory">The type factory. If <c>null</c>, <see cref="TypeFactory.Default" /> will be used.</param>
        public InterceptorHandler(IServiceLocator serviceLocator = null, object tag = null, object targetInstanceToUse = null, ITypeFactory typeFactory = null)
            : base(typeof(TService), tag, serviceLocator, targetInstanceToUse, typeFactory)
        {
            ImplementedTypes = new List<Type>();

            Callbacks = new CacheStorage<IMemberDefinition, CallbackCollection>();

            InterceptedMembers = new List<IMemberDefinition>();
        }
        #endregion

        #region IInterceptorHandler<TService,TServiceImplementation> Members
        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> Intercept(Expression<Action<TService>> method)
        {
            return InterceptMethod(method);
        }

        /// <summary>
        /// Intercepts the specified member.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="member"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> Intercept(Expression<Func<TService, object>> member)
        {
            return InterceptMethod(member);
        }

        /// <summary>
        /// Intercepts all.
        /// </summary>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptAll()
        {
            var memberDefinitions = GetMethodsToIntercept(ServiceType).Select(method => method.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts all getters.
        /// </summary>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptAllGetters()
        {
            var getters = GetMethodsToIntercept(ServiceType)
                .SelectGetters();

            var memberDefinitions = getters.Select(getter => getter.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts all setters.
        /// </summary>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptAllSetters()
        {
            var setters = GetMethodsToIntercept(ServiceType)
                .SelectSetters();

            var memberDefinitions = setters.Select(setter => setter.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts all members.
        /// </summary>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptAllMembers()
        {
            var memberDefinitions = GetMethodsToIntercept(ServiceType)
                .Select(method => method.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the specified methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="methods"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> InterceptMethods(
            params Expression<Action<TService>>[] methods)
        {
            Argument.IsNotNull("methods", methods);

            var memberDefinitions =
                methods.Select(method => method.GetMethodExpression().Method.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the where.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> InterceptWhere(Func<MethodInfo, bool> predicate)
        {
            Argument.IsNotNull("predicate", predicate);

            var methods = GetMethodsToIntercept(ServiceType)
                .Where(predicate);

            var memberDefinitions = methods.Select(method => method.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> InterceptMethod(Expression<Action<TService>> method)
        {
            Argument.IsNotNull("method", method);

            var methodExpression = method.GetMethodExpression();

            RegisterMethods(methodExpression);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public ICallbackHandler<TService, TServiceImplementation> InterceptMethod(
            Expression<Func<TService, object>> method)
        {
            Argument.IsNotNull("method", method);

            var methodExpression = method.GetMethodExpression();
            RegisterMethods(methodExpression);

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the getter of the specified property.
        /// </summary>
        /// <param name="property">The getter.</param>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptGetter(
            Expression<Func<TService, object>> property)
        {
            RegisterProperty(property, Type.EmptyTypes, "get_");

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }

        /// <summary>
        /// Intercepts the setter.
        /// </summary>
        /// <param name="property">The setter.</param>
        /// <returns></returns>
        public ICallbackHandler<TService, TServiceImplementation> InterceptSetter(
            Expression<Func<TService, object>> property)
        {
            RegisterProperty(property, new[] {property.Body.Type}, "set_");

            var callbackHandler = CreateCallBackHandler();
            return callbackHandler;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Registers the property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="prefix">The prefix.</param>
        private void RegisterProperty(Expression<Func<TService, object>> property, IList<Type> parameters,
                                      string prefix)
        {
            Argument.IsNotNull("property", property);

            var memberExpression = property.GetMemberExpression();
            var methodName = string.Format("{0}{1}", prefix, memberExpression.Member.Name);
            var memberDefinition = new MemberDefinition(methodName, parameters);

            if (!ServiceType.IsInterfaceEx())
            {
                Require.OverridableProperty<TService>(memberDefinition.MemberName);
            }

            RegisterMemberDefinitions(new[] {memberDefinition});
        }

        /// <summary>
        /// Registers the methods.
        /// </summary>
        /// <param name="methods">The methods.</param>
        private void RegisterMethods(params MethodCallExpression[] methods)
        {
            if (!ServiceType.IsInterfaceEx())
            {
                methods.ForEach(Require.OverridableMethod);
            }

            var memberDefinitions = methods.Select(expression => expression.Method.ExtractDefinition());

            RegisterMemberDefinitions(memberDefinitions);
        }

        /// <summary>
        /// Registers the member definitions.
        /// </summary>
        /// <param name="memberDefinitions">The member definitions.</param>
        private void RegisterMemberDefinitions(IEnumerable<IMemberDefinition> memberDefinitions)
        {
            lock (Callbacks)
            {
                InterceptedMembers = memberDefinitions.ToList();

                var members = InterceptedMembers.Where(member => !Callbacks.Contains(member));

                members.ForEach(member => Callbacks.Add(member, new CallbackCollection()));
            }
        }

        /// <summary>
        /// Gets the methods to intercept.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private IEnumerable<MethodInfo> GetMethodsToIntercept(Type type)
        {
            Argument.IsNotNull(() => type);

            return type.GetMethodsToIntercept()
                       .Where(method => !AttributeHelper.IsDecoratedWithAttribute<DoNotInterceptAttribute>(method));
        }

        /// <summary>
        /// Creates the call back handler.
        /// </summary>
        /// <returns></returns>
        private ICallbackHandler<TService, TServiceImplementation> CreateCallBackHandler()
        {
            var callbackHandler = TypeFactory.CreateInstanceWithParametersAndAutoCompletion<CallbackHandler<TService, TServiceImplementation>>(this);

            return callbackHandler;
        }
        #endregion
    }
}
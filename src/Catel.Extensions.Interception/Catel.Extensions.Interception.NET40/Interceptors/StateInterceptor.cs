// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateInterceptor.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Interceptors
{
    using System;
    using System.Linq;
    using Callbacks;

    /// <summary>
    /// Represents the State interceptor implementation.
    /// </summary>
    public class StateInterceptor : InterceptorBase, IStateInterceptor
    {
        #region Fields
        /// <summary>
        /// The visitor callback.
        /// </summary>
        private readonly SelectCallbackVisitor _visitor;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="StateInterceptor" /> class.
        /// </summary>
        /// <param name="memberDefinition">The method signature.</param>
        /// <param name="callbacks">The callbacks.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="memberDefinition" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="callbacks" /> is <c>null</c>.</exception>
        public StateInterceptor(IMemberDefinition memberDefinition, CallbackCollection callbacks)
        {
            Argument.IsNotNull("methodDefinition", memberDefinition);
            Argument.IsNotNull("callbacks", callbacks);

            MemberDefinition = memberDefinition;
            CallbackCollection = callbacks;

            _visitor = new SelectCallbackVisitor();
        }
        #endregion

        #region IStateInterceptor Members
        /// <summary>
        ///     Gets the callback collection.
        /// </summary>
        /// <value>
        ///     The callback collection.
        /// </value>
        public CallbackCollection CallbackCollection { get; private set; }

        /// <summary>
        ///     Gets the method signature.
        /// </summary>
        /// <value>
        ///     The method signature.
        /// </value>
        public IMemberDefinition MemberDefinition { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether OnInvoke was registered.
        /// </summary>
        /// <value>
        ///     <c>true</c> if OnInvoke was registered; otherwise, <c>false</c>.
        /// </value>
        public bool OnInvokeWasRegistered
        {
            get
            {
                lock (CallbackCollection)
                {
                    return CallbackCollection.Any(callBack => callBack is OnInvokeCallback);
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when before.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected override void OnBefore(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            CallbackCollection.ForEach(callBack => callBack.Accept(_visitor));
            _visitor.OnBeforeCallbackCollection.ForEach(callBack => callBack.Intercept(invocation));
        }

        /// <summary>
        /// Called when invoke.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected override object OnInvoke(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            object returnValue = null;
            _visitor.OnInvokeCallbackCollection.ForEach(callBack => returnValue = callBack.Intercept(invocation));
            return returnValue;
        }

        /// <summary>
        /// Called when catch.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="exception">The exception.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        protected override void OnCatch(IInvocation invocation, Exception exception)
        {
            Argument.IsNotNull("invocation", invocation);
            Argument.IsNotNull("exception", exception);

            var collection = _visitor.OnCatchCallbackCollection;

            if (!collection.Any())
            {
                throw exception;
            }

            collection.ForEach(callBack => callBack.Intercept(invocation, exception));
        }

        /// <summary>
        /// Called when finally.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected override void OnFinally(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            _visitor.OnFinallyCallbackCollection.ForEach(callBack => callBack.Intercept(invocation));
        }

        /// <summary>
        /// Called when after.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected override void OnAfter(IInvocation invocation)
        {
            Argument.IsNotNull("invocation", invocation);

            _visitor.OnAfterCallbackCollection.ForEach(callBack => callBack.Intercept(invocation));
        }

        /// <summary>
        /// Called when return.
        /// </summary>
        /// <param name="invocation">The invocation.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="invocation"/> is <c>null</c>.</exception>
        protected override object OnReturn(IInvocation invocation, object result)
        {
            Argument.IsNotNull("invocation", invocation);

            var collection = _visitor.OnReturnCallbackCollection;

            if (!collection.Any())
            {
                return result;
            }

            object returnValue = null;
            _visitor.OnReturnCallbackCollection.ForEach(callBack => returnValue = callBack.Intercept(invocation, result));
            return returnValue;
        }
        #endregion
    }
}
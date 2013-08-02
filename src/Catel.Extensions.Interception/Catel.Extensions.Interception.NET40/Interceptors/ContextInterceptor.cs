// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Interception.Interceptors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Callbacks;
    using Catel;
    using Reflection;

    /// <summary>
    /// Represents the context interceptor implementation.
    /// </summary>
    public class ContextInterceptor : IInterceptor
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextInterceptor" /> class.
        /// </summary>
        /// <param name="stateInterceptors">The state interceptors.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="stateInterceptors"/> is <c>null</c>.</exception>
        public ContextInterceptor(IList<StateInterceptor> stateInterceptors)
        {
            Argument.IsNotNull(() => stateInterceptors);

            StateInterceptorCollection = new StateInterceptorCollection(stateInterceptors);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the state interceptor collection.
        /// </summary>
        /// <value>
        /// The state interceptor collection.
        /// </value>
        public StateInterceptorCollection StateInterceptorCollection { get; private set; }
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

            var memberDefinition = invocation.Method.ExtractDefinition();

            if (memberDefinition != null)
            {
                var interceptorState = FindStateInterceptorByMemberDefinition(memberDefinition);

                var hasInterceptorState = interceptorState != null;
                var hasTargetObject = invocation.Target != null;
                var hasTargetMethod = hasInterceptorState && interceptorState.OnInvokeWasRegistered;

                Func<IInvocation, object> call = any => invocation.Method.Invoke(invocation.Target, invocation.Arguments);

                if (!hasTargetMethod && hasTargetObject && hasInterceptorState)
                {
                    interceptorState.CallbackCollection.Add(new OnInvokeCallback(call));
                    hasTargetMethod = true;
                }

                #region Execution

                if (hasInterceptorState && hasTargetMethod)
                {
                    return interceptorState.Intercept(invocation);
                }

                if (hasTargetObject)
                {
                    return call(invocation);
                }
                #endregion
            }

            throw new InvalidOperationException(string.Format("Unable to execute method: '{0}'. Speficy a target object or a target method utilizing Target() or OnInvoke() method.", invocation.Method.Name));
            
        }
        #endregion

        #region Methods
        /// <summary>
        /// Finds the state interceptor by method signature.
        /// </summary>
        /// <param name="definition">The signature.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="definition"/> is <c>null</c>.</exception>
        private StateInterceptor FindStateInterceptorByMemberDefinition(IMemberDefinition definition)
        {
            Argument.IsNotNull(() => definition);

            return StateInterceptorCollection.FirstOrDefault(interceptor => interceptor.MemberDefinition.Equals(definition));
        }
        #endregion
    }
}
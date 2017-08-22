// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WeakFunc.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Reflection;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// A weak func which allows the invocation of a command in a weak manner. This way, actions will not cause
    /// memory leaks.
    /// </summary>
    public class WeakFunc<TResult> : WeakActionBase, IWeakFunc<TResult>
    {
        /// <summary>
        /// Open instance action which allows the creation of an instance method without an actual reference
        /// to the target.
        /// </summary>
        public delegate TResult OpenInstanceAction<TTarget>(TTarget @this);

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The action that must be invoked on the action.
        /// </summary>
        private Delegate _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="func">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="func"/> is an anonymous delegate.</exception>
        public WeakFunc(object target, Func<TResult> func)
            : base(target)
        {
            Argument.IsNotNull("action", func);

            var methodInfo = func.GetMethodInfoEx();
            MethodName = methodInfo.ToString();

            if (MethodName.Contains("_AnonymousDelegate>"))
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Anonymous delegates are not supported because they are located in a private class");
            }

            var targetType = target?.GetType() ?? typeof(object);
            var delegateType = typeof(OpenInstanceAction<>).MakeGenericTypeEx(typeof(TResult), targetType);

            _action = DelegateHelper.CreateDelegate(delegateType, methodInfo);
        }

        /// <summary>
        /// Gets the name of the method that should be executed.
        /// </summary>
        /// <value>The method name.</value>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the actual delegate to invoke.
        /// </summary>
        /// <value>The method name.</value>
        /// <remarks>
        /// This property is only introduced to allow action comparison on WinRT. Do not try to use this method by yourself.
        /// </remarks>
        public Delegate Action { get { return _action; } }

        /// <summary>
        /// Executes the action. This only happens if the action's target is still alive.
        /// </summary>
        /// <param name="result"></param>
        /// <returns>
        /// <c>true</c> if the action is executed successfully; otherwise <c>false</c>.
        /// </returns>
        public bool Execute(out TResult result)
        {
            result = default(TResult);
            if (_action != null)
            {
                if (IsTargetAlive)
                {
                    try
                    {
                        result = (TResult)_action.DynamicInvoke(Target);
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            throw ex.InnerException;
                        }
                    }

                    return true;
                }

                Log.Debug("Target for '{0}' is no longer alive, weak event being garbage collected", MethodName);

                _action = null;
            }

            return false;
        }
    }

    /// <summary>
    /// A generic weak func which allows the invocation of a command in a weak manner. This way, funcs will not 
    /// cause memory leaks.
    /// </summary>
    public class WeakFunc<TParameter, TResult> : WeakActionBase, IWeakFunc<TParameter, TResult>
    {
        /// <summary>
        /// Open instance action which allows the creation of an instance method without an actual reference
        /// to the target.
        /// </summary>
        public delegate TResult OpenInstanceGenericAction<TTarget>(TTarget @this, TParameter parameter);

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The action that must be invoked on the action.
        /// </summary>
        private Delegate _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="func">The function.</param> 
        /// <exception cref="ArgumentNullException">The <paramref name="func"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="func"/> is an anonymous delegate.</exception>
        public WeakFunc(object target, Func<TParameter, TResult> func)
            : base(target)
        {
            Argument.IsNotNull("func", func);

            var methodInfo = func.GetMethodInfoEx();
            MethodName = methodInfo.ToString();

            if (MethodName.Contains("_AnonymousDelegate>"))
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Anonymous delegates are not supported because they are located in a private class");
            }

            var targetType = target?.GetType() ?? typeof(object);
            var delegateType = typeof(OpenInstanceGenericAction<>).MakeGenericTypeEx(typeof(TParameter), typeof(TResult), targetType);

            _action = DelegateHelper.CreateDelegate(delegateType, methodInfo);
        }

        /// <summary>
        /// Gets the name of the method that should be executed.
        /// </summary>
        /// <value>The method name.</value>
        public string MethodName { get; private set; }

        /// <summary>
        /// Gets the actual delegate to invoke.
        /// </summary>
        /// <value>The method name.</value>
        /// <remarks>
        /// This property is only introduced to allow action comparison on WinRT. Do not try to use this method by yourself.
        /// </remarks>
        public Delegate Action { get { return _action; } }

        /// <summary>
        /// Executes the action. This only happens if the action's target is still alive.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="result">The result</param>
        public bool Execute(TParameter parameter, out TResult result)
        {
            result = default(TResult);

            if (_action != null)
            {
                if (IsTargetAlive)
                {
                    try
                    {
                        result = (TResult)_action.DynamicInvoke(Target, parameter);
                    }
                    catch (TargetInvocationException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            throw ex.InnerException;
                        }
                    }

                    return true;
                }

                Log.Debug("Target for '{0}' is no longer alive, weak event being garbage collected", MethodName);

                _action = null;
            }

            return false;
        }

        /// <summary>
        /// Executes the object with the object parameter.
        /// <para/>
        /// The class implementing this interface is responsible for casting the <paramref name="parameter"/>
        /// to the right type and to determine whether <c>null</c> is allowed as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="result">The result</param>
        /// <returns>
        /// <c>true</c> if the action is executed successfully; otherwise <c>false</c>.
        /// </returns>
        bool IExecuteWithObject<TResult>.ExecuteWithObject(object parameter, out TResult result)
        {
            return Execute((TParameter)parameter, out result);
        }
    }
}
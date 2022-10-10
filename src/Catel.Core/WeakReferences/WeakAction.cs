namespace Catel
{
    using System;
    using Catel.Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Base class for weak actions that contain weak targets.
    /// </summary>
    public abstract class WeakActionBase : IWeakReference
    {
        /// <summary>
        /// WeakReference to the target listening for the event.
        /// </summary>
        private readonly WeakReference? _weakTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakActionBase"/> class.
        /// </summary>
        /// <param name="target">The target of the weak action.</param>
        /// <exception cref="ArgumentException">The <paramref name="target"/> is <c>null</c> or whitespace.</exception>
        protected WeakActionBase(object? target)
        {
            if (target is not null)
            {
                _weakTarget = new WeakReference(target);
            }
        }

        /// <summary>
        /// Gets the target or <c>null</c> if the target is garbage collected.
        /// </summary>
        /// <value>The target.</value>
        public object? Target { get { return (_weakTarget is not null) ? _weakTarget.Target : null; } }

        /// <summary>
        /// Gets a value indicating whether the event target has not yet been garbage collected.
        /// </summary>
        /// <value>
        /// <c>true</c> if the event target has not yet been garbage collected; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// In case of static event handlers, this property always returns <c>false</c>.
        /// </remarks>
        public bool IsTargetAlive { get { return (_weakTarget is not null) && _weakTarget.IsAlive; } }
    }

    /// <summary>
    /// A weak action which allows the invocation of a command in a weak manner. This way, actions will not cause
    /// memory leaks.
    /// </summary>
    public class WeakAction : WeakActionBase, IWeakAction
    {
        /// <summary>
        /// Open instance action which allows the creation of an instance method without an actual reference
        /// to the target.
        /// </summary>
        public delegate void OpenInstanceAction<TTarget>(TTarget @this);

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The action that must be invoked on the action.
        /// </summary>
        private Delegate? _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="action"/> is an anonymous delegate.</exception>
        public WeakAction(object target, Action action)
            : base(target)
        {
            var methodInfo = action.GetMethodInfoEx();
            if (methodInfo is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Cannot retrieve method info from provided action");
            }

            MethodName = methodInfo.ToString() ?? string.Empty;

            if (MethodName.Contains("_AnonymousDelegate>"))
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Anonymous delegates are not supported because they are located in a private class");
            }

            var targetType = (target is not null) ? target.GetType() : typeof(object);
            var delegateType = typeof(OpenInstanceAction<>).MakeGenericTypeEx(targetType);

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
        public Delegate? Action { get { return _action; } }

        /// <summary>
        /// Executes the action. This only happens if the action's target is still alive.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the action is executed successfully; otherwise <c>false</c>.
        /// </returns>
        public bool Execute()
        {
            if (_action is not null)
            {
                if (IsTargetAlive)
                {
#pragma warning disable HAA0101 // Array allocation for params parameter
                    _action.DynamicInvoke(Target);
#pragma warning restore HAA0101 // Array allocation for params parameter
                    return true;
                }

                Log.Debug("Target for '{0}' is no longer alive, weak event being garbage collected", MethodName);

                _action = null;
            }

            return false;
        }
    }

    /// <summary>
    /// A generic weak action which allows the invocation of a command in a weak manner. This way, actions will not 
    /// cause memory leaks.
    /// </summary>
    public class WeakAction<TParameter> : WeakActionBase, IWeakAction<TParameter>
    {
        /// <summary>
        /// Open instance action which allows the creation of an instance method without an actual reference
        /// to the target.
        /// </summary>
        public delegate void OpenInstanceGenericAction<TTarget>(TTarget @this, TParameter parameter);

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The action that must be invoked on the action.
        /// </summary>
        private Delegate? _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakAction"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="action">The action.</param> 
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="action"/> is an anonymous delegate.</exception>
        public WeakAction(object target, Action<TParameter> action)
            : base(target)
        {
            var methodInfo = action.GetMethodInfoEx();
            if (methodInfo is null)
            {
                throw Log.ErrorAndCreateException<CatelException>("Cannot retrieve method info from provided action");
            }

            MethodName = methodInfo.ToString() ?? string.Empty;

            if (MethodName.Contains("_AnonymousDelegate>"))
            {
                throw Log.ErrorAndCreateException<NotSupportedException>("Anonymous delegates are not supported because they are located in a private class");
            }

            var targetType = (target is not null) ? target.GetType() : typeof(object);
#pragma warning disable HAA0101 // Array allocation for params parameter
            var delegateType = typeof(OpenInstanceGenericAction<>).MakeGenericType(typeof(TParameter), targetType);
#pragma warning restore HAA0101 // Array allocation for params parameter

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
        public Delegate? Action { get { return _action; } }

        /// <summary>
        /// Executes the action. This only happens if the action's target is still alive.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public bool Execute(TParameter parameter)
        {
            if (_action is not null)
            {
                if (IsTargetAlive)
                {
#pragma warning disable HAA0101 // Array allocation for params parameter
                    _action.DynamicInvoke(Target, parameter);
#pragma warning restore HAA0101 // Array allocation for params parameter
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
        /// <returns>
        /// <c>true</c> if the action is executed successfully; otherwise <c>false</c>.
        /// </returns>
        bool IExecuteWithObject.ExecuteWithObject(object parameter)
        {
            return Execute((TParameter)parameter);
        }

        /// <summary>
        /// Executes the object with the object parameter.
        /// <para/>
        /// The class implementing this interface is responsible for casting the <paramref name="parameter"/>
        /// to the right type and to determine whether <c>null</c> is allowed as parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// <c>true</c> if the action is executed successfully; otherwise <c>false</c>.
        /// </returns>
        bool IExecuteWithObject<TParameter>.ExecuteWithObject(TParameter parameter)
        {
            return Execute(parameter);
        }
    }
}

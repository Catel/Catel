namespace Catel
{
    using System;

    /// <summary>
    /// A weak func which allows the invocation of a command in a weak manner. This way, actions will not cause
    /// memory leaks.
    /// </summary>
    public interface IWeakFunc<TResult> : IWeakReference, IExecute<TResult>
    {
        /// <summary>
        /// Gets the name of the method that should be executed.
        /// </summary>
        /// <value>The method name.</value>
        string MethodName { get; }

        /// <summary>
        /// Gets the actual delegate to invoke.
        /// </summary>
        /// <value>The method name.</value>
        /// <remarks>
        /// This property is only introduced to allow action comparison on WinRT. Do not try to use this method by yourself.
        /// </remarks>
        Delegate Action { get; }
    }

    /// <summary>
    /// A weak func which allows the invocation of a command in a weak manner. This way, actions will not cause
    /// memory leaks.
    /// </summary>
    public interface IWeakFunc<TParameter, TResult> : IWeakReference, IExecuteWithObject<TParameter, TResult>
    {
        /// <summary>
        /// Gets the name of the method that should be executed.
        /// </summary>
        /// <value>The method name.</value>
        string MethodName { get; }

        /// <summary>
        /// Gets the actual delegate to invoke.
        /// </summary>
        /// <value>The method name.</value>
        /// <remarks>
        /// This property is only introduced to allow action comparison on WinRT. Do not try to use this method by yourself.
        /// </remarks>
        Delegate Action { get; }
    }
}

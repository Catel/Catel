// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWeakAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
namespace Catel
{
    /// <summary>
    /// A weak action which allows the invocation of a command in a weak manner. This way, actions will not cause
    /// memory leaks.
    /// </summary>
    public interface IWeakAction : IWeakReference, IExecute
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
    /// A generic weak action which allows the invocation of a command in a weak manner. This way, actions will not
    /// cause memory leaks.
    /// </summary>
    /// <typeparam name="TParameter">The type of the parameter.</typeparam>
    public interface IWeakAction<TParameter> : IWeakReference, IExecuteWithObject
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

        /// <summary>
        /// Executes the action. This only happens if the action's target is still alive.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns><c>true</c> if the action is executed successfully; otherwise <c>false</c>.</returns>
        bool Execute(TParameter parameter);
    }
}

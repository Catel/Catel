// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.Windows.Threading
{
    using System;

#if NETFX_CORE
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static class DispatcherExtensions
    {
#if NET
        /// <summary>
        /// Gets the managed thread identifier for the specified dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns>The managed thread id.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher" /> is <c>null</c>.</exception>
        public static int GetThreadId(this Dispatcher dispatcher)
        {
            Argument.IsNotNull("dispatcher", dispatcher);

            return dispatcher.Thread.ManagedThreadId;
        }
#endif

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Action action)
        {
            Argument.IsNotNull("action", action);

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
#if NET
                dispatcher.Invoke(action, null);
#elif NETFX_CORE
                dispatcher.BeginInvoke(action);
#else
                dispatcher.BeginInvoke(action);
#endif
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Executes the specified delegate with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            if (dispatcher != null && !dispatcher.CheckAccess())
            {
#if NET
                dispatcher.Invoke(method, args);
#elif NETFX_CORE
                dispatcher.BeginInvoke(() => method.DynamicInvoke(args));
#else
                dispatcher.BeginInvoke(method, args);
#endif
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

#if !NETFX_CORE
        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void BeginInvoke(this Dispatcher dispatcher, Action action)
        {
            BeginInvoke(dispatcher, action, false);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void BeginInvoke(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            BeginInvoke(dispatcher, () => method.DynamicInvoke(args), false);
        }
#endif

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void BeginInvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            BeginInvoke(dispatcher, action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void BeginInvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            BeginInvoke(dispatcher, () => method.DynamicInvoke(args), true);
        }


        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public static void BeginInvoke(this Dispatcher dispatcher, Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);

            bool actionInvoked = false;

            if (dispatcher != null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
#if NETFX_CORE
                    dispatcher.BeginInvoke(action);
#else
                    dispatcher.BeginInvoke(action, null);
#endif

                    actionInvoked = true;
                }
            }

            if (!actionInvoked)
            {
                action.Invoke();
            }
        }
    }
}
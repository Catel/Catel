// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using Windows.Threading;

#if NETFX_CORE
    using global::Windows.UI.Core;
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    /// <remarks>
    /// Internally, this service uses the <see cref="DispatcherHelper"/> class to retrieve the current dispatcher. If there is
    /// no current <see cref="Dispatcher"/>, the method will be invoked manually on the current thread.
    /// </remarks>
    public class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Gets the current dispatcher.
        /// <para />
        /// Internally, this property uses the <see cref="DispatcherHelper"/>, but can be overriden if required.
        /// </summary>
        protected virtual Dispatcher CurrentDispatcher
        {
            get { return DispatcherHelper.CurrentDispatcher; }
        }

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <remarks>
        /// For target frameworks where the <see cref="Dispatcher"/> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.
        /// </remarks>
        public void Invoke(Action action)
        {
            Argument.IsNotNull("action", action);

            var dispatcher = CurrentDispatcher;
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
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <remarks>
        /// For target frameworks where the <see cref="Dispatcher"/> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public void Invoke(Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            var dispatcher = CurrentDispatcher;
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

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <remarks>
        /// For target frameworks where the <see cref="Dispatcher"/> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.
        /// </remarks>
        public void BeginInvoke(Action action)
        {
            BeginInvoke(action, false);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public void BeginInvoke(Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            BeginInvoke(() => method.DynamicInvoke(args), false);
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <remarks>
        /// For target frameworks where the <see cref="Dispatcher"/> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.
        /// </remarks>
        public void BeginInvokeIfRequired(Action action)
        {
            BeginInvoke(action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
        public void BeginInvokeIfRequired(Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            BeginInvoke(() => method.DynamicInvoke(args), true);
        }


        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">if set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        private void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);

            bool actionInvoked = false;

            var dispatcher = CurrentDispatcher;
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
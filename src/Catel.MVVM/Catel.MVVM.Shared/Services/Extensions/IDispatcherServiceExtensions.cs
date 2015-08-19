// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDispatcherServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using Catel.Windows.Threading;

    /// <summary>
    /// Extension methods for the <see cref="IDispatcherService"/>.
    /// </summary>
    public static class IDispatcherServiceExtensions
    {
        /// <summary>
        /// Executes the specified delegate with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void Invoke(this IDispatcherService dispatcherService, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            dispatcherService.Invoke(() => method.DynamicInvoke(args));
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public static void InvokeIfRequired(this IDispatcherService dispatcherService, Action action)
        {
            dispatcherService.Invoke(action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void InvokeIfRequired(this IDispatcherService dispatcherService, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            dispatcherService.Invoke(() => method.DynamicInvoke(args), true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void BeginInvoke(this IDispatcherService dispatcherService, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            dispatcherService.BeginInvoke(() => method.DynamicInvoke(args), false);
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public static void BeginInvoke(this IDispatcherService dispatcherService, Action action)
        {
            dispatcherService.BeginInvoke(action, false);
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public static void BeginInvokeIfRequired(this IDispatcherService dispatcherService, Action action)
        {
            dispatcherService.BeginInvoke(action, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void BeginInvokeIfRequired(this IDispatcherService dispatcherService, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            dispatcherService.BeginInvoke(() => method.DynamicInvoke(args), true);
        }

#if !NETFX_CORE && !SILVERLIGHT
        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        public static async Task InvokeAsync(this IDispatcherService dispatcherService, Action action)
        {
            var dispatcher = dispatcherService.CurrentDispatcher;

#if NET40
            await DispatcherExtensions.InvokeAsync(dispatcher, action);
#else
            await dispatcher.InvokeAsync(action);
#endif
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public static Task InvokeAsync(this IDispatcherService dispatcherService, Delegate method, params object[] args)
        {
            var dispatcher = dispatcherService.CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, method, args);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public static Task<T> InvokeAsync<T>(this IDispatcherService dispatcherService, Func<T> func)
        {
            var dispatcher = dispatcherService.CurrentDispatcher;

            return DispatcherExtensions.InvokeAsync(dispatcher, func);
        }
#endif
        }
}
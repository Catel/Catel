namespace Catel.Windows.Threading
{
    using System;
    using System.Windows.Threading;

    /// <summary>
    /// Extension methods for the dispatcher.
    /// </summary>
    public static partial class DispatcherExtensions
    {
        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action)
        {
            return BeginInvoke(dispatcher, action, false);
        }

        /// <summary>
        /// Executes the specified action asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            return BeginInvoke(dispatcher, action, priority, false);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), false);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Delegate method, DispatcherPriority priority, params object[] args)
        {
            Argument.IsNotNull("method", method);

            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), priority, false);
        }

        /// <summary>
        /// Executes the specified action asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            return BeginInvoke(dispatcher, action, true);
        }

        /// <summary>
        /// Executes the specified action asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            return BeginInvoke(dispatcher, action, priority, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            Argument.IsNotNull("method", method);

            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static DispatcherOperation BeginInvokeIfRequired(this Dispatcher dispatcher, Delegate method, DispatcherPriority priority, params object[] args)
        {
            Argument.IsNotNull("method", method);

            return BeginInvoke(dispatcher, () => method.DynamicInvoke(args), priority, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);

            if (dispatcher is not null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    return dispatcher.BeginInvoke(action, null);
                }
            }

            action.Invoke();
            return GetDefaultDispatcherOperation(dispatcher);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <returns>The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.</returns>
        public static DispatcherOperation BeginInvoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority, bool onlyBeginInvokeWhenNoAccess)
        {
            Argument.IsNotNull("action", action);

            if (dispatcher is not null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    return dispatcher.BeginInvoke(action, priority, null);
                }
            }

            action.Invoke();
            return GetDefaultDispatcherOperation(dispatcher);
        }

        private static readonly Action EmptyAction = new Action(() => { });

        private static DispatcherOperation GetDefaultDispatcherOperation(Dispatcher dispatcher)
        {
            // Fix for https://github.com/Catel/Catel/issues/1220

            //[SecurityCritical]
            //internal DispatcherOperation(Dispatcher dispatcher, DispatcherPriority priority, Action action)
            //: this(dispatcher, (Delegate)action, priority, (object)null, 0, (DispatcherOperationTaskSource)new DispatcherOperationTaskSource<object>(), true)
            //{
            //}

            //var dispatcherOperation = (DispatcherOperation)Activator.CreateInstance(typeof(DispatcherOperation),
            //    dispatcher, DispatcherPriority.Normal, EmptyAction);

            //return dispatcherOperation;

            // Unfortunately we will need to await a dispatcher operation anyway
            return dispatcher.BeginInvoke(EmptyAction);
        }
    }
}

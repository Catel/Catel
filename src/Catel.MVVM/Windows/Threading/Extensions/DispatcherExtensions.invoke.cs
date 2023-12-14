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
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (dispatcher is not null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action, null);
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Executes the specified action synchronously at the specified priority with the specified arguments on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (dispatcher is not null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(action, priority, null);
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
        public static void Invoke(this Dispatcher dispatcher, Delegate method, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            if (dispatcher is not null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(method, args);
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority with the specified arguments on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void Invoke(this Dispatcher dispatcher, Delegate method, DispatcherPriority priority, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            if (dispatcher is not null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(method, priority, args);
            }
            else
            {
                method.DynamicInvoke(args);
            }
        }

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
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action)
        {
            Invoke(dispatcher, action, true);
        }

        /// <summary>
        /// Executes the specified action synchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        /// <remarks>For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.</remarks>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Action action, DispatcherPriority priority)
        {
            Invoke(dispatcher, action, priority, true);
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
        public static void InvokeIfRequired(this Dispatcher dispatcher, Delegate method, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            Invoke(dispatcher, () => method.DynamicInvoke(args), true);
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on if required.
        /// <para />
        /// To check whether this is necessary, it will check whether the current thread has access to the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="method">A delegate to a method that takes parameters specified in args, which is pushed onto the Dispatcher event queue.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. Can be <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="method" /> is <c>null</c>.</exception>
        public static void InvokeIfRequired(this Dispatcher dispatcher, Delegate method, DispatcherPriority priority, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            Invoke(dispatcher, () => method.DynamicInvoke(args), priority, true);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public static void Invoke(this Dispatcher dispatcher, Action action, bool onlyBeginInvokeWhenNoAccess)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (dispatcher is not null)
            {
                if (!onlyBeginInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(action, null);
                    return;
                }
            }

            action.Invoke();
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise,
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public static void Invoke(this Dispatcher dispatcher, Action action, DispatcherPriority priority, bool onlyInvokeWhenNoAccess)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (dispatcher is not null)
            {
                if (!onlyInvokeWhenNoAccess || !dispatcher.CheckAccess())
                {
                    dispatcher.Invoke(action, priority, null);
                    return;
                }
            }

            action.Invoke();
        }
    }
}

namespace Catel.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using System.Windows.Threading;
    using DispatcherExtensions = Windows.Threading.DispatcherExtensions;
    using System.Linq;

    /// <summary>
    /// Service that allows the retrieval of the UI dispatcher.
    /// </summary>
    public class DispatcherService : IDispatcherService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly Lazy<bool> _isRunningUnitTests = new Lazy<bool>(() =>
            AppDomain.CurrentDomain.GetAssemblies().Any(x =>
            {
                var fullName = x.FullName;
                if (string.IsNullOrEmpty(fullName))
                {
                    return false;
                }

                if (fullName.StartsWithIgnoreCase("testhost,"))
                {
                    return true;
                }

                if (fullName.StartsWithIgnoreCase("Microsoft.TestPlatform"))
                {
                    return true;
                }

                return false;
            }));

        private readonly IDispatcherProviderService _dispatcherProviderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DispatcherService"/> class.
        /// </summary>
        public DispatcherService(IDispatcherProviderService dispatcherProviderService)
        {
            ArgumentNullException.ThrowIfNull(dispatcherProviderService);

            _dispatcherProviderService = dispatcherProviderService;
        }

        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        protected virtual Dispatcher CurrentDispatcher
        {
            get
            {
                var dispatcher = _dispatcherProviderService.GetApplicationDispatcher() as Dispatcher;
                if (dispatcher is null)
                {
                    throw Log.ErrorAndCreateException<CatelException>($"Cannot find application dispatcher");
                }

                return dispatcher;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual bool IsRunningUnitTests
        {
            get
            {
                return _isRunningUnitTests.Value;
            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task InvokeAsync(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            var dispatcher = CurrentDispatcher;

            var task = dispatcher.InvokeAsync(action).Task;

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="args">The arguments to pass into the method.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task InvokeAsync(Delegate method, params object[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, method, args);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <returns>The task representing the action.</returns>
        public virtual Task<T> InvokeAsync<T>(Func<T> func)
        {
            ArgumentNullException.ThrowIfNull(func);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, func);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value</param>
        /// <returns>The task representing the asynchronous operation</returns>
        public virtual Task InvokeTaskAsync(Func<Task> actionAsync)
        {
            ArgumentNullException.ThrowIfNull(actionAsync);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, actionAsync);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with supporting of CancellationToken
        /// </summary>
        /// <param name="actionAsync">The asynchronous operation without returning a value</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The task representing the asynchronous operation</returns>
        public virtual Task InvokeTaskAsync(Func<CancellationToken, Task> actionAsync, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(actionAsync);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, actionAsync, cancellationToken);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }
        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value.
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value</param>
        /// <returns>The task representing the asynchronous operation with the returning value</returns>
        public virtual Task<T> InvokeTaskAsync<T>(Func<Task<T>> funcAsync)
        {
            ArgumentNullException.ThrowIfNull(funcAsync);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, funcAsync);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified asynchronous operation on the thread that the Dispatcher was created on with the ability to return value with supporting of CancellationToken
        /// </summary>
        /// <typeparam name="T">The type of the result of the asynchronous operation</typeparam>
        /// <param name="funcAsync">The asynchronous operation which returns a value and supports cancelling</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The task representing the asynchronous operation with the returning value</returns>
        public virtual Task<T> InvokeTaskAsync<T>(Func<CancellationToken, Task<T>> funcAsync, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(funcAsync);

            var dispatcher = CurrentDispatcher;

            var task = DispatcherExtensions.InvokeAsync(dispatcher, funcAsync, cancellationToken);

            ProcessEventsDuringUnitTests(dispatcher);

            return task;
        }

        /// <summary>
        /// Executes the specified action with the specified arguments synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> is <c>null</c>.</exception>
        public virtual void Invoke(Action action, bool onlyInvokeWhenNoAccess = true)
        {
            ArgumentNullException.ThrowIfNull(action);

            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.Invoke(dispatcher, action, onlyInvokeWhenNoAccess);
        }

        /// <summary>
        /// Executes the specified delegate asynchronously with the specified arguments on the thread that the Dispatcher was created on.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="onlyBeginInvokeWhenNoAccess">If set to <c>true</c>, the action will be executed directly if possible. Otherwise, 
        /// <c>Dispatcher.BeginInvoke</c> will be used.</param>
        public virtual void BeginInvoke(Action action, bool onlyBeginInvokeWhenNoAccess = true)
        {
            ArgumentNullException.ThrowIfNull(action);

            var dispatcher = CurrentDispatcher;
            DispatcherExtensions.BeginInvoke(dispatcher, action, onlyBeginInvokeWhenNoAccess);

            ProcessEventsDuringUnitTests(dispatcher);
        }

        protected virtual void ProcessEventsDuringUnitTests(Dispatcher dispatcher)
        {
            if (!IsRunningUnitTests)
            {
                return;
            }

            DispatcherUtil.DoEvents(dispatcher);
        }

        /// <summary>
        /// Dispatcher util comes from https://stackoverflow.com/questions/1106881/using-the-wpf-dispatcher-in-unit-tests.
        /// </summary>
        private static class DispatcherUtil
        {
            public static void DoEvents(Dispatcher dispatcher)
            {
                var frame = new DispatcherFrame();
                dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
                Dispatcher.PushFrame(frame);
            }

            private static object? ExitFrame(object frame)
            {
                ((DispatcherFrame)frame).Continue = false;
                return null;
            }
        }
    }
}

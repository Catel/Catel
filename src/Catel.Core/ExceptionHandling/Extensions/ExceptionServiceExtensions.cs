// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExceptionServiceExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.ExceptionHandling
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Threading;

    /// <summary>
    /// The exception service extensions.
    /// </summary>
    public static class ExceptionServiceExtensions
    {
        /// <summary>
        /// Handles asynchronously the specified exception if possible.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <returns><c>true</c> if the exception is handled; otherwise <c>false</c>.</returns>
        public static Task<bool> HandleExceptionAsync(this IExceptionService exceptionService, Exception exception, CancellationToken cancellationToken = default(CancellationToken))
        {
            Argument.IsNotNull("exceptionService", exceptionService);

            return TaskHelper.Run(() => exceptionService.HandleException(exception), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Registers an handler for a specific exception.
        /// </summary>
        /// <typeparam name="TExceptionHandler">The type of the exception handler.</typeparam>
        /// <param name="exceptionService">The exception service.</param>
        /// <returns>The handler to use.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService"/> is <c>null</c>.</exception>
        public static IExceptionHandler Register<TExceptionHandler>(this IExceptionService exceptionService) 
            where TExceptionHandler : IExceptionHandler, new()
        {
            Argument.IsNotNull("exceptionService", exceptionService);

            var exceptionHandler = new TExceptionHandler();

            return exceptionService.Register((IExceptionHandler)exceptionHandler);
        }

        /// <summary>
        /// Processes asynchronously the specified action with possibility to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static Task ProcessWithRetryAsync(this IExceptionService exceptionService, Task action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return exceptionService.ProcessWithRetryAsync(async () => 
            {
                await action.ConfigureAwait(TaskHelper.DefaultConfigureAwaitValue);
                return default(object);
            });
        }

        /// <summary>
        /// Processes asynchronously the specified action with possibility to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static Task ProcessWithRetryAsync(this IExceptionService exceptionService, Action action, CancellationToken cancellationToken = new CancellationToken())
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return TaskHelper.Run(() => exceptionService.ProcessWithRetry(action), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Processes asynchronously the specified action with possibility to retry on error.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static Task<TResult> ProcessWithRetryAsync<TResult>(this IExceptionService exceptionService, Func<TResult> action, CancellationToken cancellationToken = new CancellationToken())
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return TaskHelper.Run(() => exceptionService.ProcessWithRetry(action), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Processes asynchrounously the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static Task ProcessWithRetryAsync(this IExceptionService exceptionService, Func<Task> action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return exceptionService.ProcessWithRetryAsync(async () =>
            {
                await action().ConfigureAwait(TaskHelper.DefaultConfigureAwaitValue);
                return default(object);
            });
        }

        /// <summary>
        /// Processes the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static void ProcessWithRetry(this IExceptionService exceptionService, Action action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            exceptionService.ProcessWithRetry(() =>
            {
                action();
                return default(object);
            });
        }
    }
}

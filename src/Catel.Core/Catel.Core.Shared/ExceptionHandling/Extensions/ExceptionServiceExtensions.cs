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

    /// <summary>
    /// The exception service extensions.
    /// </summary>
    public static class ExceptionServiceExtensions
    {
        /// <summary>
        /// Handles asynchounously the specified exception if possible.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="exception">The exception to handle.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="exception"/> is <c>null</c>.</exception>
        /// <returns><c>true</c> if the exception is handled; otherwise <c>false</c>.</returns>
        public static async Task<bool> HandleExceptionAsync(this IExceptionService exceptionService, Exception exception, CancellationToken cancellationToken = default(CancellationToken))
        {
            Argument.IsNotNull("exceptionService", exceptionService);

#if NET40 || SL5 || PCL
            return await Task.Factory.StartNew(() => exceptionService.HandleException(exception), cancellationToken).ConfigureAwait(false);
#else
            return await Task.Run(() => exceptionService.HandleException(exception), cancellationToken).ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Processes asynchrounously the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="exceptionService"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static Task ProcessWithRetryAsync(this IExceptionService exceptionService, Task action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return exceptionService.ProcessWithRetryAsync(async () => { await action.ConfigureAwait(false); return default(object); });
        }

        /// <summary>
        /// Processes asynchrounously the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static async Task ProcessWithRetryAsync(this IExceptionService exceptionService, Action action, CancellationToken cancellationToken = new CancellationToken())
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

#if NET40 || SL5 || PCL
            await Task.Factory.StartNew(() => exceptionService.ProcessWithRetry(action), cancellationToken).ConfigureAwait(false);
#else
            await Task.Run(() => exceptionService.ProcessWithRetry(action), cancellationToken).ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Processes asynchrounously the specified action with possibilty to retry on error.
        /// </summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static async Task<TResult> ProcessWithRetryAsync<TResult>(this IExceptionService exceptionService, Func<TResult> action, CancellationToken cancellationToken = new CancellationToken())
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

#if NET40 || SL5 || PCL
            return await Task.Factory.StartNew(() => exceptionService.ProcessWithRetry(action), cancellationToken).ConfigureAwait(false);
#else
            return await Task.Run(() => exceptionService.ProcessWithRetry(action), cancellationToken).ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Processes asynchrounously the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static Task ProcessWithRetryAsync(this IExceptionService exceptionService, Func<Task> action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            return exceptionService.ProcessWithRetryAsync(async () => { await action().ConfigureAwait(false); return default(object); });
        }

        /// <summary>
        /// Processes the specified action with possibilty to retry on error.
        /// </summary>
        /// <param name="exceptionService">The exception service.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static void ProcessWithRetry(this IExceptionService exceptionService, Action action)
        {
            Argument.IsNotNull("exceptionService", exceptionService);
            Argument.IsNotNull("action", action);

            exceptionService.ProcessWithRetry(() => { action(); return default(object); });
        }
    }
}
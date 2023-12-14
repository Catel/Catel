namespace Catel.Windows.Threading
{
    using System;
    using System.Threading.Tasks;

    // Required for DispatcherOperation on all platforms
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
        /// <param name="func">The async action.</param>
        /// <returns>
        /// The DispatcherOperation or <c>null</c> if the action was not dispatched but executed directly.
        /// </returns>
        /// <exception cref="ArgumentNullException">The <paramref name="func" /> is <c>null</c>.</exception>
        /// <remarks>
        /// For target frameworks where the <see cref="Dispatcher" /> class does not contain the <c>Invoke</c> method, the <c>BeginInvoke</c>
        /// method will be used instead.
        /// </remarks>
        public static async Task BeginInvokeAsync(this Dispatcher dispatcher, Func<Task> func)
        {
            ArgumentNullException.ThrowIfNull(dispatcher);
            ArgumentNullException.ThrowIfNull(func);

            var tcs = new TaskCompletionSource<object?>();

#pragma warning disable AvoidAsyncVoid // Avoid async void
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            dispatcher.BeginInvoke(async () =>
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning restore AvoidAsyncVoid // Avoid async void
            {
                try
                {
                    var task = func();
                    await task;

                    tcs.TrySetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            });

            await tcs.Task;
        }
    }
}

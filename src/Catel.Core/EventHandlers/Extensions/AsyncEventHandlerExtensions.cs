// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEventHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Logging;
    using Threading;

    /// <summary>
    /// Extensions for asynchronous event handlers.
    /// </summary>
    public static class AsyncEventHandlerExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Invokes the specified <paramref name="handler" />
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="allowParallelExecution">if set to <c>true</c>, allow parallel invocation of the handlers.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public static Task<bool> SafeInvokeAsync(this AsyncEventHandler<EventArgs> handler, object sender, bool allowParallelExecution = true)
        {
            return SafeInvokeAsync(handler, sender, EventArgs.Empty, allowParallelExecution);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler" />
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the <see cref="EventArgs" /> class.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        /// <param name="allowParallelExecution">if set to <c>true</c>, allow parallel invocation of the handlers.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public static async Task<bool> SafeInvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e, bool allowParallelExecution = true)
            where TEventArgs : EventArgs
        {
            if (handler is null)
            {
                return false;
            }

            var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();

            if (allowParallelExecution)
            {
                try
                {
                    var tasks = (from eventListener in eventListeners
                                 select eventListener(sender, e)).ToArray();

                    await TaskShim.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to invoke event handler in parallel");
                    throw;
                }
            }
            else
            {
                for (var i = 0; i < eventListeners.Length; i++)
                {
                    try
                    {
                        var eventListener = eventListeners[i];
                        await eventListener(sender, e);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Failed to invoke event handler at index '{i.ToString()}'");
                        throw;
                    }
                }
            }

            return true;
        }
    }
}

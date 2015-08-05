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

    /// <summary>
    /// Extensions for asynchronous event handlers.
    /// </summary>
    public static class AsyncEventHandlerExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Invokes the specified <paramref name="handler"/>
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        /// <typeparam name="TEventArgs">The type of the <see cref="EventArgs"/> class.</typeparam>
        /// <returns></returns>
        [ObsoleteEx(ReplacementTypeOrMember = "SafeInvokeAsync", TreatAsErrorFromVersion = "4.2", RemoveInVersion = "5.0")]
        public static Task<bool> SafeInvoke<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            return SafeInvokeAsync(handler, sender, e);
        }

        /// <summary>
        /// Invokes the specified <paramref name="handler"/>
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        /// <typeparam name="TEventArgs">The type of the <see cref="EventArgs"/> class.</typeparam>
        /// <returns></returns>
        public static async Task<bool> SafeInvokeAsync<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e)
            where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return false;
            }

            var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>().ToArray();
            for (var i = 0; i < eventListeners.Length; i++)
            {
                try
                {
                    var eventListener = eventListeners[i];
                    await eventListener(sender, e);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to invoke event handler at index '{0}'", i);
                    throw;
                }
            }

            return true;
        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncEventHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.EventHandlers.Extensions
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Extensions for asynchronous event handlers.
    /// </summary>
    public static class AsyncEventHandlerExtensions
    {
        /// <summary>
        /// Invokes the specified <paramref name="handler"/>
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        /// <typeparam name="TEventArgs">The type of the <see cref="EventArgs"/> class.</typeparam>
        /// <returns></returns>
        public static async Task<bool> SafeInvoke<TEventArgs>(this AsyncEventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs
        {
            if (handler == null)
            {
                return false;
            }

            var eventListeners = handler.GetInvocationList().Cast<AsyncEventHandler<TEventArgs>>();
            foreach (var eventListener in eventListeners)
            {
                await eventListener(sender, e);
            }

            return true;
        }
    }
}
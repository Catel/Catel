// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerExtensions.cs" company="Catel development team">
//   Copyright (c) 2011 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// Extensions for event handlers.
    /// </summary>
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Unsubscribes all the handlers from the specified event.
        /// </summary>
        /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
        /// <param name="handler">The handler.</param>
        public static void UnsubscribeAllHandlers<TEventArgs>(this EventHandler<TEventArgs> handler)
            where TEventArgs: EventArgs
        {
            if (handler != null)
            {
                var invocationList = handler.GetInvocationList();

                foreach (var d in invocationList)
                {
                    handler -= (EventHandler<TEventArgs>)d;
                }
            }
        }
    }
}

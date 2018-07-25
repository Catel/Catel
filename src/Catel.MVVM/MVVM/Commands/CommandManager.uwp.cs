// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManager.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.MVVM
{
    using System;
    using System.Runtime.CompilerServices;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using Logging;

    public partial class CommandManager
    {
        private readonly ConditionalWeakTable<FrameworkElement, CommandManagerWrapper> _subscribedViews = new ConditionalWeakTable<FrameworkElement, CommandManagerWrapper>();

        partial void SubscribeToKeyboardEventsInternal()
        {
            this.SubscribeToKeyboardEvents(Window.Current);
        }

        /// <summary>
        /// Subscribes to keyboard events.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="view"/> is <c>null</c>.</exception>
        public void SubscribeToKeyboardEvents(FrameworkElement view)
        {
            Argument.IsNotNull("view", view);

            if (!_subscribedViews.TryGetValue(view, out var commandManagerWrapper))
            {
                _subscribedViews.Add(view, new CommandManagerWrapper(view, this));
            }
        }
    }
}

#endif

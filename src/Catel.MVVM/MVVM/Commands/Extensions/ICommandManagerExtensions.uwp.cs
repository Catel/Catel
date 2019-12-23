// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandManagerExtensions.uwp.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if UWP

namespace Catel
{
    using System;
    using System.Runtime.CompilerServices;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using Logging;
    using MVVM;

    public partial class ICommandManagerExtensions
    {
        private static readonly ConditionalWeakTable<Window, ICommandManager> _commandManagerPerWindow = new ConditionalWeakTable<Window, ICommandManager>();

        /// <summary>
        /// Subscribes to keyboard events.
        /// <para />
        /// This is a convenience wrapper because Catel actually subscribes to the content of the window.
        /// </summary>
        /// <param name="commandManager">The command manager.</param>
        /// <param name="window">The view.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="commandManager"/> is <c>null</c>.</exception>
        public static void SubscribeToKeyboardEvents(this ICommandManager commandManager, Window window)
        {
            Argument.IsNotNull(nameof(commandManager), commandManager);

            if (window is null)
            {
                return;
            }

            var content = window?.Content as FrameworkElement;
            if (content is null)
            {
                if (_commandManagerPerWindow.TryGetValue(window, out var existingCommandManager))
                {
                    return;
                }

                Log.Warning($"Cannot subscribe to window '{window.GetType().Name}', content is not a FrameworkElement, will delay the subscription of events");

                _commandManagerPerWindow.Add(window, commandManager);

                window.Activated += OnWindowActivated;
                return;
            }

            commandManager.SubscribeToKeyboardEvents(content);
        }

        private static void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            var window = (Window)sender;
            window.Activated -= OnWindowActivated;

            if (_commandManagerPerWindow.TryGetValue(window, out var commandManager))
            {
                _commandManagerPerWindow.Remove(window);

                Log.Debug($"Window '{window.GetType().Name}' has been activated, retrying the keyboard subscriptions");

                SubscribeToKeyboardEvents(commandManager, window);
            }
            else
            {
                Log.Error($"Window '{window.GetType().Name}' has been activated, but could not find an ICommandManager registration in the temporary dictionary");
            }
        }
    }
}

#endif

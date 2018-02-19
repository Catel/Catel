// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandManagerExtensions.uwp.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


#if NETFX_CORE

namespace Catel
{
    using System;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using Logging;
    using MVVM;

    public partial class ICommandManagerExtensions
    {
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
            Argument.IsNotNull(() => commandManager);

            if (window == null)
            {
                return;
            }

            var content = window?.Content as FrameworkElement;
            if (content == null)
            {
                Log.Warning($"Cannot subscribe to window '{window.GetType().Name}', content is not a FrameworkElement");
                return;
            }

            commandManager.SubscribeToKeyboardEvents(content);
        }
    }
}

#endif
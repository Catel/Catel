// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManagerWrapper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !WINDOWS_PHONE

namespace Catel.MVVM
{
    using Catel.IoC;
    using Logging;
#if !WINDOWS_PHONE && !XAMARIN
    using InputGesture = Catel.Windows.Input.InputGesture;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using KeyEventArgs = global::Windows.UI.Xaml.Input.KeyRoutedEventArgs;
#else
    using System.Windows;
    using KeyEventArgs = System.Windows.Input.KeyEventArgs;
    using System;
#endif

#endif

    /// <summary>
    /// Wrapper class to support key down events and automatically invoke commands on the <see cref="ICommandManager" />.
    /// </summary>
    public class CommandManagerWrapper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ICommandManager _commandManager;

        private bool _subscribed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandManagerWrapper" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="commandManager">The command manager.</param>
        public CommandManagerWrapper(FrameworkElement view, ICommandManager commandManager = null)
        {
            Argument.IsNotNull(() => view);

            _commandManager = commandManager ?? ServiceLocator.Default.ResolveType<ICommandManager>();

            View = view;

            if (this.SubscribeToWeakGenericEvent<RoutedEventArgs>(view, "Loaded", OnViewLoaded, false) == null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Loaded', going to subscribe without weak events");

                view.Loaded += OnViewLoaded;
            }

            if (this.SubscribeToWeakGenericEvent<RoutedEventArgs>(view, "Unloaded", OnViewUnloaded, false) == null)
            {
                Log.Debug("Failed to use weak events to subscribe to 'view.Unloaded', going to subscribe without weak events");

                view.Unloaded += OnViewUnloaded;
            }

            Subscribe();
        }

        /// <summary>
        /// Gets the view.
        /// </summary>
        /// <value>The view.</value>
        protected FrameworkElement View { get; private set; }

        private void Subscribe()
        {
            if (_subscribed)
            {
                return;
            }

#if NET
            View.PreviewKeyDown += OnKeyDown;
#else
            View.KeyDown += OnKeyDown;
#endif

            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed)
            {
                return;
            }

#if NET
            View.PreviewKeyDown -= OnKeyDown;
#else
            View.KeyDown -= OnKeyDown;
#endif

            _subscribed = false;
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            Subscribe();
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            Unsubscribe();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Handled)
            {
                // Don't get in the way of already handled KeyDown events
                return;
            }

            // TODO: consider caching or something like that
            var commandNames = _commandManager.GetCommands();

            foreach (var commandName in commandNames)
            {
                bool keyHandled = false;

                var inputGesture = _commandManager.GetInputGesture(commandName);
                if (inputGesture != null)
                {
                    if (inputGesture.Matches(e))
                    {
                        keyHandled = true;
                        _commandManager.ExecuteCommand(commandName);
                        break;
                    }
                }

                if (keyHandled)
                {
                    e.Handled = true;
                }
            }
        }
    }
}

#endif

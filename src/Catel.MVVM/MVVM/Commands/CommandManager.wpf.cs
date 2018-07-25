// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandManager.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2018 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.MVVM
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;
    using Logging;

    public partial class CommandManager
    {
        private readonly ConditionalWeakTable<FrameworkElement, CommandManagerWrapper> _subscribedViews = new ConditionalWeakTable<FrameworkElement, CommandManagerWrapper>();

        private bool _subscribedToApplicationActivedEvent;

        partial void SubscribeToKeyboardEventsInternal()
        {
            var application = Application.Current;
            if (application == null)
            {
                Log.Warning("Application.Current is null, cannot subscribe to keyboard events");
                return;
            }

            FrameworkElement mainView = application.MainWindow;
            if (mainView == null)
            {
                if (!_subscribedToApplicationActivedEvent)
                {
                    application.Activated += (sender, e) => SubscribeToKeyboardEvents();
                    _subscribedToApplicationActivedEvent = true;
                    Log.Info("Application.MainWindow is null, cannot subscribe to keyboard events, subscribed to Application.Activated event");
                }

                return;
            }

            SubscribeToKeyboardEvents(mainView);
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
                // Note: also check for dispatcher, see https://github.com/Catel/Catel/issues/1205
                var app = Application.Current;
                var dispatcher = Dispatcher.CurrentDispatcher;
                if (app != null && ReferenceEquals(app.Dispatcher, dispatcher))
                {
                    _subscribedViews.Add(view, new CommandManagerWrapper(view, this));

                    var mainWindow = app.MainWindow;
                    if (ReferenceEquals(mainWindow, view))
                    {
                        EventManager.RegisterClassHandler(typeof(Window), Window.LoadedEvent, new RoutedEventHandler(OnWindowLoaded));
                    }
                }
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            var view = sender as FrameworkElement;
            if (view != null)
            {
                SubscribeToKeyboardEvents(view);
            }
        }
    }
}

#endif

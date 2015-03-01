// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dispatcherhelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN

namespace Catel.Windows.Threading
{
    using System;

#if NETFX_CORE
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;

    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
#else
    using System.Windows.Threading;
#endif

#if SILVERLIGHT
    using System.Windows;
#endif

    /// <summary>
    /// Dispatcher helper class.
    /// </summary>
    public static class DispatcherHelper
    {
        private static Dispatcher _dispatcher;

#if NETFX_CORE
        /// <summary>
        /// Checks the access of the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher"/> is <c>null</c>.</exception>
        public static bool CheckAccess(this CoreDispatcher dispatcher)
        {
            Argument.IsNotNull("dispatcher", dispatcher);

            return dispatcher.HasThreadAccess;
        }

        /// <summary>
        /// Begins the invocation of an action using the dispatcher.
        /// </summary>
        /// <param name="dispatcher">The dispatcher.</param>
        /// <param name="action">The action.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="dispatcher"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action"/> is <c>null</c>.</exception>
        public static void BeginInvoke(this CoreDispatcher dispatcher, Action action)
        {
            Argument.IsNotNull("dispatcher", dispatcher);
            Argument.IsNotNull("action", action);

            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }
#endif

#if NET
        private static readonly DispatcherOperationCallback exitFrameCallback = ExitFrame;

        /// <summary>
        /// Processes all UI messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {
            // Create new nested message pump.
            var nestedFrame = new DispatcherFrame();

            // Dispatch a callback to the current message queue, when getting called,
            // this callback will end the nested message loop.
            // note that the priority of this callback should be lower than that of UI event messages.
            DispatcherOperation exitOperation = Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background, exitFrameCallback, nestedFrame);

            // pump the nested message loop, the nested message loop will immediately
            // process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            // If the "exitFrame" callback is not finished, abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
            {
                exitOperation.Abort();
            }
        }

        private static object ExitFrame(object state)
        {
            var frame = state as DispatcherFrame;

            // Exit the nested message loop.
            frame.Continue = false;

            return null;
        }
#endif

        /// <summary>
        /// Gets the current dispatcher. This property is compatible with WPF, Silverlight, Windows Phone, etc, and also works
        /// when there is no application object (for example, during unit tests).
        /// </summary>
        /// <value>The current dispatcher.</value>
        public static Dispatcher CurrentDispatcher
        {
            get
            {
                if (_dispatcher == null)
                {
                    _dispatcher = GetCurrentDispatcher();
                }

                return _dispatcher;
            }
        }

        /// <summary>
        /// Gets the current dispatcher.
        /// </summary>
        /// <returns></returns>
        private static Dispatcher GetCurrentDispatcher()
        {
#if NET
            var currentApplication = System.Windows.Application.Current;
            if (currentApplication != null)
            {
                return currentApplication.Dispatcher;
            }
            
            return Dispatcher.CurrentDispatcher;
#elif NETFX_CORE
            var window = Window.Current;
            if (window != null)
            {
                return window.Dispatcher;
            }

            var coreWindow = CoreWindow.GetForCurrentThread();
            if (coreWindow != null)
            {
                return coreWindow.Dispatcher;
            }

            return null;
#else
            try
            {
                if ((Application.Current != null) && (Application.Current.RootVisual != null))
                {
                    return Application.Current.RootVisual.Dispatcher;
                }
            }
            catch (Exception)
            {
                // Possible when running unit tests, swallow
            }

            try
            {
                if (Deployment.Current != null)
                {
                    return Deployment.Current.Dispatcher;
                }
            }
            catch (Exception)
            {
                // Possible when running unit tests, swallow
            }

            return null;
#endif
        }
    }
}

#endif
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Dispatcherhelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !XAMARIN && !XAMARIN_FORMS

namespace Catel.Windows.Threading
{
    using System;
    
#if UWP
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using Dispatcher = global::Windows.UI.Core.CoreDispatcher;
    using System.Linq;
#else
    using System.Windows.Threading;
#endif

    /// <summary>
    /// Dispatcher helper class.
    /// </summary>
    public static class DispatcherHelper
    {
        private static Dispatcher _dispatcher;

#if UWP
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

#pragma warning disable 4014
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
#pragma warning restore 4014
        }
#endif

#if NET || NETCORE
        private static readonly DispatcherOperationCallback ExitFrameCallback = ExitFrame;

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
                DispatcherPriority.Background, ExitFrameCallback, nestedFrame);

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
        /// Gets the current dispatcher.
        /// </summary>
        /// <value>The current dispatcher.</value>
        public static Dispatcher CurrentDispatcher
        {
            get
            {
                if (_dispatcher is null)
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
#if NET || NETCORE
            var currentApplication = System.Windows.Application.Current;
            if (currentApplication != null)
            {
                return currentApplication.Dispatcher;
            }
            
            return Dispatcher.CurrentDispatcher;
#elif UWP
            var firstView = global::Windows.ApplicationModel.Core.CoreApplication.Views.FirstOrDefault();
            if (firstView != null)
            {
                return firstView.Dispatcher;
            }

            var window = Window.Current;
            if (window != null)
            {
                return window.Dispatcher;
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

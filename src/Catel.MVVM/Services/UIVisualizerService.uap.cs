#if UWP

namespace Catel.Services
{
    using System;
    using System.Threading.Tasks;
    using MVVM;
    using Logging;
    using Reflection;
    using Catel.Windows.Threading;
    using global::Windows.Foundation;
    using Threading;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;

    /// <summary>
    /// Service to show modal or non-modal popup windows.
    /// <para/>
    /// All windows will have to be registered manually or are be resolved via the <see cref="Catel.MVVM.IViewLocator"/>.
    /// </summary>
    public partial class UIVisualizerService
    {
        #region Methods
        /// <summary>
        /// This creates the window from a key.
        /// </summary>
        /// <param name="name">The name that the window is registered with.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>The created window.</returns>    
        protected virtual ContentDialog CreateWindow(string name, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            Type windowType;

            lock (RegisteredWindows)
            {
                if (!RegisteredWindows.TryGetValue(name, out windowType))
                {
                    return null;
                }
            }

            return CreateWindow(windowType, data, completedProc, isModal);
        }

        /// <summary>
        /// This creates the window of the specified type.
        /// </summary>
        /// <param name="windowType">The type of the window.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        /// <returns>The created window.</returns>
        protected virtual ContentDialog CreateWindow(Type windowType, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            var window = ViewHelper.ConstructViewWithViewModel(windowType, data) as ContentDialog;

            //if (isModal)
            //{
            //    var activeWindow = GetActiveWindow();
            //    if (window != activeWindow)
            //    {
            //        PropertyHelper.TrySetPropertyValue(window, "Owner", activeWindow, false);
            //    }
            //}

            if ((window != null) && (completedProc != null))
            {
                HandleCloseSubscription(window, data, completedProc, isModal);
            }

            return window;
        }

        /// <summary>
        /// Handles the close subscription.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="data">The data that will be set as data context.</param>
        /// <param name="completedProc">The completed callback.</param>
        /// <param name="isModal">True if this is a ShowDialog request.</param>
        protected virtual void HandleCloseSubscription(ContentDialog window, object data, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            TypedEventHandler<ContentDialog, ContentDialogClosedEventArgs> closed = null;
            closed = (sender, e) =>
            {
                bool? dialogResult = null;

                switch (e.Result)
                {
                    case ContentDialogResult.None:
                        dialogResult = null;
                        break;

                    case ContentDialogResult.Primary:
                        dialogResult = true;
                        break;

                    case ContentDialogResult.Secondary:
                        dialogResult = false;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                completedProc(this, new UICompletedEventArgs(data, isModal ? dialogResult : null));

                window.Closed -= closed;
            };

            window.Closed += closed;
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="data">The data.</param>
        /// <param name="showModal">If <c>true</c>, the window should be shown as modal.</param>
        /// <returns><c>true</c> if the window is closed with success; otherwise <c>false</c> or <c>null</c>.</returns>
        protected virtual async Task<UIVisualizerResult> ShowWindowAsync(ContentDialog window, object data, bool showModal)
        {
            // Note: no async/await because we use a TaskCompletionSource

            var result = await window.ShowAsync();
            bool? boolResult = null;

            switch (result)
            {
                case ContentDialogResult.None:
                    boolResult = showModal ? false : (bool?)null;

                case ContentDialogResult.Primary:
                    boolResult = true;

                case ContentDialogResult.Secondary:
                    boolResult = false;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new UIVisualizerResult(boolResult, data, window.DataContext);
        }
        #endregion
    }
}

#endif

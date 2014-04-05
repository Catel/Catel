// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.wp.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT
namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Navigation;
    using Catel.Windows;
    using Microsoft.Phone.Controls;

    public partial class NavigationAdapter
    {
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected PhoneApplicationFrame RootFrame { get; private set; }

        partial void Initialize()
        {
            RootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is PhoneApplicationFrame) as PhoneApplicationFrame;
            if (RootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<CancelEventArgs>(RootFrame, "BackKeyPress", OnBackKeyPress);
            this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(RootFrame, "Navigating", OnNavigatingEvent);
            this.SubscribeToWeakGenericEvent<NavigationEventArgs>(RootFrame, "Navigated", OnNavigatedEvent);
        }

        partial void Uninitialize()
        {
            // No need because we are using weak events for phone
            //RootFrame.BackKeyPress -= OnBackKeyPress;
            //RootFrame.Navigating -= OnNavigatingEvent;
            //RootFrame.Navigated -= OnNavigatedEvent;
        }

        partial void DetermineNavigationContext()
        {
            var page = GetNavigationTarget<PhoneApplicationPage>();
            var navigationContext = page.NavigationContext;

            if (navigationContext != null)
            {
                foreach (string key in navigationContext.QueryString.Keys)
                {
                    NavigationContext.Values[key] = navigationContext.QueryString[key];
                }
            }
        }

        /// <summary>
        /// Determines whether the navigation can be handled by this adapter.
        /// </summary>
        /// <returns><c>true</c> if the navigation can be handled by this adapter; otherwise, <c>false</c>.</returns>
        protected override bool CanHandleNavigation()
        {
            var content = RootFrame.Content;
            return ReferenceEquals(content, NavigationTarget);
        }

        /// <summary>
        /// Called when the <c>Navigated</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigationEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the RootFrame) when navigating away to prevent memory
        /// leaks.
        /// <para />
        /// Please, do not call this method yourself, otherwise you can seriously ruin your apps.
        /// </remarks>
        public void OnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            // We are navigating away
            var eventArgs = new NavigatingEventArgs(e.Uri.ToString(), e.NavigationMode.Convert());
            RaiseNavigatingAway(eventArgs);

            e.Cancel = eventArgs.Cancel;
        }

        /// <summary>
        /// Called when the <c>Navigating</c> event is invoked.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NavigatingCancelEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is public due to the fact that weak events are used. Otherwise, the navigation completed events
        /// could not be handled (because we unsubscribed from the RootFrame) when navigating away to prevent memory
        /// leaks.
        /// <para />
        /// Please, do not call this method yourself, otherwise you can seriously ruin your apps.
        /// </remarks>
        public void OnNavigatedEvent(object sender, NavigationEventArgs e)
        {
            var eventArgs = new NavigatedEventArgs(e.Uri.ToString(), NavigationMode.Unknown);
            HandleNavigatedEvent(eventArgs);
        }
    }
}
#endif
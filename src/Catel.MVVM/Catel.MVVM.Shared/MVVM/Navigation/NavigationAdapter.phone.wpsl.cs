// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.phone.wp.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT
namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Navigation;
    using Catel.Windows;
    using IoC;
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
            var rootFrame = NavigationRoot as PhoneApplicationFrame;
            if (rootFrame == null)
            {
                return;
            }

            //this.SubscribeToWeakGenericEvent<CancelEventArgs>(rootFrame, "BackKeyPress", OnBackKeyPress);
            this.SubscribeToWeakGenericEvent<NavigatingCancelEventArgs>(rootFrame, "Navigating", OnNavigatingEvent);
            this.SubscribeToWeakGenericEvent<NavigationEventArgs>(rootFrame, "Navigated", OnNavigatedEvent);
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
        /// Gets the navigation URI for the target page.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>System.String.</returns>
        protected override string GetNavigationUri(object target)
        {
            var dependencyResolver = this.GetDependencyResolver();
            var urlLocator = dependencyResolver.Resolve<IUrlLocator>();

            return urlLocator.ResolveUrl(NavigationTargetType);
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
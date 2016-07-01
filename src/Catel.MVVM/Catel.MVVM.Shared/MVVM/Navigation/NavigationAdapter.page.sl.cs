// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.sl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if SL5
namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Catel.Windows;
    using IoC;

    public partial class NavigationAdapter
    {
        private Frame RootFrame { get; set; }

        partial void Initialize()
        {
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame == null)
            {
                return;
            }

            rootFrame.Navigating += OnNavigatingEvent;
            rootFrame.Navigated += OnNavigatedEvent;
        }

        partial void Uninitialize()
        {
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame == null)
            {
                return;
            }

            rootFrame.Navigating -= OnNavigatingEvent;
            rootFrame.Navigated -= OnNavigatedEvent;
        }

        partial void DetermineNavigationContext()
        {
            var page = GetNavigationTarget<Page>();
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
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame == null)
            {
                return false;
            }

            var content = rootFrame.Content;
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

        private void OnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            // We are navigating away
            var eventArgs = new NavigatingEventArgs(e.Uri.ToString(), e.NavigationMode.Convert());
            RaiseNavigatingAway(eventArgs);

            e.Cancel = eventArgs.Cancel;
        }

        private void OnNavigatedEvent(object sender, NavigationEventArgs e)
        {
            var eventArgs = new NavigatedEventArgs(e.Uri.ToString(), NavigationMode.Unknown);
            HandleNavigatedEvent(eventArgs);
        }
    }
}
#endif
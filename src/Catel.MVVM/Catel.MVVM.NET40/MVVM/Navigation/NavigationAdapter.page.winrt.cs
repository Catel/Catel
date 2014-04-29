// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE
namespace Catel.MVVM.Navigation
{
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;

    public partial class NavigationAdapter
    {
        private object _lastNavigationContext;

        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected Frame RootFrame { get; private set; }

        partial void Initialize()
        {
            RootFrame = Window.Current.Content as Frame ?? ((Page)NavigationTarget).Frame;
            if (RootFrame == null)
            {
                return;
            }

            RootFrame.Navigating += OnNavigatingEvent;
            RootFrame.Navigated += OnNavigatedEvent;
        }

        partial void Uninitialize()
        {
            RootFrame.Navigating -= OnNavigatingEvent;
            RootFrame.Navigated -= OnNavigatedEvent;
        }

        partial void DetermineNavigationContext()
        {
            NavigationContext.Values["context"] = _lastNavigationContext;
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

        private void OnNavigatingEvent(object sender, NavigatingCancelEventArgs e)
        {
            var sourcePage = string.Empty;
            if (e.SourcePageType != null)
            {
                sourcePage = e.SourcePageType.ToString();
            }

            // We are navigating away
            var eventArgs = new NavigatingEventArgs(sourcePage, e.NavigationMode.Convert());
            RaiseNavigatingAway(eventArgs);

            e.Cancel = eventArgs.Cancel;
        }

        private void OnNavigatedEvent(object sender, NavigationEventArgs e)
        {
            var uriString = string.Empty;
            if (e.SourcePageType != null)
            {
                uriString = e.SourcePageType.ToString();
            }

            var eventArgs = new NavigatedEventArgs(uriString, NavigationMode.Unknown);
            HandleNavigatedEvent(eventArgs);

            _lastNavigationContext = e.Parameter;
        }
    }
}
#endif
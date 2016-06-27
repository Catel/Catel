// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE

namespace Catel.MVVM.Navigation
{
    using System.Collections.Generic;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Navigation;
    using IoC;

#if WINDOWS_PHONE
    using global::Windows.Phone.UI.Input;
#endif

    public partial class NavigationAdapter
    {
        private static Dictionary<string, object> _lastGlobalNavigationContext;
        private Dictionary<string, object> _lastNavigationContext;

        partial void Initialize()
        {
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame == null)
            {
                return;
            }

            rootFrame.Navigating += OnNavigatingEvent;
            rootFrame.Navigated += OnNavigatedEvent;

#if WINDOWS_PHONE
            HardwareButtons.BackPressed += OnBackPressed; 
#endif
        }

        partial void Uninitialize()
        {
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigating -= OnNavigatingEvent;
                rootFrame.Navigated -= OnNavigatedEvent;
            }

#if WINDOWS_PHONE
            HardwareButtons.BackPressed -= OnBackPressed; 
#endif
        }

#if WINDOWS_PHONE
        private void OnBackPressed(object sender, BackPressedEventArgs e) 
        { 
            var rootFrame = NavigationRoot as Frame;
            if (rootFrame != null && rootFrame.CanGoBack) 
            { 
                rootFrame.GoBack();
 
                // Indicate the back button press is handled so the app does not exit 
                e.Handled = true; 
            } 
        } 
#endif

        partial void DetermineNavigationContext()
        {
            if (_lastNavigationContext == null)
            {
                _lastNavigationContext = new Dictionary<string, object>();

                if (_lastGlobalNavigationContext != null)
                {
                    foreach (var value in _lastGlobalNavigationContext)
                    {
                        _lastNavigationContext[value.Key] = value.Value;
                    }
                }
            }

            foreach (var value in _lastNavigationContext)
            {
                NavigationContext.Values[value.Key] = value.Value;
            }
        }

        /// <summary>
        /// Determines whether the navigation can be handled by this adapter.
        /// </summary>
        /// <returns><c>true</c> if the navigation can be handled by this adapter; otherwise, <c>false</c>.</returns>
        protected override bool CanHandleNavigation()
        {
            InitializeNavigationService(false);

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

            _lastGlobalNavigationContext = e.Parameter as Dictionary<string, object>;
        }
    }
}
#endif
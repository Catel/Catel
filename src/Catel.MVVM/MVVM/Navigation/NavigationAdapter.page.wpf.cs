// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Catel.Logging;
    using IoC;
    using System.Collections.Generic;

    public partial class NavigationAdapter
    {
        private static Dictionary<string, object> _lastGlobalNavigationContext;
        private Dictionary<string, object> _lastNavigationContext;

        partial void Initialize()
        {
            HandleNavigatedOnLoaded = false;

            var navigationFrame = NavigationRoot as Frame;
            if (navigationFrame != null)
            {
                Log.Debug("Initializing navigation adapter using frame");

                navigationFrame.Navigating += OnNavigatingEvent;
                navigationFrame.Navigated += OnNavigatedEvent;
            }
            else
            {
                Log.Debug("Initializing navigation adapter using application");

                var app = Application.Current;

                app.Navigating += OnNavigatingEvent;
                app.Navigated += OnNavigatedEvent;
            }
        }

        partial void Uninitialize()
        {
            var navigationFrame = NavigationRoot as Frame;
            if (navigationFrame != null)
            {
                Log.Debug("Uninitializing navigation adapter using frame");

                navigationFrame.Navigating -= OnNavigatingEvent;
                navigationFrame.Navigated -= OnNavigatedEvent;
            }
            else
            {
                Log.Debug("Uninitializing navigation adapter using application");

                var app = Application.Current;

                app.Navigating -= OnNavigatingEvent;
                app.Navigated -= OnNavigatedEvent;
            }
        }

        partial void DetermineNavigationContext()
        {
            if (_lastNavigationContext is null)
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
            object content = null;

            var navigationFrame = NavigationRoot as Frame;
            if (navigationFrame != null)
            {
                content = navigationFrame.Content;
            }
            else
            {
                content = Application.Current.MainWindow.Content;
            }

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
            // CTL-906: clear current navication context if (re) navigating to the same view
            if (e.IsNavigationForView(NavigationTargetType))
            {
                _lastNavigationContext = null;
            }

            var sourceDictionary = e.ExtraData as Dictionary<string, object>;
            if (sourceDictionary is null)
            {
                sourceDictionary = new Dictionary<string, object>();
                sourceDictionary["context"] = e.ExtraData;
            }

            _lastGlobalNavigationContext = sourceDictionary;

            var eventArgs = new NavigatedEventArgs(e.Uri.ToString(), NavigationMode.Unknown);
            HandleNavigatedEvent(eventArgs);
        }
    }
}
#endif

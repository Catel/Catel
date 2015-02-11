﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.wpf.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET
namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Navigation;
    using Catel.Logging;
    using IoC;

    public partial class NavigationAdapter
    {
        private object _lastNavigationContext;

        partial void Initialize()
        {
            Application.Current.Navigating += OnNavigatingEvent;
            Application.Current.Navigated += OnNavigatedEvent;
        }

        partial void Uninitialize()
        {
            Application.Current.Navigating -= OnNavigatingEvent;
            Application.Current.Navigated -= OnNavigatedEvent;
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
            var content = Application.Current.MainWindow.Content;
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

            _lastNavigationContext = e.ExtraData;
        }
    }
}
#endif
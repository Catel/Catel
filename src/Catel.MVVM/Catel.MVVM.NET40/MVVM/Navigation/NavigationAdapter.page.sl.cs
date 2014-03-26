// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.sl.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if SL5
namespace Catel.MVVM.Navigation
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Catel.Windows;

    public partial class NavigationAdapter
    {
        private Frame RootFrame { get; set; }

        partial void Initialize()
        {
            RootFrame = Application.Current.RootVisual.FindVisualDescendant(e => e is Frame) as Frame;
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
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationAdapter.page.winrt.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE
namespace Catel.MVVM.Navigation
{
    public partial class NavigationAdapter
    {      
        /// <summary>
        /// Gets the root frame.
        /// </summary>
        /// <value>The root frame.</value>
        protected Frame RootFrame { get; private set; }

partial void Initialize()
{
            RootFrame = Window.Current.Content as Frame ?? ((Page)TargetView).Frame;
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
// TODO: Store just like WPF
            var navigationContext = e.Parameter;
        }
    }
}
#endif
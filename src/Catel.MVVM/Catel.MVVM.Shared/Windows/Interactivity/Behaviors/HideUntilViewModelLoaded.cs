﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HideUntilViewModelLoaded.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !WIN80 && !XAMARIN


namespace Catel.Windows.Interactivity
{
    using System;
    using Logging;
    using MVVM;
    using Reflection;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Hides the view until the view model is loaded.
    /// </summary>
    public class HideUntilViewModelLoaded : BehaviorBase<FrameworkElement>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer == null)
            {
                string error = string.Format("This behavior can only be used on IViewModelContainer classes, '{0}' does not implement; IViewModelContainer", AssociatedObject.GetType().GetSafeFullName());

                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            viewModelContainer.ViewModelChanged += OnViewModelChanged;

            UpdateVisibility();
        }

        /// <summary>
        /// Uninitializes this instance.
        /// </summary>
        protected override void Uninitialize()
        {
            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer != null)
            {
                viewModelContainer.ViewModelChanged -= OnViewModelChanged;
            }

            base.Uninitialize();
        }

        private void OnViewModelChanged(object sender, EventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            var viewModelContainer = AssociatedObject as IViewModelContainer;
            if (viewModelContainer != null)
            {
                AssociatedObject.Visibility = (viewModelContainer.ViewModel == null) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}

#endif
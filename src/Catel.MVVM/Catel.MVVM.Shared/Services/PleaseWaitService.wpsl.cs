// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if WINDOWS_PHONE && SILVERLIGHT

namespace Catel.Services
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using MVVM.Properties;
    using Reflection;

    public partial class PleaseWaitService
    {
        #region Fields
        private Popup _containerPopup;
        private Grid _grid;
        private TextBlock _statusTextBlock;
        private FrameworkElement _busyIndicator;
        #endregion

        partial void SetStatus(string status)
        {
            _statusTextBlock.Text = status;
        }

        partial void InitializeBusyIndicator()
        {
            if (_busyIndicator != null)
            {
                return;
            }

            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            _grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });

            var backgroundBorder = new Border();
            backgroundBorder.Background = new SolidColorBrush(Colors.Gray);
            backgroundBorder.Opacity = 0.5;
            backgroundBorder.SetValue(Grid.RowSpanProperty, 4);
            _grid.Children.Add(backgroundBorder);

            _statusTextBlock = new TextBlock();
            _statusTextBlock.Style = (Style)Application.Current.Resources["PhoneTextNormalStyle"];
            _statusTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _statusTextBlock.SetValue(Grid.RowProperty, 1);
            _grid.Children.Add(_statusTextBlock);

            _busyIndicator = ConstructBusyIndicator();
            _busyIndicator.SetValue(Grid.RowProperty, 2);
            _grid.Children.Add(_busyIndicator);

            _containerPopup = new Popup();
            _containerPopup.VerticalAlignment = VerticalAlignment.Center;
            _containerPopup.HorizontalAlignment = HorizontalAlignment.Center;
            _containerPopup.Child = _grid;

            double rootWidth = Application.Current.Host.Content.ActualWidth;
            double rootHeight = Application.Current.Host.Content.ActualHeight;

            _busyIndicator.Width = rootWidth;

            _grid.Width = rootWidth;
            _grid.Height = rootHeight;

            _containerPopup.UpdateLayout();

            return;
        }

        partial void HideBusyIndicator()
        {
            _busyIndicator.Visibility = Visibility.Collapsed;
            _containerPopup.IsOpen = false;

            var rootVisual = (FrameworkElement)Application.Current.RootVisual;
            if (rootVisual != null)
            {
                rootVisual.IsHitTestVisible = true;
            }
        }

        partial void ShowBusyIndicator()
        {
            // If equal and already visible, just exit
            if (_containerPopup.IsOpen)
            {
                return;
            }

            _containerPopup.IsOpen = true;

            PropertyHelper.TrySetPropertyValue(_busyIndicator, "IsIndeterminate", true);
            _busyIndicator.Visibility = Visibility.Visible;

            var rootVisual = (FrameworkElement)Application.Current.RootVisual;
            if (rootVisual != null)
            {
                rootVisual.UpdateLayout();
                rootVisual.IsHitTestVisible = false;
            }

            _containerPopup.UpdateLayout();
        }

        /// <summary>
        /// Constructors the busy indicator, which is by default the <see cref="ProgressBar"/>. However, it is wise
        /// to create a custom implementation and use the <c>PerformanceProgressBar</c> that can be found in the
        /// <a href="http://silverlight.codeplex.com/releases/view/75888">Windows Phone 7 toolkit</a>.
        /// <para />
        /// To create a custom implementation, create a new class based on this class and override only the
        /// <see cref="ConstructBusyIndicator"/> method.
        /// </summary>
        /// <returns>The busy indicator which will be used by this service.</returns>
        protected virtual FrameworkElement ConstructBusyIndicator()
        {
            return new ProgressBar();
        }
    }
}

#endif
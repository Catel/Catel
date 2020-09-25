#if UWP

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation

namespace Catel.Services
{
    using System;
    using Reflection;
    using Windows.Threading;
    using global::Windows.UI;
    using global::Windows.UI.Core;
    using global::Windows.UI.Xaml;
    using global::Windows.UI.Xaml.Controls;
    using global::Windows.UI.Xaml.Controls.Primitives;
    using global::Windows.UI.Xaml.Media;

    public partial class BusyIndicatorService
    {
#region Fields
        private FrameworkElement _busyIndicator;
        private Popup _containerPopup;
        private Grid _grid;
        private TextBlock _statusTextBlock;
#endregion

#region IPleaseWaitService Members
        partial void SetStatus(string status)
        {
#pragma warning disable 4014
            _busyIndicator.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (status is null)
                {
                    status = string.Empty;
                }

                if (_statusTextBlock != null)
                {
                    _statusTextBlock.Text = status;
                }
            });
#pragma warning restore 4014
        }

        partial void InitializeBusyIndicator()
        {
            if (_busyIndicator != null)
            {
                return;
            }

            _grid = new Grid();
            _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});
            _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Auto)});
            _grid.RowDefinitions.Add(new RowDefinition {Height = new GridLength(1, GridUnitType.Star)});

            var backgroundBorder = new Border();
            backgroundBorder.Background = new SolidColorBrush(Colors.Gray);
            backgroundBorder.Opacity = 0.5;
            backgroundBorder.SetValue(Grid.RowSpanProperty, 4);
            _grid.Children.Add(backgroundBorder);

            _statusTextBlock = new TextBlock();
            //_statusTextBlock.Style = (Style)Application.Current.Resources["PhoneTextNormalStyle"];
            _statusTextBlock.HorizontalAlignment = HorizontalAlignment.Center;
            _statusTextBlock.SetValue(Grid.RowProperty, 1);
            _grid.Children.Add(_statusTextBlock);

            _busyIndicator = ConstructorBusyIndicator();
            _busyIndicator.SetValue(Grid.RowProperty, 2);
            _grid.Children.Add(_busyIndicator);

            _containerPopup = new Popup();
            _containerPopup.VerticalAlignment = VerticalAlignment.Center;
            _containerPopup.HorizontalAlignment = HorizontalAlignment.Center;
            _containerPopup.Child = _grid;

            double rootWidth = Window.Current.Bounds.Width;
            double rootHeight = Window.Current.Bounds.Height;

            _busyIndicator.Width = rootWidth;

            _grid.Width = rootWidth;
            _grid.Height = rootHeight;

            _containerPopup.UpdateLayout();
        }

        partial void ShowBusyIndicator(bool indeterminate)
        {
            // If equal and already visible, just exit
            if (_containerPopup.IsOpen)
            {
                return;
            }

            _containerPopup.IsOpen = true;

            PropertyHelper.TrySetPropertyValue(_busyIndicator, "IsIndeterminate", indeterminate, false);
            _busyIndicator.Visibility = Visibility.Visible;

            var currentWindow = Window.Current;
            if (currentWindow != null)
            {
                var windowContent = currentWindow.Content as FrameworkElement;
                if (windowContent != null)
                {
                    windowContent.UpdateLayout();
                    windowContent.IsHitTestVisible = false;
                    //EffectsHelper.Dimm(windowContent);
                    windowContent.Opacity = 1;
                }
            }

            _containerPopup.UpdateLayout();
        }

        partial void HideBusyIndicator()
        {
            var dispatcher = DispatcherHelper.CurrentDispatcher;
            if (!dispatcher.HasThreadAccess)
            {
                dispatcher.BeginInvoke(Hide);
                return;
            }

            _busyIndicator.Visibility = Visibility.Collapsed;
            _containerPopup.IsOpen = false;

            var currentWindow = Window.Current;
            if (currentWindow != null)
            {
                var windowContent = currentWindow.Content as FrameworkElement;
                if (windowContent != null)
                {
                    windowContent.Opacity = 0;
                    //EffectsHelper.Undimm(windowContent);
                    windowContent.IsHitTestVisible = true;
                }
            }
        }

        /// <summary>
        /// Constructors the busy indicator, which is by default the <see cref="ProgressBar"/>.
        /// <para />
        /// To create a custom implementation, create a new class based on this class and override only the
        /// <see cref="ConstructorBusyIndicator"/> method.
        /// </summary>
        /// <returns>The busy indicator which will be used by this service.</returns>
        protected virtual FrameworkElement ConstructorBusyIndicator()
        {
            return new ProgressBar();
        }
#endregion
    }
}

#endif

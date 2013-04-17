// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.MVVM.Services
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using Properties;
    using Reflection;

    /// <summary>
    /// Windows phone 7 implementation of the please wait service.
    /// <para />
    /// By default,  the <see cref="ProgressBar"/>. However, it is wise to create a custom implementation and use the 
    /// <c>PerformanceProgressBar</c> that can be found in the <a href="http://silverlight.codeplex.com/releases/view/75888">Windows Phone 7 toolkit</a>.
    /// <para />
    /// To create a custom implementation, create a new class based on this class and override only the
    /// <see cref="ConstructorBusyIndicator"/> method.
    /// </summary>
    public class PleaseWaitService : IPleaseWaitService
    {
        #region Fields
        private int _showCounter;

        private Popup _containerPopup;
        private Grid _grid;
        private TextBlock _statusTextBlock;
        private FrameworkElement _busyIndicator;
        #endregion

        /// <summary>
        /// Shows the please wait window with the specified status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        /// <remarks>
        /// When this method is used, the <see cref="M:Catel.MVVM.Services.IPleaseWaitService.Hide"/> method must be called to hide the window again.
        /// </remarks>
        public void Show(string status = "")
        {
            if (string.IsNullOrEmpty(status))
            {
                status = Resources.PleaseWait;
            }

            UpdateStatus(status);

            ShowBusyIndicator();
        }

        /// <summary>
        /// Shows the please wait window with the specified status text and executes the work delegate (in a background thread). When the work
        /// is finished, the please wait window will be automatically closed.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        /// <remarks></remarks>
        public void Show(PleaseWaitWorkDelegate workDelegate, string status = "")
        {
            Argument.IsNotNull("workDelegate", workDelegate);

            if (!InitializeBusyIndicator())
            {
                return;
            }

            Show(status);

            workDelegate();

            Hide();
        }

        /// <summary>
        /// Updates the status text.
        /// </summary>
        /// <param name="status">The status. When the string is <c>null</c> or empty, the default please wait text will be used.</param>
        public void UpdateStatus(string status)
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            _statusTextBlock.Text = status;
        }

        /// <summary>
        /// Updates the status and shows a progress bar with the specified status text. The percentage will be automatically calculated.
        /// <para/>
        /// The busy indicator will automatically hide when the <paramref name="totalItems"/> is larger than <paramref name="currentItem"/>.
        /// <para/>
        /// When providing the <paramref name="statusFormat"/>, it is possible to use <c>{0}</c> (represents current item) and
        /// <c>{1}</c> (represents total items).
        /// </summary>
        /// <param name="currentItem">The current item.</param>
        /// <param name="totalItems">The total items.</param>
        /// <param name="statusFormat">The status format. Can be empty, but not <c>null</c>.</param>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref name="currentItem"/> is smaller than zero.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="statusFormat"/> is <c>null</c>.</exception>
        /// <remarks></remarks>
        public void UpdateStatus(int currentItem, int totalItems, string statusFormat = "")
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            if (currentItem > totalItems)
            {
                Hide();
                return;
            }

            UpdateStatus(string.Format(statusFormat, currentItem, totalItems));

            ShowBusyIndicator();
        }

        /// <summary>
        /// Hides this please wait window.
        /// </summary>
        public void Hide()
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            _busyIndicator.Visibility = Visibility.Collapsed;
            _containerPopup.IsOpen = false;

            var rootVisual = (FrameworkElement)Application.Current.RootVisual;
            if (rootVisual != null)
            {
                rootVisual.IsHitTestVisible = true;
            }

            _showCounter = 0;
        }

        /// <summary>
        /// Increases the number of clients that show the please wait window. The implementing class
        /// is responsible for holding a counter internally which a call to this method will increase.
        /// <para/>
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// decrease the counter, make a call to <see cref="Pop"/>.
        /// <para/>
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        /// <param name="status">The status to change the text to.</param>
        /// <remarks></remarks>
        public void Push(string status = "")
        {
            UpdateStatus(status);

            _showCounter++;

            if (_showCounter > 0)
            {
                Show();
            }
        }

        /// <summary>
        /// Decreases the number of clients that show the please wait window. The implementing class
        /// is responsible for holding a counter internally which a call to this method will decrease.
        /// <para/>
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// increase the counter, make a call to <see cref="Pop"/>.
        /// <para/>
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        /// <remarks></remarks>
        public void Pop()
        {
            if (_showCounter > 0)
            {
                _showCounter--;
            }

            if (_showCounter <= 0)
            {
                Hide();
            }
        }

        /// <summary>
        /// Shows the busy indicator with the specified mode.
        /// </summary>
        protected virtual void ShowBusyIndicator()
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

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
        /// <see cref="ConstructorBusyIndicator"/> method.
        /// </summary>
        /// <returns>The busy indicator which will be used by this service.</returns>
        protected virtual FrameworkElement ConstructorBusyIndicator()
        {
            return new ProgressBar();
        }

        /// <summary>
        /// Initializes the busy indicator by injecting it into the root visual.
        /// </summary>
        /// <returns><c>true</c> if the busy indicator is initialized successfully; otherwise <c>false</c>.</returns>
        protected virtual bool InitializeBusyIndicator()
        {
            if (_busyIndicator != null)
            {
                return true;
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

            _busyIndicator = ConstructorBusyIndicator();
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

            return true;
        }
    }
}
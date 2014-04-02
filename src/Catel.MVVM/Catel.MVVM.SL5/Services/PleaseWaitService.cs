// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Services
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Data;
    using MVVM.Properties;
    using Popup = System.Windows.Controls.Primitives.Popup;

    /// <summary>
    /// Please wait service to show a please wait window during background activities. This service uses the <see cref="System.Windows.Controls.BusyIndicator"/>
    /// for the actual displaying of the please wait status to the user.
    /// </summary>
    public class PleaseWaitService : IPleaseWaitService
    {
        #region Classes
        /// <summary>
        /// BusyIndicatorDataContext Data object class which fully supports serialization, property changed notifications,
        /// backwards compatibility and error checking.
        /// </summary>
        public class BusyIndicatorDataContext : ModelBase
        {
            /// <summary>
            /// Gets or sets the progress in percentages.
            /// </summary>
            public int Percentage
            {
                get { return GetValue<int>(PercentageProperty); }
                set { SetValue(PercentageProperty, value); }
            }

            /// <summary>
            /// Register the Percentage property so it is known in the class.
            /// </summary>
            public static readonly PropertyData PercentageProperty = RegisterProperty("Percentage", typeof(int), 0);

            /// <summary>
            /// Gets or sets a value whether the busy indicator is busy.
            /// </summary>
            public bool IsBusy
            {
                get { return GetValue<bool>(IsBusyProperty); }
                set { SetValue(IsBusyProperty, value); }
            }

            /// <summary>
            /// Register the IsBusy property so it is known in the class.
            /// </summary>
            public static readonly PropertyData IsBusyProperty = RegisterProperty("IsBusy", typeof(bool), false);

            /// <summary>
            /// Gets the status.
            /// </summary>
            public string Status
            {
                get { return GetValue<string>(StatusProperty); }
                set { SetValue(StatusProperty, value); }
            }

            /// <summary>
            /// Register the Status property so it is known in the class.
            /// </summary>
            public static readonly PropertyData StatusProperty = RegisterProperty("Status", typeof(string), string.Empty);
        }
        #endregion

        #region Constants
        private const double DefaultBusyIndicatorWidth = 154;
        private const double DefaultBusyIndicatorHeight = 59;
        #endregion

        #region Fields
        private int _showCounter;
        private Popup _containerPopup;
        private readonly WeakReference _parent;

        /// <summary>
        /// The busy indicator.
        /// </summary>
        protected FrameworkElement BusyIndicator;

        private readonly BusyIndicatorDataContext _busyIndicatorDataContext = new BusyIndicatorDataContext();

        private int _skipLayoutUpdateCount;
        private bool _isIndeterminate = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitService" /> class.
        /// </summary>
        public PleaseWaitService()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PleaseWaitService" /> class.
        /// </summary>
        /// <param name="parentElement">The parent element. If <c>null</c>, the application root will be used.</param>
        public PleaseWaitService(FrameworkElement parentElement)
        {
            _parent = new WeakReference(parentElement ?? Application.Current.RootVisual as FrameworkElement);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Shows the please wait window with the specified status text.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <remarks>
        /// When this method is used, the <see cref="Hide"/> method must be called to hide the window again.
        /// </remarks>
        public virtual void Show(string status = "")
        {
            if (string.IsNullOrEmpty(status))
            {
                status = Resources.PleaseWait;
            }

            UpdateStatus(status);

            Show(true);
        }

        /// <summary>
        /// Shows the please wait window with the specified status text and executes the work delegate (in a background thread). When the work 
        /// is finished, the please wait window will be automatically closed.
        /// </summary>
        /// <param name="workDelegate">The work delegate.</param>
        /// <param name="status">The status.</param>
        public virtual void Show(PleaseWaitWorkDelegate workDelegate, string status = "")
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
        /// <param name="status">The status.</param>
        public virtual void UpdateStatus(string status)
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            _busyIndicatorDataContext.Status = status;
        }

        /// <summary>
        /// Updates the status and shows a progress bar with the specified status text. The percentage will be automatically calculated.
        /// <para />
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
        public virtual void UpdateStatus(int currentItem, int totalItems, string statusFormat = "")
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
            _busyIndicatorDataContext.Percentage = (100 / totalItems) * currentItem;

            Show(false);
        }

        /// <summary>
        /// Hides this please wait window.
        /// </summary>
        public virtual void Hide()
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            _busyIndicatorDataContext.IsBusy = false;
            _containerPopup.IsOpen = false;

            var parent = _parent.Target as FrameworkElement;
            if (parent != null)
            {
                parent.IsHitTestVisible = true;
            }

            _showCounter = 0;
        }

        /// <summary>
        /// Increases the number of clients that show the please wait window. The implementing class 
        /// is responsible for holding a counter internally which a call to this method will increase.
        /// <para />
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// decrease the counter, make a call to <see cref="Pop"/>.
        /// <para />
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        public virtual void Push(string status = "")
        {
            UpdateStatus(status);

            _showCounter++;

            if (_showCounter > 0)
            {
                Show(status);
            }
        }

        /// <summary>
        /// Decreases the number of clients that show the please wait window. The implementing class 
        /// is responsible for holding a counter internally which a call to this method will decrease.
        /// <para />
        /// As long as the internal counter is not zero (0), the please wait window will stay visible. To
        /// increase the counter, make a call to <see cref="Pop"/>.
        /// <para />
        /// A call to <see cref="Show(string)"/> or one of its overloads will not increase the internal counter. A
        /// call to <see cref="Hide"/> will reset the internal counter to zero (0) and thus hide the window.
        /// </summary>
        public virtual void Pop()
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
        /// <param name="indeterminate">if set to <c>true</c>, the indeterminate template is loaded. Otherwise, the determinate template is loaded.</param>
        protected virtual void Show(bool indeterminate)
        {
            if (!InitializeBusyIndicator())
            {
                return;
            }

            // If equal and already visible, just exit
            if ((indeterminate == _isIndeterminate) && (_containerPopup.IsOpen))
            {
                return;
            }

            var busyIndicator = BusyIndicator as BusyIndicator;
            if (busyIndicator != null)
            {
                if (indeterminate != _isIndeterminate)
                {
                    if (indeterminate)
                    {
                        busyIndicator.ProgressBarStyle = Application.Current.Resources["busyIndicatorIndeterminateProgressBar"] as Style;
                    }
                    else
                    {
                        busyIndicator.ProgressBarStyle = Application.Current.Resources["busyIndicatorDeterminateProgressBar"] as Style;
                    }

                    _isIndeterminate = indeterminate;
                }
            }

            _containerPopup.IsOpen = true;
            _busyIndicatorDataContext.IsBusy = true;

            var parent = _parent.Target as FrameworkElement;
            if (parent != null)
            {
                try
                {
                    parent.UpdateLayout();
                }
                catch (LayoutCycleException)
                {
                    // Ignore
                }

                parent.IsHitTestVisible = false;
            }

            //_containerPopup.UpdateLayout();
        }

        /// <summary>
        /// Creates the busy indicator, which will be automatically centered if needed.
        /// </summary>
        /// <returns>FrameworkElement.</returns>
        protected virtual FrameworkElement CreateBusyIndicator()
        {
            var busyIndicator = new BusyIndicator();

            busyIndicator.SetBinding(System.Windows.Controls.BusyIndicator.BusyContentProperty, new Binding("Status"));
            busyIndicator.SetBinding(System.Windows.Controls.BusyIndicator.IsBusyProperty, new Binding("IsBusy"));

            return busyIndicator;
        }

        /// <summary>
        /// Initializes the busy indicator by injecting it into the root visual.
        /// </summary>
        /// <returns><c>true</c> if the busy indicator is initialized successfully; otherwise <c>false</c>.</returns>
        protected virtual bool InitializeBusyIndicator()
        {
            if (BusyIndicator != null)
            {
                return true;
            }

            BusyIndicator = CreateBusyIndicator();
            BusyIndicator.DataContext = _busyIndicatorDataContext;

            _containerPopup = new Popup();
            _containerPopup.VerticalAlignment = VerticalAlignment.Center;
            _containerPopup.HorizontalAlignment = HorizontalAlignment.Center;
            _containerPopup.Child = BusyIndicator;

            _containerPopup.LayoutUpdated += (sender, e) =>
            {
                if (Application.Current == null)
                {
                    return;
                }

                // 1 is the default old behavior but since we set both horizontal and vertical offset, we might
                // be causing a "Layout cycle"
                _skipLayoutUpdateCount++;
                if (_skipLayoutUpdateCount < 2)
                {
                    return;
                }

                _skipLayoutUpdateCount = 0;

                var parent = _parent.Target as FrameworkElement;
                if (parent == null)
                {
                    return;
                }

                try
                {
                    if (parent.ActualHeight == 0.0d && parent.ActualWidth == 0.0d)
                    {
                        Hide();
                        return;
                    }

                    var transform = parent.TransformToVisual(Application.Current.RootVisual);
                    var offset = transform.Transform(new Point(0, 0));
                    double parentTop = offset.Y;
                    double parentLeft = offset.X;

                    // Allow fallback to root (Host.Content)
                    double parentWidth = Application.Current.Host.Content.ActualWidth;
                    double parentHeight = Application.Current.Host.Content.ActualHeight;

                    if (_parent != null)
                    {
                        parentWidth = (parent.ActualWidth > 0) ? parent.ActualWidth : parentWidth;
                        parentHeight = (parent.ActualHeight > 0) ? parent.ActualHeight : parentHeight;
                    }

                    BusyIndicator.Measure(new System.Windows.Size(parentWidth, parentHeight));
                    var busyIndicatorSize = BusyIndicator.DesiredSize;

                    double indicatorWidth = (busyIndicatorSize.Width > 0) ? busyIndicatorSize.Width : DefaultBusyIndicatorWidth;
                    double indicatorHeight = (busyIndicatorSize.Height > 0) ? busyIndicatorSize.Height : DefaultBusyIndicatorHeight;

                    _containerPopup.HorizontalOffset = parentLeft + ((parentWidth / 2) - (indicatorWidth / 2));
                    _containerPopup.VerticalOffset = parentTop + ((parentHeight / 2) - (indicatorHeight / 2));
                }
                catch (Exception)
                {
                    // Ignore
                }
            };

            return true;
        }
        #endregion
    }
}

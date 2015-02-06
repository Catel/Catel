// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PleaseWaitService.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if SL5

namespace Catel.Services
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using Data;
    using Popup = System.Windows.Controls.Primitives.Popup;

    public partial class PleaseWaitService
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
        private Popup _containerPopup;
        private WeakReference _parent = new WeakReference(Application.Current.RootVisual as FrameworkElement);

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
        /// <param name="parentElement">The parent element. If <c>null</c>, the application root will be used.</param>
        /// <param name="languageService">The language service.</param>
        public PleaseWaitService(FrameworkElement parentElement, ILanguageService languageService)
            : this(languageService)
        {
            _parent = new WeakReference(parentElement ?? Application.Current.RootVisual as FrameworkElement);
        }
        #endregion

        #region Methods
        partial void SetStatus(string status)
        {
            _busyIndicatorDataContext.Status = status;
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

        partial void InitializeBusyIndicator()
        {
            if (BusyIndicator != null)
            {
                return;
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

            return;
        }

        partial void ShowBusyIndicator(bool indeterminate)
        {
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
        }

        partial void HideBusyIndicator()
        {
            _busyIndicatorDataContext.IsBusy = false;
            _containerPopup.IsOpen = false;

            var parent = _parent.Target as FrameworkElement;
            if (parent != null)
            {
                parent.IsHitTestVisible = true;
            }
        }
        #endregion
    }
}

#endif
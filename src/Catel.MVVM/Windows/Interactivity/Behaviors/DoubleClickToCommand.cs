// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleClickToCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Logging;

#if UWP
    using global::Windows.UI.Xaml;
    using TimerTickEventArgs = System.Object;
#else
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using TimerTickEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// This behavior allows any element that supports a double click to command for every element
    /// that supports <c>MouseLeftButtonDown</c>.
    /// </summary>
    public class DoubleClickToCommand : CommandBehaviorBase<FrameworkElement>
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly DispatcherTimer _timer;

        private readonly Action _action;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleClickToCommand"/> class.
        /// </summary>
        public DoubleClickToCommand()
            : this(null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoubleClickToCommand"/> class.
        /// </summary>
        /// <param name="action">The action to execute on double click. This is very useful when the behavior is added
        /// via code and an action must be invoked instead of a command.</param>
        /// <param name="doubleClickMilliseconds">The double click acceptance window in milliseconds.</param>
        public DoubleClickToCommand(Action action, int doubleClickMilliseconds = 500)
        {
            if (doubleClickMilliseconds < 0)
            {
                doubleClickMilliseconds = 50;
            }

            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, doubleClickMilliseconds)
            };

            _timer.Tick += OnTimerTick;

            _action = action;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to automatically fix the ItemTemplate in a ListBox.
        /// </summary>
        /// <value>
        /// <c>true</c> if the ItemTemplate in a ListBox should automatically be fixed; otherwise, <c>false</c>.
        /// </value>
        public bool AutoFixListBoxItemTemplate
        {
            get { return (bool)GetValue(AutoFixListBoxItemTemplateProperty); }
            set { SetValue(AutoFixListBoxItemTemplateProperty, value); }
        }

        /// <summary>
        /// The property definition for the <see cref="AutoFixListBoxItemTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoFixListBoxItemTemplateProperty =
            DependencyProperty.Register("AutoFixListBoxItemTemplate", typeof(bool), typeof(DoubleClickToCommand), new PropertyMetadata(true));
        #endregion

        #region Methods
        /// <summary>
        /// Gets the hit elements.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns>Enumerable of hit elements.</returns>
        protected virtual IEnumerable<UIElement> GetHitElements(Point mousePosition)
        {
            var element = AssociatedObject as UIElement;
            if (element is null)
            {
                return Enumerable.Empty<UIElement>();
            }

#if SILVERLIGHT
            var mousePositionOffset = element.TransformToVisual(Application.Current.RootVisual).Transform(mousePosition);
            return VisualTreeHelper.FindElementsInHostCoordinates(mousePositionOffset, element);
#else

            var elements = new List<UIElement>();
            VisualTreeHelper.HitTest(element, null, hit =>
                    {
                        if (hit.VisualHit is UIElement)
                        {
                            elements.Add((UIElement)hit.VisualHit);
                        }

                        return HitTestResultBehavior.Continue;
                    }, new PointHitTestParameters(mousePosition));

            return elements;
#endif
        }

        /// <summary>
        /// Determines whether the element is hit.
        /// </summary>
        /// <param name="mousePosition">The mouse position.</param>
        /// <returns><c>true</c> if the element is hit at the mouse position; otherwise, <c>false</c>.</returns>
        protected virtual bool IsElementHit(Point mousePosition)
        {
            if (AssociatedObject is DataGrid)
            {
                return GetHitElements(mousePosition).OfType<DataGridRow>().FirstOrDefault() != null;
            }

            return true;
        }

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true);

            if (AutoFixListBoxItemTemplate)
            {
                var associatedObjectAsGrid = AssociatedObject as Grid;
                if (associatedObjectAsGrid is null)
                {
                    Log.Debug("AutoFixListBoxItemTemplate is set to true, but AssociatedObject is not a grid so no action will be taken");
                    return;
                }

                if (associatedObjectAsGrid.Background is null)
                {
                    associatedObjectAsGrid.Background = new SolidColorBrush(Colors.Transparent);
                }

                var contentPresenter = VisualTreeHelper.GetParent(associatedObjectAsGrid) as ContentPresenter;
                if (contentPresenter != null)
                {
                    Log.Debug("AutoFixListBoxItemTemplate is set to true, setting the HorizontalAlignment of the parent to Stretch");
                    contentPresenter.HorizontalAlignment = HorizontalAlignment.Stretch;
                }
            }
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(OnMouseButtonDown));

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Called when the <see cref="UIElement.MouseLeftButtonDown"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args instance containing the event data.</param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!IsEnabled)
            {
                return;
            }

            if (!_timer.IsEnabled)
            {
                _timer.Start();
                return;
            }

            _timer.Stop();

            if (IsElementHit(e.GetPosition(AssociatedObject)))
            {
                if (_action != null)
                {
                    Log.Debug("Executing action");

                    _action();
                }
                else
                {
                    ExecuteCommand();
                }
            }
        }

        /// <summary>
        /// Called when the <see cref="DispatcherTimer.Tick"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, TimerTickEventArgs e)
        {
            _timer.Stop();
        }
        #endregion
    }
}

#endif

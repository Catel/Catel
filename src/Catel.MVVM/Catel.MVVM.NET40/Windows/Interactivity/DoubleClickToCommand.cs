// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoubleClickToCommand.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Logging;

#if !NET
    using System.Windows.Input;
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
        public DoubleClickToCommand(Action action)
        {
            _timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 0, 0, 200)
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
        /// 	<c>true</c> if the ItemTemplate in a ListBox should automatically be fixed; otherwise, <c>false</c>.
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
        /// Called when the associated object is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
#if NET
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(OnMouseButtonDown), true);
#else
            AssociatedObject.MouseLeftButtonDown += OnMouseButtonDown;
#endif

            if (AutoFixListBoxItemTemplate)
            {
                var associatedObjectAsGrid = AssociatedObject as Grid;
                if (associatedObjectAsGrid == null)
                {
                    Log.Debug("AutoFixListBoxItemTemplate is set to true, but AssociatedObject is not a grid so no action will be taken");
                    return;
                }

                if (associatedObjectAsGrid.Background == null)
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
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
#if NET
            AssociatedObject.RemoveHandler(UIElement.MouseLeftButtonDownEvent, new RoutedEventHandler(OnMouseButtonDown));
#else
            AssociatedObject.MouseLeftButtonDown -= OnMouseButtonDown;
#endif

            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
        }

#if NET
        /// <summary>
        /// Called when the <see cref="UIElement.MouseLeftButtonDown"/> occurs.
        /// </summary>
        private void OnMouseButtonDown(object sender, RoutedEventArgs e)
#else
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
#endif
        {
            if (!_timer.IsEnabled)
            {
                _timer.Start();
                return;
            }

            _timer.Stop();

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

        /// <summary>
        /// Called when the <see cref="DispatcherTimer.Tick"/> occurs.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
        }
        #endregion
    }
}

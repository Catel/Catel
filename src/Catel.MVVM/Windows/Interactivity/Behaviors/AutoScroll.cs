﻿namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Xaml.Behaviors;
    using Data;

    /// <summary>
    /// The scroll direction.
    /// </summary>
    public enum ScrollDirection
    {
        /// <summary>
        /// Scroll to top.
        /// </summary>
        Top,

        /// <summary>
        /// Scroll to bottom.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Automatically scrolls to the bottom when the scrollbar is at the bottom.
    /// </summary>
    public class AutoScroll : BehaviorBase<ItemsControl>
    {
        private bool _isScrollbarAtEnd;
        private ScrollViewer? _scrollViewer;
        private INotifyCollectionChanged? _collection;

        /// <summary>
        /// A boolean that determines whether the behavior should automatically scroll as soon as the 
        /// control is loaded.
        /// <para />
        /// The default value is <c>true</c>.
        /// </summary>
        public bool ScrollOnLoaded
        {
            get { return (bool)GetValue(ScrollOnLoadedProperty); }
            set { SetValue(ScrollOnLoadedProperty, value); }
        }

        /// <summary>
        /// The scroll on loaded property.
        /// </summary>
        public static readonly DependencyProperty ScrollOnLoadedProperty =
            DependencyProperty.Register(nameof(ScrollOnLoaded), typeof(bool), typeof(AutoScroll), new PropertyMetadata(true));

        /// <summary>
        /// The scroll direction.
        /// <para />
        /// The default value is <see cref="Catel.Windows.Interactivity.ScrollDirection.Bottom"/>.
        /// </summary>
        public ScrollDirection ScrollDirection
        {
            get { return (ScrollDirection)GetValue(ScrollDirectionProperty); }
            set { SetValue(ScrollDirectionProperty, value); }
        }

        /// <summary>
        /// The scroll direction property.
        /// </summary>
        public static readonly DependencyProperty ScrollDirectionProperty =
            DependencyProperty.Register(nameof(ScrollDirection), typeof(ScrollDirection), typeof(AutoScroll), new PropertyMetadata(ScrollDirection.Bottom));

        /// <summary>
        /// The scroll threshold in which the behavior will scroll down even when it is not fully down.
        /// <para />
        /// The default value is <c>5</c>.
        /// </summary>
        public int ScrollThreshold
        {
            get { return (int)GetValue(ScrollThresholdProperty); }
            set { SetValue(ScrollThresholdProperty, value); }
        }

        /// <summary>
        /// The scroll threshold property.
        /// </summary>
        public static readonly DependencyProperty ScrollThresholdProperty =
            DependencyProperty.Register(nameof(ScrollThreshold), typeof(int), typeof(AutoScroll), new PropertyMetadata(5));

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.SubscribeToDependencyProperty("ItemsSource", OnItemsSourceChanged);

            SubscribeToCollection();

            _scrollViewer = AssociatedObject.FindVisualDescendantByType<ScrollViewer>();
            if (_scrollViewer is not null)
            {
                _scrollViewer.ScrollChanged += OnScrollChanged;
            }

            base.OnAssociatedObjectLoaded();
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.UnsubscribeFromDependencyProperty("ItemsSource", OnItemsSourceChanged);

            UnsubscribeFromCollection();

            base.OnAssociatedObjectUnloaded();
        }

        private void OnItemsSourceChanged(object? sender, DependencyPropertyValueChangedEventArgs e)
        {
            UnsubscribeFromCollection();
            SubscribeToCollection();
        }

        private void UnsubscribeFromCollection()
        {
            if (_collection is not null)
            {
                _collection.CollectionChanged -= OnCollectionChanged;
                _collection = null;
            }
        }

        private void SubscribeToCollection()
        {
            if (IsAssociatedObjectLoaded)
            {
                _collection = AssociatedObject.ItemsSource as INotifyCollectionChanged;
                if (_collection is not null)
                {
                    _collection.CollectionChanged += OnCollectionChanged;

                    if (ScrollOnLoaded)
                    {
                        ScrollToEnd();
                    }
                }
            }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isScrollbarAtEnd)
            {
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            if (_scrollViewer is null)
            {
                return;
            }

            if (!IsEnabled)
            {
                return;
            }

            switch (ScrollDirection)
            {
                case ScrollDirection.Top:
                    _scrollViewer.ScrollToTop();
                    break;

                case ScrollDirection.Bottom:
                    _scrollViewer.ScrollToBottom();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if (_scrollViewer is null)
            {
                return;
            }

            switch (ScrollDirection)
            {
                case ScrollDirection.Top:
                    _isScrollbarAtEnd = e.VerticalOffset <= ScrollThreshold;
                    break;

                case ScrollDirection.Bottom:
                    _isScrollbarAtEnd = e.VerticalOffset >= _scrollViewer.ScrollableHeight - ScrollThreshold;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

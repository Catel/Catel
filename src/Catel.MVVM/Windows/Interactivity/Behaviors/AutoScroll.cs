// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollToBottom.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET || NETCORE

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
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
        private ScrollViewer _scrollViewer;
        private INotifyCollectionChanged _collection;

        #region Properties
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
            DependencyProperty.Register("ScrollOnLoaded", typeof(bool), typeof(AutoScroll), new PropertyMetadata(true));

        /// <summary>
        /// The scoll direction.
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
            DependencyProperty.Register("ScrollDirection", typeof(ScrollDirection), typeof(AutoScroll), new PropertyMetadata(ScrollDirection.Bottom));

        /// <summary>
        /// The scoll threshold in which the behavior will scroll down even when it is not fully down.
        /// <para />
        /// The default value is <c>5</c>.
        /// </summary>
        public int ScrollTreshold
        {
            get { return (int)GetValue(ScrollTresholdProperty); }
            set { SetValue(ScrollTresholdProperty, value); }
        }

        /// <summary>
        /// The scroll treshold property.
        /// </summary>
        public static readonly DependencyProperty ScrollTresholdProperty =
            DependencyProperty.Register("ScrollTreshold", typeof(int), typeof(AutoScroll), new PropertyMetadata(5));
        #endregion

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.SubscribeToDependencyProperty("ItemsSource", OnItemsSourceChanged);

            SubscribeToCollection();

            _scrollViewer = AssociatedObject.FindVisualDescendantByType<ScrollViewer>();
            if (_scrollViewer != null)
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

        private void OnItemsSourceChanged(object sender, DependencyPropertyValueChangedEventArgs e)
        {
            UnsubscribeFromCollection();
            SubscribeToCollection();
        }

        private void UnsubscribeFromCollection()
        {
            if (_collection != null)
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
                if (_collection != null)
                {
                    _collection.CollectionChanged += OnCollectionChanged;

                    if (ScrollOnLoaded)
                    {
                        ScrollToEnd();
                    }
                }
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isScrollbarAtEnd)
            {
                ScrollToEnd();
            }
        }

        private void ScrollToEnd()
        {
            if (_scrollViewer == null)
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

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            switch (ScrollDirection)
            {
                case ScrollDirection.Top:
                    _isScrollbarAtEnd = e.VerticalOffset <= ScrollTreshold;
                    break;

                case ScrollDirection.Bottom:
                    _isScrollbarAtEnd = e.VerticalOffset >= _scrollViewer.ScrollableHeight - ScrollTreshold;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

#endif

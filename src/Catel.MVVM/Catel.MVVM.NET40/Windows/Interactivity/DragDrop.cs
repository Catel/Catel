// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragDrop.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using DragDropHelpers;

    /// <summary>
    /// An attached behavior that allows you to drag and drop items among various ItemsControls, e.g. ItemsControl, ListBox, TabControl, etc.
    /// </summary>
    /// <remarks>
    /// This code was originally found at http://wpfbehaviorlibrary.codeplex.com/, which is based on http://gallery.expression.microsoft.com/DragDropBehavior/.
    /// </remarks>
    public class DragDrop : BehaviorBase<ItemsControl>
    {
        #region Fields
        private const int DragWaitCounterLimit = 10;
        private const string DefaultDataFormatString = "Catel.Windows.Interactivity.DataFormat";

        private bool _myIsMouseDown;
        private bool _myIsDragging;
        private object _myData;
        private Point _myDragStartPosition;
        private DragAdorner _myDragAdorner;
        private DropAdorner _myDropAdorner;
        private int _myDragScrollWaitCounter = DragWaitCounterLimit;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the type of the items in the ItemsControl. 
        /// </summary>
        /// <value>The type of the item.</value>
        public Type ItemType { get; set; }

        /// <summary>
        /// Gets or sets the data template of the items to use while dragging. 
        /// </summary>
        /// <value>The data template.</value>
        public DataTemplate DataTemplate { get; set; }

        /// <summary>
        /// Gets or sets the drop indication.
        /// </summary>
        /// <value>The drop indication.</value>
        /// <remarks>The default is vertical.</remarks>
        public Orientation DropIndication
        {
            get { return _dropIndication; }
            set { _dropIndication = value; }
        }

        private Orientation _dropIndication = Orientation.Vertical;
        #endregion

        #region Button Events
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var itemsControl = (ItemsControl) sender;
            var p = e.GetPosition(itemsControl);

            _myData = UIHelpers.GetItemFromPointInItemsControl(itemsControl, p);
            if (_myData == null)
            {
                return;
            }

            _myIsMouseDown = true;
            _myDragStartPosition = p;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_myIsMouseDown)
            {
                return;
            }

            var itemsControl = (ItemsControl) sender;
            var currentPosition = e.GetPosition(itemsControl);

            if ((_myIsDragging == false) &&
                ((Math.Abs(currentPosition.X - _myDragStartPosition.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                (Math.Abs(currentPosition.Y - _myDragStartPosition.Y) > SystemParameters.MinimumVerticalDragDistance)))
            {
                DragStarted(itemsControl);
            }
        }

        private void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ResetState();
            DetachAdorners();
        }
        #endregion

        #region Drag Events
        private void OnDragOver(object sender, DragEventArgs e)
        {
            var itemsControl = (ItemsControl) sender;
            if (!DataIsPresent(e) || !CanDrop(itemsControl, GetData(e)))
            {
                return;
            }
            
            UpdateDragAdorner(e.GetPosition(itemsControl));
            UpdateDropAdorner(itemsControl, e);
            HandleDragScrolling(itemsControl, e);

            e.Handled = true;
        }

        private void OnDragEnter(object sender, DragEventArgs e)
        {
            var itemsControl = (ItemsControl) sender;
            if (!DataIsPresent(e) || !CanDrop(itemsControl, GetData(e)))
            {
                return;
            }

            var data = GetData(e);
            InitializeDragAdorner(itemsControl, data, e.GetPosition(itemsControl));
            InitializeDropAdorner(itemsControl, e);

            e.Handled = true;
        }

        private void OnPreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (!e.EscapePressed)
            {
                return;
            }

            e.Action = DragAction.Cancel;
            ResetState();
            DetachAdorners();

            e.Handled = true;
        }

        private void OnPreviewDrop(object sender, DragEventArgs e)
        {
            DetachAdorners();
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var itemsControl = (ItemsControl) sender;

            DetachAdorners();
            e.Handled = true;

            if (DataIsPresent(e) && CanDrop(itemsControl, GetData(e)))
            {
                var itemToAddOrMove = GetData(e);
                var isControlPressed = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0);

                if (isControlPressed &&
                    !itemsControl.Items.IsNullOrEmpty() &&
                    itemsControl.Items.Contains(itemToAddOrMove))
                {
                    e.Effects = DragDropEffects.None;
                    return;
                }

                if (isControlPressed)
                {
                    e.Effects = DragDropEffects.Copy;
                    AddItem(itemsControl, itemToAddOrMove, GetInsertionIndex(itemsControl, e));
                }
                else
                {
                    e.Effects = DragDropEffects.Move;
                    RemoveItem(itemsControl, itemToAddOrMove);
                    AddItem(itemsControl, itemToAddOrMove, GetInsertionIndex(itemsControl, e));
                }
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void OnDragLeave(object sender, DragEventArgs e)
        {
            DetachAdorners();
            e.Handled = true;
        }
        #endregion

        /// <summary>
        /// Called when the associated object is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
            AssociatedObject.AllowDrop = true;

            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
            AssociatedObject.Drop += OnDrop;
            AssociatedObject.PreviewDrop += OnPreviewDrop;
            AssociatedObject.QueryContinueDrag += OnPreviewQueryContinueDrag;
            AssociatedObject.DragEnter += OnDragEnter;
            AssociatedObject.DragOver += OnDragOver;
            AssociatedObject.DragLeave += OnDragLeave;
        }

        /// <summary>
        /// Called when the associated object is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
            AssociatedObject.AllowDrop = true;

            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            AssociatedObject.Drop -= OnDrop;
            AssociatedObject.PreviewDrop -= OnPreviewDrop;
            AssociatedObject.QueryContinueDrag -= OnPreviewQueryContinueDrag;
            AssociatedObject.DragEnter -= OnDragEnter;
            AssociatedObject.DragOver -= OnDragOver;
            AssociatedObject.DragLeave -= OnDragLeave;
        }

        #region Overridable by Inheritors
        /// <summary>
        /// Called when an item is added to <paramref name="itemsControl"/>.
        /// </summary>
        /// <param name="itemsControl">The items control <paramref name="item"/> is to be added to.</param>
        /// <param name="item">The item to be added.</param>
        /// <param name="insertIndex">Index <paramref name="item"/> should be inserted at.</param>
        protected virtual void AddItem(ItemsControl itemsControl, object item, int insertIndex)
        {
            if (itemsControl == null)
            {
                return;
            }

            var iList = itemsControl.ItemsSource as IList;
            if (iList != null)
            {
                iList.Insert(insertIndex, item);
            }
            else
            {
                itemsControl.Items.Insert(insertIndex, item);
            }
        }

        /// <summary>
        /// Removes the item from <paramref name="itemsControl"/>.
        /// </summary>
        /// <param name="itemsControl">The items control to remove <paramref name="itemToRemove"/> from.</param>
        /// <param name="itemToRemove">The item to remove.</param>
        protected virtual void RemoveItem(ItemsControl itemsControl, object itemToRemove)
        {
            if (itemsControl == null)
            {
                return;
            }

            if (itemToRemove == null)
            {
                return;
            }
            if (itemsControl.ItemsSource != null)
            {
                var source = itemsControl.ItemsSource as IList;
                if (source != null)
                {
                    source.Remove(itemToRemove);
                }
            }
            else
            {
                itemsControl.Items.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Determines whether <paramref name="item"/> can be dragged from or within the specified items control.
        /// </summary>
        /// <param name="itemsControl">The drag source.</param>
        /// <param name="item">The item to be dragged.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> can be dragged; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanDrag(ItemsControl itemsControl, object item)
        {
            return (ItemType == null) || ItemType.IsInstanceOfType(item);
        }

        /// <summary>
        /// Determines whether <paramref name="item"/> can be dropped onto the specified items control.
        /// </summary>
        /// <param name="itemsControl">The drop target.</param>
        /// <param name="item">The item that would be dropped.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="item"/> can be dropped; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool CanDrop(ItemsControl itemsControl, object item)
        {
            return (ItemType == null) || ItemType.IsInstanceOfType(item);
        }
        #endregion

        private void DragStarted(ItemsControl itemsControl)
        {
            if (!CanDrag(itemsControl, _myData))
            {
                return;
            }

            _myIsDragging = true;

            var dObject = ItemType != null ? new DataObject(ItemType, _myData) : new DataObject(DefaultDataFormatString, _myData);

            System.Windows.DragDrop.DoDragDrop(itemsControl, dObject, DragDropEffects.Copy | DragDropEffects.Move);

            ResetState();
        }

        private void HandleDragScrolling(FrameworkElement itemsControl, DragEventArgs e)
        {
            var verticalMousePosition = UIHelpers.GetRelativeVerticalMousePosition(itemsControl, e.GetPosition(itemsControl));

            if (verticalMousePosition != UIHelpers.RelativeVerticalMousePosition.Middle)
            {
                if (_myDragScrollWaitCounter == DragWaitCounterLimit)
                {
                    _myDragScrollWaitCounter = 0;

                    var scrollViewer = UIHelpers.GetVisualDescendent<ScrollViewer>(itemsControl);
                    if (scrollViewer != null && scrollViewer.ComputedVerticalScrollBarVisibility == Visibility.Visible)
                    {
                        e.Effects = DragDropEffects.Scroll;

                        var movementSize = (verticalMousePosition == UIHelpers.RelativeVerticalMousePosition.Top) ? 1.0 : -1.0;
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + movementSize);
                    }
                }
                else
                {
                    _myDragScrollWaitCounter++;
                }
            }
            else
            {
                e.Effects = ((e.KeyStates & DragDropKeyStates.ControlKey) != 0) ? DragDropEffects.Copy : DragDropEffects.Move;
            }
        }

        private int GetInsertionIndex(ItemsControl itemsControl, DragEventArgs e)
        {
            var dropTargetContainer = UIHelpers.GetItemContainerFromPointInItemsControl(itemsControl, e.GetPosition(itemsControl));
            if (dropTargetContainer != null)
            {
                var index = itemsControl.ItemContainerGenerator.IndexFromContainer(dropTargetContainer);
                if (IsDropPointBeforeItem(itemsControl, e))
                {
                    return index;
                }
                return index + 1;
            }
            return itemsControl.Items.Count;
        }

        private bool IsDropPointBeforeItem(ItemsControl itemsControl, DragEventArgs e)
        {
            var selectedItemContainer = UIHelpers.GetItemContainerFromPointInItemsControl(itemsControl, e.GetPosition(itemsControl)) as FrameworkElement;
            if (selectedItemContainer == null)
            {
                return false;
            }

            var relativePosition = e.GetPosition(selectedItemContainer);

            if (DropIndication == Orientation.Horizontal)
            {
                return relativePosition.X < selectedItemContainer.ActualWidth/2;
            }
            return relativePosition.Y < selectedItemContainer.ActualHeight/2;
        }

        private void ResetState()
        {
            _myIsMouseDown = false;
            _myIsDragging = false;
            _myData = null;
            _myDragScrollWaitCounter = DragWaitCounterLimit;
        }

        private void InitializeDragAdorner(UIElement itemsControl, object dragData, Point startPosition)
        {
            if (DataTemplate == null)
            {
                return;
            }
            if (_myDragAdorner != null)
            {
                return;
            }

            var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
            if (adornerLayer == null)
            {
                return;
            }

            _myDragAdorner = new DragAdorner(dragData, DataTemplate, itemsControl, adornerLayer);
            _myDragAdorner.UpdatePosition(startPosition.X, startPosition.Y);
        }

        private void UpdateDragAdorner(Point currentPosition)
        {
            if (_myDragAdorner != null)
            {
                _myDragAdorner.UpdatePosition(currentPosition.X, currentPosition.Y);
            }
        }

        private void InitializeDropAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_myDropAdorner != null)
            {
                return;
            }
            var adornerLayer = AdornerLayer.GetAdornerLayer(itemsControl);
            var itemContainer = UIHelpers.GetItemContainerFromPointInItemsControl(itemsControl, e.GetPosition(itemsControl));
            if (adornerLayer == null || itemContainer == null)
            {
                return;
            }
            var isPointInTopHalf = IsDropPointBeforeItem(itemsControl, e);
            var isOrientationHorizontal = (DropIndication == Orientation.Horizontal);
            _myDropAdorner = new DropAdorner(isPointInTopHalf, isOrientationHorizontal, itemContainer, adornerLayer);
        }

        private void UpdateDropAdorner(ItemsControl itemsControl, DragEventArgs e)
        {
            if (_myDropAdorner == null)
            {
                return;
            }
            _myDropAdorner.IsTopHalf = IsDropPointBeforeItem(itemsControl, e);
            _myDropAdorner.InvalidateVisual();
        }

        private void DetachAdorners()
        {
            if (_myDropAdorner != null)
            {
                _myDropAdorner.Dispose();
                _myDropAdorner = null;
            }
            if (_myDragAdorner != null)
            {
                _myDragAdorner.Dispose();
                _myDragAdorner = null;
            }
        }

        private bool DataIsPresent(DragEventArgs e)
        {
            if (ItemType == null)
            {
                return !(e.Data.GetFormats().IsNullOrEmpty());
            }
            if (e.Data.GetDataPresent(ItemType))
            {
                return true;
            }

            var format = e.Data.GetFormats().FirstOrDefault();
            if (string.IsNullOrEmpty(format))
            {
                return false;
            }

            var data = e.Data.GetData(format);
            return data != null && ItemType.IsInstanceOfType(data);
        }

        private object GetData(DragEventArgs e)
        {
            if ((ItemType != null) && (e.Data.GetDataPresent(ItemType)))
            {
                return e.Data.GetData(ItemType);
            }

            var format = e.Data.GetFormats().FirstOrDefault();
            if (string.IsNullOrEmpty(format))
            {
                return null;
            }

            var data = e.Data.GetData(format);
            if (data == null)
            {
                return null;
            }

            if (ItemType != null && !ItemType.IsInstanceOfType(data))
            {
                return null;
            }

            return data;
        }

        #region IDisposable Members
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                //Free managed resources
                DetachAdorners();
            }
        }
        #endregion
    }
}
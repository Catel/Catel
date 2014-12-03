// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewStack.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.MVVM.Views
{
    using System;
    using System.Collections.Generic;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
#else
    using System.Windows;
#endif

    /// <summary>
    /// Class containing a view stack and whether the stack is currently loaded.
    /// </summary>
    public class ViewStack
    {
        private readonly ViewStack _parentViewStack;
        private readonly WeakViewInfo _viewInfo;
        private readonly List<ViewStack> _children = new List<ViewStack>();

        private bool _isViewStackLoaded;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStack" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="isViewLoaded">if set to <c>true</c>, the view is loaded.</param>
        public ViewStack(IView view, bool isViewLoaded)
            : this(view, isViewLoaded, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewStack" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="isViewLoaded">if set to <c>true</c>, the view is loaded.</param>
        /// <param name="parentViewStack">The parent view stack. Can be <c>null</c> for root view stacks.</param>
        private ViewStack(IView view, bool isViewLoaded, ViewStack parentViewStack)
        {
            Argument.IsNotNull("view", view);

            _viewInfo = new WeakViewInfo(view, isViewLoaded);
            _viewInfo.Loaded += OnViewLoaded;
            _viewInfo.Unloaded += OnViewUnloaded;

            _parentViewStack = parentViewStack;
            if (parentViewStack != null)
            {
                _parentViewStack.ViewStackLoaded += OnParentViewStackLoaded;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this view stack is outdated, meaning it can be removed.
        /// </summary>
        /// <value><c>true</c> if this instance is outdated; otherwise, <c>false</c>.</value>
        public bool IsOutdated
        {
            get { return !_viewInfo.IsAlive; }
        }

        /// <summary>
        /// Gets a value indicating whether this view stack is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is view stack loaded; otherwise, <c>false</c>.</value>
        public bool IsViewStackLoaded
        {
            get { return _isViewStackLoaded; }
            private set
            {
                if (_isViewStackLoaded == value)
                {
                    return;
                }

                _isViewStackLoaded = value;

                var eventArgs = new ViewStackPartEventArgs(_viewInfo.View);

                if (_isViewStackLoaded)
                {
                    ViewStackLoaded.SafeInvoke(this, eventArgs);
                }
                else
                {
                    ViewStackUnloaded.SafeInvoke(this, eventArgs);
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the current view has been loaded.
        /// </summary>
        public event EventHandler<ViewStackPartEventArgs> ViewLoaded;

        /// <summary>
        /// Occurs when the current view has been unloaded.
        /// </summary>
        public event EventHandler<ViewStackPartEventArgs> ViewUnloaded;

        /// <summary>
        /// Occurs when one of the child views is loaded.
        /// </summary>
        public event EventHandler<ViewStackPartEventArgs> ViewStackLoaded;

        /// <summary>
        /// Occurs when one of the child views is loaded.
        /// </summary>
        public event EventHandler<ViewStackPartEventArgs> ViewStackUnloaded;
        #endregion

        #region Methods
        /// <summary>
        /// Notifies the that parent is ready to accept loaded messages.
        /// </summary>
        public void NotifyThatParentIsReadyToAcceptLoadedMessages()
        {
            MarkAsLoaded();

            CheckForOutdatedChildren();

            foreach (var child in _children)
            {
                if (!child.IsOutdated)
                {
                    child.NotifyThatParentIsReadyToAcceptLoadedMessages();
                }
            }
        }

        /// <summary>
        /// Adds a new child to the stack.
        /// </summary>
        /// <param name="viewStack">The view stack.</param>
        /// <param name="parentViewStack">The parent view stack.</param>
        /// <returns><c>true</c> if added, <c>false</c> otherwise.</returns>
        public bool AddChild(ViewStack viewStack, ViewStack parentViewStack)
        {
            Argument.IsNotNull("viewStack", viewStack);
            Argument.IsNotNull("parentViewStack", parentViewStack);

            if (ReferenceEquals(this, parentViewStack))
            {
                viewStack.ViewStackLoaded += OnChildViewStackLoaded;
                viewStack.ViewStackUnloaded += OnChildViewStackUnloaded;
                viewStack.ViewLoaded += OnChildViewLoaded;
                viewStack.ViewUnloaded += OnChildViewUnloaded;

                _children.Add(viewStack);

                return true;
            }

            foreach (var child in _children)
            {
                if (child.AddChild(viewStack, parentViewStack))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Adds a new child to the stack.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="parentViewStack">The parent view stack.</param>
        /// <returns><c>true</c> if added, <c>false</c> otherwise.</returns>
        public bool AddChild(IView view, ViewStack parentViewStack)
        {
            Argument.IsNotNull("view", view);
            Argument.IsNotNull("parentViewStack", parentViewStack);

            var viewStack = new ViewStack(view, false, parentViewStack);

            return AddChild(viewStack, parentViewStack);
        }

        /// <summary>
        /// Disposes this instance.
        /// <para />
        /// Not using <see cref="IDisposable"/> to prevent other auto systems from kicking in.
        /// </summary>
        public void Dispose()
        {
            if (_parentViewStack != null)
            {
                _parentViewStack.ViewStackLoaded -= OnParentViewStackLoaded;
            }

            UnsubscribeLayoutUpdated();
        }

        /// <summary>
        /// Marks the view stack as loaded.
        /// </summary>
        public void MarkAsLoaded()
        {
            if (_isViewStackLoaded)
            {
                return;
            }

            if (_parentViewStack != null && !_parentViewStack.IsViewStackLoaded)
            {
                return;
            }

            IsViewStackLoaded = true;
        }

        /// <summary>
        /// Marks the view stack as unloaded.
        /// </summary>
        public void MarkAsUnloaded()
        {
            if (!_isViewStackLoaded)
            {
                return;
            }

            IsViewStackLoaded = false;
        }

        /// <summary>
        /// Checks for outdated children and removes them if necessary.
        /// </summary>
        public void CheckForOutdatedChildren()
        {
            for (int i = 0; i < _children.Count; i++)
            {
                var child = _children[i];
                if (child.IsOutdated)
                {
                    child.ViewStackLoaded -= OnChildViewStackLoaded;
                    child.ViewStackUnloaded -= OnChildViewStackUnloaded;
                    child.ViewLoaded -= OnChildViewLoaded;
                    child.ViewUnloaded -= OnChildViewUnloaded;

                    child.Dispose();

                    _children.RemoveAt(i--);
                }
                else
                {
                    child.CheckForOutdatedChildren();
                }
            }
        }

        /// <summary>
        /// Determines whether this view stack contains the specified view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns><c>true</c> if this view stack contains the specified view; otherwise, <c>false</c>.</returns>
        public bool ContainsView(IView view)
        {
            if (ReferenceEquals(_viewInfo.View, view))
            {
                return true;
            }

            foreach (var child in _children)
            {
                if (child.ContainsView(view))
                {
                    return true;
                }
            }

            return false;
        }

        private void SubscribeLayoutUpdated()
        {
#if SILVERLIGHT
            _viewInfo.LayoutUpdated += OnViewLayoutUpdated;

            var view = _viewInfo.View;
            if (view != null)
            {
                view.Dispatch(() => ((FrameworkElement)view).InvalidateMeasure());
            }
#endif
        }

        private void UnsubscribeLayoutUpdated()
        {
#if SILVERLIGHT
            _viewInfo.LayoutUpdated -= OnViewLayoutUpdated;
#endif
        }

        private void OnViewLayoutUpdated(object sender, EventArgs e)
        {
            UnsubscribeLayoutUpdated();

            RaiseViewLoaded();
        }

        private void RaiseViewLoaded()
        {
            ViewLoaded.SafeInvoke(this, new ViewStackPartEventArgs(_viewInfo.View));

            MarkAsLoaded();
        }

        private void RaiseViewUnloaded()
        {
            ViewUnloaded.SafeInvoke(this, new ViewStackPartEventArgs(_viewInfo.View));
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
#if SILVERLIGHT
            SubscribeLayoutUpdated();
#else
            RaiseViewLoaded();
#endif
        }

        private void OnViewUnloaded(object sender, EventArgs e)
        {
            UnsubscribeLayoutUpdated();

            MarkAsUnloaded();

            RaiseViewUnloaded();
        }

        private void OnChildViewLoaded(object sender, ViewStackPartEventArgs e)
        {
            MarkAsLoaded();
        }

        private void OnParentViewStackLoaded(object sender, ViewStackPartEventArgs e)
        {
            if (!_viewInfo.IsLoaded)
            {
                return;
            }

            MarkAsLoaded();
        }

        private void OnChildViewUnloaded(object sender, ViewStackPartEventArgs e)
        {
            // no logic yet
        }

        private void OnChildViewStackLoaded(object sender, ViewStackPartEventArgs e)
        {
            // Pass on as routed event
            ViewStackLoaded.SafeInvoke(this, e);
        }

        private void OnChildViewStackUnloaded(object sender, ViewStackPartEventArgs e)
        {
            // Pass on as routed event
            ViewStackUnloaded.SafeInvoke(this, e);
        }
        #endregion
    }
}
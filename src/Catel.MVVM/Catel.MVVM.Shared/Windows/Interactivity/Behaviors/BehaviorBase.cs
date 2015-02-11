﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BehaviorBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if !WIN80 && !XAMARIN

namespace Catel.Windows.Interactivity
{
    using System.Windows;
    using IoC;
    using MVVM.Views;

#if NETFX_CORE
    using global::Windows.UI.Xaml;
    using UIEventArgs = global::Windows.UI.Xaml.RoutedEventArgs;
#else
    using System;
    using System.Windows.Interactivity;
    using UIEventArgs = System.EventArgs;
#endif

    /// <summary>
    /// Behavior base class that handles a safe unsubscribe and clean up because the default
    /// behavior class does not always call <see cref="Behavior.OnDetaching"/>.
    /// <para />
    /// This class also adds some specific features such as <see cref="ValidateRequiredProperties"/>
    /// which is automatically called when the behavior is attached.
    /// </summary>
    /// <typeparam name="T">The <see cref="IView"/> this behavior should attach to.</typeparam>
    public abstract class BehaviorBase<T> : Behavior<T>, IBehavior
        where T : FrameworkElement
    {
        #region Fields
        private bool _isClean = true;
        private int _loadCounter;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="Behavior{T}.AssociatedObject"/> is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssociatedObjectLoaded { get { return _loadCounter > 0; } }

        /// <summary>
        /// Gets a value indicating whether this instance is in design mode.
        /// </summary>
        /// <value><c>true</c> if this instance is in design mode; otherwise, <c>false</c>.</value>
        protected bool IsInDesignMode
        {
            get { return CatelEnvironment.IsInDesignMode; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        protected override void OnAttached()
        {
            if (IsInDesignMode)
            {
                return;
            }

            base.OnAttached();

            AssociatedObject.Loaded += OnAssociatedObjectLoadedInternal;

            _isClean = false;

            ValidateRequiredProperties();

            Initialize();
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            if (IsInDesignMode)
            {
                return;
            }

            CleanUp();

            if (AssociatedObject != null)
            {
                AssociatedObject.Loaded -= OnAssociatedObjectLoadedInternal;
            }

            base.OnDetaching();
        }

        /// <summary>
        /// Validates the required properties. This method is called when the object is attached, but before
        /// the <see cref="Initialize"/> is invoked.
        /// </summary>
        protected virtual void ValidateRequiredProperties()
        {
        }

        /// <summary>
        /// Initializes the behavior. This method is called instead of the <see cref="OnAttached"/> which is sealed
        /// to protect the additional behavior.
        /// </summary>
        protected virtual void Initialize()
        {
            
        }

        /// <summary>
        /// Uninitializes the behavior. This method is called when <see cref="OnDetaching"/> is called, or when the
        /// <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// <para />
        /// If dependency properties are used, it is very important to use 
        /// <see cref="DependencyObject.ClearValue(DependencyProperty)"/> to clear the value
        /// of the dependency properties in this method.
        /// </summary>
        protected virtual void Uninitialize()
        {
            
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded. This method is introduced to prevent
        /// double initialization when the <see cref="Behavior{T}.AssociatedObject"/> is already loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectLoadedInternal(object sender, UIEventArgs e)
        {
            _loadCounter++;

            // Yes, 1, because we just increased the counter
            if (_loadCounter != 1)
            {
                return;
            }

            AssociatedObject.Unloaded += OnAssociatedObjectUnloadedInternal;

            OnAssociatedObjectLoaded();
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is loaded.
        /// </summary>
        protected virtual void OnAssociatedObjectLoaded()
        {
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded. This method is introduced to prevent
        /// double uninitialization when the <see cref="Behavior{T}.AssociatedObject"/> is already unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectUnloadedInternal(object sender, UIEventArgs e)
        {
            _loadCounter--;

            if (_loadCounter != 0)
            {
                return;
            }

            OnAssociatedObjectUnloaded();

            //CleanUp();
        }

        /// <summary>
        /// Called when the <see cref="Behavior{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        protected virtual void OnAssociatedObjectUnloaded()
        {
        }

        /// <summary>
        /// Actually cleans up the behavior because <see cref="OnDetaching"/> is not always called.
        /// </summary>
        private void CleanUp()
        {
            if (_isClean)
            {
                return;
            }

            _isClean = true;

            if (AssociatedObject != null)
            {
                AssociatedObject.Unloaded -= OnAssociatedObjectUnloadedInternal;
            }

            Uninitialize();
        }
        #endregion
    }
}

#endif
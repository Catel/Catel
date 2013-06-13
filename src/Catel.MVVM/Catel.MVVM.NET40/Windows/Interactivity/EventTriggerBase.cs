// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventTriggerBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Windows.Interactivity
{
    using System;
    using System.Windows;

    /// <summary>
    /// Trigger base class that handles a safe unsubscribe and clean up because the default
    /// Trigger class does not always call <see cref="OnDetaching"/>.
    /// <para />
    /// This class also adds some specific features such as <see cref="ValidateRequiredProperties"/>
    /// which is automatically called when the trigger is attached.
    /// </summary>
    /// <typeparam name="T">The <see cref="FrameworkElement"/> this trigger should attach to.</typeparam>
    public abstract class EventTriggerBase<T> : System.Windows.Interactivity.EventTriggerBase<T>
        where T : FrameworkElement
    {
        #region Fields
        private bool _isClean = true;
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether the <c>AssociatedObject</c> is loaded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <c>AssociatedObject</c> is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsAssociatedObjectLoaded { get; private set; }

        /// <summary>
        /// Gets the object to which the trigger is attached.
        /// </summary>
        /// <value>The associated object.</value>
        protected new T AssociatedObject { get { return (T)base.AssociatedObject; } }
        #endregion

        #region Methods
        /// <summary>
        /// Specifies the name of the Event this EventTriggerBase is listening for.
        /// </summary>
        protected override string GetEventName()
        {
            throw new InvalidOperationException("This method MUST be overriden and the base cannot be called");
        }

        /// <summary>
        /// Called after the action is attached to an AssociatedObject.
        /// </summary>
        protected override sealed void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += OnAssociatedObjectLoadedInternal;

            _isClean = false;
            IsAssociatedObjectLoaded = false;

            ValidateRequiredProperties();

            Initialize();
        }

        /// <summary>
        /// Called when the action is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        protected override sealed void OnDetaching()
        {
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
        /// Initializes the trigger action. This method is called instead of the <see cref="OnAttached"/> which is sealed
        /// to protect the additional trigger action.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Uninitializes the behavior. This method is called when <see cref="OnDetaching"/> is called, or when the
        /// <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is unloaded.
        /// <para />
        /// If dependency properties are used, it is very important to use <see cref="DependencyObject.ClearValue(System.Windows.DependencyProperty)"/> 
        /// to clear the value of the dependency properties in this method.
        /// </summary>
        protected virtual void Uninitialize()
        {
        }

        /// <summary>
        /// Called when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is loaded. This method is introduced to prevent
        /// double initialization when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is already loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectLoadedInternal(object sender, EventArgs e)
        {
            if (IsAssociatedObjectLoaded)
            {
                return;
            }

            AssociatedObject.Unloaded += OnAssociatedObjectUnloadedInternal;

            IsAssociatedObjectLoaded = true;

            OnAssociatedObjectLoaded(sender, e);
        }

        /// <summary>
        /// Called when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnAssociatedObjectLoaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Called when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is unloaded. This 
        /// method is introduced to prevent double uninitialization when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is already unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnAssociatedObjectUnloadedInternal(object sender, EventArgs e)
        {
            if (!IsAssociatedObjectLoaded)
            {
                return;
            }

            IsAssociatedObjectLoaded = false;

            OnAssociatedObjectUnloaded(sender, e);

            CleanUp();
        }

        /// <summary>
        /// Called when the <see cref="System.Windows.Interactivity.TriggerBase{T}.AssociatedObject"/> is unloaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnAssociatedObjectUnloaded(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Actually cleans up the trigger action because <see cref="OnDetaching"/> is not always called.
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
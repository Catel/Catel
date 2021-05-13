// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Behavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if UWP

namespace Catel.Windows.Interactivity
{
    using System;
    using Logging;
    using Reflection;
    using global::Windows.UI.Xaml;
    using IXamlBehavior = Microsoft.Xaml.Interactivity.IBehavior;
    
    /// <summary>
    /// Base class for the behavior because the SDK for WinRT only ships with interfaces.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : DependencyObject, IXamlBehavior
        where T : DependencyObject
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private int _attachedCounter;

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> is attached.
        /// </summary>
        /// <value>The associated object.</value>
        public T AssociatedObject { get; private set; }
        #endregion

        #region IBehavior Members
        /// <summary>
        /// Gets the <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> is attached.
        /// </summary>
        /// <value>The associated object.</value>
        DependencyObject IXamlBehavior.AssociatedObject
        {
            get { return AssociatedObject; }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> will be attached.</param>
        /// <exception cref="System.InvalidOperationException">The associated object is not of the expected type.</exception>
        void IXamlBehavior.Attach(DependencyObject associatedObject)
        {
            _attachedCounter++;
            if (_attachedCounter > 1)
            {
                return;
            }

            if (associatedObject is not null && typeof(T).IsInstanceOfTypeEx(associatedObject.GetType()))
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>("Invalid target type '{0}', expected '{1}'", associatedObject.GetType().GetSafeFullName(false), typeof (T).GetSafeFullName(false));
            }

            AssociatedObject = associatedObject as T;
            OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        void IXamlBehavior.Detach()
        {
            _attachedCounter--;
            if (_attachedCounter > 0)
            {
                return;
            }

            if (_attachedCounter == 0)
            {
                OnDetaching();
            }
            
            _attachedCounter = 0;

            // Note: CTL-850 don't set AssociatedObject to null, we need to be able to unsubscribe from events
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when the associated object is attached.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// Called when the associated object is detached.
        /// </summary>
        protected virtual void OnDetaching()
        {
        }
        #endregion
    }
}

#endif

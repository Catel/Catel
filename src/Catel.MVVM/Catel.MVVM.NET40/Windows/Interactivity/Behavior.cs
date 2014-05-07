// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Behavior.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NETFX_CORE && !WIN80

namespace Catel.Windows.Interactivity
{
    using System;
    using Logging;
    using Reflection;
    using global::Windows.UI.Xaml;
    using Microsoft.Xaml.Interactivity;

    /// <summary>
    /// Base class for the behavior because the SDK in WIN81 only ships with interfaces.
    /// <para />
    /// This class tries to mimic the WPF, Silverlight and Windows Phone behavior class to allow reusage of the behaviors in Catel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Behavior<T> : DependencyObject, IBehavior
        where T : DependencyObject
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
        DependencyObject IBehavior.AssociatedObject
        {
            get { return AssociatedObject; }
        }

        /// <summary>
        /// Attaches to the specified object.
        /// </summary>
        /// <param name="associatedObject">The <see cref="T:Windows.UI.Xaml.DependencyObject" /> to which the <seealso cref="T:Microsoft.Xaml.Interactivity.IBehavior" /> will be attached.</param>
        /// <exception cref="System.InvalidOperationException">The associated object is not of the expected type.</exception>
        void IBehavior.Attach(DependencyObject associatedObject)
        {
            if (associatedObject != null && typeof(T).IsAssignableFromEx(associatedObject.GetType()))
            {
                string error = string.Format("Invalid target type '{0}', expected '{1}'", associatedObject.GetType().GetSafeFullName(), typeof (T).GetSafeFullName());
                Log.Error(error);
                throw new InvalidOperationException(error);
            }

            AssociatedObject = associatedObject as T;
            OnAttached();
        }

        /// <summary>
        /// Detaches this instance from its associated object.
        /// </summary>
        void IBehavior.Detach()
        {
            OnDetaching();
            AssociatedObject = null;
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
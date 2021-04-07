// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildAwareModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2017 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

#if NET || NETCORE || NETSTANDARD
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Class that is aware of changes of child objects by using the <see cref="ChangeNotificationWrapper"/>.
    /// </summary>
    public abstract class ChildAwareModelBase : ValidatableModelBase
    {
        /// <summary>
        /// The change notification wrappers for all property values.
        /// </summary>
        private Dictionary<string, ChangeNotificationWrapper> _propertyValueChangeNotificationWrappers;

        /// <summary>
        /// Initializes the <see cref="ChildAwareModelBase"/> class.
        /// </summary>
        static ChildAwareModelBase()
        {
            DefaultDisableEventSubscriptionsOfChildValuesValue = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildAwareModelBase"/> class.
        /// </summary>
        protected ChildAwareModelBase()
        {
            InitializeChildAwareModelBase();
        }

        /// <summary>
        /// Gets or sets a value indicating whether event subscriptions of child values should be disabled.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if event subscriptions of child values should be disabled; otherwise, <c>false</c>.</value>
        protected bool DisableEventSubscriptionsOfChildValues { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether event subscriptions of child values should be disabled.
        /// </summary>
        public static bool DefaultDisableEventSubscriptionsOfChildValuesValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object should handle (thus invoke the specific events) when
        /// a property or collection value has changed.
        /// </summary>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        protected bool HandlePropertyAndCollectionChanges { get; set; }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void InitializeChildAwareModelBase()
        {
            HandlePropertyAndCollectionChanges = true;
            DisableEventSubscriptionsOfChildValues = DefaultDisableEventSubscriptionsOfChildValuesValue;
        }

        /// <summary>
        /// Sets the value fast without checking for any constraints or additional logic such as change notifications. This
        /// means that if this method is used incorrectly, it can throw random exceptions.
        /// <para />
        /// This is a wrapper around the _propertyValues field. Don't use the field directly, always use
        /// this method because it takes care of locking and event subscriptions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="value">The value.</param>
        protected override void SetValueToPropertyBag<TValue>(string propertyName, TValue value)
        {
            base.SetValueToPropertyBag<TValue>(propertyName, value);

            HandleObjectEventsSubscription(propertyName, BoxingCache.GetBoxedValue(value));
        }

        /// <summary>
        /// Handles the object events subscription. This means that the old value will be removed from the event subscriptions, and
        /// the new value will be subscribed to.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyValue">The property value.</param>
        private void HandleObjectEventsSubscription(string propertyName, object propertyValue)
        {
            if (DisableEventSubscriptionsOfChildValues)
            {
                return;
            }

            lock (_lock)
            {
                if (_propertyValueChangeNotificationWrappers is null)
                {
                    _propertyValueChangeNotificationWrappers = new Dictionary<string, ChangeNotificationWrapper>();
                }

                if (_propertyValueChangeNotificationWrappers.TryGetValue(propertyName, out var oldWrapper))
                {
                    oldWrapper.PropertyChanged -= OnPropertyObjectPropertyChanged;
                    oldWrapper.CollectionChanged -= OnPropertyObjectCollectionChanged;
                    oldWrapper.CollectionItemPropertyChanged -= OnPropertyObjectCollectionItemPropertyChanged;
                    oldWrapper.UnsubscribeFromAllEvents();
                }

                if (!ChangeNotificationWrapper.IsUsefulForObject(propertyValue))
                {
                    if (oldWrapper is not null)
                    {
                        _propertyValueChangeNotificationWrappers.Remove(propertyName);
                    }
                }
                else
                {
                    var wrapper = new ChangeNotificationWrapper(propertyValue);
                    wrapper.PropertyChanged += OnPropertyObjectPropertyChanged;
                    wrapper.CollectionChanged += OnPropertyObjectCollectionChanged;
                    wrapper.CollectionItemPropertyChanged += OnPropertyObjectCollectionItemPropertyChanged;
                    _propertyValueChangeNotificationWrappers[propertyName] = wrapper;
                }
            }
        }

        /// <summary>
        /// Called when a property that implements <see cref="INotifyPropertyChanged"/> raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // It is possible that the sender used string.Empty or null for the property name, then exit
            var propertyName = e.PropertyName;
            if (string.IsNullOrEmpty(propertyName))
            {
                return;
            }

            RaisePropertyChanged(sender, e, true, false);

            if (IsValidationProperty(propertyName))
            {
                Validate(true);
            }
        }

        /// <summary>
        /// Called when a property that implements <see cref="INotifyCollectionChanged"/> raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetDirty(string.Empty);

            Validate(true);
        }

        /// <summary>
        /// Called when a property inside a collection that implements <see cref="INotifyCollectionChanged"/> that implements
        /// <see cref="INotifyPropertyChanged"/> raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyObjectCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetDirty(string.Empty);

            if (IsValidationProperty(e.PropertyName))
            {
                Validate(true);
            }
        }
    }
}

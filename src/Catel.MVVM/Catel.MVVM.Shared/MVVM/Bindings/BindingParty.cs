// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindingParty.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if XAMARIN

namespace Catel.MVVM
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Contains information about a specific binding party (either source or target).
    /// </summary>
    public class BindingParty : IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly string _toStringValue;
        private readonly PropertyInfo _propertyInfo;
        private readonly string _propertyName;
        private readonly WeakReference _instance;

        private ChangeNotificationWrapper _changeNotificationWrapper;
        private List<IWeakEventListener> _weakEventListeners = new List<IWeakEventListener>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingParty" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="propertyName">Name of the property.</param>
        public BindingParty(object instance, string propertyName)
        {
            Argument.IsNotNull("instance", instance);

            _instance = new WeakReference(instance);
            _propertyName = propertyName;

            var instanceType = instance.GetType();
            _toStringValue = string.Format("{0}.{1}", instanceType.Name, _propertyName);
            _propertyInfo = instanceType.GetPropertyEx(_propertyName);
            if (_propertyInfo == null)
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Property '{0}' not found, cannot create binding", _toStringValue);
            }

            _changeNotificationWrapper = new ChangeNotificationWrapper(instance);
            _changeNotificationWrapper.PropertyChanged += OnInstancePropertyChanged;
        }

        #region Events
        /// <summary>
        /// Occurs when the value has changed.
        /// </summary>
        public event EventHandler<EventArgs> ValueChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance of the binding party.
        /// <para />
        /// Note that this value is stored in a weak reference and can be <c>null</c> if garbage collected.
        /// </summary>
        /// <value>The instance.</value>
        public object Instance
        {
            get { return (_instance != null && _instance.IsAlive) ? _instance.Target : null; }
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get { return _propertyName; }
        }
        #endregion

        #region Methods
        private void OnInstancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.PropertyName) || string.Equals(e.PropertyName, _propertyName))
            {
                RaiseValueChanged();
            }
        }

        /// <summary>
        /// Converts the current instance to a string.
        /// </summary>
        /// <returns>The string representation of this object.</returns>
        public override string ToString()
        {
            return _toStringValue;
        }

        /// <summary>
        /// Adds the event so it will be used as source to raise the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        public void AddEvent<TEventArgs>(string eventName)
            where TEventArgs : EventArgs
        {
            Argument.IsNotNull("eventName", eventName);

            Log.Debug("Adding subscription to event '{0}'", eventName);

            var target = _instance.Target;
            if (target == null)
            {
                Log.ErrorAndThrowException<InvalidOperationException>("Target is no longer alive, cannot add event subscription");
            }

            var weakEventListener = this.SubscribeToWeakGenericEvent<TEventArgs>(target, eventName, (sender, e) => RaiseValueChanged());
            _weakEventListeners.Add(weakEventListener);
        }

        /// <summary>
        /// Gets the property value.
        /// <para />
        /// Note that the property value will be <c>null</c> if the <see cref="Instance"/> is garbage collected.
        /// </summary>
        /// <returns>The property value.</returns>
        public object GetPropertyValue()
        {
            var instance = Instance;
            if (instance == null)
            {
                return null;
            }

            return _propertyInfo.GetValue(instance);
        }

        /// <summary>
        /// Sets the property value.
        /// <para />
        /// Note that the property value will not be set if the <see cref="Instance"/> is garbage collected.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public void SetPropertyValue(object newValue)
        {
            var instance = Instance;
            if (instance == null)
            {
                return;
            }

            _propertyInfo.SetValue(instance, newValue);
        }

        /// <summary>
        /// Raises the <see cref="ValueChanged"/> event.
        /// </summary>
        private void RaiseValueChanged()
        {
            ValueChanged.SafeInvoke(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_changeNotificationWrapper != null)
                {
                    _changeNotificationWrapper.PropertyChanged -= OnInstancePropertyChanged;
                    _changeNotificationWrapper.UnsubscribeFromAllEvents();
                    _changeNotificationWrapper = null;
                }

                foreach (var weakEventListener in _weakEventListeners)
                {
                    weakEventListener.Detach();
                }

                _weakEventListeners.Clear();
            }
        }
        #endregion
    }
}

#endif
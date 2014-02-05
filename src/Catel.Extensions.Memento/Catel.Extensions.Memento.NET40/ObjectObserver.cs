// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectObserver.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Memento
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Data;
    using IoC;
    using Logging;
    using Reflection;

    /// <summary>
    /// Observer that will observe changes of the the object injected into this observer. Each change will automatically
    /// be registered in the <see cref="IMementoService"/>.
    /// </summary>
    public class ObjectObserver : ObserverBase
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Collection containing the previous values of the object.
        /// </summary>
        private readonly Dictionary<string, object> _previousPropertyValues = new Dictionary<string, object>();

        /// <summary>
        /// Subscription to the weak event listener.
        /// </summary>
        private IWeakEventListener _weakEventListener;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectObserver"/> class.
        /// </summary>
        /// <param name="propertyChanged">The property changed.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="mementoService">The memento service. If <c>null</c>, the service will be retrieved from the <see cref="IServiceLocator"/>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyChanged"/> is <c>null</c>.</exception>
        public ObjectObserver(INotifyPropertyChanged propertyChanged, object tag = null, IMementoService mementoService = null)
            : base(tag, mementoService)
        {
            Argument.IsNotNull("propertyChanged", propertyChanged);

            Log.Debug("Initializing ObjectObserver for type '{0}'", propertyChanged.GetType().Name);

            _weakEventListener = this.SubscribeToWeakPropertyChangedEvent(propertyChanged, OnPropertyChanged);

            InitializeDefaultValues(propertyChanged);

            Log.Debug("Initialized ObjectObserver for type '{0}'", propertyChanged.GetType().Name);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method must be public because the <see cref="IWeakEventListener"/> is used.
        /// </remarks>
        public void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var modelBase = sender as ModelBase;
            if (modelBase != null)
            {
                if ((string.CompareOrdinal(e.PropertyName, "HasErrors") == 0) || (string.CompareOrdinal(e.PropertyName, "IsDirty") == 0) ||
                    (string.CompareOrdinal(e.PropertyName, "HasWarnings") == 0))
                {
                    return;
                }
            }

            if (ShouldPropertyBeIgnored(sender, e.PropertyName))
            {
                return;
            }

            object oldValue = _previousPropertyValues[e.PropertyName];
            _previousPropertyValues[e.PropertyName] = PropertyHelper.GetPropertyValue(sender, e.PropertyName);
            object newValue = _previousPropertyValues[e.PropertyName];

            MementoService.Add(new PropertyChangeUndo(sender, e.PropertyName, oldValue, newValue, Tag));
        }

        /// <summary>
        /// Initializes the default values.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="obj"/> is <c>null</c>.</exception>
        private void InitializeDefaultValues(object obj)
        {
            Argument.IsNotNull("obj", obj);

            var properties = obj.GetType().GetPropertiesEx(BindingFlagsHelper.GetFinalBindingFlags(true, false));
            foreach (var property in properties)
            {
                if (!ShouldPropertyBeIgnored(obj, property.Name))
                {
                    _previousPropertyValues[property.Name] = PropertyHelper.GetPropertyValue(obj, property.Name);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property should be ignored.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property to check.</param>
        /// <returns><c>true</c> if the property should be ignored; otherwise <c>false</c>.</returns>
        private bool ShouldPropertyBeIgnored(object obj, string propertyName)
        {
            var objectType = obj.GetType();
            var propertyInfo = objectType.GetPropertyEx(propertyName);
            var ignore = AttributeHelper.IsDecoratedWithAttribute<IgnoreMementoSupportAttribute>(propertyInfo);
            if (ignore)
            {
                Log.Debug("Ignored property '{0}' because it is decorated with the IgnoreMementoSupportAttribute", propertyName);
            }

            return ignore;
        }

        /// <summary>
        /// Clears all the values and unsubscribes any existing change notifications.
        /// </summary>
        public override void CancelSubscription()
        {
            Log.Debug("Canceling property change subscription");

            if (_weakEventListener != null)
            {
                _weakEventListener.Detach();
                _weakEventListener = null;
            }

            _previousPropertyValues.Clear();

            Log.Debug("Canceled property change subscription");
        }
        #endregion
    }
}
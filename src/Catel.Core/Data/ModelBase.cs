// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using Logging;

#if NET || NETCORE || NETSTANDARD
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
#if NET || NETCORE || NETSTANDARD
    [Serializable]
#endif
    public abstract partial class ModelBase : ObservableObject, IModel
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The property values.
        /// </summary>
        internal IPropertyBag _propertyBag;

        /// <summary>
        /// Lock object.
        /// </summary>
        internal readonly object _lock = new object();

        internal SuspensionContext _changeCallbacksSuspensionContext;
        internal SuspensionContext _changeNotificationsSuspensionContext;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ModelBase"/> class.
        /// </summary>
        static ModelBase()
        {
            PropertyDataManager = PropertyDataManager.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        protected ModelBase()
        {
            Initialize();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether property change notifications are currently disabled for all instances.
        /// </summary>
        /// <value><c>true</c> if property change notifications should be disabled for all instances; otherwise, <c>false</c>.</value>
        /// TODO: Try to revert to internal but is required by XAMARIN_FORMS
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        [XmlIgnore]
        public static bool DisablePropertyChangeNotifications { get; set; }

        /// <summary>
        /// Gets the property data manager that manages the properties of this object.
        /// </summary>
        /// <value>The property data manager.</value>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        [XmlIgnore]
        internal static PropertyDataManager PropertyDataManager { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object should always invoke the <see cref="ObservableObject.PropertyChanged"/> event,
        /// even when the actual value of a property has not changed.
        /// <para />
        /// Enabling this property is useful when using this class in a WPF environment.
        /// </summary>
        /// <remarks>
        /// By default, this property is <c>false</c>.
        /// </remarks>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        protected bool AlwaysInvokeNotifyChanged { get; set; }

        /// <summary>
        /// Gets the name of the object. By default, this is the hash code of all the properties combined.
        /// </summary>
        /// <value>The name of the key.</value>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        [XmlIgnore]
        string IModel.KeyName
        {
            get { return GetHashCode().ToString(); }
        }

        /// <summary>
        /// Gets a value indicating whether the object is currently in an edit session, started by the <see cref="IEditableObject.BeginEdit"/> method.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently in an edit session; otherwise, <c>false</c>.
        /// </value>
        bool IModel.IsInEditSession
        {
            get { return _backup != null; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is dirty (contains unsaved data).
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsDirty
        {
            // Note: we know what we are doing, use GetValueFromPropertyBag (but not SetValueFast)
            get { return GetValueFromPropertyBag<bool>(IsDirtyProperty.Name); }
            protected set { SetValue(IsDirtyProperty, value); }
        }

        /// <summary>
        /// Register the IsDirty property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IsDirtyProperty = RegisterProperty("IsDirty", typeof(bool), false, null, false, true, true);

        /// <summary>
        /// Gets or sets a value indicating whether this object is currently read-only. When the object is read-only, values can only be read, not set.
        /// </summary>
#if NET || NETCORE || NETSTANDARD
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsReadOnly
        {
            // Note: we know what we are doing, use GetValueFromPropertyBag (but not SetValueFast)
            get { return GetValueFromPropertyBag<bool>(IsReadOnlyProperty.Name); }
            protected set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Register the IsReadOnly property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IsReadOnlyProperty = RegisterProperty("IsReadOnly", typeof(bool), false,
            (sender, e) => ((ModelBase)sender).RaisePropertyChanged("IsEditable"), false, true, true);
        #endregion

        #region Methods
        /// <summary>
        /// Allows the initialization of custom properties. This is a virtual method that is called
        /// inside the constructor before the object is fully constructed.
        /// <para />
        /// This might be considered as bad or as a hack, but it's a good way to be able to inject
        /// custom properties before any actual logic is handled by derived classes.
        /// </summary>
        /// <remarks>
        /// Only use when you really know what you are doing.
        /// </remarks>
        protected virtual void InitializeCustomProperties()
        {
        }

        /// <summary>
        /// Initializes the object by setting default values.
        /// </summary>
        private void Initialize()
        {
            AlwaysInvokeNotifyChanged = false;

            _propertyBag = CreatePropertyBag();

            InitializeProperties();

            InitializeCustomProperties();
        }

        // ReSharper disable RedundantOverridenMember

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString();
        }

        // ReSharper restore RedundantOverridenMember

        /// <summary>
        /// Suspends the change callbacks whenever a property has been called. This is very useful when
        /// there are expensive property change callbacks registered with a property that need to be
        /// temporarily disabled.
        /// </summary>
        /// <returns></returns>
        public IDisposable SuspendChangeCallbacks()
        {
            var token = new DisposableToken<ModelBase>(this, x =>
            {
                lock (_lock)
                {
                    if (_changeCallbacksSuspensionContext is null)
                    {
                        _changeCallbacksSuspensionContext = new SuspensionContext();
                    }

                    _changeCallbacksSuspensionContext.Increment();
                }
            },
            x =>
            {
                lock (_lock)
                {
                    var suspensionContext = _changeCallbacksSuspensionContext;
                    if (suspensionContext != null)
                    {
                        suspensionContext.Decrement();

                        if (suspensionContext.Counter == 0)
                        {
                            _changeCallbacksSuspensionContext = null;
                        }
                    }
                }

                // Note: don't invoke the "missed" callbacks
            });

            return token;
        }

        /// <summary>
        /// Suspends the change notifications until the disposed object has been released.
        /// </summary>
        /// <param name="raiseOnResume">if set to <c>true</c>, the notifications are invoked on resume.</param>
        /// <returns>A disposable object.</returns>
        public IDisposable SuspendChangeNotifications(bool raiseOnResume = true)
        {
            var token = new DisposableToken<ModelBase>(this, x =>
            {
                lock (_lock)
                {
                    if (_changeNotificationsSuspensionContext is null)
                    {
                        _changeNotificationsSuspensionContext = new SuspensionContext();
                    }

                    _changeNotificationsSuspensionContext.Increment();
                }
            },
            x =>
            {
                SuspensionContext suspensionContext;

                lock (_lock)
                {
                    suspensionContext = _changeNotificationsSuspensionContext;
                    if (suspensionContext != null)
                    {
                        suspensionContext.Decrement();

                        if (suspensionContext.Counter == 0)
                        {
                            _changeNotificationsSuspensionContext = null;
                        }
                    }
                }

                if (raiseOnResume)
                {
                    if (suspensionContext != null && suspensionContext.Counter == 0)
                    {
                        var properties = suspensionContext.Properties;

                        foreach (var property in properties)
                        {
                            RaisePropertyChanged(property);
                        }
                    }
                }
            });

            return token;
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Invokes the property changed for all registered properties.
        /// </summary>
        /// <remarks>
        /// Using this method does not set the <see cref="IsDirty"/> property to <c>true</c>.
        /// </remarks>
        internal void RaisePropertyChangedForAllRegisteredProperties()
        {
            var catelTypeInfo = PropertyDataManager.GetCatelTypeInfo(GetType());
            foreach (var propertyData in catelTypeInfo.GetCatelProperties())
            {
                if (!IsModelBaseProperty(propertyData.Key))
                {
                    RaisePropertyChanged(this, new PropertyChangedEventArgs(propertyData.Key), false, true);
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is overriden en does not call the base because lots of additional logic is added in this class. The
        /// <see cref="RaisePropertyChanged(object,System.ComponentModel.PropertyChangedEventArgs,bool,bool)"/> will explicitly call 
        /// <see cref="ObservableObject.RaisePropertyChanged(object, PropertyChangedEventArgs)"/>.
        /// <para />
        /// If this method is overriden, it is very important to call the base.
        /// </remarks>
        protected override void RaisePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(sender, e, true, false);
        }

        /// <summary>
        /// Invoked when a property value has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="updateIsDirty">if set to <c>true</c>, the <see cref="IsDirty"/> property is set and automatic validation is allowed.</param>
        /// <param name="isRefreshCallOnly">if set to <c>true</c>, the call is only to refresh updates (for example, for the IDataErrorInfo 
        /// implementation). If this value is <c>false</c>, the custom change handlers will not be called.</param>
        protected void RaisePropertyChanged(object sender, PropertyChangedEventArgs e, bool updateIsDirty, bool isRefreshCallOnly)
        {
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                if (!DisablePropertyChangeNotifications)
                {
                    // Call & exit, we can't handle "update them all" property change notifications
                    base.RaisePropertyChanged(this, new PropertyChangedEventArgs(e.PropertyName));
                }

                return;
            }

            // If this is an internal data object base property, just leave
            if (IsModelBaseProperty(e.PropertyName))
            {
                var senderAsModelBase = sender as ModelBase;
                if ((senderAsModelBase != null) && (string.Equals(e.PropertyName, IsDirtyProperty.Name, StringComparison.Ordinal)))
                {
                    // Maybe this is a child object informing us that it's not dirty any longer
                    if (!senderAsModelBase.GetValue<bool>(e.PropertyName) && !ReferenceEquals(this, sender))
                    {
                        // Ignore
                        return;
                    }

                    // A child became dirty, we are dirty as well
                    if (!ReferenceEquals(this, sender))
                    {
                        IsDirty = true;
                    }
                    else
                    {
                        if (!DisablePropertyChangeNotifications)
                        {
                            // Explicitly call base because we have overridden the behavior
                            var eventArgs = new PropertyChangedEventArgs(e.PropertyName);
                            base.RaisePropertyChanged(this, eventArgs);
                        }
                    }

                    return;
                }
            }

            if (ReferenceEquals(this, sender))
            {
                if (!isRefreshCallOnly)
                {
                    SuspensionContext callbackSuspensionContext;

                    lock (_lock)
                    {
                        callbackSuspensionContext = _changeCallbacksSuspensionContext;
                    }

                    if (callbackSuspensionContext != null)
                    {
                        callbackSuspensionContext.Add(e.PropertyName);
                    }
                    else if (IsPropertyRegistered(e.PropertyName))
                    {
                        var propertyData = GetPropertyData(e.PropertyName);

                        var handler = propertyData.PropertyChangedEventHandler;
                        if (handler != null)
                        {
                            handler(this, e);
                        }
                    }
                }

                if (!DisablePropertyChangeNotifications)
                {
                    // Explicitly call base because we have overridden the behavior
                    base.RaisePropertyChanged(this, e);
                }
            }

            if (updateIsDirty)
            {
                SetDirty(e.PropertyName);
            }
        }

        /// <summary>
        /// Determines whether a specific property change should update <c>IsDirty</c> to <c>true</c>.
        /// </summary>
        /// <returns><c>true</c> if <c>IsDirty</c> should be set to <c>true</c> when the specified property has changed, <c>false</c> otherwise.</returns>
        protected virtual bool ShouldPropertyChangeUpdateIsDirty(string propertyName)
        {
            if (propertyName.Equals(nameof(IsDirty)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void SetDirty(string propertyName)
        {
            if (ShouldPropertyChangeUpdateIsDirty(propertyName))
            {
                IsDirty = true;
            }
        }
        #endregion
    }
}

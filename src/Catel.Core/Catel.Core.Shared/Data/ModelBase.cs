// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Xml.Serialization;
    using IoC;
    using Logging;
    using Runtime.Serialization;

#if NET
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
#if NET
    [Serializable]
#endif
    public abstract partial class ModelBase : ObservableObject, IModel
    {
        #region Fields
        /// <summary>
        /// The log.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#if NET
        /// <summary>
        /// The empty streaming context.
        /// </summary>
        [field: NonSerialized]
        private static readonly StreamingContext EmptyStreamingContext = new StreamingContext();
#endif

        /// <summary>
        /// The property values.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        internal readonly PropertyBag _propertyBag = new PropertyBag();

        /// <summary>
        /// Lock object.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        internal readonly object _lock = new object();

        /// <summary>
        /// Backing field for the <see cref="LeanAndMeanModel"/> property. Because it has custom logic, it needs a backing field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _leanAndMeanModel;

#if NET
        [field: NonSerialized]
#endif
        internal SuspensionContext _changeCallbacksSuspensionContext;

#if NET
        [field: NonSerialized]
#endif
        internal SuspensionContext _changeNotificationsSuspensionContext;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ModelBase"/> class.
        /// </summary>
        static ModelBase()
        {
            PropertyDataManager = PropertyDataManager.Default;

            DefaultSerializer = IoCConfiguration.DefaultDependencyResolver.Resolve<ISerializer>();
        }

#if !NET
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        protected ModelBase()
        {
            // Note: this initializes the model without serialization context

            Initialize();
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        /// <remarks>
        /// Must have a public constructor in order to be serializable.
        /// </remarks>
        // ReSharper disable PublicConstructorInAbstractClass
        protected ModelBase()
            // ReSharper restore PublicConstructorInAbstractClass
            : this(null, EmptyStreamingContext)
        {
            // Do not write anything in this constructor. Use the Initialize method or the
            // OnInitializing or OnInitialized methods instead.
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether all models should behave as a lean and mean model.
        /// <para />
        /// To find out what lean and mean means, see <see cref="LeanAndMeanModel"/>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if all models should behave as lean and mean; otherwise, <c>false</c>.</value>
#if NET
        [Browsable(false)]
#endif
        [XmlIgnore]
        public static bool GlobalLeanAndMeanModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this model should behave as a lean and mean model.
        /// <para />
        /// A lean and mean model will not raise any change notification events.
        /// </summary>
        /// <value><c>true</c> if this is a lean and mean model; otherwise, <c>false</c>.</value>
#if NET
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected internal bool LeanAndMeanModel
        {
            get { return _leanAndMeanModel || GlobalLeanAndMeanModel; }
            set { _leanAndMeanModel = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether property change notifications are currently disabled for all instances.
        /// </summary>
        /// <value><c>true</c> if property change notifications should be disabled for all instances; otherwise, <c>false</c>.</value>
        /// TODO: Try to revert to internal but is required by XAMARIN_FORMS
#if NET
        [Browsable(false)]
#endif
        [XmlIgnore]
        public static bool DisablePropertyChangeNotifications { get; set; }

        /// <summary>
        /// Gets the property data manager that manages the properties of this object.
        /// </summary>
        /// <value>The property data manager.</value>
#if NET
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
#if NET
        [Browsable(false)]
#endif
        protected bool AlwaysInvokeNotifyChanged { get; set; }

        /// <summary>
        /// Gets the name of the object. By default, this is the hash code of all the properties combined.
        /// </summary>
        /// <value>The name of the key.</value>
#if NET
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
#if NET
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
        public static readonly PropertyData IsDirtyProperty = RegisterProperty("IsDirty", typeof(bool), false, null, false, true, true);

        /// <summary>
        /// Gets or sets a value indicating whether this object is currently read-only. When the object is read-only, values can only be read, not set.
        /// </summary>
#if NET
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
        public static readonly PropertyData IsReadOnlyProperty = RegisterProperty("IsReadOnly", typeof(bool), false,
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
            Serializer = DefaultSerializer;
            SerializationConfiguration = DefaultSerializationConfiguration;

            AlwaysInvokeNotifyChanged = false;

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
                    if (_changeCallbacksSuspensionContext == null)
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
                    if (_changeNotificationsSuspensionContext == null)
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
        /// <see cref="ObservableObject.RaisePropertyChanged(object, AdvancedPropertyChangedEventArgs)"/>.
        /// <para />
        /// If this method is overriden, it is very important to call the base.
        /// </remarks>
        protected override void RaisePropertyChanged(object sender, AdvancedPropertyChangedEventArgs e)
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
                    base.RaisePropertyChanged(this, new AdvancedPropertyChangedEventArgs(sender, e.PropertyName));
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
                    if (senderAsModelBase.GetValue<bool>(e.PropertyName) == false)
                    {
                        if (!ReferenceEquals(this, sender))
                        {
                            // Ignore
                            return;
                        }
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
                            var eventArgs = new AdvancedPropertyChangedEventArgs(sender, this, e.PropertyName);
                            base.RaisePropertyChanged(this, eventArgs);
                        }
                    }

                    return;
                }
            }

            if (ReferenceEquals(this, sender))
            {
                AdvancedPropertyChangedEventArgs eventArgs;
                var advancedEventArgs = e as AdvancedPropertyChangedEventArgs;
                if (advancedEventArgs != null)
                {
                    eventArgs = new AdvancedPropertyChangedEventArgs(this, advancedEventArgs);
                }
                else
                {
                    eventArgs = new AdvancedPropertyChangedEventArgs(sender, this, e.PropertyName);
                }

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
                            handler(this, eventArgs);
                        }
                    }
                }

                if (!DisablePropertyChangeNotifications)
                {
                    // Explicitly call base because we have overridden the behavior
                    base.RaisePropertyChanged(this, eventArgs);
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

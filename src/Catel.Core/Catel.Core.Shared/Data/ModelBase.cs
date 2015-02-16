﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using IoC;
    using Logging;
    using Runtime.Serialization;

    #region Enums
    /// <summary>
    /// Enumeration containing all the available serialization modes for the <see cref="ModelBase"/> class.
    /// </summary>
    public enum SerializationMode
    {
#if NET
        /// <summary>
        /// Serialize using the <see cref="System.Runtime.Serialization.Formatters.Binary.BinaryFormatter"/>.
        /// </summary>
        Binary,
#endif

        /// <summary>
        /// Serialize using the <see cref="DataContractSerializer"/>.
        /// </summary>
        Xml
    }
    #endregion

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
#if NET
    [Serializable]
#endif
    public abstract partial class ModelBase : ObservableObject, IModel
    {
        #region Constants
        /// <summary>
        /// The type that is used for internal serialization.
        /// </summary>
        internal static readonly Type InternalSerializationType = typeof(List<PropertyValue>);
        #endregion

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
        /// Dictionary of initialized types.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly HashSet<Type> _initializedTypes = new HashSet<Type>();

        /// <summary>
        /// Lock object for the <see cref="_initializedTypes"/> field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly object _initializedTypesLock = new object();

        /// <summary>
        /// The property values.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        internal readonly PropertyBag _propertyBag = new PropertyBag();

        /// <summary>
        /// The change notification wrappers for all property values.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ChangeNotificationWrapper> _propertyValueChangeNotificationWrappers = new Dictionary<string, ChangeNotificationWrapper>();

        /// <summary>
        /// Lock object for the <see cref="_propertyBag"/> field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        internal readonly object _propertyValuesLock = new object();

        /// <summary>
        /// The parent object of the current object.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private IParent _parent;

        /// <summary>
        /// Backing field for the <see cref="LeanAndMeanModel"/> property. Because it has custom logic, it needs a backing field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private bool _leanAndMeanModel;

        /// <summary>
        /// Backing field for the <see cref="EqualityComparer"/> property. Because it has custom logic, it needs a backing field.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private IModelEqualityComparer _equalityComparer;

        /// <summary>
        /// Backing field for the <see cref="GetHashCode"/> method so it only has to be calculated once to gain the best performance possible.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private int? _hashCode;

#if NET
        [field: NonSerialized]
#endif
        private event EventHandler<EventArgs> _initialized;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ModelBase"/> class.
        /// </summary>
        static ModelBase()
        {
            PropertyDataManager = PropertyDataManager.Default;
            DefaultValidateUsingDataAnnotationsValue = true;
        }

#if !NET
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        protected ModelBase()
        {
            OnInitializing();

            Initialize();

            FinishInitializationAfterConstructionOrDeserialization();

            OnInitialized();
        }
#else
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase"/> class.
        /// </summary>
        /// <remarks>
        /// Must have a public constructor in order to be serializable.
        /// </remarks>
        // ReSharper disable PublicConstructorInAbstractClass
        public ModelBase()
            // ReSharper restore PublicConstructorInAbstractClass
            : this(null, EmptyStreamingContext)
        {
            // Do not write anything in this constructor. Use the Initialize method or the
            // OnInitializing or OnInitialized methods instead.
        }
#endif
        #endregion

        #region Events
        /// <summary>
        /// Occurs when the object is initialized.
        /// </summary>
        event EventHandler<EventArgs> IModel.Initialized
        {
            add { _initialized += value; }
            remove { _initialized -= value; }
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(ModelBase firstObject, ModelBase secondObject)
        {
            return Equals(firstObject, secondObject);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="firstObject">The first object.</param>
        /// <param name="secondObject">The second object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(ModelBase firstObject, ModelBase secondObject)
        {
            return !(firstObject == secondObject);
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the equality comparer used to compare model bases with each other.
        /// </summary>
        /// <value>The equality comparer.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected IModelEqualityComparer EqualityComparer
        {
            get
            {
                if (_equalityComparer == null)
                {
                    var dependencyResolver = this.GetDependencyResolver();

                    _equalityComparer = dependencyResolver.Resolve<IModelEqualityComparer>();
                }

                return _equalityComparer;
            }
            set
            {
                _equalityComparer = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether all models should behave as a lean and mean model.
        /// <para />
        /// To find out what lean and mean means, see <see cref="LeanAndMeanModel"/>.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if all models should behave as lean and mean; otherwise, <c>false</c>.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public static bool GlobalLeanAndMeanModel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this model should behave as a lean and mean model.
        /// <para />
        /// A lean and mean model will not handle any validation code, nor will it raise any change notification events.
        /// </summary>
        /// <value><c>true</c> if this is a lean and mean model; otherwise, <c>false</c>.</value>
#if NET || SILVERLIGHT
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
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        internal static bool DisablePropertyChangeNotifications { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether event subscriptions of child values should be disabled.
        /// <para />
        /// The default value is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if event subscriptions of child values should be disabled; otherwise, <c>false</c>.</value>
        protected bool DisableEventSubscriptionsOfChildValues { get; set; }

        /// <summary>
        /// Gets the property data manager that manages the properties of this object.
        /// </summary>
        /// <value>The property data manager.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        internal static PropertyDataManager PropertyDataManager { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object is subscribed to all childs.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        private bool SubscribedToEvents { get; set; }

        /// <summary>
        /// Gets a value indicating whether this object is currently initializing.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is currently initializing; otherwise, <c>false</c>.
        /// </value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool IsInitializing { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this object is initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if this object is initialized; otherwise, <c>false</c>.
        /// </value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the deserialized data is available, which means that
        /// OnDeserialized is invoked.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        private bool IsDeserializedDataAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the object is fully deserialized.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        private bool IsDeserialized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object should always invoke the <see cref="ObservableObject.PropertyChanged"/> event,
        /// even when the actual value of a property has not changed.
        /// <para />
        /// Enabling this property is useful when using this class in a WPF environment.
        /// </summary>
        /// <remarks>
        /// By default, this property is <c>false</c>.
        /// </remarks>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        protected bool AlwaysInvokeNotifyChanged { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object should handle (thus invoke the specific events) when
        /// a property of collection value has changed.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        protected bool HandlePropertyAndCollectionChanges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this object should automatically validate itself when a property value
        /// has changed.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        protected bool AutomaticallyValidateOnPropertyChanged { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        IParent IParent.Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Gets the name of the object. By default, this is the hash code of all the properties combined.
        /// </summary>
        /// <value>The name of the key.</value>
#if NET || SILVERLIGHT
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
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsDirty
        {
            get { return GetValue<bool>(IsDirtyProperty); }
            protected set { SetValue(IsDirtyProperty, value); }
        }

        /// <summary>
        /// Register the IsDirty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IsDirtyProperty = RegisterProperty("IsDirty", typeof(bool), false, false, null, false, true, true);

        /// <summary>
        /// Gets or sets a value indicating whether this object is currently read-only. When the object is read-only, values can only be read, not set.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsReadOnly
        {
            get { return GetValue<bool>(IsReadOnlyProperty); }
            protected set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// Register the IsReadOnly property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IsReadOnlyProperty = RegisterProperty("IsReadOnly", typeof(bool), false, false,
            (sender, e) => ((ModelBase)sender).RaisePropertyChanged("IsEditable"), false, true, true);

        /// <summary>
        /// Gets a value indicating whether the deserialization has succeeded. If automatic deserialization fails, the object
        /// should try to deserialize manually.
        /// </summary>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool DeserializationSucceeded { get; private set; }

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
        /// Called when the object is being initialized.
        /// </summary>
        protected virtual void OnInitializing()
        {
            IsInitializing = true;
        }

        /// <summary>
        /// Called when the object is initialized.
        /// </summary>
        protected virtual void OnInitialized()
        {
            IsInitializing = false;
            IsInitialized = true;

            _initialized.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            _deserialized.SafeInvoke(this);
        }

        /// <summary>
        /// Initializes the object by setting default values.
        /// </summary>
        private void Initialize()
        {
            SuspendValidation = DefaultSuspendValidationValue;
            ValidateUsingDataAnnotations = DefaultValidateUsingDataAnnotationsValue;
            DeserializationSucceeded = false;
            HandlePropertyAndCollectionChanges = true;
            AlwaysInvokeNotifyChanged = false;
            AutomaticallyValidateOnPropertyChanged = true;

            var type = GetType();

#if !WINDOWS_PHONE && !NETFX_CORE && !PCL && !NET35
            lock (_propertyValuesIgnoredOrFailedForValidation)
            {
                if (!_propertyValuesIgnoredOrFailedForValidation.ContainsKey(type))
                {
                    _propertyValuesIgnoredOrFailedForValidation.Add(type, new HashSet<string>());

                    // Ignore modelbase properties
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("EqualityComparer");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("LeanAndMeanModel");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("DisableEventSubscriptionsOfChildValues");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("IsInitializing");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("IsInitialized");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("ContainsNonSerializableMembers");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("AlwaysInvokeNotifyChanged");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("HandlePropertyAndCollectionChanges");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("AutomaticallyValidateOnPropertyChanged");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("DeserializationSucceeded");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("IsValidating");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("SuspendValidation");
                    _propertyValuesIgnoredOrFailedForValidation[type].Add("HideValidationResults");
                }
            }
#endif

            InitializeProperties();

            InitializeCustomProperties();
        }

        /// <summary>
        /// Finishes the deserialization (both binary and xml)
        /// </summary>
        internal void FinishDeserialization()
        {
            Log.Debug("Finished deserialization of '{0}'", GetType().Name);

            // Data is now considered deserialized
            IsDeserialized = true;

            FinishInitializationAfterConstructionOrDeserialization();

            IsDirty = false;

            OnDeserialized();
        }

        /// <summary>
        /// Finishes the initialization after construction or deserialization.
        /// </summary>
        private void FinishInitializationAfterConstructionOrDeserialization()
        {
            var catelTypeInfo = PropertyDataManager.GetCatelTypeInfo(GetType());
            foreach (var propertyData in catelTypeInfo.GetCatelProperties())
            {
                if (propertyData.Value.SetParent)
                {
                    lock (_propertyValuesLock)
                    {
                        var propertyValue = GetValueFast(propertyData.Key);
                        var propertyValueAsModelBase = propertyValue as ModelBase;
                        var propertyValueAsIEnumerable = propertyValue as IEnumerable;

                        if (propertyValueAsModelBase != null)
                        {
                            propertyValueAsModelBase.SetParent(this);
                        }
                        else if (propertyValueAsIEnumerable != null)
                        {
                            foreach (var obj in propertyValueAsIEnumerable)
                            {
                                var objAsModelBase = obj as ModelBase;
                                if (objAsModelBase != null)
                                {
                                    objAsModelBase.SetParent(this);
                                }
                            }
                        }
                    }
                }
            }

            //SubscribeAllObjectsToNotifyChangedEvents();
        }

        /// <summary>
        /// Sets the new parent of this object.
        /// </summary>
        /// <param name="parent">The new parent.</param>
        protected void SetParent(IParent parent)
        {
            _parent = parent;

            RaisePropertyChanged("Parent");
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            // Note: at first we only implemented the EqualityComparer, but the IEqualityComparer of Microsoft
            // throws an exception when the 2 types are not the same. Although MS does recommend not to throw exceptions,
            // they do it themselves. Check for null and check the types before feeding it to the equality comparer.

            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var equalityComparer = EqualityComparer;
            return equalityComparer.Equals(this, obj);
        }

        // ReSharper disable RedundantOverridenMember

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
            {
                var equalityComparer = EqualityComparer;
                _hashCode = equalityComparer.GetHashCode(this);
            }

            return _hashCode.Value;
        }

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
        /// Clears the <see cref="IsDirty"/> on all childs.
        /// </summary>
        protected void ClearIsDirtyOnAllChilds()
        {
            ClearIsDirtyOnAllChilds(this, new HashSet<IModel>());
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> on all childs.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="handledReferences">The already handled references, required to prevent circular stackoverflows.</param>
        private static void ClearIsDirtyOnAllChilds(object obj, HashSet<IModel> handledReferences)
        {
            var objAsModelBase = obj as ModelBase;
            var objAsIEnumerable = obj as IEnumerable;

            if (objAsModelBase != null)
            {
                if (handledReferences.Contains(objAsModelBase))
                {
                    return;
                }

                objAsModelBase.IsDirty = false;
                handledReferences.Add(objAsModelBase);

                var catelTypeInfo = PropertyDataManager.GetCatelTypeInfo(obj.GetType());
                foreach (var property in catelTypeInfo.GetCatelProperties())
                {
                    object value = objAsModelBase.GetValue(property.Value);

                    ClearIsDirtyOnAllChilds(value, handledReferences);
                }
            }
            else if (objAsIEnumerable != null)
            {
                foreach (var childItem in objAsIEnumerable)
                {
                    if (childItem is ModelBase)
                    {
                        ClearIsDirtyOnAllChilds(childItem, handledReferences);
                    }
                }
            }
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

            lock (_propertyValuesLock)
            {
                if (_propertyValueChangeNotificationWrappers.ContainsKey(propertyName))
                {
                    var oldWrapper = _propertyValueChangeNotificationWrappers[propertyName];
                    if (oldWrapper != null)
                    {
                        oldWrapper.PropertyChanged -= OnPropertyObjectPropertyChanged;
                        oldWrapper.CollectionChanged -= OnPropertyObjectCollectionChanged;
                        oldWrapper.CollectionItemPropertyChanged -= OnPropertyObjectCollectionItemPropertyChanged;
                        oldWrapper.UnsubscribeFromAllEvents();
                    }
                }

                if (!ChangeNotificationWrapper.IsUsefulForObject(propertyValue))
                {
                    _propertyValueChangeNotificationWrappers[propertyName] = null;
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
            if (string.IsNullOrEmpty(e.PropertyName))
            {
                return;
            }

            RaisePropertyChanged(sender, e, true, false);
        }

        /// <summary>
        /// Called when a property that implements <see cref="INotifyCollectionChanged"/> raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SetDirtyAndAutomaticallyValidate(string.Empty, true);
        }

        /// <summary>
        /// Called when a property inside a collection that implements <see cref="INotifyCollectionChanged"/> that implements
        /// <see cref="INotifyPropertyChanged"/> raises the event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyObjectCollectionItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, "IsDirty", StringComparison.Ordinal))
            {
                return;
            }

            SetDirtyAndAutomaticallyValidate(string.Empty, true);
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        /// Invokes the property changed for all registered properties.
        /// </summary>
        /// <remarks>
        /// Using this method does not set the <see cref="IsDirty"/> property to <c>true</c>, nor will
        /// it cause the object to validate itself automatically, even when the <see cref="AutomaticallyValidateOnPropertyChanged"/>
        /// is set to <c>true</c>.
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
        /// <param name="setDirtyAndAllowAutomaticValidation">if set to <c>true</c>, the <see cref="IsDirty"/> property is set and automatic validation is allowed.</param>
        /// <param name="isRefreshCallOnly">if set to <c>true</c>, the call is only to refresh updates (for example, for the IDataErrorInfo 
        /// implementation). If this value is <c>false</c>, the custom change handlers will not be called.</param>
        private void RaisePropertyChanged(object sender, PropertyChangedEventArgs e, bool setDirtyAndAllowAutomaticValidation, bool isRefreshCallOnly)
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

            if (HandlePropertyAndCollectionChanges)
            {
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
                        if (IsPropertyRegistered(e.PropertyName))
                        {
                            var propertyData = GetPropertyData(e.PropertyName);
                            if (propertyData.PropertyChangedEventHandler != null)
                            {
                                propertyData.PropertyChangedEventHandler(this, eventArgs);
                            }
                        }
                    }

                    if (!DisablePropertyChangeNotifications)
                    {
                        // Explicitly call base because we have overridden the behavior
                        base.RaisePropertyChanged(this, eventArgs);
                    }
                }
            }

            SetDirtyAndAutomaticallyValidate(e.PropertyName, setDirtyAndAllowAutomaticValidation);
        }

        /// <summary>
        /// Sets the <see cref="IsDirty"/> property and automatically validate if required.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="setDirtyAndAllowAutomaticValidation">If set to <c>true</c>, the <see cref="IsDirty"/> property is set and automatic validation is allowed.</param>
        private void SetDirtyAndAutomaticallyValidate(string propertyName, bool setDirtyAndAllowAutomaticValidation)
        {
            // Are we not validating or is this a warning or error message?
            if (setDirtyAndAllowAutomaticValidation && !IsValidating &&
                (string.CompareOrdinal(propertyName, WarningMessageProperty) != 0) &&
                (string.CompareOrdinal(propertyName, HasWarningsMessageProperty) != 0) &&
                (string.CompareOrdinal(propertyName, ErrorMessageProperty) != 0) &&
                (string.CompareOrdinal(propertyName, HasErrorsMessageProperty) != 0))
            {
                IsDirty = true;
                IsValidated = false;
            }

            if (AutomaticallyValidateOnPropertyChanged && setDirtyAndAllowAutomaticValidation)
            {
                Validate();
            }
        }
        #endregion
    }
}

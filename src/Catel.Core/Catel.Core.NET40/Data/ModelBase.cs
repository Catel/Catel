// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Collections;

    using Reflection;
    using Logging;
    using Runtime.Serialization;

    #region Enums
    /// <summary>
    /// Enumeration containing all the available serialization modes for the <see cref="ModelBase{TModel}"/> class.
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

        /// <summary>
        /// Dictionary of initialized types.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private static readonly Dictionary<Type, bool> _initializedTypes = new Dictionary<Type, bool>();

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

#if !WP7
        /// <summary>
        /// The change notification wrappers for all property values.
        /// </summary>
#if NET
        [field: NonSerialized]
#endif
        private readonly Dictionary<string, ChangeNotificationWrapper> _propertyValueChangeNotificationWrappers = new Dictionary<string, ChangeNotificationWrapper>();
#endif

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
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="ModelBase"/> class.
        /// </summary>
        static ModelBase()
        {
            PropertyDataManager = PropertyDataManager.Default;
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
            : this(null, new StreamingContext())
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
#if NET
        [field: NonSerialized]
#endif
        public event EventHandler Initialized;
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
            if (ReferenceEquals(firstObject, secondObject))
            {
                return true;
            }

            if (((object)firstObject == null) || ((object)secondObject == null))
            {
                return false;
            }

            // Fix for issue 6633 (see http://catel.codeplex.com/workitem/6633)
            // Check types before the "expensive" operation of checking all property values
            if (firstObject.GetType() != secondObject.GetType())
            {
                return false;
            }

            lock (firstObject._propertyValuesLock)
            {
                foreach (var propertyValue in firstObject._propertyBag.GetAllProperties())
                {
                    // Only check if this is not an internal data object base property
                    if (!firstObject.IsModelBaseProperty(propertyValue.Key))
                    {
                        object valueA = propertyValue.Value;
                        if (!secondObject.IsPropertyRegistered(propertyValue.Key))
                        {
                            return false;
                        }

                        object valueB = secondObject.GetValue(propertyValue.Key);

                        if (!ReferenceEquals(valueA, valueB))
                        {
                            if ((valueA == null) || (valueB == null))
                            {
                                return false;
                            }

                            // Is this an IEnumerable (but not a string)?
                            var valueAAsIEnumerable = valueA as IEnumerable;
                            if ((valueAAsIEnumerable != null) && !(valueA is string))
                            {
                                // Yes, loop all sub items and check them
                                if (!CollectionHelper.IsEqualTo(valueAAsIEnumerable, (IEnumerable)valueB))
                                {
                                    return false;
                                }
                            }
                            else
                            {
                                // No, check objects via equals method
                                if (!valueA.Equals(valueB))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
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
        protected bool LeanAndMeanModel
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
        /// Gets a value indicating whether this instance contains non-serializable members.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance contains non-serializable members; otherwise, <c>false</c>.
        /// </value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        protected bool ContainsNonSerializableMembers { get; private set; }

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
        /// Gets the <see cref="SerializationMode"/> of this object.
        /// </summary>
        /// <value>The serialization mode.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public SerializationMode Mode { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the object is currently in an edit session, started by the <see cref="IEditableObject.BeginEdit"/> method.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is currently in an edit session; otherwise, <c>false</c>.
        /// </value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsInEditSession
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
        /// Gets a value indicating whether this object is editable. This is the opposite of the <see cref="IsReadOnly"/> property.
        /// </summary>
        /// <value><c>true</c> if this object is editable; otherwise, <c>false</c>.</value>
#if NET || SILVERLIGHT
        [Browsable(false)]
#endif
        [XmlIgnore]
        public bool IsEditable
        {
            get { return !IsReadOnly; }
        }

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

            Initialized.SafeInvoke(this);
        }

        /// <summary>
        /// Called when the object is deserialized.
        /// </summary>
        protected virtual void OnDeserialized()
        {
            Deserialized.SafeInvoke(this);
        }

        /// <summary>
        /// Initializes the object by setting default values.
        /// </summary>
        private void Initialize()
        {
            ValidationContext = new ValidationContext();

            DeserializationSucceeded = false;
            HandlePropertyAndCollectionChanges = true;
            AlwaysInvokeNotifyChanged = false;
            AutomaticallyValidateOnPropertyChanged = true;
#if NET
            Mode = SerializationMode.Binary;
#else
            Mode = SerializationMode.Xml;
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
            foreach (var propertyData in PropertyDataManager.GetProperties(GetType()))
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
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal void SetValue(string name, object value)
        {
            SetValue(name, value, true, true);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <param name="validateAttributes">If set to <c>true</c>, the validation attributes on the property will be validated.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        internal void SetValue(string name, object value, bool notifyOnChange, bool validateAttributes)
        {
            var property = GetPropertyData(name);
            if ((value == null) && !TypeHelper.IsTypeNullable(property.Type))
            {
                throw new PropertyNotNullableException(name, GetType());
            }

            SetValue(property, value, notifyOnChange, validateAttributes);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <param name="value">Value of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal void SetValue(PropertyData property, object value)
        {
            Argument.IsNotNull("property", property);

            SetValue(property, value, true, true);
        }

        /// <summary>
        /// Sets the value of a specific property.
        /// </summary>
        /// <param name="property">The property to set.</param>
        /// <param name="value">Value of the property.</param>
        /// <param name="notifyOnChange">If <c>true</c>, the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be invoked.</param>
        /// <param name="validateAttributes">If set to <c>true</c>, the validation attributes on the property will be validated.</param>
        /// <exception cref="PropertyNotNullableException">The property is not nullable, but <paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        internal void SetValue(PropertyData property, object value, bool notifyOnChange, bool validateAttributes)
        {
            Argument.IsNotNull("property", property);

            // Is the object currently read-only (and aren't we changing that)?
            if (IsReadOnly)
            {
                if (property != IsReadOnlyProperty)
                {
                    Log.Warning("Cannot set property '{0}', object is currently read-only", property.Name);
                    return;
                }
            }

            if (property.IsCalculatedProperty)
            {
                Log.Warning("Cannot set property '{0}', the property is a calculated property", property.Name);
                return;
            }

            if (!LeanAndMeanModel)
            {
                if ((value != null) && !property.Type.IsInstanceOfTypeEx(value))
                {
                    if (!value.GetType().IsCOMObjectEx())
                    {
                        throw new InvalidPropertyValueException(property.Name, property.Type, value.GetType());
                    }
                }
            }

            lock (_propertyValuesLock)
            {
                object oldValue = GetValueFast(property.Name);
                bool areOldAndNewValuesEqual = ObjectHelper.AreEqualReferences(oldValue, value);

                if (notifyOnChange && (AlwaysInvokeNotifyChanged || !areOldAndNewValuesEqual) && !LeanAndMeanModel)
                {
                    var propertyChangingEventArgs = new AdvancedPropertyChangingEventArgs(property.Name);

                    RaisePropertyChanging(this, propertyChangingEventArgs);

                    if (propertyChangingEventArgs.Cancel)
                    {
                        Log.Debug("Change of property '{0}.{1}' is canceled in PropertyChanging event", GetType().FullName, property.Name);
                        return;
                    }
                }

                // Validate before assigning, dynamic properties will cause exception
                if (validateAttributes && !LeanAndMeanModel)
                {
                    ValidatePropertyUsingAnnotations(property.Name, value);
                }

                if (!areOldAndNewValuesEqual)
                {
                    SetValueFast(property.Name, value);
                }

                if (notifyOnChange && (AlwaysInvokeNotifyChanged || !areOldAndNewValuesEqual) && !LeanAndMeanModel)
                {
                    RaisePropertyChanged(property.Name, oldValue, value);
                }
            }
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
        private void SetValueFast(string propertyName, object value)
        {
            lock (_propertyValuesLock)
            {
                _propertyBag.SetPropertyValue(propertyName, value);

                HandleObjectEventsSubscription(propertyName, value);

                IsValidated = false;
            }
        }

        /// <summary>
        /// Gets the value fast without checking for any constraints. This means that if this method is used incorrectly,
        /// it can throw random exceptions.
        /// <para />
        /// This is a wrapper around the _propertyValues field. Don't use the field directly, always use
        /// this method because it takes care of locking and event subscriptions.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The value of the property.</returns>
        internal object GetValueFast(string propertyName)
        {
            return _propertyBag.GetPropertyValue<object>(propertyName);
        }

        /// <summary>
        /// Gets the value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected internal object GetValue(string name)
        {
            Argument.IsNotNullOrEmpty("name", name);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), name);

            return GetValue(propertyData);
        }

        /// <summary>
        /// Gets the typed value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected TValue GetValue<TValue>(string name)
        {
            Argument.IsNotNullOrEmpty("name", name);

            var propertyData = PropertyDataManager.GetPropertyData(GetType(), name);

            return GetValue<TValue>(propertyData);
        }

        /// <summary>
        /// Gets the value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected object GetValue(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            if (property.IsCalculatedProperty)
            {
                return PropertyHelper.GetPropertyValue(this, property.Name);
            }

            return GetValueFast(property.Name);
        }

        /// <summary>
        /// Gets the typed value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Object value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected TValue GetValue<TValue>(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            object obj = GetValue(property);

            return ((obj != null) && (obj is TValue)) ? (TValue)obj : default(TValue);
        }

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        object IModel.GetDefaultValue(string name)
        {
            return GetPropertyData(name).GetDefaultValue();
        }

        /// <summary>
        /// Returns the default value of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        object IModel.GetDefaultValue(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            return ((IModel)this).GetDefaultValue(property.Name);
        }

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the 1.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModel.GetDefaultValue<TValue>(string name)
        {
            object obj = ((IModel)this).GetDefaultValue(name);

            return (obj is TValue) ? (TValue)obj : default(TValue);
        }

        /// <summary>
        /// Returns the typed default value of a specific property.
        /// </summary>
        /// <typeparam name="TValue">The type of the 1.</typeparam>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Default value of the property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        TValue IModel.GetDefaultValue<TValue>(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            return ((IModel)this).GetDefaultValue<TValue>(property.Name);
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
            // ReSharper disable RedundantCast

            if ((object)obj == null)
            {
                return false;
            }

            // ReSharper restore RedundantCast

            var objAsModelBase = obj as ModelBase;
            if (objAsModelBase == null)
            {
                return false;
            }

            // ReSharper disable RedundantCast

            return (ModelBase)this == objAsModelBase;

            // ReSharper restore RedundantCast
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
            return base.GetHashCode();
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
            ClearIsDirtyOnAllChilds(this, new List<IModel>());
        }

        /// <summary>
        /// Clears the <see cref="IsDirty"/> on all childs.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="handledReferences">The already handled references, required to prevent circular stackoverflows.</param>
        private static void ClearIsDirtyOnAllChilds(object obj, List<IModel> handledReferences)
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

                var properties = PropertyDataManager.GetProperties(obj.GetType());
                foreach (var property in properties)
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
#if !WP7
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

                if (ObjectHelper.IsNull(propertyValue))
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
#endif
        }

#if !WP7
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
            SetDirtyAndAutomaticallyValidate(string.Empty, true);
        }

#endif
        #endregion

        #region Property handling

#if !NETFX_CORE
        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">If set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">If set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">If set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, TValue defaultValue, 
            Action<TModel, AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

#if !PCL
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("The member type of the body of the property expression should be a property");
            }
#endif

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty(propertyName, typeof(TValue), defaultValue, (sender, args) =>
                {
                    if (propertyChangedEventHandler != null)
                    {
                        propertyChangedEventHandler.Invoke((TModel)sender, args);
                    }
                }, includeInSerialization, includeInBackup, setParent);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TModel">The model type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">If set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">If set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns><see cref="PropertyData" /> containing the property information.</returns>
        /// <exception cref="System.ArgumentException">The member type of the body of the <paramref name="propertyExpression" /> of should be <c>MemberTypes.Property</c>.</exception>
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyExpression" /> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TModel, TValue>(Expression<Func<TModel, TValue>> propertyExpression, Func<TValue> createDefaultValue = null,
            Action<TModel, AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true, 
            bool includeInBackup = true, bool setParent = true)
        {
            Argument.IsNotNull("propertyExpression", propertyExpression);
            Argument.IsOfType("propertyExpression.Body", propertyExpression.Body, typeof(MemberExpression));

            var memberExpression = (MemberExpression)propertyExpression.Body;

#if !PCL
            if (memberExpression.Member.MemberType != MemberTypes.Property)
            {
                throw new ArgumentException("The member type of the body of the property expression should be a property");
            }
#endif

            object realDefaultValue = createDefaultValue;
            if (createDefaultValue == null)
            {
                realDefaultValue = default(TValue);
            }

            var propertyName = memberExpression.Member.Name;
            return RegisterProperty(propertyName, typeof(TValue), realDefaultValue, (sender, args) =>
            {
                if (propertyChangedEventHandler != null)
                {
                    propertyChangedEventHandler.Invoke((TModel)sender, args);
                }
            }, includeInSerialization, includeInBackup, setParent);
        }
#endif

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty<TValue>(string name, Type type, TValue defaultValue,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            var value = defaultValue as Delegate;
            if (value != null)
            {
                return RegisterProperty(name, type, () => value.DynamicInvoke(), setParent, propertyChangedEventHandler,
                    includeInSerialization, includeInBackup, false);
            }

            return RegisterProperty(name, type, () => defaultValue, setParent, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        public static PropertyData RegisterProperty(string name, Type type, Func<object> createDefaultValue = null,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool setParent = true)
        {
            return RegisterProperty(name, type, createDefaultValue, setParent, propertyChangedEventHandler, includeInSerialization,
                includeInBackup, false);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static PropertyData RegisterProperty<TValue>(string name, Type type, TValue defaultValue, bool setParent = true,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            var value = defaultValue as Delegate;
            if (value != null)
            {
                return RegisterProperty(name, type, () => value.DynamicInvoke(), setParent, propertyChangedEventHandler,
                    includeInSerialization, includeInBackup, isModelBaseProperty);
            }

            return RegisterProperty(name, type, () => defaultValue, setParent, propertyChangedEventHandler,
                includeInSerialization, includeInBackup, isModelBaseProperty);
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="createDefaultValue">The delegate that creates the default value. If <c>null</c>, a delegate returning the default 
        /// value (<c>null</c> for reference types, <c>Activator.CreateInstance(type)</c> for value types).</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        /// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> is <c>null</c>.</exception>
        private static PropertyData RegisterProperty(string name, Type type, Func<object> createDefaultValue = null, bool setParent = true,
            EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true,
            bool includeInBackup = true, bool isModelBaseProperty = false)
        {
            Argument.IsNotNullOrWhitespace("name", name);
            Argument.IsNotNull("type", type);

            if (createDefaultValue == null)
            {
                createDefaultValue = () => type.IsValueTypeEx() ? Activator.CreateInstance(type) : null;
            }

            var isSerializable = true;

#if NET
            isSerializable = type.GetTypeInfo().IsInterface || type.GetTypeInfo().IsSerializable;
#endif

            var property = new PropertyData(name, type, createDefaultValue, setParent, propertyChangedEventHandler, isSerializable,
                includeInSerialization, includeInBackup, isModelBaseProperty, false);
            return property;
        }

        /// <summary>
        /// Initializes all the properties for this object.
        /// </summary>
        private void InitializeProperties()
        {
            var type = GetType();

            var registeredPropertyData = PropertyDataManager.RegisterProperties(type);

            foreach (var propertyData in registeredPropertyData)
            {
                if (!propertyData.IsSerializable)
                {
                    object[] allowNonSerializableMembersAttributes = type.GetCustomAttributesEx(typeof(AllowNonSerializableMembersAttribute), true);
                    if (allowNonSerializableMembersAttributes.Length == 0)
                    {
                        throw new InvalidPropertyException(propertyData.Name);
                    }

                    ContainsNonSerializableMembers = true;
                }

                InitializeProperty(propertyData);
            }

            lock (_initializedTypesLock)
            {
                if (!_initializedTypes.ContainsKey(type))
                {
                    _initializedTypes.Add(type, true);
                }
            }
        }

        /// <summary>
        /// Initializes a specific property for this object after the object is already constructed and initialized.
        /// <para />
        /// Normally, properties are automatically registered in the constructor. If properties should be registered
        /// via runtime behavior, this method must be used.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        protected void InitializePropertyAfterConstruction(PropertyData property)
        {
            Argument.IsNotNull("property", property);

            bool isCalculatedProperty = false;

            var type = GetType();

            var reflectedProperty = type.GetPropertyEx(property.Name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (reflectedProperty == null)
            {
                Log.Warning("Property '{0}' is registered after construction of type '{1}', but could not be found using reflection", property.Name, type.FullName);
            }
            else
            {
                isCalculatedProperty = !reflectedProperty.CanWrite;
            }

            InitializeProperty(property, true, isCalculatedProperty);
        }

        /// <summary>
        /// Initializes a specific property for this object.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="lateRegistration">If set to <c>true</c>, the property is assumed to be registered after the official initialization.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="property" /> is <c>null</c>.</exception>
        /// <exception cref="InvalidPropertyException">The name of the property is invalid.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        private void InitializeProperty(PropertyData property, bool lateRegistration = false, bool isCalculatedProperty = false)
        {
            Argument.IsNotNull("property", property);

            InitializeProperty(property.Name, property.Type, property.GetDefaultValue(), property.SetParent, property.PropertyChangedEventHandler,
                property.IsSerializable, property.IncludeInSerialization, property.IncludeInBackup, property.IsModelBaseProperty, lateRegistration, isCalculatedProperty);
        }

        /// <summary>
        /// Initializes a specific property for this object.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="type">Type of the property.</param>
        /// <param name="defaultValue">Default value of the property.</param>
        /// <param name="setParent">if set to <c>true</c>, the parent of the property will be set.</param>
        /// <param name="propertyChangedEventHandler">The property changed event handler.</param>
        /// <param name="isSerializable">if set to <c>true</c>, the property is serializable.</param>
        /// <param name="includeInSerialization">if set to <c>true</c>, the property should be included in the serialization.</param>
        /// <param name="includeInBackup">if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.</param>
        /// <param name="isModelBaseProperty">if set to <c>true</c>, the property is declared by the <see cref="ModelBase"/>.</param>
        /// <param name="lateRegistration">if set to <c>true</c>, the property is assumed to be registered after the official initialization.</param>
        /// <param name="isCalculatedProperty">if set to <c>true</c>, the property is a calculated property.</param>
        /// <exception cref="InvalidPropertyException">The <paramref name="name"/> is <c>null</c> or whitespace.</exception>
        /// <exception cref="PropertyAlreadyRegisteredException">The property is already registered.</exception>
        private void InitializeProperty(string name, Type type, object defaultValue, bool setParent, EventHandler<AdvancedPropertyChangedEventArgs> propertyChangedEventHandler,
            bool isSerializable, bool includeInSerialization, bool includeInBackup, bool isModelBaseProperty, bool lateRegistration, bool isCalculatedProperty)
        {
            Argument.IsNotNullOrWhitespace("name", name);

            var objectType = GetType();
            if ((defaultValue == null) && !TypeHelper.IsTypeNullable(type))
            {
                throw new PropertyNotNullableException(name, objectType);
            }

            lock (_initializedTypesLock)
            {
                if (!_initializedTypes.ContainsKey(objectType) || !_initializedTypes[objectType] || lateRegistration)
                {
                    if (!IsPropertyRegistered(name))
                    {
                        var propertyData = new PropertyData(name, type, defaultValue, setParent, propertyChangedEventHandler,
                            isSerializable, includeInSerialization, includeInBackup, isModelBaseProperty, isCalculatedProperty);
                        PropertyDataManager.RegisterProperty(objectType, name, propertyData);
                    }
                }
            }

            lock (_propertyValuesLock)
            {
                if (!_propertyBag.IsPropertyAvailable(name))
                {
                    SetValueFast(name, defaultValue);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property is a property declared by the <see cref="ModelBase"/> itself.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>
        /// <c>true</c> if the specified property is a property declared by the <see cref="ModelBase"/> itself; otherwise, <c>false</c>.
        /// </returns>
        protected bool IsModelBaseProperty(string name)
        {
            if (!IsPropertyRegistered(name))
            {
                return false;
            }

            return GetPropertyData(name).IsModelBaseProperty;
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns>True if the property is registered, otherwise false.</returns>
        protected internal bool IsPropertyRegistered(string name)
        {
            return IsPropertyRegistered(GetType(), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <typeparam name="T">Type of the object for which to check.</typeparam>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegistered<T>(string name)
        {
            return IsPropertyRegistered(typeof(T), name);
        }

        /// <summary>
        /// Returns whether a specific property is registered.
        /// </summary>
        /// <param name="type">The type of the object for which to check.</param>
        /// <param name="name">Name of the property.</param>
        /// <returns>
        /// True if the property is registered, otherwise false.
        /// </returns>
        protected static bool IsPropertyRegistered(Type type, string name)
        {
            return PropertyDataManager.IsPropertyRegistered(type, name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyData"/> for the specified property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>The <see cref="PropertyData"/>.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        protected PropertyData GetPropertyData(string name)
        {
            return PropertyDataManager.GetPropertyData(GetType(), name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the specified property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns><see cref="PropertyInfo"/> or <c>null</c> if no property info is found.</returns>
        protected PropertyInfo GetPropertyInfo(PropertyData property)
        {
            return GetPropertyInfo(property.Name);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> for the specified property.
        /// </summary>
        /// <param name="property">The name of the property.</param>
        /// <returns><see cref="PropertyInfo"/> or <c>null</c> if no property info is found.</returns>
        protected PropertyInfo GetPropertyInfo(string property)
        {
            return GetType().GetPropertyEx(property, BindingFlagsHelper.GetFinalBindingFlags(true, false));
        }

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        Type IModel.GetPropertyType(string name)
        {
            return GetPropertyData(name).Type;
        }

        /// <summary>
        /// Returns the type of a specific property.
        /// </summary>
        /// <param name="property"><see cref="PropertyData"/> of the property.</param>
        /// <returns>Type of the property.</returns>
        /// <exception cref="PropertyNotRegisteredException">The property is not registered.</exception>
        Type IModel.GetPropertyType(PropertyData property)
        {
            return ((IModel)this).GetPropertyType(property.Name);
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
            foreach (var propertyData in PropertyDataManager.GetProperties(GetType()))
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
                // Maybe this is a child object informing us that it's not dirty any longer
                var senderAsModelBase = sender as ModelBase;
                if ((senderAsModelBase != null) && (e.PropertyName == IsDirtyProperty.Name))
                {
                    if (senderAsModelBase.GetValue<bool>(e.PropertyName) == false)
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
                    if (e is AdvancedPropertyChangedEventArgs)
                    {
                        eventArgs = new AdvancedPropertyChangedEventArgs(this, (AdvancedPropertyChangedEventArgs)e);
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

        #region ICloneable Members
        /// <summary>
        /// Clones the current object.
        /// </summary>
        /// <returns>Clone of the object or <c>null</c> if unsuccessful.</returns>
        public object Clone()
        {
            // First, try without redirects (why would we even need them, it costs a lot of performance)
            object clone = Clone(false);
            if (clone != null)
            {
                return clone;
            }

            // If we got here, cloning without redirects failed, try again with redirects as a last resort
            clone = Clone(true);
            return clone;
        }

        /// <summary>
        /// Clones the current object with the option to enable redirects.
        /// </summary>
        /// <param name="enableRedirects">if set to <c>true</c>, enable supports for redirects.</param>
        /// <returns>
        /// Clone of the object or <c>null</c> if unsuccessful.
        /// </returns>
        private object Clone(bool enableRedirects)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    var serializer = SerializationFactory.GetXmlSerializer();

                    serializer.Serialize(this, stream);

                    stream.Position = 0L;

                    object clone = serializer.Deserialize(GetType(), stream);
                    return clone;
                }                
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                return null;
            }
        }
        #endregion
    }

    /// <summary>
    /// Abstract class that serves as a base class for serializable objects.
    /// </summary>
    /// <typeparam name="TModel">Type that the class should hold (same as the defined type).</typeparam>
#if NET
    [Serializable]
    [System.Xml.Serialization.XmlSchemaProvider("GetGenericModelBaseXmlSchema")]
#endif
    [ObsoleteEx(Message = "Generic class is no longer being used, use the non-generic base instead", TreatAsErrorFromVersion = "3.4", RemoveInVersion = "4.0", Replacement = "ModelBase")]
    public abstract class ModelBase<TModel> : ModelBase
        where TModel : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase{TModel}"/> class.
        /// </summary>
        protected ModelBase()
        {
        }

#if NET
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelBase{TModel}"/> class.
        /// </summary>
        /// <para />
        /// Only constructor for the ModelBase.
        /// <param name="info">SerializationInfo object, null if this is the first time construction.</param>
        /// <param name="context">StreamingContext object, simple pass a default new StreamingContext() if this is the first time construction.</param>
        /// <remarks>
        /// Call this method, even when constructing the object for the first time (thus not deserializing).
        /// </remarks>
        protected ModelBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Registers a property that will be automatically handled by this object.
        /// </summary>
        /// <typeparam name="TValue">
        /// The value type.
        /// </typeparam>
        /// <param name="propertyExpression">
        /// The property expression.
        /// </param>
        /// <param name="defaultValue">
        /// Default value of the property.
        /// </param>
        /// <param name="propertyChangedEventHandler">
        /// The property changed event handler.
        /// </param>
        /// <param name="includeInSerialization">
        /// if set to <c>true</c>, the property should be included in the serialization.
        /// </param>
        /// <param name="includeInBackup">
        /// if set to <c>true</c>, the property should be included in the backup when handling IEditableObject.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="propertyExpression"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The member type of the body of the <paramref name="propertyExpression"/> of should be <see cref="MemberTypes.Property"/>.
        /// </exception>
        /// <returns>
        /// <see cref="PropertyData"/> containing the property information.
        /// </returns>
        public static PropertyData RegisterProperty<TValue>(Expression<Func<TModel, TValue>> propertyExpression, TValue defaultValue = default(TValue), Action<TModel, AdvancedPropertyChangedEventArgs> propertyChangedEventHandler = null, bool includeInSerialization = true, bool includeInBackup = true)
        {
            return RegisterProperty<TModel, TValue>(propertyExpression, defaultValue, propertyChangedEventHandler, includeInSerialization, includeInBackup);
        }

        /// <summary>
        /// Gets XML schema for this class.
        /// <para />
        /// Implemented to support WCF serialization for all types deriving from this type.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        /// <returns>System.Xml.XmlQualifiedName.</returns>
        public static System.Xml.XmlQualifiedName GetGenericModelBaseXmlSchema(System.Xml.Schema.XmlSchemaSet schemaSet)
        {
            return XmlSchemaManager.GetXmlSchema(typeof(ModelBase<TModel>), schemaSet);
        }
#endif

        /// <summary>
        /// Checks whether this object equals another object of the same type.
        /// </summary>
        /// <param name="other">The other object.</param>
        /// <returns><c>true</c> if the objects are equal; otherwise <c>false</c></returns>
        public bool Equals(TModel other)
        {
            return Equals((object)other);
        }
    }
}

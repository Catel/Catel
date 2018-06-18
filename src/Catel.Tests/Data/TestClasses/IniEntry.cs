namespace Catel.Tests.Data
{
    using System;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// IniEntry Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class IniEntry : ComparableModelBase
    {
        #region Serialization test code
        [ExcludeFromSerialization]
        public int _onSerializingCalls;
        [ExcludeFromSerialization]
        public int _onSerializedCalls;
        [ExcludeFromSerialization]
        public int _onDeserializingCalls;
        [ExcludeFromSerialization]
        public int _onDeserializedCalls;

        public void ClearSerializationCounters()
        {
            _onSerializingCalls = 0;
            _onSerializedCalls = 0;
            _onDeserializingCalls = 0;
            _onDeserializedCalls = 0;
        }

        protected override void OnSerializing()
        {
            _onSerializingCalls++;

            base.OnSerializing();
        }

        protected override void OnSerialized()
        {
            _onSerializedCalls++;

            base.OnSerialized();
        }

        protected override void OnDeserializing()
        {
            _onDeserializingCalls++;

            base.OnDeserializing();
        }

        protected override void OnDeserialized()
        {
            _onDeserializedCalls++;

            base.OnDeserialized();
        }
        #endregion

        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public IniEntry()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected IniEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the group.
        /// </summary>
        public string Group
        {
            get { return GetValue<string>(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData GroupProperty = RegisterProperty("Group", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the key.
        /// </summary>
        public string Key
        {
            get { return GetValue<string>(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData KeyProperty = RegisterProperty("Key", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return GetValue<string>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueProperty = RegisterProperty("Value", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the ini entry type.
        /// </summary>
        public IniEntryType IniEntryType
        {
            get { return GetValue<IniEntryType>(IniEntryTypeProperty); }
            set { SetValue(IniEntryTypeProperty, value); }
        }

        /// <summary>
        /// Register the IniEntryType property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IniEntryTypeProperty = RegisterProperty("IniEntryType", typeof(IniEntryType), IniEntryType.Public);
        #endregion

        #region Methods
        /// <summary>
        ///   Allows a test to invoke the Notify Property Changed on an object.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "propertyExpression">The property expression.</param>
        public new void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            base.RaisePropertyChanged(propertyExpression);
        }

        /// <summary>
        ///   Allows a test to invoke the Notify Property Changed on an object.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public new void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        ///   Sets whether this object is read-only or not.
        /// </summary>
        /// <param name = "isReadOnly">if set to <c>true</c>, the object will become read-only.</param>
        public void SetReadOnly(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        public void SetValue<TValue>(string propertyName, TValue value)
        {
            base.SetValue(propertyName, value);
        }

        public new TValue GetValue<TValue>(string propertyName)
        {
            return base.GetValue<TValue>(propertyName);
        }
        #endregion
    }
}

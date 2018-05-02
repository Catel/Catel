namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    /// <summary>
    ///   ObjectWithDefaultValues Data object class which fully supports serialization, property changed notifications,
    ///   backwards compatibility and error checking.
    /// </summary>
    public class ObjectWithDefaultValues : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        ///   ValueType_NoDefaultValue.
        /// </summary>
        public int ValueType_NoDefaultValue
        {
            get { return GetValue<int>(ValueType_NoDefaultValueProperty); }
            set { SetValue(ValueType_NoDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_NoDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_NoDefaultValueProperty = RegisterProperty("ValueType_NoDefaultValue", typeof(int));

        /// <summary>
        ///   ValueType_DefaultValueViaValue.
        /// </summary>
        public int ValueType_DefaultValueViaValue
        {
            get { return GetValue<int>(ValueType_DefaultValueViaValueProperty); }
            set { SetValue(ValueType_DefaultValueViaValueProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_DefaultValueViaValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_DefaultValueViaValueProperty = RegisterProperty("ValueType_DefaultValueViaValue", typeof(int), 5);

        /// <summary>
        ///   ValueType_DefaultValueViaCallback.
        /// </summary>
        public int ValueType_DefaultValueViaCallback
        {
            get { return GetValue<int>(ValueType_DefaultValueViaCallbackProperty); }
            set { SetValue(ValueType_DefaultValueViaCallbackProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_DefaultValueViaCallback property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_DefaultValueViaCallbackProperty = RegisterProperty("ValueType_DefaultValueViaCallback", typeof(int), () => 10);

        /// <summary>
        ///   ReferenceType_NoDefaultValue.
        /// </summary>
        public Collection<int> ReferenceType_NoDefaultValue
        {
            get { return GetValue<Collection<int>>(ReferenceType_NoDefaultValueProperty); }
            set { SetValue(ReferenceType_NoDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_NoDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_NoDefaultValueProperty = RegisterProperty("ReferenceType_NoDefaultValue", typeof(Collection<int>));

        /// <summary>
        ///   ReferenceType_DefaultValueViaValue.
        /// </summary>
        public Collection<int> ReferenceType_DefaultValueViaValue
        {
            get { return GetValue<Collection<int>>(ReferenceType_DefaultValueViaValueProperty); }
            set { SetValue(ReferenceType_DefaultValueViaValueProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_DefaultValueViaValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_DefaultValueViaValueProperty = RegisterProperty("ReferenceType_DefaultValueViaValue", typeof(Collection<int>), new Collection<int>());

        /// <summary>
        ///   ReferenceType_DefaultValueViaCallback.
        /// </summary>
        public Collection<int> ReferenceType_DefaultValueViaCallback
        {
            get { return GetValue<Collection<int>>(ReferenceType_DefaultValueViaCallbackProperty); }
            set { SetValue(ReferenceType_DefaultValueViaCallbackProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_DefaultValueViaCallback property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_DefaultValueViaCallbackProperty = RegisterProperty("ReferenceType_DefaultValueViaCallback", typeof(Collection<int>), () => new Collection<int>());
        #endregion

        #region Methods
        #endregion
    }
}
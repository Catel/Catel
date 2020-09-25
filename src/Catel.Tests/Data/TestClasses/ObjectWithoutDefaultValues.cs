namespace Catel.Tests.Data
{
    using Catel.Data;

    public class ObjectWithoutDefaultValues : ModelBase
    {
        /// <summary>
        ///   Gets or sets a value type.
        /// </summary>
        public int ValueType
        {
            get { return GetValue<int>(ValueTypeProperty); }
            set { SetValue(ValueTypeProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ValueTypeProperty = RegisterProperty("ValueType", typeof(int), 1);

        /// <summary>
        ///   Gets or sets a value type without default value.
        /// </summary>
        public int ValueTypeWithoutDefaultValue
        {
            get { return GetValue<int>(ValueTypeWithoutDefaultValueProperty); }
            set { SetValue(ValueTypeWithoutDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ValueTypeWithoutDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ValueTypeWithoutDefaultValueProperty = RegisterProperty<int>("ValueTypeWithoutDefaultValue");

        /// <summary>
        ///   Gets or sets a reference type.
        /// </summary>
        public object ReferenceType
        {
            get { return GetValue<object>(ReferenceTypeProperty); }
            set { SetValue(ReferenceTypeProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ReferenceTypeProperty = RegisterProperty<object>("ReferenceType", new object());

        /// <summary>
        ///   Gets or sets a reference type without default value.
        /// </summary>
        public object ReferenceTypeWithoutDefaultValue
        {
            get { return GetValue<object>(ReferenceTypeWithoutDefaultValueProperty); }
            set { SetValue(ReferenceTypeWithoutDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceTypeWithoutDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ReferenceTypeWithoutDefaultValueProperty = RegisterProperty<object>("ReferenceTypeWithoutDefaultValue");
    }
}

namespace Catel.Tests.Data
{
    using Catel.Data;

    public class ObjectWithNonSerializableMembers : ModelBase
    {
        /// <summary>
        ///   Gets or sets a non-serializable value.
        /// </summary>
        public NonSerializableClass NonSerializableValue
        {
            get { return GetValue<NonSerializableClass>(NonSerializableValueProperty); }
            set { SetValue(NonSerializableValueProperty, value); }
        }

        /// <summary>
        ///   Register the NonSerializableValue property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData NonSerializableValueProperty = RegisterProperty("NonSerializableValue", typeof(NonSerializableClass), null);
    }
}
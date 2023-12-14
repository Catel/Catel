namespace Catel.Tests.Data
{
    using System;
    using Catel.Data;

    /// <summary>
    /// Extended class of the <see cref="IniEntry"/> class.
    /// </summary>
    [Serializable]
    public class ExtendedIniEntry : IniEntry
    {
        /// <summary>
        ///   Enum to test the comparison of enums when registering the same property multiple times.
        /// </summary>
        public new enum IniEntryType
        {
            /// <summary>
            ///   New ini type.
            /// </summary>
            New,

            /// <summary>
            ///   Old ini type.
            /// </summary>
            Old
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ExtendedIniEntry()
        {
        }

        /// <summary>
        ///   Gets or sets the default value.
        /// </summary>
        public string DefaultValue
        {
            get { return GetValue<string>(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData DefaultValueProperty = RegisterProperty("DefaultValue", string.Empty);

        /// <summary>
        ///   Gets or sets the property value.
        /// </summary>
        public IniEntryType Type
        {
            get { return GetValue<IniEntryType>(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData TypeProperty = RegisterProperty("Type", IniEntryType.Old);
    }
}

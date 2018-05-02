namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using Catel.Data;

    /// <summary>
    /// Extended class of the <see cref="IniEntry"/> class.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ExtendedIniEntry : IniEntry
    {
        #region Enums
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
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ExtendedIniEntry()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ExtendedIniEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
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
        public static readonly PropertyData DefaultValueProperty = RegisterProperty("DefaultValue", typeof(string), string.Empty);

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
        public static readonly PropertyData TypeProperty = RegisterProperty("Type", typeof(IniEntryType), IniEntryType.Old);
        #endregion
    }
}
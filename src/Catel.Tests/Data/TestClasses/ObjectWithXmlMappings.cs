namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Catel.Data;

#if NET || NETCORE
    [Serializable]
#endif
    public class ObjectWithXmlMappings : SavableModelBase<ObjectWithXmlMappings>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithXmlMappings()
        {
        }

#if NET || NETCORE
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithXmlMappings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the property without an xml mapping.
        /// </summary>
        public string PropertyWithoutMapping
        {
            get { return GetValue<string>(PropertyWithoutMappingProperty); }
            set { SetValue(PropertyWithoutMappingProperty, value); }
        }

        /// <summary>
        ///   Register the PropertyWithoutMapping property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PropertyWithoutMappingProperty = RegisterProperty("PropertyWithoutMapping", typeof(string), "withoutMapping");

        /// <summary>
        /// Gets or sets a value that should be ignored.
        /// </summary>
        [XmlIgnore]
        public string IgnoredProperty
        {
            get { return GetValue<string>(IgnoredPropertyProperty); }
            set { SetValue(IgnoredPropertyProperty, value); }
        }

        /// <summary>
        /// Register the IgnoredProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IgnoredPropertyProperty = RegisterProperty("IgnoredProperty", typeof(string), "ignored");

        /// <summary>
        ///   Gets or sets the property with an xml mapping.
        /// </summary>
        [XmlElement("MappedXmlProperty")]
        public string PropertyWithMapping
        {
            get { return GetValue<string>(PropertyWithMappingProperty); }
            set { SetValue(PropertyWithMappingProperty, value); }
        }

        /// <summary>
        ///   Register the PropertyWithMapping property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PropertyWithMappingProperty = RegisterProperty("PropertyWithMapping", typeof(string), "withMapping");
        #endregion
    }
}

namespace Catel.Tests.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Catel.Data;

    [Serializable]
    public class ObjectWithXmlMappings : SavableModelBase<ObjectWithXmlMappings>
    {
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithXmlMappings()
        {
        }

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
        public static readonly IPropertyData PropertyWithoutMappingProperty = RegisterProperty("PropertyWithoutMapping", "withoutMapping");

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
        public static readonly IPropertyData IgnoredPropertyProperty = RegisterProperty("IgnoredProperty", "ignored");

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
        public static readonly IPropertyData PropertyWithMappingProperty = RegisterProperty("PropertyWithMapping", "withMapping");
    }
}

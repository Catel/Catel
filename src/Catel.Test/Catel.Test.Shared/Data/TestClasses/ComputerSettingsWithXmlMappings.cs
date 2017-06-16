namespace Catel.Test.Data
{
    using System;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Catel.Data;

    /// <summary>
    /// ComputerSettingsWithXmlMappings Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ComputerSettingsWithXmlMappings : SavableModelBase<ComputerSettingsWithXmlMappings>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ComputerSettingsWithXmlMappings()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ComputerSettingsWithXmlMappings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the computer name.
        /// </summary>
        [XmlElement("MappedComputerName")]
        public string ComputerName
        {
            get { return GetValue<string>(ComputerNameProperty); }
            set { SetValue(ComputerNameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ComputerNameProperty = RegisterProperty("ComputerName", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the collection of ini files.
        /// </summary>
        [XmlElement("IniFiles")]
        public ObservableCollection<IniFile> IniFileCollection
        {
            get { return GetValue<ObservableCollection<IniFile>>(IniFileCollectionProperty); }
            set { SetValue(IniFileCollectionProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IniFileCollectionProperty = RegisterProperty("IniFileCollection", typeof(ObservableCollection<IniFile>),
            () => new ObservableCollection<IniFile>());
        #endregion
    }
}
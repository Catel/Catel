namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;

    /// <summary>
    /// IniFile Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class IniFile : ComparableModelBase
    {
        public IniFile()
        {
            IniEntryCollection = new List<IniEntry>();
        }

        /// <summary>
        ///   Gets or sets the filename.
        /// </summary>
        public string FileName
        {
            get { return GetValue<string>(FileNameProperty); }
            set { SetValue(FileNameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData FileNameProperty = RegisterProperty("FileName", string.Empty);

        /// <summary>
        ///   Gets or sets the collection of ini entries..
        /// </summary>
        /// <remarks>
        ///   This type is a ObservableCollection{T} by purpose.
        /// </remarks>
        public List<IniEntry> IniEntryCollection
        {
            get { return GetValue<List<IniEntry>>(IniEntryCollectionProperty); }
            set { SetValue(IniEntryCollectionProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IniEntryCollectionProperty = RegisterProperty<List<IniEntry>>("IniEntryCollection");
    }
}

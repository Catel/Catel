namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// IniFile Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class IniFile : ComparableModelBase
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
        public IniFile()
        {
            IniEntryCollection = new List<IniEntry>();
        }
        #endregion

        #region Properties
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
        #endregion
    }
}

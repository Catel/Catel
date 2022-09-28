namespace Catel.Tests.Data
{
    using System;
    using System.Collections.ObjectModel;
    using Catel.Data;
    using Catel.Runtime.Serialization;

    /// <summary>
    /// ComputerSettings Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    [Serializable]
    public class ComputerSettings : ComparableModelBase
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

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ComputerSettings()
        {
            IniFileCollection = InitializeDefaultIniFileCollection();
        }
        
        /// <summary>
        ///   Gets or sets the computer name.
        /// </summary>
        public string ComputerName
        {
            get { return GetValue<string>(ComputerNameProperty); }
            set { SetValue(ComputerNameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData ComputerNameProperty = RegisterProperty("ComputerName", string.Empty);

        /// <summary>
        ///   Gets or sets the collection of ini files.
        /// </summary>
        /// <remarks>
        ///   This type is an ObservableCollection{T} by purpose.
        /// </remarks>
        public ObservableCollection<IniFile> IniFileCollection
        {
            get { return GetValue<ObservableCollection<IniFile>>(IniFileCollectionProperty); }
            set { SetValue(IniFileCollectionProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IniFileCollectionProperty = RegisterProperty<ObservableCollection<IniFile>>("IniFileCollection");

        /// <summary>
        ///   Initializes the default ini file collection.
        /// </summary>
        /// <returns>New <see cref = "ObservableCollection{T}" />.</returns>
        private static ObservableCollection<IniFile> InitializeDefaultIniFileCollection()
        {
            var result = new ObservableCollection<IniFile>();

            // Add 3 files
            result.Add(ModelBaseTestHelper.CreateIniFileObject("Initial file 1", new[] { ModelBaseTestHelper.CreateIniEntryObject("G1", "K1", "V1") }));
            result.Add(ModelBaseTestHelper.CreateIniFileObject("Initial file 2", new[] { ModelBaseTestHelper.CreateIniEntryObject("G2", "K2", "V2") }));
            result.Add(ModelBaseTestHelper.CreateIniFileObject("Initial file 3", new[] { ModelBaseTestHelper.CreateIniEntryObject("G3", "K3", "V3") }));

            return result;
        }
    }
}

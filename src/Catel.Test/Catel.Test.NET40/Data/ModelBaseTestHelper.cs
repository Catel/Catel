// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseTestHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

#if !WINDOWS_PHONE
    using System.ComponentModel.DataAnnotations;
#endif

    using Catel.Data;

    #region Test classes
    /// <summary>
    ///   ObjectWithDefaultValues Data object class which fully supports serialization, property changed notifications,
    ///   backwards compatibility and error checking.
    /// </summary>
    public class ObjectWithDefaultValues : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        #endregion

        #region Properties
        /// <summary>
        ///   ValueType_NoDefaultValue.
        /// </summary>
        public int ValueType_NoDefaultValue
        {
            get { return GetValue<int>(ValueType_NoDefaultValueProperty); }
            set { SetValue(ValueType_NoDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_NoDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_NoDefaultValueProperty = RegisterProperty("ValueType_NoDefaultValue", typeof(int));

        /// <summary>
        ///   ValueType_DefaultValueViaValue.
        /// </summary>
        public int ValueType_DefaultValueViaValue
        {
            get { return GetValue<int>(ValueType_DefaultValueViaValueProperty); }
            set { SetValue(ValueType_DefaultValueViaValueProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_DefaultValueViaValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_DefaultValueViaValueProperty = RegisterProperty("ValueType_DefaultValueViaValue", typeof(int), 5);

        /// <summary>
        ///   ValueType_DefaultValueViaCallback.
        /// </summary>
        public int ValueType_DefaultValueViaCallback
        {
            get { return GetValue<int>(ValueType_DefaultValueViaCallbackProperty); }
            set { SetValue(ValueType_DefaultValueViaCallbackProperty, value); }
        }

        /// <summary>
        ///   Register the ValueType_DefaultValueViaCallback property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueType_DefaultValueViaCallbackProperty = RegisterProperty("ValueType_DefaultValueViaCallback", typeof(int), () => 10);

        /// <summary>
        ///   ReferenceType_NoDefaultValue.
        /// </summary>
        public Collection<int> ReferenceType_NoDefaultValue
        {
            get { return GetValue<Collection<int>>(ReferenceType_NoDefaultValueProperty); }
            set { SetValue(ReferenceType_NoDefaultValueProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_NoDefaultValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_NoDefaultValueProperty = RegisterProperty("ReferenceType_NoDefaultValue", typeof(Collection<int>));

        /// <summary>
        ///   ReferenceType_DefaultValueViaValue.
        /// </summary>
        public Collection<int> ReferenceType_DefaultValueViaValue
        {
            get { return GetValue<Collection<int>>(ReferenceType_DefaultValueViaValueProperty); }
            set { SetValue(ReferenceType_DefaultValueViaValueProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_DefaultValueViaValue property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_DefaultValueViaValueProperty = RegisterProperty("ReferenceType_DefaultValueViaValue", typeof(Collection<int>), new Collection<int>());

        /// <summary>
        ///   ReferenceType_DefaultValueViaCallback.
        /// </summary>
        public Collection<int> ReferenceType_DefaultValueViaCallback
        {
            get { return GetValue<Collection<int>>(ReferenceType_DefaultValueViaCallbackProperty); }
            set { SetValue(ReferenceType_DefaultValueViaCallbackProperty, value); }
        }

        /// <summary>
        ///   Register the ReferenceType_DefaultValueViaCallback property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ReferenceType_DefaultValueViaCallbackProperty = RegisterProperty("ReferenceType_DefaultValueViaCallback", typeof(Collection<int>), () => new Collection<int>());
        #endregion

        #region Methods
        #endregion
    }

    /// <summary>
    /// ValidationTest Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ValidationTest : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ValidationTest()
        {
            ErrorWhenEmpty = "noerror";
            WarningWhenEmpty = "nowarning";
            BusinessRuleErrorWhenEmpty = "noerror";
            BusinessRuleWarningWhenEmpty = "noerror";
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ValidationTest(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        public new bool AutomaticallyValidateOnPropertyChanged
        {
            get { return base.AutomaticallyValidateOnPropertyChanged; }
            set { base.AutomaticallyValidateOnPropertyChanged = value; }
        }

        /// <summary>
        ///   Gets or sets field that returns an error when empty.
        /// </summary>
        public string ErrorWhenEmpty
        {
            get { return GetValue<string>(ErrorWhenEmptyProperty); }
            set { SetValue(ErrorWhenEmptyProperty, value); }
        }

        /// <summary>
        ///   Register the ErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ErrorWhenEmptyProperty = RegisterProperty("ErrorWhenEmpty", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets field that returns a warning when empty.
        /// </summary>
        public string WarningWhenEmpty
        {
            get { return GetValue<string>(WarningWhenEmptyProperty); }
            set { SetValue(WarningWhenEmptyProperty, value); }
        }

        /// <summary>
        ///   Register the WarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData WarningWhenEmptyProperty = RegisterProperty("WarningWhenEmpty", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets field that returns a business rule error when empty.
        /// </summary>
        public string BusinessRuleErrorWhenEmpty
        {
            get { return GetValue<string>(BusinessRuleErrorWhenEmptyProperty); }
            set { SetValue(BusinessRuleErrorWhenEmptyProperty, value); }
        }

        /// <summary>
        ///   Register the BusinessRuleErrorWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BusinessRuleErrorWhenEmptyProperty = RegisterProperty("BusinessRuleErrorWhenEmpty", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets field that returns a business rule warning when empty.
        /// </summary>
        public string BusinessRuleWarningWhenEmpty
        {
            get { return GetValue<string>(BusinessRuleWarningWhenEmptyProperty); }
            set { SetValue(BusinessRuleWarningWhenEmptyProperty, value); }
        }

        /// <summary>
        ///   Register the BusinessRuleWarningWhenEmpty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BusinessRuleWarningWhenEmptyProperty = RegisterProperty("BusinessRuleWarningWhenEmpty", typeof(string), string.Empty);

        public new bool HideValidationResults
        {
            get { return base.HideValidationResults; }
            set { base.HideValidationResults = value; }
        }
        #endregion

        #region Methods
        public void SetFieldErrorOutsideValidation()
        {
            if (string.IsNullOrEmpty(ErrorWhenEmpty))
            {
                SetFieldValidationResult(FieldValidationResult.CreateError(ErrorWhenEmptyProperty, "Should not be empty"));
            }
        }

        public void SetFieldWarningOutsideValidation()
        {
            if (string.IsNullOrEmpty(WarningWhenEmpty))
            {
                SetFieldValidationResult(FieldValidationResult.CreateWarning(WarningWhenEmptyProperty, "Should not be empty"));
            }
        }

        public void SetBusinessRuleErrorOutsideValidation()
        {
            if (string.IsNullOrEmpty(BusinessRuleErrorWhenEmpty))
            {
                SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateError("BusinessRuleErrorWhenEmpty should not be empty"));
            }
        }

        public void SetBusinessRuleWarningOutsideValidation()
        {
            if (string.IsNullOrEmpty(BusinessRuleWarningWhenEmpty))
            {
                SetBusinessRuleValidationResult(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarningWhenEmpty should not be empty"));
            }
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(ErrorWhenEmpty))
            {
                validationResults.Add(FieldValidationResult.CreateError(ErrorWhenEmptyProperty, "Cannot be empty"));
            }

            if (string.IsNullOrEmpty(WarningWhenEmpty))
            {
                validationResults.Add(FieldValidationResult.CreateWarning(WarningWhenEmptyProperty, "Should not be empty"));
            }
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(BusinessRuleErrorWhenEmpty))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("BusinessRuleErrorWhenEmpty should not be empty"));
            }

            if (string.IsNullOrEmpty(BusinessRuleWarningWhenEmpty))
            {
                validationResults.Add(BusinessRuleValidationResult.CreateWarning("BusinessRuleWarningWhenEmpty should not be empty"));
            }
        }
        #endregion
    }

    /// <summary>
    /// DynamicObject Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class DynamicObject : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public DynamicObject()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected DynamicObject(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        // TODO: Define your custom properties here using the dataprop code snippet
        #endregion

        #region Methods
        public static PropertyData RegisterProperty(string name, Type type)
        {
            return ModelBase.RegisterProperty(name, type);
        }

        public new void InitializePropertyAfterConstruction(PropertyData propertyData)
        {
            base.InitializePropertyAfterConstruction(propertyData);
        }

        public void SetValue<TValue>(string propertyName, TValue value)
        {
            base.SetValue(propertyName, value);
        }

        public new TValue GetValue<TValue>(string propertyName)
        {
            return base.GetValue<TValue>(propertyName);
        }
        #endregion
    }

    /// <summary>
    /// Just a dummy enum to test enum (de)serialization.
    /// </summary>
    public enum IniEntryType
    {
        Public,

        Private
    }

    /// <summary>
    /// IniEntry Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class IniEntry : SavableModelBase<IniEntry>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public IniEntry()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected IniEntry(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the group.
        /// </summary>
        public string Group
        {
            get { return GetValue<string>(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData GroupProperty = RegisterProperty("Group", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the key.
        /// </summary>
        public string Key
        {
            get { return GetValue<string>(KeyProperty); }
            set { SetValue(KeyProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData KeyProperty = RegisterProperty("Key", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the value.
        /// </summary>
        public string Value
        {
            get { return GetValue<string>(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueProperty = RegisterProperty("Value", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the ini entry type.
        /// </summary>
        public IniEntryType IniEntryType
        {
            get { return GetValue<IniEntryType>(IniEntryTypeProperty); }
            set { SetValue(IniEntryTypeProperty, value); }
        }

        /// <summary>
        /// Register the IniEntryType property so it is known in the class.
        /// </summary>
        public static readonly PropertyData IniEntryTypeProperty = RegisterProperty("IniEntryType", typeof(IniEntryType), IniEntryType.Public);
        #endregion

        #region Methods
        /// <summary>
        ///   Allows a test to invoke the Notify Property Changed on an object.
        /// </summary>
        /// <typeparam name = "TProperty">The type of the property.</typeparam>
        /// <param name = "propertyExpression">The property expression.</param>
        public new void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            base.RaisePropertyChanged(propertyExpression);
        }

        /// <summary>
        ///   Allows a test to invoke the Notify Property Changed on an object.
        /// </summary>
        /// <param name = "propertyName">Name of the property.</param>
        public new void RaisePropertyChanged(string propertyName)
        {
            base.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        ///   Sets whether this object is read-only or not.
        /// </summary>
        /// <param name = "isReadOnly">if set to <c>true</c>, the object will become read-only.</param>
        public void SetReadOnly(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }

        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(Group))
            {
                validationResults.Add(FieldValidationResult.CreateError(GroupProperty, "Group is mandator"));
            }

            if (string.IsNullOrEmpty(Key))
            {
                validationResults.Add(FieldValidationResult.CreateError(KeyProperty, "Key is mandator"));
            }

            if (string.IsNullOrEmpty(Value))
            {
                validationResults.Add(FieldValidationResult.CreateError(ValueProperty, "Value is mandator"));
            }
        }

        public void SetValue<TValue>(string propertyName, TValue value)
        {
            base.SetValue(propertyName, value);
        }

        public new TValue GetValue<TValue>(string propertyName)
        {
            return base.GetValue<TValue>(propertyName);
        }
        #endregion
    }

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

    /// <summary>
    /// IniFile Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class IniFile : SavableModelBase<IniFile>
    {
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

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected IniFile(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
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
        public static readonly PropertyData FileNameProperty = RegisterProperty("FileName", typeof(string), string.Empty);

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
        public static readonly PropertyData IniEntryCollectionProperty = RegisterProperty("IniEntryCollection", typeof(List<IniEntry>), null);
        #endregion

        #region Methods
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                validationResults.Add(FieldValidationResult.CreateError(FileNameProperty, "File name is mandatory"));
            }
        }
        #endregion
    }

    /// <summary>
    /// ComputerSettings Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ComputerSettings : SavableModelBase<ComputerSettings>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ComputerSettings()
        {
            IniFileCollection = InitializeDefaultIniFileCollection();
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ComputerSettings(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
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
        public static readonly PropertyData ComputerNameProperty = RegisterProperty("ComputerName", typeof(string), string.Empty);

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
        public static readonly PropertyData IniFileCollectionProperty = RegisterProperty("IniFileCollection", typeof(ObservableCollection<IniFile>));
        #endregion

        #region Methods
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
        #endregion
    }

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

    #region Enums
    public enum Gender
    {
        Male,

        Female
    }
    #endregion

#if NET
    [Serializable]
#endif
    public class ObjectWithCustomType : SavableModelBase<ObjectWithCustomType>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithCustomType()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithCustomType(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return GetValue<string>(FirstNameProperty); }
            set { SetValue(FirstNameProperty, value); }
        }

        /// <summary>
        ///   Register the FirstName property so it is known in the class.
        /// </summary>
        public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the gender.
        /// </summary>
        public Gender Gender
        {
            get { return GetValue<Gender>(GenderProperty); }
            set { SetValue(GenderProperty, value); }
        }

        /// <summary>
        ///   Register the Gender property so it is known in the class.
        /// </summary>
        public static readonly PropertyData GenderProperty = RegisterProperty("Gender", typeof(Gender), Gender.Male);
        #endregion

        #region Methods
        #endregion
    }

#if NET
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

#if NET
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

#if NET
    [Serializable]
#endif
    public class ObjectWithPrivateMembers : SavableModelBase<ObjectWithPrivateMembers>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers()
        {
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithPrivateMembers(string privateMemberValue)
        {
            // Store values
            PrivateMember = privateMemberValue;
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithPrivateMembers(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the public member.
        /// </summary>
        public string PublicMember
        {
            get { return GetValue<string>(PublicMemberProperty); }
            set { SetValue(PublicMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PublicMemberProperty = RegisterProperty("PublicMember", typeof(string), "Public member");

        /// <summary>
        ///   Gets or sets the private member.
        /// </summary>
        private string PrivateMember
        {
            get { return GetValue<string>(PrivateMemberProperty); }
            set { SetValue(PrivateMemberProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData PrivateMemberProperty = RegisterProperty("PrivateMember", typeof(string), "Private member");
        #endregion
    }

#if NET
    [Serializable]
#endif
    public class ObjectWithPrivateConstructor : SavableModelBase<ObjectWithPrivateConstructor>
    {
        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        protected ObjectWithPrivateConstructor()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ObjectWithPrivateConstructor" /> class.
        /// </summary>
        /// <param name = "myValue">My value.</param>
        public ObjectWithPrivateConstructor(string myValue)
        {
            // Store values
            MyValue = myValue;
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithPrivateConstructor(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets my value.
        /// </summary>
        public string MyValue
        {
            get { return GetValue<string>(MyValueProperty); }
            set { SetValue(MyValueProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData MyValueProperty = RegisterProperty("MyValue", typeof(string), string.Empty);
        #endregion
    }

#if NET
    [Serializable]
#endif
    public class Parent : SavableModelBase<Parent>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Parent()
        {
        }

        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public Parent(string name)
        {
            // Store values
            Name = name;
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected Parent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the name of the parent.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);

        /// <summary>
        ///   Gets or sets the child collection.
        /// </summary>
        public Collection<Child> Children
        {
            get { return GetValue<Collection<Child>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ChildrenProperty = RegisterProperty("Children", typeof(Collection<Child>), new Collection<Child>());
        #endregion

        #region Methods
        /// <summary>
        ///   Creates a new child object.
        /// </summary>
        /// <param name = "name">The name.</param>
        /// <returns>New created child.</returns>
        public Child CreateChild(string name)
        {
            Child child = new Child(this, name);
            Children.Add(child);
            return child;
        }
        #endregion
    }

#if NET
    [Serializable]
#endif
    public class Child : SavableModelBase<Child>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
#if SILVERLIGHT
        public
#else
        protected
#endif
 Child()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Child" /> class.
        /// </summary>
        /// <param name = "parent">The parent.</param>
        /// <param name = "name">The name.</param>
        public Child(Parent parent, string name)
        {
            // Set parent
            SetParent(parent);

            // Store values
            Name = name;
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected Child(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the name of the child.
        /// </summary>
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>
        ///   Register the property so it is known in the class.
        /// </summary>
        public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), string.Empty);
        #endregion
    }
    
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
        public static readonly PropertyData ValueTypeProperty = RegisterProperty("ValueType", typeof(int), 1);

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
        public static readonly PropertyData ValueTypeWithoutDefaultValueProperty = RegisterProperty("ValueTypeWithoutDefaultValue", typeof(int));

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
        public static readonly PropertyData ReferenceTypeProperty = RegisterProperty("ReferenceType", typeof(object), new object());

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
        public static readonly PropertyData ReferenceTypeWithoutDefaultValueProperty = RegisterProperty("ReferenceTypeWithoutDefaultValue", typeof(object));
    }

    public class NonSerializableClass
    {   
    }

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
        public static readonly PropertyData NonSerializableValueProperty = RegisterProperty("NonSerializableValue", typeof(NonSerializableClass), null);
    }

#if !NETFX_CORE
    public class ClassWithPropertiesRegisteredByNonMagicStringOverload : ModelBase
    {
        public static readonly PropertyData StringPropertyProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, string>(instance => instance.StringProperty);
        
        public static readonly PropertyData StringPropertyWithSpecifiedDefaultValueProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, string>(instance => instance.StringPropertyWithSpecifiedDefaultValue, "NonNullOrEmptyDefaultValue");

        public static readonly PropertyData IntPropertyWithPropertyChangeNoticationProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, int>(instance => instance.IntPropertyWithPropertyChangeNotication , default(int), (s, e) => s.OnIntPropertyWithPropertyChangeNoticationChanged());

        public static readonly PropertyData IntPropertyExcludedFromSerializationAndBackupProperty = RegisterProperty<ClassWithPropertiesRegisteredByNonMagicStringOverload, int>(instance => instance.IntPropertyExcludedFromSerializationAndBackup, default(int), null, false, false);

        public int IntPropertyExcludedFromSerializationAndBackup
        {
            get { return GetValue<int>(IntPropertyExcludedFromSerializationAndBackupProperty); }
            set { SetValue(IntPropertyExcludedFromSerializationAndBackupProperty, value); }
        }

        public string StringPropertyWithSpecifiedDefaultValue
        {
            get { return GetValue<string>(StringPropertyWithSpecifiedDefaultValueProperty); }
            set { SetValue(StringPropertyWithSpecifiedDefaultValueProperty, value); }
        }

        public string StringProperty
        {
            get { return GetValue<string>(StringPropertyProperty); }
            set { SetValue(StringPropertyProperty, value); }
        }

        public int IntPropertyWithPropertyChangeNotication
        {
            get { return GetValue<int>(IntPropertyWithPropertyChangeNoticationProperty); }
            set { SetValue(IntPropertyWithPropertyChangeNoticationProperty, value); }
        }

        public int IntPropertyWithPropertyChangeNoticationsCount { get; private set; }

        private void OnIntPropertyWithPropertyChangeNoticationChanged()
        {
            this.IntPropertyWithPropertyChangeNoticationsCount++;
        }
    }
#endif

    [AllowNonSerializableMembers]
    public class ObjectWithNonSerializableMembersDecoratedWithAllowNonSerializableMembersAttribute : ModelBase
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
        public static readonly PropertyData NonSerializableValueProperty = RegisterProperty("NonSerializableValue", typeof(NonSerializableClass), null);
    }

#if NET
    [Serializable]
#endif
    public class ObjectWithValidation : ModelBase
    {
        #region Constants
        public const string ValueThatHasNoWarningsOrErrors = "NoWarningsOrErrors";
        public const string ValueThatCausesFieldWarning = "FieldWarning";
        public const string ValueThatCausesBusinessWarning = "BusinessWarning";
        public const string ValueThatCausesFieldError = "FieldError";
        public const string ValueThatCausesBusinessError = "BusinessError";
        #endregion

        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ObjectWithValidation()
        {
            NonCatelPropertyWithAnnotations = "default value";
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ObjectWithValidation(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion

        #region Properties
        /// <summary>
        ///   Gets or sets the value to validate.
        /// </summary>
        public string ValueToValidate
        {
            get { return GetValue<string>(ValueToValidateProperty); }
            set { SetValue(ValueToValidateProperty, value); }
        }

        /// <summary>
        ///   Register the ValueToValidate property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueToValidateProperty = RegisterProperty("ValueToValidate", typeof(string), ValueThatHasNoWarningsOrErrors);

#if !WINDOWS_PHONE
        [Required(ErrorMessage = "Non-catel is required")]
        public string NonCatelPropertyWithAnnotations { get; set; }

        [Required(ErrorMessage = "Non-catel is required")]
        public string NonCatelCalculatedPropertyWithAnnotations { get { return "default value"; } }

        /// <summary>
        /// Gets or sets the object with annotation validation.
        /// </summary>
        [Required(ErrorMessage = "Field is required")]
        public string ValueWithAnnotations
        {
            get { return GetValue<string>(ValueWithAnnotationsProperty); }
            set { SetValue(ValueWithAnnotationsProperty, value); }
        }

        /// <summary>
        /// Register the ValueWithAnnotations property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ValueWithAnnotationsProperty = RegisterProperty("ValueWithAnnotations", typeof(string), "value");
#endif
        #endregion

        #region Methods
        /// <summary>
        ///   Validates the fields.
        /// </summary>
        protected override void ValidateFields(List<IFieldValidationResult> validationResults)
        {
            if (ValueToValidate == ValueThatCausesFieldWarning)
            {
                validationResults.Add(FieldValidationResult.CreateWarning(ValueToValidateProperty, "Field warning"));
            }

            if (ValueToValidate == ValueThatCausesFieldError)
            {
                validationResults.Add(FieldValidationResult.CreateError(ValueToValidateProperty, "Field error"));
            }
        }

        /// <summary>
        ///   Validates the business rules.
        /// </summary>
        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (ValueToValidate == ValueThatCausesBusinessWarning)
            {
                validationResults.Add(BusinessRuleValidationResult.CreateWarning("Business rule warning"));
            }

            if (ValueToValidate == ValueThatCausesBusinessError)
            {
                validationResults.Add(BusinessRuleValidationResult.CreateError("Business rule error"));
            }
        }
        #endregion
    }

#if NET
    [Serializable]
#endif
    public class ClassWithoutPropertiesA : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ClassWithoutPropertiesA()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ClassWithoutPropertiesA(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion
    }

#if NET
    [Serializable]
#endif
    public class ClassWithoutPropertiesB : ModelBase
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        ///   Initializes a new object from scratch.
        /// </summary>
        public ClassWithoutPropertiesB()
        {
        }

#if NET
        /// <summary>
        ///   Initializes a new object based on <see cref = "SerializationInfo" />.
        /// </summary>
        /// <param name = "info"><see cref = "SerializationInfo" /> that contains the information.</param>
        /// <param name = "context"><see cref = "StreamingContext" />.</param>
        protected ClassWithoutPropertiesB(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
#endif
        #endregion
    }

    #region Inheritance testing
    /// <summary>
    /// ModelBase Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [KnownType(typeof(ModelA)), KnownType(typeof(ModelB)), Serializable]
#endif
    public class Model : SavableModelBase<Model>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public Model() { }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected Model(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        /// <summary>
        /// Gets or sets the A property.
        /// </summary>
        public string A
        {
            get { return GetValue<string>(AProperty); }
            set { SetValue(AProperty, value); }
        }

        /// <summary>
        /// Register the A property so it is known in the class.
        /// </summary>
        public static readonly PropertyData AProperty = RegisterProperty("A", typeof(string), string.Empty);
    }

    /// <summary>
    /// ModelA Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ModelA : Model
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelA() { }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected ModelA(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        /// <summary>
        /// Gets or sets property B.
        /// </summary>
        public string B
        {
            get { return GetValue<string>(BProperty); }
            set { SetValue(BProperty, value); }
        }

        /// <summary>
        /// Register the B property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BProperty = RegisterProperty("B", typeof(string), string.Empty);
    }

    /// <summary>
    /// ModelB Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    public class ModelB : Model
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelB() { }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected ModelB(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        /// <summary>
        /// Gets or sets property C.
        /// </summary>
        public string C
        {
            get { return GetValue<string>(CProperty); }
            set { SetValue(CProperty, value); }
        }

        /// <summary>
        /// Register the C property so it is known in the class.
        /// </summary>
        public static readonly PropertyData CProperty = RegisterProperty("C", typeof(string), string.Empty);
    }

    /// <summary>
    /// ModelC Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
#if NET
    [Serializable]
#endif
    [KnownType(typeof(ModelA)), KnownType(typeof(ModelB))]
    public class ModelC : SavableModelBase<ModelC>
    {
        #region Fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new object from scratch.
        /// </summary>
        public ModelC() { }

#if NET
        /// <summary>
        /// Initializes a new object based on <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
        /// <param name="context"><see cref="StreamingContext"/>.</param>
        protected ModelC(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
#endif
        #endregion

        /// <summary>
        /// Gets or sets the D property.
        /// </summary>
        public string D
        {
            get { return GetValue<string>(DProperty); }
            set { SetValue(DProperty, value); }
        }

        /// <summary>
        /// Register the D property so it is known in the class.
        /// </summary>
        public static readonly PropertyData DProperty = RegisterProperty("D", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the E property.
        /// </summary>
        public Model E
        {
            get { return GetValue<Model>(EProperty); }
            set { SetValue(EProperty, value); }
        }

        /// <summary>
        /// Register the E property so it is known in the class.
        /// </summary>
        public static readonly PropertyData EProperty = RegisterProperty("E", typeof(Model), null);
    }
    #endregion

    /// <summary>
    /// ClassWithValidator Data object class which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class ClassWithValidator : ModelBase
    {
        /// <summary>
        /// Gets or sets the warning property.
        /// </summary>
        public string WarningProperty
        {
            get { return GetValue<string>(WarningPropertyProperty); }
            set { SetValue(WarningPropertyProperty, value); }
        }

        /// <summary>
        /// Register the WarningProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData WarningPropertyProperty = RegisterProperty("WarningProperty", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the error property.
        /// </summary>
        public string ErrorProperty
        {
            get { return GetValue<string>(ErrorPropertyProperty); }
            set { SetValue(ErrorPropertyProperty, value); }
        }

        /// <summary>
        /// Register the ErrorProperty property so it is known in the class.
        /// </summary>
        public static readonly PropertyData ErrorPropertyProperty = RegisterProperty("ErrorProperty", typeof(string), string.Empty);

        /// <summary>
        /// Gets or sets the business rule error.
        /// </summary>
        public string BusinessRuleError
        {
            get { return GetValue<string>(BusinessRuleErrorProperty); }
            set { SetValue(BusinessRuleErrorProperty, value); }
        }

        /// <summary>
        /// Register the BusinessRuleError property so it is known in the class.
        /// </summary>
        public static readonly PropertyData BusinessRuleErrorProperty = RegisterProperty("BusinessRuleError", typeof(string), string.Empty);
    }

    public class TestValidatorProvider : ValidatorProviderBase
    {
        protected override IValidator GetValidator(Type targetType)
        {
            Argument.IsNotNull("targetType", targetType);

            return new TestValidator();
        }
    }

    public class TestValidator : ValidatorBase<ClassWithValidator>
    {
        public int ValidateCount { get; private set; }

        protected override void Validate(ClassWithValidator instance, Catel.Data.ValidationContext validationContext)
        {
            ValidateCount++;
        }

        public int BeforeValidationCount { get; private set; }

        protected override void BeforeValidation(ClassWithValidator instance, List<IFieldValidationResult> previousFieldValidationResults, List<IBusinessRuleValidationResult> previousBusinessRuleValidationResults)
        {
            BeforeValidationCount++;
        }

        public int BeforeValidateFieldsCount { get; private set; }

        protected override void BeforeValidateFields(ClassWithValidator instance, List<IFieldValidationResult> previousValidationResults)
        {
            BeforeValidateFieldsCount++;
        }

        public int ValidateFieldsCount { get; private set; }

        protected override void ValidateFields(ClassWithValidator instance, List<IFieldValidationResult> validationResults)
        {
            ValidateFieldsCount++;

            validationResults.Add(FieldValidationResult.CreateWarning(ClassWithValidator.WarningPropertyProperty, "Warning"));
            validationResults.Add(FieldValidationResult.CreateError(ClassWithValidator.ErrorPropertyProperty, "Error"));
        }

        public int AfterValidateFieldsCount { get; private set; }

        protected override void AfterValidateFields(ClassWithValidator instance, List<IFieldValidationResult> validationResults)
        {
            AfterValidateFieldsCount++;
        }

        public int BeforeValidateBusinessRulesCount { get; private set; }

        protected override void BeforeValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> previousValidationResults)
        {
            BeforeValidateBusinessRulesCount++;
        }

        public int ValidateBusinessRulesCount { get; private set; }

        protected override void ValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> validationResults)
        {
            ValidateBusinessRulesCount++;

            validationResults.Add(BusinessRuleValidationResult.CreateError("Error"));
        }

        public int AfterValidateBusinessRulesCount { get; private set; }

        protected override void AfterValidateBusinessRules(ClassWithValidator instance, List<IBusinessRuleValidationResult> validationResults)
        {
            AfterValidateBusinessRulesCount++;
        }

        public int AfterValidationCount { get; private set; }

        protected override void AfterValidation(ClassWithValidator instance, List<IFieldValidationResult> fieldValidationResults, List<IBusinessRuleValidationResult> businessRuleValidationResults)
        {
            AfterValidationCount++;
        }
    }
    #endregion

    /// <summary>
    ///   <see cref = "ModelBase" /> test helper class.
    /// </summary>
    public abstract class ModelBaseTestHelper
    {
        /// <summary>
        ///   Creates the ini entry object.
        /// </summary>
        /// <param name = "group">The group.</param>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        /// <returns>New <see cref = "IniEntry" />.</returns>
        public static IniEntry CreateIniEntryObject(string group, string key, string value)
        {
            return new IniEntry
                       {
                           Group = group,
                           Key = key,
                           Value = value
                       };
        }

        /// <summary>
        ///   Creates the ini entry object.
        /// </summary>
        /// <returns>New <see cref = "IniEntry" />.</returns>
        public static IniEntry CreateIniEntryObject()
        {
            return CreateIniEntryObject("MyGroup", "MyKey", "MyValue");
        }

        /// <summary>
        ///   Creates the ini file object.
        /// </summary>
        /// <param name = "fileName">Name of the file.</param>
        /// <param name = "iniEntries">The ini entries.</param>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static IniFile CreateIniFileObject(string fileName, IEnumerable<IniEntry> iniEntries)
        {
            // Ini file
            IniFile iniFile = new IniFile
                                  {
                                      FileName = fileName
                                  };

            // Add entries
            foreach (IniEntry iniEntry in iniEntries)
            {
                iniFile.IniEntryCollection.Add(iniEntry);
            }

            // Return result
            return iniFile;
        }

        /// <summary>
        ///   Creates the ini file object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static IniFile CreateIniFileObject()
        {
            // Declare variables
            var iniEntries = new List<IniEntry>();

            // Create collection
            for (int i = 0; i < 3; i++)
            {
                var iniEntry = CreateIniEntryObject(string.Format("Group {0}", i),
                                                    string.Format("Key {0}", i),
                                                    string.Format("Value {0}", i));

                iniEntry.IniEntryType = (i % 2 == 0) ? IniEntryType.Public : IniEntryType.Private;

                iniEntries.Add(iniEntry);
            }

            // Create object
            return CreateIniFileObject("MyIniFile", iniEntries);
        }

        /// <summary>
        ///   Creates the computer settings object.
        /// </summary>
        /// <param name = "computerName">Name of the computer.</param>
        /// <param name = "iniFiles">The ini files.</param>
        /// <returns>New <see cref = "ComputerSettings" />.</returns>
        public static ComputerSettings CreateComputerSettingsObject(string computerName, IEnumerable<IniFile> iniFiles)
        {
            // Computer settings
            var computerSettings = new ComputerSettings
            {
                ComputerName = computerName
            };

            // Add entries
            foreach (var iniFile in iniFiles)
            {
                computerSettings.IniFileCollection.Add(iniFile);
            }

            computerSettings.SetValue("IsDirty", false);
            return computerSettings;
        }

        /// <summary>
        ///   Creates the computer settings object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static ComputerSettings CreateComputerSettingsObject()
        {
            // Declare variables
            var iniFiles = new List<IniFile>();

            // Create collection
            for (int i = 0; i < 3; i++)
            {
                // Create object
                IniFile iniFile = CreateIniFileObject();
                iniFile.FileName = string.Format("MyFile {0}", i);
                iniFiles.Add(iniFile);
            }

            // Create object
            return CreateComputerSettingsObject("MyComputer", iniFiles);
        }

        /// <summary>
        ///   Creates the computer settings object with xml mappings.
        /// </summary>
        /// <param name = "computerName">Name of the computer.</param>
        /// <param name = "iniFiles">The ini files.</param>
        /// <returns>New <see cref = "ComputerSettings" />.</returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsWithXmlMappingsObject(string computerName, IEnumerable<IniFile> iniFiles)
        {
            // Copy and return
            return CreateComputerSettingsCopy(CreateComputerSettingsObject(computerName, iniFiles));
        }

        /// <summary>
        ///   Creates the computer settings with xml mappings object with some predefined values.
        /// </summary>
        /// <returns>New <see cref = "IniFile" />.</returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsWithXmlMappingsObject()
        {
            // Copy and return
            return CreateComputerSettingsCopy(CreateComputerSettingsObject());
        }

        /// <summary>
        ///   Creates the computer settings copy.
        /// </summary>
        /// <param name = "computerSettings">The computer settings.</param>
        /// <returns></returns>
        public static ComputerSettingsWithXmlMappings CreateComputerSettingsCopy(ComputerSettings computerSettings)
        {
            // Copy the properties
            ComputerSettingsWithXmlMappings computerSettingsWithXmlMappings = new ComputerSettingsWithXmlMappings();
            computerSettingsWithXmlMappings.ComputerName = computerSettings.ComputerName;
            computerSettingsWithXmlMappings.IniFileCollection = computerSettings.IniFileCollection;

            // Return result
            return computerSettingsWithXmlMappings;
        }

        /// <summary>
        /// Creates the hierarchical graph with inheritance.
        /// </summary>
        /// <returns></returns>
        public static ModelC CreateHierarchicalGraphWithInheritance()
        {
            var modelC = new ModelC
            {
                D = "D",
                E = new ModelA
                {
                    A = "A",
                    B = "B"
                }
            };

            return modelC;
        }
    }
}
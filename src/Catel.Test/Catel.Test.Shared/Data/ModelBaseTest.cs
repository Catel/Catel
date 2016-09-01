namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Catel.Data;
    using Catel.Runtime.Serialization;
    using NUnit.Framework;

    [TestFixture]
    public class ModelBaseTest
    {
        #region Fields
#if !NETFX_CORE
        private FilesHelper _filesHelper;
#endif
        #endregion

        #region Initialization and cleanup
#if !NETFX_CORE
        [SetUp]
        public void Initialize()
        {
            if (_filesHelper == null)
            {
                _filesHelper = new FilesHelper();
            }
        }

        [TearDown]
        public void CleanUp()
        {
            if (_filesHelper != null)
            {
                _filesHelper.CleanUp();
                _filesHelper = null;
            }
        }
#endif
        #endregion

        #region Equal tests
        [TestCase]
        public void Equals_Generic()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            var obj2 = ModelBaseTestHelper.CreateIniEntryObject();

            // Equals
            Assert.IsTrue(obj1.Equals(obj2));
            Assert.IsTrue(obj2.Equals(obj1));
        }

        [TestCase]
        public void Equals_Generic_Null()
        {
            // Create 2 objects
            IniEntry obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            IniEntry obj2 = null;

            // Equals
            Assert.IsFalse(obj1.Equals(obj2));
        }

        [TestCase]
        public void Equals_DifferentClassesEqualProperties()
        {
            ClassWithoutPropertiesA a = new ClassWithoutPropertiesA();
            ClassWithoutPropertiesB b = new ClassWithoutPropertiesB();

            Assert.AreNotEqual(a, b);
            Assert.IsFalse(a == b);
        }

        /// <summary>
        /// Tests the Equals method 1 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel1()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            var obj2 = ModelBaseTestHelper.CreateIniEntryObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        /// <summary>
        /// Tests the Equals method 2 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel2()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniFileObject();
            var obj2 = ModelBaseTestHelper.CreateIniFileObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        /// <summary>
        /// Tests the Equals method 3 level deep.
        /// </summary>
        [TestCase]
        public void EqualsLevel3()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateComputerSettingsObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        [TestCase]
        public void Equals_AreNotEqual()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateIniFileObject();

            // Equals
            Assert.AreNotEqual(obj1, obj2);
        }
        #endregion

        #region Parent/child tests
        /// <summary>
        /// Tests the parent and child relations.
        /// </summary>
        [TestCase]
        public void TestParentAndChildRelationsWhenCreating()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(parent, ((IParent)child).Parent);
        }

#if NET
        /// <summary>
        /// Tests the parent and child relations after deserialization.
        /// </summary>
        [TestCase]
        public void TestParentAndChildRelationsWhenBinaryDeserializing()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(((IParent)child).Parent, parent);

            using (var memoryStream = new MemoryStream())
            {
                parent.Save(memoryStream, SerializationMode.Binary, null);

                memoryStream.Position = 0L;

                var loadedParent = ModelBase.Load<Parent>(memoryStream, SerializationMode.Binary, null);

                Assert.AreEqual(parent, ((IParent)loadedParent.Children[0]).Parent);
            }
        }
#endif

        /// <summary>
        /// Tests the parent and child relations after deserialization.
        /// </summary>
        [TestCase]
        public void TestParentAndChildRelationsWhenXmlDeserializing()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(((IParent)child).Parent, parent);

            Parent loadedParent;
            using (var memoryStream = new MemoryStream())
            {
                parent.Save(memoryStream, SerializationMode.Xml, null);

                memoryStream.Position = 0L;

                loadedParent = ModelBase.Load<Parent>(memoryStream, SerializationMode.Xml, null);
            }

            Assert.AreEqual(parent, ((IParent)loadedParent.Children[0]).Parent);
        }
        #endregion

        #region Multiple inheritance tests
        /// <summary>
        /// Creates the and verify properties on inherited class. This test is used to determine
        /// if the properties defined in a derived class are also registered properly.
        /// </summary>
        [TestCase]
        public void CreateAndVerifyPropertiesOnInheritedClass()
        {
            // Create extend ini entry
            var extendedIniEntry = new ExtendedIniEntry();

            // Try to set original properties
            extendedIniEntry.Value = "MyValue";

            // Try to set new properties
            extendedIniEntry.DefaultValue = "DefaultValue";
        }
        #endregion

        #region Read-only tests
        [TestCase]
        public void ReadOnlyTest()
        {
            // Declare variables
            const string testValue = "new value that shouldn't exist";
            string originalValue, actualValue;

            IniEntry iniEntry = ModelBaseTestHelper.CreateIniEntryObject();

            // Test whether the object can be set to read-only
            originalValue = iniEntry.Value;
            iniEntry.SetReadOnly(true);
            iniEntry.Value = testValue;
            actualValue = iniEntry.Value;
            Assert.AreEqual(originalValue, actualValue);

            // Test whether the object can be set to edit mode again
            iniEntry.SetReadOnly(false);
            iniEntry.Value = testValue;
            actualValue = iniEntry.Value;
            Assert.AreEqual(testValue, actualValue);
        }
        #endregion

        #region Default values
        [TestCase]
        public void DefaultValues_ValueType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(0, obj.ValueType_NoDefaultValue);
        }

        [TestCase]
        public void DefaultValues_ValueType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(5, obj.ValueType_DefaultValueViaValue);
        }

        [TestCase]
        public void DefaultValues_ValueType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(10, obj.ValueType_DefaultValueViaCallback);
        }

        [TestCase]
        public void DefaultValues_ReferenceType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(null, obj.ReferenceType_NoDefaultValue);
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreNotEqual(null, obj.ReferenceType_DefaultValueViaValue);
            Assert.IsInstanceOf(typeof(Collection<int>), obj.ReferenceType_DefaultValueViaValue);
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_SameInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.IsTrue(ReferenceEquals(obj1.ReferenceType_DefaultValueViaValue, obj2.ReferenceType_DefaultValueViaValue));
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreNotEqual(null, obj.ReferenceType_DefaultValueViaCallback);
            Assert.IsInstanceOf(typeof(Collection<int>), obj.ReferenceType_DefaultValueViaCallback);
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_DifferentInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.IsFalse(ReferenceEquals(obj1.ReferenceType_DefaultValueViaCallback, obj2.ReferenceType_DefaultValueViaCallback));
        }
        #endregion

        #region INotifyPropertyChanging tests
        public class ChangingTest : ModelBase
        {
            protected override void OnPropertyChanging(AdvancedPropertyChangingEventArgs e)
            {
                if (string.Equals(e.PropertyName, CancallableProperty.Name))
                {
                    e.Cancel = true;
                }

                base.OnPropertyChanging(e);
            }

            /// <summary>
            /// Gets or sets the not cancallable property.
            /// </summary>
            public string NonCancallable
            {
                get { return GetValue<string>(NonCancallableProperty); }
                set { SetValue(NonCancallableProperty, value); }
            }

            /// <summary>
            /// Register the NonCancallable property so it is known in the class.
            /// </summary>
            public static readonly PropertyData NonCancallableProperty = RegisterProperty("NonCancallable", typeof(string), "non-cancallable");

            /// <summary>
            /// Gets or sets the cancallable property.
            /// </summary>
            public string Cancallable
            {
                get { return GetValue<string>(CancallableProperty); }
                set { SetValue(CancallableProperty, value); }
            }

            /// <summary>
            /// Register the Cancallable property so it is known in the class.
            /// </summary>
            public static readonly PropertyData CancallableProperty = RegisterProperty("Cancallable", typeof(string), "cancallable");
        }

        [TestCase]
        public void NotifyPropertyChanging()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanging += (sender, e) =>
            {
                if (isInvoked)
                {
                    return;
                }

                isInvoked = true;

                // Check property
                if (string.Compare(e.PropertyName, "Value") != 0)
                {
                    Assert.Fail("Wrong PropertyChanging property name");
                }
            };

            obj.Value = "MyNewValue";

            Assert.IsTrue(isInvoked, "PropertyChanging was not invoked");
        }

        [TestCase]
        public void NotifyPropertyChangingDoesNotCallForEqualPropertyValue()
        {
            var changingTest = new ChangingTest();
            changingTest.PropertyChanging += (sender, e) => Assert.Fail("Not expected a propertychanging event");

            changingTest.NonCancallable = ChangingTest.NonCancallableProperty.GetDefaultValue<string>();
        }

        [TestCase]
        public void NotifyPropertyChangingPreventsSetValue()
        {
            var changingTest = new ChangingTest();

            Assert.AreEqual("non-cancallable", changingTest.NonCancallable);
            Assert.AreEqual("cancallable", changingTest.Cancallable);

            changingTest.NonCancallable = "test";
            changingTest.Cancallable = "test";

            Assert.AreEqual("test", changingTest.NonCancallable);
            Assert.AreEqual("cancallable", changingTest.Cancallable);
        }
        #endregion

        #region Attribute validation
#if NET
        [TestCase]
        public void AttributeValidation_DoNotValidate()
        {
            var instance = new ObjectWithValidation();

            instance.SetValue(ObjectWithValidation.ValueWithAnnotationsProperty.Name, string.Empty, true, false);
            var fieldValidations = instance.GetValidationContext().GetFieldValidations(ObjectWithValidation.ValueWithAnnotationsProperty.Name);

            Assert.AreEqual(0, fieldValidations.Count);

            instance.SetValue(ObjectWithValidation.ValueWithAnnotationsProperty.Name, null, true, true);
            fieldValidations = instance.GetValidationContext().GetFieldValidations(ObjectWithValidation.ValueWithAnnotationsProperty.Name);

            Assert.AreEqual(1, fieldValidations.Count);
        }
#endif
        #endregion

        #region INotifyPropertyChanged tests
        [TestCase]
        public void NotifyPropertyChanged_Automatic()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                    {
                                        if (!isInvoked)
                                        {
                                            if (string.Compare(e.PropertyName, "Value") != 0)
                                            {
                                                Assert.Fail("Wrong PropertyChanged property name");
                                            }
                                        }

                                        isInvoked = true;
                                    };

            obj.Value = "MyNewValue";

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void NotifyPropertyChanged_ManualByExpression()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (!isInvoked)
                {
                    if (string.Compare(e.PropertyName, "Value") != 0)
                    {
                        Assert.Fail("Wrong PropertyChanged property name");
                    }
                }

                isInvoked = true;
            };

            obj.RaisePropertyChanged((() => obj.Value));

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void NotifyPropertyChanged_ManualByStringLiteral()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                if (!isInvoked)
                {
                    if (string.Compare(e.PropertyName, "Value") != 0)
                    {
                        Assert.Fail("Wrong PropertyChanged property name");
                    }
                }

                isInvoked = true;
            };

            obj.RaisePropertyChanged("Value");

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void NotifyPropertyChanged_OldValueSupport()
        {
            var obj = new IniEntry();

            object oldValue = null;
            object newValue = null;

            obj.PropertyChanged += (sender, e) =>
                                       {
                                           var advancedE = (AdvancedPropertyChangedEventArgs)e;

                                           if (advancedE.PropertyName == "Key")
                                           {
                                               Assert.IsTrue(advancedE.IsOldValueMeaningful);
                                               Assert.IsTrue(advancedE.IsNewValueMeaningful);

                                               oldValue = advancedE.OldValue;
                                               newValue = advancedE.NewValue;
                                           }
                                       };

            obj.Key = "new value";

            Assert.AreEqual(IniEntry.KeyProperty.GetDefaultValue(), oldValue);
            Assert.AreEqual("new value", newValue);
        }

        [TestCase]
        public void InvokePropertyChangedForAllRegisteredProperties()
        {
            List<string> expectedProperties = new List<string>();
            List<string> actualProperties = new List<string>();

            expectedProperties.Add(IniEntry.GroupProperty.Name);
            expectedProperties.Add(IniEntry.KeyProperty.Name);
            expectedProperties.Add(IniEntry.ValueProperty.Name);
            expectedProperties.Add(IniEntry.IniEntryTypeProperty.Name);

            var obj = ModelBaseTestHelper.CreateIniEntryObject();
            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                       {
                                           actualProperties.Add(e.PropertyName);
                                       };

            obj.RaisePropertyChangedForAllRegisteredProperties();

            Assert.AreEqual(expectedProperties.Count, actualProperties.Count);
            foreach (string property in expectedProperties)
            {
                if (!actualProperties.Contains(property))
                {
                    Assert.Fail("No PropertyChanged event for " + property);
                }
            }
        }
        #endregion

        [TestFixture]
        public class TheHideValidationResultsProperty
        {
            [TestCase]
            public void HidesTheFieldErrorsWhenTrue()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
                obj.HideValidationResults = true;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);
            }

            [TestCase]
            public void HidesTheBusinessRuleErrorsWhenTrue()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
                obj.HideValidationResults = true;

                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj).Error);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj).Error);
            }

            [TestCase]
            public void HidesTheFieldWarningsWhenTrue()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
                obj.HideValidationResults = true;

                obj.WarningWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);
            }

            [TestCase]
            public void HidesTheBusinessRuleWarningsWhenTrue()
            {
                var obj = new ValidationTest();
                var validation = obj as IModelValidation;
                obj.HideValidationResults = true;

                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsFalse(validation.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj).Warning);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj).Warning);
            }
        }

        [TestFixture]
        public class TheIsDirtyProperty
        {
            [TestCase]
            public void IsFalseByDefault()
            {
                var model = new ObjectWithCustomType();

                Assert.IsFalse(model.IsDirty);
            }

            [TestCase]
            public void IsTrueAfterChange()
            {
                var model = new ObjectWithCustomType();
                model.FirstName = "myNewFirstName";

                Assert.IsTrue(model.IsDirty);
            }
        }

        #region IsDirty with children test
        [TestCase]
        public void IsDirtyWithChildrenWhenSavingChild()
        {
            // Create a collection
            var computerSettings = ModelBaseTestHelper.CreateComputerSettingsObject();
            SaveObjectToMemoryStream(computerSettings);
            Assert.IsFalse(computerSettings.IsDirty);

            // Make a chance in the lowest level (but only if ObservableCollection, that is the only supported type)
            computerSettings.IniFileCollection[0].FileName = "is dirty should be enabled now";
            Assert.IsTrue(computerSettings.IniFileCollection[0].IsDirty);
            Assert.IsTrue(computerSettings.IsDirty);

            // Save the lowest level (so the parent stays dirty)
            SaveObjectToMemoryStream(computerSettings.IniFileCollection[0].IniEntryCollection[0]);
            Assert.IsFalse(computerSettings.IniFileCollection[0].IniEntryCollection[0].IsDirty);
            Assert.IsTrue(computerSettings.IsDirty);
        }

        [TestCase]
        public void IsDirtyWithChildrenWhenSavingParent()
        {
            // Create a collection
            var computerSettings = ModelBaseTestHelper.CreateComputerSettingsObject();
            SaveObjectToMemoryStream(computerSettings);
            Assert.IsFalse(computerSettings.IsDirty);

            // Make a chance in the lowest level (but only if ObservableCollection, that is the only supported type)
            computerSettings.IniFileCollection[0].FileName = "is dirty should be enabled now 2";
            Assert.IsTrue(computerSettings.IniFileCollection[0].IsDirty);
            Assert.IsTrue(computerSettings.IsDirty);

            // Save the top level
            SaveObjectToMemoryStream(computerSettings);
            Assert.IsFalse(computerSettings.IniFileCollection[0].IniEntryCollection[0].IsDirty);
            Assert.IsFalse(computerSettings.IsDirty);
        }
        #endregion

        #region Get/set value
        [TestCase]
        public void GetValue_Null()
        {
            var entry = new IniEntry();

            ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => entry.GetValue<string>(null));
        }

        [TestCase]
        public void GetValue_NonExistingProperty()
        {
            var entry = new IniEntry();

            ExceptionTester.CallMethodAndExpectException<PropertyNotRegisteredException>(() => entry.GetValue<string>("Non-existing property"));
        }

        [TestCase]
        public void GetValue_ExistingProperty()
        {
            var entry = new IniEntry();
            entry.Key = "key value";
            var value = entry.GetValue<string>(IniEntry.KeyProperty.Name);
            Assert.AreEqual("key value", value);
        }
        #endregion

        #region Late property registration
        [TestCase]
        public void InitializePropertyAfterConstruction_Null()
        {
            var obj = new DynamicObject();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => obj.InitializePropertyAfterConstruction(null));
        }

        [TestCase]
        public void InitializePropertyAfterConstruction_SingleInstanceConstruction()
        {
            var obj = new DynamicObject();
            var dynamicProperty = DynamicObject.RegisterProperty("DynamicProperty", typeof(int));
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.AreEqual(5, obj.GetValue<int>(dynamicProperty.Name));
        }

        [TestCase]
        public void InitializePropertyAfterConstruction_DoubleInstanceConstruction()
        {
            // Added because of a bug where double instantiation would not initialize the properties correctly
            // the 2nd time
            var obj = new DynamicObject();
            var dynamicProperty = DynamicObject.RegisterProperty("DynamicProperty", typeof(int));
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.AreEqual(5, obj.GetValue<int>(dynamicProperty.Name));

            obj = new DynamicObject();
            dynamicProperty = DynamicObject.RegisterProperty("DynamicProperty", typeof(int));
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.AreEqual(5, obj.GetValue<int>(dynamicProperty.Name));
        }
        #endregion

        #region Non magic string property registration overload
#if !NETFX_CORE
        [TestCase]
        public void PropertiesAreActuallyRegistered()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.IsTrue(instance.IsPropertyRegistered("StringProperty"));
            Assert.IsTrue(instance.IsPropertyRegistered("StringPropertyWithSpecifiedDefaultValue"));
            Assert.IsTrue(instance.IsPropertyRegistered("IntPropertyWithPropertyChangeNotication"));
            Assert.IsTrue(instance.IsPropertyRegistered("IntPropertyExcludedFromSerializationAndBackup"));
        }

        [TestCase]
        public void PropertiesAreActuallyRegisteredWithDefaultValues()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.GetDefaultValue<string>(), instance.StringPropertyWithSpecifiedDefaultValue);
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.GetDefaultValue<string>(), instance.StringProperty);
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.GetDefaultValue<int>(), instance.IntPropertyWithPropertyChangeNotication);
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.GetDefaultValue<int>(), instance.IntPropertyExcludedFromSerializationAndBackup);
        }

        [TestCase]
        public void PropertiesAreActuallyRegisteredWithTheSpecifiedConfigurationForSerializationAndBackup()
        {
            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.IncludeInSerialization);
            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.IncludeInSerialization);
            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.IncludeInSerialization);

            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.IncludeInBackup);
            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.IncludeInBackup);
            Assert.AreEqual(true, ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.IncludeInBackup);

            Assert.AreEqual(false, ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.IncludeInSerialization);
            Assert.AreEqual(false, ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.IncludeInBackup);
        }

        [TestCase]
        public void PropertiesAreRegisteredWithPropertyChangeNotification()
        {
            Assert.IsNotNull(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.PropertyChangedEventHandler);

            var random = new Random();
            int maxPropertyChanges = random.Next(0, 15);
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            for (int i = 0; i < maxPropertyChanges; i++)
            {
                instance.IntPropertyWithPropertyChangeNotication = random.Next(1000);
            }

            Assert.IsTrue(0 <= instance.IntPropertyWithPropertyChangeNoticationsCount && instance.IntPropertyWithPropertyChangeNoticationsCount <= maxPropertyChanges);
        }

        public class TestModelWithGenericPropertyRegistrations : ModelBase
        {
            public bool ValueType
            {
                get { return GetValue<bool>(ValueTypeProperty); }
                set { SetValue(ValueTypeProperty, value); }
            }

            public static readonly PropertyData ValueTypeProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, bool>(o => o.ValueType);

            public bool ValueTypeWithDefaultValue
            {
                get { return GetValue<bool>(ValueTypeWithDefaultValueProperty); }
                set { SetValue(ValueTypeWithDefaultValueProperty, value); }
            }

            public static readonly PropertyData ValueTypeWithDefaultValueProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, bool>(o => o.ValueTypeWithDefaultValue, () => true);

            public object ReferenceType
            {
                get { return GetValue<object>(ReferenceTypeProperty); }
                set { SetValue(ReferenceTypeProperty, value); }
            }

            public static readonly PropertyData ReferenceTypeProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, object>(o => o.ReferenceType);

            public object ReferenceTypeWithDefaultValue
            {
                get { return GetValue<object>(ReferenceTypeWithDefaultValueProperty); }
                set { SetValue(ReferenceTypeWithDefaultValueProperty, value); }
            }

            public static readonly PropertyData ReferenceTypeWithDefaultValueProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, object>(o => o.ReferenceTypeWithDefaultValue, () => new object());
        }

        [TestCase]
        public void PropertiesAreRegisteredWithGenerics()
        {
            var model = new TestModelWithGenericPropertyRegistrations();

            Assert.IsTrue(model.IsPropertyRegistered("ValueType"));
            Assert.IsTrue(model.IsPropertyRegistered("ValueTypeWithDefaultValue"));
            Assert.IsTrue(model.IsPropertyRegistered("ReferenceType"));
            Assert.IsTrue(model.IsPropertyRegistered("ReferenceTypeWithDefaultValue"));
        }
#endif
        #endregion

        /// <summary>
        /// Saves the object to memory stream so the <see cref="IModel.IsDirty" /> property is set to false.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="configuration">The configuration.</param>
        internal static void SaveObjectToMemoryStream(ISavableModel obj, ISerializationConfiguration configuration = null)
        {
            using (var memoryStream = new MemoryStream())
            {
                obj.Save(memoryStream, configuration);
            }
        }
    }
}

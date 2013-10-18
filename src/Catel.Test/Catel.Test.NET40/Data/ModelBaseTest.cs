namespace Catel.Test.Data
{
    using System.Collections.ObjectModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using Catel.Data;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    [TestClass]
    public class ModelBaseTest
    {
        #region Fields
#if !NETFX_CORE
        private FilesHelper _filesHelper;
#endif
        #endregion

        #region Initialization and cleanup
#if !NETFX_CORE
        [TestInitialize]
        public void Initialize()
        {
            if (_filesHelper == null)
            {
                _filesHelper = new FilesHelper();
            }
        }

        [TestCleanup]
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
        [TestMethod]
        public void Equals_Generic()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            var obj2 = ModelBaseTestHelper.CreateIniEntryObject();

            // Equals
            Assert.IsTrue(obj1.Equals(obj2));
            Assert.IsTrue(obj2.Equals(obj1));
        }

        [TestMethod]
        public void Equals_Generic_Null()
        {
            // Create 2 objects
            IniEntry obj1 = ModelBaseTestHelper.CreateIniEntryObject();
            IniEntry obj2 = null;

            // Equals
            Assert.IsFalse(obj1.Equals(obj2));
        }

        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void EqualsLevel3()
        {
            // Create 2 objects
            var obj1 = ModelBaseTestHelper.CreateComputerSettingsObject();
            var obj2 = ModelBaseTestHelper.CreateComputerSettingsObject();

            // Equals
            Assert.AreEqual(obj1, obj2);
        }

        [TestMethod]
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
        [TestMethod]
        public void TestParentAndChildRelationsWhenCreating()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(parent, ((IParent)child).Parent);
        }

#if !NETFX_CORE
        /// <summary>
        /// Tests the parent and child relations after deserialization.
        /// </summary>
        [TestMethod]
        public void TestParentAndChildRelationsWhenBinaryDeserializing()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(((IParent)child).Parent, parent);

            var file = _filesHelper.GetTempFile();
            parent.Save(file);
#if SILVERLIGHT
            file.Position = 0L;
#endif
            var loadedParent = SavableModelBase<Parent>.Load(file);

            Assert.AreEqual(parent, ((IParent)loadedParent.Children[0]).Parent);
        }
#endif

        /// <summary>
        /// Tests the parent and child relations after deserialization.
        /// </summary>
        [TestMethod]
        public void TestParentAndChildRelationsWhenXmlDeserializing()
        {
            var parent = new Parent("Parent");
            var child = parent.CreateChild("Child");

            Assert.AreEqual(((IParent)child).Parent, parent);

            Parent loadedParent;
            using (var memoryStream = new MemoryStream())
            {
                parent.Save(memoryStream, SerializationMode.Xml);

                memoryStream.Position = 0L;

                loadedParent = SavableModelBase<Parent>.Load(memoryStream, SerializationMode.Xml);
            }

            Assert.AreEqual(parent, ((IParent)loadedParent.Children[0]).Parent);
        }
        #endregion

        #region Multiple inheritance tests
        /// <summary>
        /// Creates the and verify properties on inherited class. This test is used to determine
        /// if the properties defined in a derived class are also registered properly.
        /// </summary>
        [TestMethod]
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
        [TestMethod]
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
        [TestMethod]
        public void DefaultValues_ValueType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(0, obj.ValueType_NoDefaultValue);
        }

        [TestMethod]
        public void DefaultValues_ValueType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(5, obj.ValueType_DefaultValueViaValue);
        }

        [TestMethod]
        public void DefaultValues_ValueType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(10, obj.ValueType_DefaultValueViaCallback);
        }

        [TestMethod]
        public void DefaultValues_ReferenceType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreEqual(null, obj.ReferenceType_NoDefaultValue);
        }

        [TestMethod]
        public void DefaultValues_ReferenceType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreNotEqual(null, obj.ReferenceType_DefaultValueViaValue);
            Assert.IsInstanceOfType(obj.ReferenceType_DefaultValueViaValue, typeof(Collection<int>));
        }

        [TestMethod]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_SameInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.IsTrue(ReferenceEquals(obj1.ReferenceType_DefaultValueViaValue, obj2.ReferenceType_DefaultValueViaValue));
        }

        [TestMethod]
        public void DefaultValues_ReferenceType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.AreNotEqual(null, obj.ReferenceType_DefaultValueViaCallback);
            Assert.IsInstanceOfType(obj.ReferenceType_DefaultValueViaCallback, typeof(Collection<int>));
        }

        [TestMethod]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_DifferentInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.IsFalse(ReferenceEquals(obj1.ReferenceType_DefaultValueViaCallback, obj2.ReferenceType_DefaultValueViaCallback));
        }
        #endregion

#if NET
        #region Allow non serializable members tests
        [TestMethod]
        public void ObjectWithoutAllowNonSerializableAttribute()
        {
            // "object should crash because it contains a non-serializable value, but is not decorated with the AllowNonSerializableMembersAttribute"
            ExceptionTester.CallMethodAndExpectException<InvalidPropertyException>(() => new ObjectWithNonSerializableMembers());
        }

        [TestMethod]
        public void ObjectWithAllowNonSerializableAttribute()
        {
            var obj = new ObjectWithNonSerializableMembersDecoratedWithAllowNonSerializableMembersAttribute();
            obj.ToString();
        }
        #endregion
#endif

        #region IClonable tests
        /// <summary>
        /// Tests the deep-clone functionality for 1 level deep.
        /// </summary>
        [TestMethod]
        public void CloneLevel1()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();
            var clonedObj = obj.Clone();

            Assert.AreEqual(obj, clonedObj);
        }

        /// <summary>
        /// Tests the deep-clone functionality for 2 levels deep.
        /// </summary>
        [TestMethod]
        public void CloneLevel2()
        {
            var obj = ModelBaseTestHelper.CreateIniFileObject();
            var clonedObj = obj.Clone();

            Assert.AreEqual(obj, clonedObj);
        }

        /// <summary>
        /// Tests the deep-clone functionality for 3 levels deep.
        /// </summary>
        [TestMethod]
        public void CloneLevel3()
        {
            var obj = ModelBaseTestHelper.CreateComputerSettingsObject();
            var clonedObj = obj.Clone();

            Assert.AreEqual(obj, clonedObj);
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

        [TestMethod]
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

        [TestMethod]
        public void NotifyPropertyChangingDoesNotCallForEqualPropertyValue()
        {
            var changingTest = new ChangingTest();
            changingTest.PropertyChanging += (sender, e) => Assert.Fail("Not expected a propertychanging event");

            changingTest.NonCancallable = ChangingTest.NonCancallableProperty.GetDefaultValue<string>();
        }

        [TestMethod]
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
#if NET || SL4 || SL5
        [TestMethod]
        public void AttributeValidation_DoNotValidate()
        {
            var instance = new ObjectWithValidation();

            instance.SetValue(ObjectWithValidation.ValueWithAnnotationsProperty.Name, string.Empty, true, false);
            var fieldValidations = instance.ValidationContext.GetFieldValidations(ObjectWithValidation.ValueWithAnnotationsProperty.Name);

            Assert.AreEqual(0, fieldValidations.Count);

            instance.SetValue(ObjectWithValidation.ValueWithAnnotationsProperty.Name, null, true, true);
            fieldValidations = instance.ValidationContext.GetFieldValidations(ObjectWithValidation.ValueWithAnnotationsProperty.Name);

            Assert.AreEqual(1, fieldValidations.Count);
        }
#endif
        #endregion

        #region INotifyPropertyChanged tests
        [TestMethod]
        public void NotifyPropertyChanged_Automatic()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
                                    {
                                        isInvoked = true;

                                        if (string.Compare(e.PropertyName, "Value") != 0)
                                        {
                                            Assert.Fail("Wrong PropertyChanged property name");
                                        }
                                    };

            obj.Value = "MyNewValue";

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestMethod]
        public void NotifyPropertyChanged_ManualByExpression()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                isInvoked = true;

                if (string.Compare(e.PropertyName, "Value") != 0)
                {
                    Assert.Fail("Wrong PropertyChanged property name");
                }
            };

            obj.RaisePropertyChanged((() => obj.Value));

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestMethod]
        public void NotifyPropertyChanged_ManualByStringLiteral()
        {
            var obj = ModelBaseTestHelper.CreateIniEntryObject();

            bool isInvoked = false;

            obj.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                isInvoked = true;

                if (string.Compare(e.PropertyName, "Value") != 0)
                {
                    Assert.Fail("Wrong PropertyChanged property name");
                }
            };

            obj.RaisePropertyChanged("Value");

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestClass]
        public class TheHideValidationResultsProperty
        {
            [TestMethod]
            public void HidesTheFieldErrorsWhenTrue()
            {
                var obj = new ValidationTest();
                obj.HideValidationResults = true;

                obj.ErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj)["ErrorWhenEmpty"]);
            }

            [TestMethod]
            public void HidesTheBusinessRuleErrorsWhenTrue()
            {
                var obj = new ValidationTest();
                obj.HideValidationResults = true;

                obj.BusinessRuleErrorWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasErrors);
                Assert.AreEqual(string.Empty, ((IDataErrorInfo)obj).Error);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataErrorInfo)obj).Error);
            }

            [TestMethod]
            public void HidesTheFieldWarningsWhenTrue()
            {
                var obj = new ValidationTest();
                obj.HideValidationResults = true;

                obj.WarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj)["WarningWhenEmpty"]);
            }

            [TestMethod]
            public void HidesTheBusinessRuleWarningsWhenTrue()
            {
                var obj = new ValidationTest();
                obj.HideValidationResults = true;

                obj.BusinessRuleWarningWhenEmpty = string.Empty;

                Assert.IsTrue(obj.HasWarnings);
                Assert.AreEqual(string.Empty, ((IDataWarningInfo)obj).Warning);

                obj.HideValidationResults = false;

                Assert.AreNotEqual(string.Empty, ((IDataWarningInfo)obj).Warning);
            }
        }

        [TestClass]
        public class TheIsDirtyProperty
        {
            [TestMethod]
            public void IsFalseByDefault()
            {
                var model = new ObjectWithCustomType();

                Assert.IsFalse(model.IsDirty);
            }

            [TestMethod]
            public void IsTrueAfterChange()
            {
                var model = new ObjectWithCustomType();
                model.FirstName = "myNewFirstName";

                Assert.IsTrue(model.IsDirty);
            }
        }

        #region IsDirty with children test
        [TestMethod]
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

        [TestMethod]
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
        [TestMethod]
        public void GetValue_Null()
        {
            IniEntry entry = new IniEntry();

            ExceptionTester.CallMethodAndExpectException<ArgumentException>(() => entry.GetValue<string>(null));
        }

        [TestMethod]
        public void GetValue_NonExistingProperty()
        {
            IniEntry entry = new IniEntry();

            ExceptionTester.CallMethodAndExpectException<PropertyNotRegisteredException>(() => entry.GetValue<string>("Non-existing property"));
        }

        [TestMethod]
        public void GetValue_ExistingProperty()
        {
            IniEntry entry = new IniEntry();
            entry.Key = "key value";
            var value = entry.GetValue<string>(IniEntry.KeyProperty.Name);
            Assert.AreEqual("key value", value);
        }
        #endregion

        #region Late property registration
        [TestMethod]
        public void InitializePropertyAfterConstruction_Null()
        {
            var obj = new DynamicObject();
            ExceptionTester.CallMethodAndExpectException<ArgumentNullException>(() => obj.InitializePropertyAfterConstruction(null));
        }

        [TestMethod]
        public void InitializePropertyAfterConstruction_SingleInstanceConstruction()
        {
            var obj = new DynamicObject();
            var dynamicProperty = DynamicObject.RegisterProperty("DynamicProperty", typeof(int));
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.AreEqual(5, obj.GetValue<int>(dynamicProperty.Name));
        }

        [TestMethod]
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
        [TestMethod]
        public void PropertiesAreActuallyRegistered()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.IsTrue(instance.IsPropertyRegistered("StringProperty"));
            Assert.IsTrue(instance.IsPropertyRegistered("StringPropertyWithSpecifiedDefaultValue"));
            Assert.IsTrue(instance.IsPropertyRegistered("IntPropertyWithPropertyChangeNotication"));
            Assert.IsTrue(instance.IsPropertyRegistered("IntPropertyExcludedFromSerializationAndBackup"));
        }

        [TestMethod]
        public void PropertiesAreActuallyRegisteredWithDefaultValues()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.GetDefaultValue<string>(), instance.StringPropertyWithSpecifiedDefaultValue); 
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.GetDefaultValue<string>(), instance.StringProperty);
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.GetDefaultValue<int>(), instance.IntPropertyWithPropertyChangeNotication);
            Assert.AreEqual(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.GetDefaultValue<int>(), instance.IntPropertyExcludedFromSerializationAndBackup); 
        }       
        
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
        /// Saves the object to memory stream so the <see cref="IModel.IsDirty"/> property is set to false.
        /// </summary>
        /// <param name="obj">The object.</param>
        internal static void SaveObjectToMemoryStream(ISavableModel obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                obj.Save(memoryStream);
            }
        }
    }
}

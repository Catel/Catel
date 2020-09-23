namespace Catel.Tests.Data
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
#if !UWP
        private FilesHelper _filesHelper;
#endif
        #endregion

        #region Initialization and cleanup
#if !UWP
        [SetUp]
        public void Initialize()
        {
            if (_filesHelper is null)
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

        #region Get/set value
        [TestCase]
        public void GetValue_Null()
        {
            var entry = new IniEntry();

            Assert.Throws<ArgumentException>(() => entry.GetValue<string>(null));
        }

        [TestCase]
        public void GetValue_NonExistingProperty()
        {
            var entry = new IniEntry();

            Assert.Throws<PropertyNotRegisteredException>(() => entry.GetValue<string>("Non-existing property"));
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
            Assert.Throws<ArgumentNullException>(() => obj.InitializePropertyAfterConstruction(null));
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
    }
}

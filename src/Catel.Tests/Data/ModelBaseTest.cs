namespace Catel.Tests.Data
{
    using System.Collections.ObjectModel;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;
    using NUnit.Framework;
    using Catel.Tests.Data.TestClasses;

    [TestFixture]
    public class ModelBaseTest
    {
        private FilesHelper _filesHelper;

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
            if (_filesHelper is not null)
            {
                _filesHelper.CleanUp();
                _filesHelper = null;
            }
        }

        #region Default values
        [TestCase]
        public void DefaultValues_ValueType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ValueType_NoDefaultValue, Is.EqualTo(0));
        }

        [TestCase]
        public void DefaultValues_ValueType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ValueType_DefaultValueViaValue, Is.EqualTo(5));
        }

        [TestCase]
        public void DefaultValues_ValueType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ValueType_DefaultValueViaCallback, Is.EqualTo(10));
        }

        [TestCase]
        public void DefaultValues_ReferenceType_NoDefaultValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ReferenceType_NoDefaultValue, Is.EqualTo(null));
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ReferenceType_DefaultValueViaValue, Is.Not.EqualTo(null));
            Assert.That(obj.ReferenceType_DefaultValueViaValue, Is.InstanceOf(typeof(Collection<int>)));
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_SameInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.That(ReferenceEquals(obj1.ReferenceType_DefaultValueViaValue, obj2.ReferenceType_DefaultValueViaValue), Is.True);
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaCallback()
        {
            var obj = new ObjectWithDefaultValues();

            Assert.That(obj.ReferenceType_DefaultValueViaCallback, Is.Not.EqualTo(null));
            Assert.That(obj.ReferenceType_DefaultValueViaCallback, Is.InstanceOf(typeof(Collection<int>)));
        }

        [TestCase]
        public void DefaultValues_ReferenceType_DefaultValueViaValue_DifferentInstanceForAllClasses()
        {
            var obj1 = new ObjectWithDefaultValues();
            var obj2 = new ObjectWithDefaultValues();

            Assert.That(ReferenceEquals(obj1.ReferenceType_DefaultValueViaCallback, obj2.ReferenceType_DefaultValueViaCallback), Is.False);
        }
        #endregion

        #region INotifyPropertyChanged tests
        [TestCase]
        public void NotifyPropertyChanged_Automatic()
        {
            var obj = new Child();

            bool isInvoked = false;

            obj.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
                                    {
                                        if (!isInvoked)
                                        {
                                            if (string.Compare(e.PropertyName, nameof(Child.Name)) != 0)
                                            {
                                                Assert.Fail("Wrong PropertyChanged property name");
                                            }
                                        }

                                        isInvoked = true;
                                    };

            obj.Name = "MyNewValue";

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void NotifyPropertyChanged_ManualByExpression()
        {
            var model = new Child();

            bool isInvoked = false;

            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (!isInvoked)
                {
                    if (string.Compare(e.PropertyName, nameof(Child.Name)) != 0)
                    {
                        Assert.Fail("Wrong PropertyChanged property name");
                    }
                }

                isInvoked = true;
            };

            model.RaisePropertyChanged((() => model.Name));

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void NotifyPropertyChanged_ManualByStringLiteral()
        {
            var model = new Child();

            bool isInvoked = false;

            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                if (!isInvoked)
                {
                    if (string.Compare(e.PropertyName, nameof(Child.Name)) != 0)
                    {
                        Assert.Fail("Wrong PropertyChanged property name");
                    }
                }

                isInvoked = true;
            };

            model.RaisePropertyChanged(nameof(Child.Name));

            if (!isInvoked)
            {
                Assert.Fail("PropertyChanged was not invoked");
            }
        }

        [TestCase]
        public void InvokePropertyChangedForAllRegisteredProperties()
        {
            var expectedProperties = new List<string>();
            var actualProperties = new List<string>();

            expectedProperties.Add(PersonTestModel.FirstNameProperty.Name);
            expectedProperties.Add(PersonTestModel.LastNameProperty.Name);

            var model = new PersonTestModel();
            model.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
            {
                actualProperties.Add(e.PropertyName);
            };

            model.RaisePropertyChangedForAllRegisteredProperties();

            Assert.That(actualProperties.Count, Is.EqualTo(expectedProperties.Count));
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

                Assert.That(model.IsDirty, Is.False);
            }

            [TestCase]
            public void IsTrueAfterChange()
            {
                var model = new ObjectWithCustomType();
                model.FirstName = "myNewFirstName";

                Assert.That(model.IsDirty, Is.True);
            }
        }

        #region Get/set value
        [TestCase]
        public void GetValue_Null()
        {
            var entry = new PersonTestModel();

            Assert.Throws<ArgumentException>(() => entry.GetValueWrapper<string>(null));
        }

        [TestCase]
        public void GetValue_NonExistingProperty()
        {
            var entry = new PersonTestModel();

            Assert.Throws<PropertyNotRegisteredException>(() => entry.GetValueWrapper<string>("Non-existing property"));
        }

        [TestCase]
        public void GetValue_ExistingProperty()
        {
            var entry = new PersonTestModel();
            entry.FirstName = "key value";
            var value = entry.GetValueWrapper<string>(PersonTestModel.FirstNameProperty.Name);
            Assert.That(value, Is.EqualTo("key value"));
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
            var dynamicProperty = DynamicObject.RegisterProperty<int>("DynamicProperty");
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.That(obj.GetValue<int>(dynamicProperty.Name), Is.EqualTo(5));
        }

        [TestCase]
        public void InitializePropertyAfterConstruction_DoubleInstanceConstruction()
        {
            // Added because of a bug where double instantiation would not initialize the properties correctly
            // the 2nd time
            var obj = new DynamicObject();
            var dynamicProperty = DynamicObject.RegisterProperty<int>("DynamicProperty");
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.That(obj.GetValue<int>(dynamicProperty.Name), Is.EqualTo(5));

            obj = new DynamicObject();
            dynamicProperty = DynamicObject.RegisterProperty<int>("DynamicProperty");
            obj.InitializePropertyAfterConstruction(dynamicProperty);

            obj.SetValue(dynamicProperty.Name, 5);
            Assert.That(obj.GetValue<int>(dynamicProperty.Name), Is.EqualTo(5));
        }
        #endregion

        #region Non magic string property registration overload
        [TestCase]
        public void PropertiesAreActuallyRegistered()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.That(instance.IsPropertyRegistered("StringProperty"), Is.True);
            Assert.That(instance.IsPropertyRegistered("StringPropertyWithSpecifiedDefaultValue"), Is.True);
            Assert.That(instance.IsPropertyRegistered("IntPropertyWithPropertyChangeNotication"), Is.True);
            Assert.That(instance.IsPropertyRegistered("IntPropertyExcludedFromSerializationAndBackup"), Is.True);
        }

        [TestCase]
        public void PropertiesAreActuallyRegisteredWithDefaultValues()
        {
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            Assert.That(instance.StringPropertyWithSpecifiedDefaultValue, Is.EqualTo(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.GetDefaultValue<string>()));
            Assert.That(instance.StringProperty, Is.EqualTo(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.GetDefaultValue<string>()));
            Assert.That(instance.IntPropertyWithPropertyChangeNotication, Is.EqualTo(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.GetDefaultValue<int>()));
            Assert.That(instance.IntPropertyExcludedFromSerializationAndBackup, Is.EqualTo(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.GetDefaultValue<int>()));
        }

        [TestCase]
        public void PropertiesAreActuallyRegisteredWithTheSpecifiedConfigurationForSerializationAndBackup()
        {
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.IncludeInSerialization, Is.EqualTo(true));
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.IncludeInSerialization, Is.EqualTo(true));
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.IncludeInSerialization, Is.EqualTo(true));

            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyWithSpecifiedDefaultValueProperty.IncludeInBackup, Is.EqualTo(true));
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.StringPropertyProperty.IncludeInBackup, Is.EqualTo(true));
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.IncludeInBackup, Is.EqualTo(true));

            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.IncludeInSerialization, Is.EqualTo(false));
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyExcludedFromSerializationAndBackupProperty.IncludeInBackup, Is.EqualTo(false));
        }

        [TestCase]
        public void PropertiesAreRegisteredWithPropertyChangeNotification()
        {
            Assert.That(ClassWithPropertiesRegisteredByNonMagicStringOverload.IntPropertyWithPropertyChangeNoticationProperty.PropertyChangedEventHandler, Is.Not.Null);

            var random = new Random();
            int maxPropertyChanges = random.Next(0, 15);
            var instance = new ClassWithPropertiesRegisteredByNonMagicStringOverload();
            for (int i = 0; i < maxPropertyChanges; i++)
            {
                instance.IntPropertyWithPropertyChangeNotication = random.Next(1000);
            }

            Assert.That(0 <= instance.IntPropertyWithPropertyChangeNoticationsCount && instance.IntPropertyWithPropertyChangeNoticationsCount <= maxPropertyChanges, Is.True);
        }

        public class TestModelWithGenericPropertyRegistrations : ModelBase
        {
            public bool ValueType
            {
                get { return GetValue<bool>(ValueTypeProperty); }
                set { SetValue(ValueTypeProperty, value); }
            }

            public static readonly IPropertyData ValueTypeProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, bool>(o => o.ValueType);

            public bool ValueTypeWithDefaultValue
            {
                get { return GetValue<bool>(ValueTypeWithDefaultValueProperty); }
                set { SetValue(ValueTypeWithDefaultValueProperty, value); }
            }

            public static readonly IPropertyData ValueTypeWithDefaultValueProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, bool>(o => o.ValueTypeWithDefaultValue, () => true);

            public object ReferenceType
            {
                get { return GetValue<object>(ReferenceTypeProperty); }
                set { SetValue(ReferenceTypeProperty, value); }
            }

            public static readonly IPropertyData ReferenceTypeProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, object>(o => o.ReferenceType);

            public object ReferenceTypeWithDefaultValue
            {
                get { return GetValue<object>(ReferenceTypeWithDefaultValueProperty); }
                set { SetValue(ReferenceTypeWithDefaultValueProperty, value); }
            }

            public static readonly IPropertyData ReferenceTypeWithDefaultValueProperty = RegisterProperty<TestModelWithGenericPropertyRegistrations, object>(o => o.ReferenceTypeWithDefaultValue, () => new object());
        }

        [TestCase]
        public void PropertiesAreRegisteredWithGenerics()
        {
            var model = new TestModelWithGenericPropertyRegistrations();

            Assert.That(model.IsPropertyRegistered("ValueType"), Is.True);
            Assert.That(model.IsPropertyRegistered("ValueTypeWithDefaultValue"), Is.True);
            Assert.That(model.IsPropertyRegistered("ReferenceType"), Is.True);
            Assert.That(model.IsPropertyRegistered("ReferenceTypeWithDefaultValue"), Is.True);
        }
        #endregion
    }
}

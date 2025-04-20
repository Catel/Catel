namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;

    using NUnit.Framework;

    public partial class ModelBaseFacts
    {
        [TestFixture]
        public class TheUnregisterPropertyMethod
        {
            [TestCase]
            public void CanUnregisterRuntimeProperty()
            {
                var model = new ModelWithRuntimeProperties();
                var property = ModelBase.RegisterProperty<string>("RuntimePropertyBeingUnregistered");
                model.InitializePropertyAfterConstruction(property);

                Assert.That(PropertyDataManager.Default.IsPropertyRegistered(typeof(ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered"), Is.True);

                ModelBase.UnregisterProperty(typeof(ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered");

                Assert.That(PropertyDataManager.Default.IsPropertyRegistered(typeof(ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered"), Is.False);
            }
        }

        [TestFixture]
        public class TheMemoryLeakChecks
        {
            [TestCase, Explicit]
            public void DoesNotLeakForModelBaseWithPropertiesThatSupportPropertyChanged()
            {
                var model = new PersonTestModel();
                var weakReference = new WeakReference(model);

                Assert.That(weakReference.IsAlive, Is.True);

                model = null;
                GC.Collect();

                Assert.That(weakReference.IsAlive, Is.False);
            }
        }

        [TestFixture]
        public class TheCalculatedPropertiesChecks
        {
            [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
            public class BoolRequiredAttribute : ValidationAttribute
            {
                private readonly bool _value;

                public BoolRequiredAttribute(bool value = true)
                {
                    _value = value;
                }

                public override bool IsValid(object obj)
                {
                    if (obj is null)
                    {
                        return false;
                    }

                    if (!(obj is bool))
                    {
                        return false;
                    }

                    var b = (bool)obj;
                    if (b == _value)
                    {
                        return true;
                    }

                    return false;
                }
            }

            public class ComputedPropertiesModel : ModelBase
            {
                protected override void InitializeCustomProperties()
                {
                    var propertyData = RegisterProperty<bool>(nameof(ComputedProperty));

                    InitializePropertyAfterConstruction(propertyData);
                }

                public bool ComputedProperty
                {
                    get
                    {
                        return true;
                    }
                }
            }

            public class LatePropertyRegistrationModel : ValidatableModelBase
            {
                protected override void InitializeCustomProperties()
                {
                    var propertyData = RegisterProperty<bool>(nameof(CanSave));

                    InitializePropertyAfterConstruction(propertyData);
                }

                [BoolRequired]
                public bool CanSave
                {
                    get
                    {
                        return true;
                    }
                }
            }

            [TestCase]
            public void CorrectlyHandlesLateRegistrationOfCalculatedProperties()
            {
                var model = new LatePropertyRegistrationModel();
                var validation = model as IValidatableModel;

                validation.Validate(true);
                Assert.That(validation.HasErrors, Is.False);

                var propertyData = PropertyDataManager.Default.GetPropertyData(typeof(LatePropertyRegistrationModel), nameof(LatePropertyRegistrationModel.CanSave));
                Assert.That(propertyData.IsCalculatedProperty, Is.True);
            }

            [TestCase]
            public void CorrectlyRetrievesCalculatedPropertyValues()
            {
                var model = new ComputedPropertiesModel();

                var propertyData = PropertyDataManager.Default.GetPropertyData(typeof(ComputedPropertiesModel), nameof(ComputedPropertiesModel.ComputedProperty));
                Assert.That(propertyData.IsCalculatedProperty, Is.True);

                var propertyValue = ((IModelEditor)model).GetValue<bool>(nameof(ComputedPropertiesModel.ComputedProperty));
                Assert.That(propertyValue, Is.True);
            }
        }

        [TestFixture]
        public class TheEqualsChecks
        {
            public interface ITestModel
            {

            }

            public class TestModel : ModelBase, ITestModel
            {

            }

#pragma warning disable 659
            public class TestModelWithCustomizedEquals : ModelBase, ITestModel
            {
                public override bool Equals(object obj)
                {
#pragma warning disable 252,253
                    return this == obj;
#pragma warning restore 252,253
                }
            }
#pragma warning restore 659

            //[TestCase]
            //public void EqualsWorksWithoutProperties()
            //{
            //    var collection = new ObservableCollection<ITestModel>();
            //    var a = new TestModel();
            //    var b = new TestModel();

            //    AddToCollection(collection, a);
            //    AddToCollection(collection, b);

            //    Assert.AreEqual(2, collection.Count);
            //    Assert.IsTrue(collection.Contains(a));
            //    Assert.IsTrue(collection.Contains(b));
            //}

            [TestCase]
            public void EqualsWorksWithoutPropertiesOverridingEqualsMethod()
            {
                //Solution1: overide the Equal Method of myclass 
                var collection = new ObservableCollection<ITestModel>();
                var a = new TestModelWithCustomizedEquals();
                var b = new TestModelWithCustomizedEquals();
                AddToCollection(collection, a);
                AddToCollection(collection, b);

                Assert.That(collection.Count, Is.EqualTo(2));
                Assert.That(collection.Contains(a), Is.True);
                Assert.That(collection.Contains(b), Is.True);
            }

            [TestCase]
            public void EqualsWorksWithoutPropertiesCustomizingAddMethod()
            {
                //Solution2:not using  the default Contains methord of ICollection<T>
                var collection = new ObservableCollection<ITestModel>();
                var a = new TestModelWithCustomizedEquals();
                var b = new TestModelWithCustomizedEquals();
                AddToCollection_CompareByReference(collection, a);
                AddToCollection_CompareByReference(collection, b);

                Assert.That(collection.Count, Is.EqualTo(2));
                Assert.That(collection.Contains(a), Is.True);
                Assert.That(collection.Contains(b), Is.True);
            }

            private static void AddToCollection(ObservableCollection<ITestModel> collection, ITestModel m)
            {
                if (!collection.Contains(m))
                {
                    collection.Add(m);
                }
            }

            private static void AddToCollection_CompareByReference(ObservableCollection<ITestModel> collection, ITestModel m)
            {
                if (!Contains_CompareByReference(collection, m))
                {
                    collection.Add(m);
                }
            }

            private static bool Contains_CompareByReference<T>(IEnumerable<T> collection, T item)
            {
                foreach (var document in collection)
                {
                    if (ReferenceEquals(document, item))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [TestFixture]
        public class HandlePrivatePropertyDataRegistrations
        {
            public class Person : ModelBase
            {
                public string FirstName
                {
                    get { return GetValue<string>(FirstNameProperty); }
                    set { SetValue(FirstNameProperty, value); }
                }

                private static readonly IPropertyData FirstNameProperty = RegisterProperty(nameof(FirstName), typeof(string), null);
            }

            public class JohnDoe : Person
            {
                public string LastName
                {
                    get { return GetValue<string>(LastNameProperty); }
                    set { SetValue(LastNameProperty, value); }
                }

                private static readonly IPropertyData LastNameProperty = RegisterProperty(nameof(LastName), typeof(string), null);
            }

            [Test]
            public void CorrectlyRegistersProperties()
            {
                var catelTypeInfo = new CatelTypeInfo(typeof(JohnDoe));

                Assert.That(catelTypeInfo.GetPropertyData("FirstName"), Is.Not.Null);
                Assert.That(catelTypeInfo.GetPropertyData("LastName"), Is.Not.Null);
            }
        }

        [TestFixture]
        public class The_Constructor_Method
        {
            [Test]
            public void Prevents_Running_Change_Notifications_Async()
            {
                var model = new ModelWithChangeHandler();

                Assert.That(model.B, Is.EqualTo("default value"));
                Assert.That(model.ChangeCount, Is.EqualTo(0));

                model.B = "new value";

                Assert.That(model.B, Is.EqualTo("new value"));
                Assert.That(model.ChangeCount, Is.EqualTo(1));
            }

            [Test]
            public void Correctly_Knows_That_The_Object_Is_Working()
            {
                var model = new ModelWithChangeHandler();

                Assert.That(model.B, Is.EqualTo("default value"));
                Assert.That(model.ChangeCount, Is.EqualTo(0));

                var parent = new ParentObject(model);

                Assert.That(model.B, Is.EqualTo("new value"));
                Assert.That(model.ChangeCount, Is.EqualTo(1));
            }

            private class ParentObject
            {
                public ParentObject(ModelWithChangeHandler injected)
                {
                    injected.B = "new value";
                }
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Tests.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;

    using NUnit.Framework;

#if !UWP
    using Catel.MVVM;
    using System.Windows.Data;
#endif

    public partial class ModelBaseFacts
    {
        [TestFixture]
        public class TheUnregisterPropertyMethod
        {
            [TestCase]
            public void CanUnregisterRuntimeProperty()
            {
                var model = new ModelWithRuntimeProperties();
                var property = ModelBase.RegisterProperty("RuntimePropertyBeingUnregistered", typeof (string));
                model.InitializePropertyAfterConstruction(property);

                Assert.IsTrue(PropertyDataManager.Default.IsPropertyRegistered(typeof(ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered"));

                ModelBase.UnregisterProperty(typeof (ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered");

                Assert.IsFalse(PropertyDataManager.Default.IsPropertyRegistered(typeof(ModelWithRuntimeProperties), "RuntimePropertyBeingUnregistered"));
            }
        }

        [TestFixture]
        public class TheMemoryLeakChecks
        {
            [TestCase]
            public void DoesNotLeakForModelBaseWithPropertiesThatSupportPropertyChanged()
            {
                var model = new PersonTestModel();
                var weakReference = new WeakReference(model);

                Assert.IsTrue(weakReference.IsAlive);

                model = null;
                GC.Collect();

                Assert.IsFalse(weakReference.IsAlive);
            }
        }

#if NET || NETCORE
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

            public class LatePropertyRegistrationModel : ValidatableModelBase
            {
                protected override void InitializeCustomProperties()
                {
                    var propertyData = RegisterProperty("CanSave", typeof(bool));

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
                Assert.IsFalse(validation.HasErrors);

                var propertyData = PropertyDataManager.Default.GetPropertyData(typeof(LatePropertyRegistrationModel), "CanSave");
                Assert.IsTrue(propertyData.IsCalculatedProperty);
            }
        }
#endif

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

                Assert.AreEqual(2, collection.Count);
                Assert.IsTrue(collection.Contains(a));
                Assert.IsTrue(collection.Contains(b));
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

                Assert.AreEqual(2, collection.Count);
                Assert.IsTrue(collection.Contains(a));
                Assert.IsTrue(collection.Contains(b));
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
    }
}

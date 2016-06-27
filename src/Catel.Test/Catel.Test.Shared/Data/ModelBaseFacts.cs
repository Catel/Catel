// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Catel.Data;

    using NUnit.Framework;

#if !NETFX_CORE
    using Catel.MVVM;
    using System.Windows.Data;
#endif

    public partial class ModelBaseFacts
    {
        public class CollectionModel : ModelBase
        {
            public CollectionModel(bool initializeValues)
            {
                Items = new ObservableCollection<ObservableCollection<CollectionModel>>();

                if (initializeValues)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        var innerCollection = new ObservableCollection<CollectionModel>();
                        Items.Add(innerCollection);

                        for (int j = 0; j < 3; j++)
                        {
                            innerCollection.Add(new CollectionModel(false) { Name = string.Format("Subitem {0}", j + 1) });
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public ObservableCollection<ObservableCollection<CollectionModel>> Items
            {
                get { return GetValue<ObservableCollection<ObservableCollection<CollectionModel>>>(ItemsProperty); }
                set { SetValue(ItemsProperty, value); }
            }

            /// <summary>
            /// Register the Items property so it is known in the class.
            /// </summary>
            public static readonly PropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<ObservableCollection<CollectionModel>>));

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public string Name
            {
                get { return GetValue<string>(NameProperty); }
                set { SetValue(NameProperty, value); }
            }

            /// <summary>
            /// Register the Name property so it is known in the class.
            /// </summary>
            public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
        }

        public class LeanAndMeanModel : ModelBase
        {
            public bool LeanAndMeanModelWrapper
            {
                get { return LeanAndMeanModel; }
                set { LeanAndMeanModel = value; }
            }

            [Required]
            public string FirstName
            {
                get { return GetValue<string>(FirstNameProperty); }
                set { SetValue(FirstNameProperty, value); }
            }

            public static readonly PropertyData FirstNameProperty = RegisterProperty("FirstName", typeof(string), string.Empty);
        }

        public class ModelWithRuntimeProperties : ModelBase
        {
            
        }

        public class TestModel : ModelBase
        {
            public TestModel()
            {
                CollectionModel = new ObservableCollection<LeanAndMeanModel>();
                CollectionModel.Add(new LeanAndMeanModel());
            }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public LeanAndMeanModel ModelWithPropertyChanged
            {
                get { return GetValue<LeanAndMeanModel>(ModelWithPropertyChangedProperty); }
                set { SetValue(ModelWithPropertyChangedProperty, value); }
            }

            /// <summary>
            /// Register the ModelWithPropertyChanged property so it is known in the class.
            /// </summary>
            public static readonly PropertyData ModelWithPropertyChangedProperty = RegisterProperty("ModelWithPropertyChanged", typeof(LeanAndMeanModel), () => new LeanAndMeanModel());

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            public ObservableCollection<LeanAndMeanModel> CollectionModel
            {
                get { return GetValue<ObservableCollection<LeanAndMeanModel>>(CollectionModelProperty); }
                set { SetValue(CollectionModelProperty, value); }
            }

            /// <summary>
            /// Register the CollectionModel property so it is known in the class.
            /// </summary>
            public static readonly PropertyData CollectionModelProperty = RegisterProperty("CollectionModel", typeof(ObservableCollection<LeanAndMeanModel>), null);
        }

        [TestFixture]
        public class TheLeanAndMeanModelProperty
        {
            [TestCase]
            public void SuspendsValidation()
            {
                var model = new LeanAndMeanModel();
                var validation = model as IModelValidation;

                model.LeanAndMeanModelWrapper = true;

                Assert.IsFalse(validation.HasErrors);

                model.FirstName = null;

                Assert.IsFalse(validation.HasErrors);
            }

            [TestCase]
            public void SuspendsChangeNotifications()
            {
                var counter = 0;

                var model = new LeanAndMeanModel();
                model.PropertyChanged += (sender, e) => counter++;
                model.LeanAndMeanModelWrapper = true;

                model.FirstName = "Geert";

                Assert.AreEqual(0, counter);
            }
        }

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
                var model = new TestModel();
                var weakReference = new WeakReference(model);

                Assert.IsTrue(weakReference.IsAlive);

                model = null;
                GC.Collect();

                Assert.IsFalse(weakReference.IsAlive);
            }
        }

#if NET
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
                    if (obj == null)
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

            public class LatePropertyRegistrationModel : ModelBase
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
                var validation = model as IModelValidation;

                validation.Validate(true);
                Assert.IsFalse(validation.HasErrors);

                var propertyData = PropertyDataManager.Default.GetPropertyData(typeof(LatePropertyRegistrationModel), "CanSave");
                Assert.IsTrue(propertyData.IsCalculatedProperty);
            }
        }
#endif

        [TestFixture]
        public class ThePropertiesWithEventSubscriptionsChecks
        {
            public class ModelWithObservableCollection : ModelBase
            {
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public ObservableCollection<int> Collection
                {
                    get { return GetValue<ObservableCollection<int>>(CollectionProperty); }
                    set { SetValue(CollectionProperty, value); }
                }

                /// <summary>
                /// Register the Collection property so it is known in the class.
                /// </summary>
                public static readonly PropertyData CollectionProperty = RegisterProperty("Collection", typeof(ObservableCollection<int>), () => new ObservableCollection<int>());

                public bool HasCollectionChanged { get; private set; }

                public bool HasPropertyChanged { get; private set; }

                protected override void OnPropertyObjectCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
                {
                    HasCollectionChanged = true;
                }

                protected override void OnPropertyObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    HasPropertyChanged = true;
                }
            }

#if NET
            public class ModelWithCollectionViewSource : ViewModelBase
            {
                /// <summary>
                /// Gets or sets the property value.
                /// </summary>
                public CollectionView Collection
                {
                    get { return GetValue<CollectionView>(CollectionProperty); }
                    set { SetValue(CollectionProperty, value); }
                }

                /// <summary>
                /// Register the Collection property so it is known in the class.
                /// </summary>
                public static readonly PropertyData CollectionProperty = RegisterProperty("Collection", typeof(CollectionView), () => new CollectionView(new List<int>() { 1, 2, 3 }));
            }
#endif

            [TestCase]
            public void AllowsRegistrationOfObservableCollection()
            {
                var model = new ModelWithObservableCollection();
                model.Collection = new ObservableCollection<int>(new List<int>() { 1, 2, 3 });

                model.Collection.Add(4);

                Assert.IsTrue(model.HasCollectionChanged);
            }

#if NET
            [TestCase]
            public void AllowsRegistrationOfCollectionViewSource()
            {
                var model = new ModelWithCollectionViewSource();
                model.Collection = new CollectionView(new List<int>() { 1, 2, 3 });
            }
#endif

            [TestCase]
            public void RegistersChangeNotificationsOfDefaultValues()
            {
                var model = new ModelWithObservableCollection();

                model.Collection.Add(4);

                Assert.IsTrue(model.HasCollectionChanged);
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
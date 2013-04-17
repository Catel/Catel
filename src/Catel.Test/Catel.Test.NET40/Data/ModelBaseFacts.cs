// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModelBaseFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
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

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Catel.MVVM;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [AllowNonSerializableMembers]
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

        [TestClass]
        public class TheLeanAndMeanModelProperty
        {
            [TestMethod]
            public void SuspendsValidation()
            {
                var model = new LeanAndMeanModel();

                model.LeanAndMeanModelWrapper = true;

                Assert.IsFalse(model.HasErrors);

                model.FirstName = null;

                Assert.IsFalse(model.HasErrors);
            }

            [TestMethod]
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

        [TestClass]
        public class TheMemoryLeakChecks
        {
            [TestMethod]
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
        [TestClass]
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

            [TestMethod]
            public void CorrectlyHandlesLateRegistrationOfCalculatedProperties()
            {
                var model = new LatePropertyRegistrationModel();

                model.Validate(true);
                Assert.IsFalse(model.HasErrors);

                var propertyData = PropertyDataManager.Default.GetPropertyData(typeof (LatePropertyRegistrationModel), "CanSave");
                Assert.IsTrue(propertyData.IsCalculatedProperty);
            }
        }
#endif

        [TestClass]
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

            [TestMethod]
            public void AllowsRegistrationOfObservableCollection()
            {
                var model = new ModelWithObservableCollection();
                model.Collection = new ObservableCollection<int>(new List<int>() { 1, 2, 3 });

                model.Collection.Add(4);

                Assert.IsTrue(model.HasCollectionChanged);
            }

#if NET
            [TestMethod]
            public void AllowsRegistrationOfCollectionViewSource()
            {
                var model = new ModelWithCollectionViewSource();
                model.Collection = new CollectionView(new List<int>() { 1, 2, 3 });
            }
#endif

            [TestMethod]
            public void RegistersChangeNotificationsOfDefaultValues()
            {
                var model = new ModelWithObservableCollection();

                model.Collection.Add(4);

                Assert.IsTrue(model.HasCollectionChanged);
            }
        }

        //[TestClass]
        //public class TheSupportCollectionNotifyChangedEventsProperty
        //{
        //    [TestMethod]
        //    public void GetsNoChangeNotificationsForChildCollections()
        //    {
        //        var model = new CollectionModel(true);

        //        var propertyChanged = false;
        //        model.PropertyChanged += (sender, e) => propertyChanged = true;

        //        model.Items[0][0].Name = "test";

        //        Assert.IsTrue(propertyChanged);
        //    }
        //}
    }
}
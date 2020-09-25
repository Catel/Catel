// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Media;
    using Catel.Collections;
    using Catel.Data;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Runtime.Serialization.Json;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TestModels;
    using JsonSerializer = Catel.Runtime.Serialization.Json.JsonSerializer;

    public partial class GenericSerializationFacts
    {
        [TestFixture]
        public class CatelModelAdvancedSerializationFacts
        {
            [Serializable]
            public abstract class AbstractBase : ComparableModelBase
            {
                public AbstractBase()
                {

                }

                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                public static readonly IPropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
            }

            [Serializable]
            public class DerivedClass : AbstractBase
            {
                public DerivedClass()
                {

                }
            }

            [Serializable]
            public class ContainerClass : ComparableModelBase
            {
                public ContainerClass()
                {
                    Items = new ObservableCollection<AbstractBase>();
                }

                public ObservableCollection<AbstractBase> Items
                {
                    get { return GetValue<ObservableCollection<AbstractBase>>(ItemsProperty); }
                    set { SetValue(ItemsProperty, value); }
                }

                public static readonly IPropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<AbstractBase>), null);
            }

            [TestCase]
            public void CorrectlySerializesCustomizedModels()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                testModel.SetValue(TestModel.ExcludedProtectedCatelPropertyProperty, "excluded");

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, serializer, config);

                    Assert.AreEqual(null, clonedModel._excludedField, description);
                    Assert.AreEqual("included", clonedModel._includedField, description);

                    Assert.AreEqual(null, clonedModel.ExcludedRegularProperty, description);
                    Assert.AreEqual("included", clonedModel.IncludedRegularProperty, description);

                    Assert.AreEqual(null, clonedModel.ExcludedCatelProperty, description);
                    Assert.AreEqual("included", clonedModel.IncludedCatelProperty, description);

                    Assert.AreEqual(null, ((IModelEditor)clonedModel).GetValue<object>(TestModel.ExcludedProtectedCatelPropertyProperty.Name), description);
                });
            }

            [TestCase]
            public void CorrectlyIncludesNonSerializablePropertiesIncludedWithAttributes()
            {
                var model = new CTL550Model();
                model.Color = Colors.Red;

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer, config);

                    Assert.AreEqual(Colors.Red, clonedModel.Color, description);
                });
            }

            [TestCase]
            public void CorrectlyHandlesSerializationOfCollectionsWithAbstractClasses()
            {
                var collection = new CatelModelAdvancedSerializationFacts.ContainerClass();
                collection.Items.Add(new CatelModelAdvancedSerializationFacts.DerivedClass { Name = "item 1" });

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(collection, serializer, config);

                    Assert.AreEqual(collection, clonedGraph, description);
                }, false);
            }

            [TestCase]
            public void CorrectlyHandlesNullValues()
            {
                var testModel = new TestModel();

                testModel.IncludedCatelProperty = null;

                var editableObject = testModel as IEditableObject;
                editableObject.BeginEdit();

                testModel.IncludedCatelProperty = "included";

                editableObject.CancelEdit();

                Assert.IsNull(testModel.IncludedCatelProperty);
            }

            [TestCase]
            public void CorrectlyHandlesSameInstancesInGraph()
            {
                var graph = SerializationTestHelper.CreateComplexCircularTestModelGraph();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer, config);

                    Assert.IsNotNull(clonedGraph, description);
                    Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel), description);
                }, false);
            }

            [TestCase]
            public void CorrectlyHandlesSameInstancesInGraphUsingCollections()
            {
                var testModel = new TestModelWithNestedListMembers();

                var level2_1 = new TestModelWithNestedListMembers_Level2
                {
                    Name = "John Doe"
                };

                var level1_1 = new TestModelWithNestedListMembers_Level1
                {
                    Name = "A",
                };

                level1_1.Children.Add(level2_1);

                var level1_2 = new TestModelWithNestedListMembers_Level1
                {
                    Name = "B",
                };

                level1_2.Children.Add(level2_1);

                testModel.Children.Add(level1_1);
                testModel.Children.Add(level1_2);

                TestSerializationOnXmlSerializer((serializer, config, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(testModel, serializer, config);

                    Assert.IsNotNull(clonedGraph, description);
                    Assert.IsTrue(ReferenceEquals(clonedGraph.Children[0].Children[0], clonedGraph.Children[1].Children[0]), description);
                }, false);
            }

            [TestCase] // CTL-550
            public void CorrectlyHandlesSameInstancesOfNonCatelObjectsInGraph()
            {
                var graph = new ReusedCollectionsModel();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer, config);

                    Assert.IsNotNull(clonedGraph.Collection1, description);
                    Assert.IsNotNull(clonedGraph.Collection2, description);

                    Assert.AreEqual(5, clonedGraph.Collection1.Count, description);
                    Assert.AreEqual(5, clonedGraph.Collection2.Count, description);

                    Assert.IsTrue(ReferenceEquals(clonedGraph.Collection1, clonedGraph.Collection2), description);
                }, false);
            }
        }
    }
}

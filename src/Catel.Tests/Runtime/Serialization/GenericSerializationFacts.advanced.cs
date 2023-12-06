namespace Catel.Tests.Runtime.Serialization
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Media;
    using Catel.Data;
    using NUnit.Framework;
    using TestModels;

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

                public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);
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

                public static readonly IPropertyData ItemsProperty = RegisterProperty("Items", () => new ObservableCollection<AbstractBase>());
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

                    Assert.That(clonedModel._excludedField, Is.EqualTo(null), description);
                    Assert.That(clonedModel._includedField, Is.EqualTo("included"), description);

                    Assert.That(clonedModel.ExcludedRegularProperty, Is.EqualTo(null), description);
                    Assert.That(clonedModel.IncludedRegularProperty, Is.EqualTo("included"), description);

                    Assert.That(clonedModel.ExcludedCatelProperty, Is.EqualTo(string.Empty), description);
                    Assert.That(clonedModel.IncludedCatelProperty, Is.EqualTo("included"), description);

                    Assert.That(((IModelEditor)clonedModel).GetValue<object>(TestModel.ExcludedProtectedCatelPropertyProperty.Name), Is.EqualTo(string.Empty), description);
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

                    Assert.That(clonedModel.Color, Is.EqualTo(Colors.Red), description);
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

                    Assert.That(clonedGraph, Is.EqualTo(collection), description);
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

                Assert.That(testModel.IncludedCatelProperty, Is.Null);
            }

            [TestCase]
            public void CorrectlyHandlesSameInstancesInGraph()
            {
                var graph = SerializationTestHelper.CreateComplexCircularTestModelGraph();

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer, config);

                    Assert.IsNotNull(clonedGraph, description);
                    Assert.That(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel), Is.True, description);
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
                    Assert.That(ReferenceEquals(clonedGraph.Children[0].Children[0], clonedGraph.Children[1].Children[0]), Is.True, description);
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

                    Assert.That(clonedGraph.Collection1.Count, Is.EqualTo(5), description);
                    Assert.That(clonedGraph.Collection2.Count, Is.EqualTo(5), description);

                    Assert.That(ReferenceEquals(clonedGraph.Collection1, clonedGraph.Collection2), Is.True, description);
                }, false);
            }
        }
    }
}

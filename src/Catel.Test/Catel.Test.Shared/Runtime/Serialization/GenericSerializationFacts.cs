// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Data;
    using NUnit.Framework;
    using TestModels;

    public class GenericSerializationFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
            [TestCase]
            public void SerializationLevel1()
            {
                var originalObject = ModelBaseTestHelper.CreateIniEntryObject();

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }

            [TestCase]
            public void SerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }

            [TestCase]
            public void SerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }

            [TestCase]
            public void SerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }

            [TestCase]
            public void SerializationWithCustomTypes()
            {
                var originalObject = new ObjectWithCustomType();
                originalObject.FirstName = "Test";
                originalObject.Gender = Gender.Female;

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }

#if NET
            [TestCase]
            public void SerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject);
                });
            }
#endif

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexHierarchy();

                TestSerializationOnAllSerializers(serializer =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer);

                    Assert.IsTrue(complexHierarchy == deserializedObject);
                });
            }
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
            [Serializable]
            public abstract class AbstractBase : ModelBase
            {
                public AbstractBase()
                {

                }

#if NET
                protected AbstractBase(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }
#endif

                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);
            }

            [Serializable]
            public class DerivedClass : AbstractBase
            {
                public DerivedClass()
                {

                }

#if NET
                protected DerivedClass(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }
#endif
            }

            [Serializable]
            public class ContainerClass : ModelBase
            {
                public ContainerClass()
                {
                    Items = new ObservableCollection<AbstractBase>();
                }

#if NET
                protected ContainerClass(SerializationInfo info, StreamingContext context)
                    : base(info, context) { }
#endif

                public ObservableCollection<AbstractBase> Items
                {
                    get { return GetValue<ObservableCollection<AbstractBase>>(ItemsProperty); }
                    set { SetValue(ItemsProperty, value); }
                }

                public static readonly PropertyData ItemsProperty = RegisterProperty("Items", typeof(ObservableCollection<AbstractBase>), null);
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

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, serializer);

                    Assert.AreEqual(null, clonedModel._excludedField);
                    Assert.AreEqual("included", clonedModel._includedField);

                    Assert.AreEqual(null, clonedModel.ExcludedRegularProperty);
                    Assert.AreEqual("included", clonedModel.IncludedRegularProperty);

                    Assert.AreEqual(null, clonedModel.ExcludedCatelProperty);
                    Assert.AreEqual("included", clonedModel.IncludedCatelProperty);

                    Assert.AreEqual(null, clonedModel.GetValue(TestModel.ExcludedProtectedCatelPropertyProperty.Name));
                });
            }

            [TestCase]
            public void CorrectlyIncludesNonSerializablePropertiesIncludedWithAttributes()
            {
                var model = new CTL550Model();
                model.Color = Colors.Red;

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer);

                    Assert.AreEqual(Colors.Red, clonedModel.Color);
                });
            }

            [TestCase]
            public void CorrectlyHandlesSerializationOfCollectionsWithAbstractClasses()
            {
                var collection = new ContainerClass();
                collection.Items.Add(new DerivedClass { Name = "item 1" });

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(collection, serializer);

                    Assert.AreEqual(collection, clonedGraph);
                });
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

                TestSerializationOnAllSerializers(serializer =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer);

                    Assert.IsNotNull(clonedGraph);
                    Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel));
                });
            }
        }

        [TestFixture]
        public class TheWarmupMethod
        {
            [TestCase]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new Type[] { typeof(CircularTestModel), typeof(TestModel) };

                TestSerializationOnAllSerializers(serializer =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(typesToWarmup),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestCase]
            public void WarmsUpAllTypes()
            {
                TestSerializationOnAllSerializers(serializer =>
                {
                    TimeMeasureHelper.MeasureAction(5, string.Format("{0} serializer warmup", serializer.GetType().Name),
                        () => serializer.Warmup(),
                        () =>
                        {
                            TypeCache.InitializeTypes();

                            ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                            ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                        });
                });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }

        private static void TestSerializationOnAllSerializers(Action<IModelBaseSerializer> action)
        {
            var serializers = new List<IModelBaseSerializer>();

            serializers.Add(SerializationFactory.GetXmlSerializer());
            serializers.Add(SerializationFactory.GetBinarySerializer());

            foreach (var serializer in serializers)
            {
                action(serializer);
            }
        }
    }
}
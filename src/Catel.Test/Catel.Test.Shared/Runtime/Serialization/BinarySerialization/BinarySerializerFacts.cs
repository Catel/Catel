// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinarySerializerFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.Linq;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;
    using Catel.Test.Data;

    using NUnit.Framework;

    public class BinarySerializerFacts
    {
        [TestFixture]
        public class BasicSerializationFacts
        {
            [TestCase]
            public void BinarySerializationLevel1()
            {
                var originalObject = ModelBaseTestHelper.CreateIniEntryObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestCase]
            public void BinarySerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestCase]
            public void BinarySerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestCase]
            public void BinarySerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }

            [TestCase]
            public void BinarySerializationWithPrivateMembers()
            {
                // Create new object
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                // Test
                var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(originalObject, clonedObject);
            }
        }

        [TestFixture]
        public class AdvancedSerializationFacts
        {
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

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetBinarySerializer());

                Assert.AreEqual(null, clonedModel._excludedField);
                Assert.AreEqual("included", clonedModel._includedField);

                Assert.AreEqual(null, clonedModel.ExcludedRegularProperty);
                Assert.AreEqual("included", clonedModel.IncludedRegularProperty);

                Assert.AreEqual(null, clonedModel.ExcludedCatelProperty);
                Assert.AreEqual("included", clonedModel.IncludedCatelProperty);
            }

            [TestCase]
            public void CorrectlyHandlesSameInstancesInGraph()
            {
                var graph = SerializationTestHelper.CreateComplexCircularTestModelGraph();

                var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, SerializationFactory.GetBinarySerializer());

                Assert.IsNotNull(clonedGraph);
                Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel));
            }
        }

        [TestFixture]
        public class TheWarmupMethod
        {
            [TestCase]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new Type[] { typeof(CircularTestModel), typeof(TestModel) };

                var serializer = SerializationFactory.GetBinarySerializer();

                TimeMeasureHelper.MeasureAction(5, "Binary serializer warmup",
                    () => serializer.Warmup(typesToWarmup),
                    () =>
                    {
                        TypeCache.InitializeTypes();

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestCase]
            public void WarmsUpAllTypes()
            {
                var serializer = SerializationFactory.GetBinarySerializer();

                TimeMeasureHelper.MeasureAction(5, "Binary serializer warmup",
                    () => serializer.Warmup(),
                    () =>
                    {
                        TypeCache.InitializeTypes();

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }
    }
}

#endif
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Runtime.Serialization;

#if NETFX_CORE
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
    using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

    public class XmlSerializerFacts
    {
        [TestClass]
        public class AdvancedSerializationFacts
        {
            [TestMethod]
            public void CorrectlySerializesCustomizedModels()
            {
                var testModel = new TestModel();

                testModel._excludedField = "excluded";
                testModel._includedField = "included";

                testModel.ExcludedRegularProperty = "excluded";
                testModel.IncludedRegularProperty = "included";

                testModel.ExcludedCatelProperty = "excluded";
                testModel.IncludedCatelProperty = "included";

                var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetXmlSerializer());

                Assert.AreEqual(null, clonedModel._excludedField);
                Assert.AreEqual("included", clonedModel._includedField);

                Assert.AreEqual(null, clonedModel.ExcludedRegularProperty);
                Assert.AreEqual("included", clonedModel.IncludedRegularProperty);

                Assert.AreEqual(null, clonedModel.ExcludedCatelProperty);
                Assert.AreEqual("included", clonedModel.IncludedCatelProperty);
            }

            [TestMethod]
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

            // Note: Disabled because it has too much impact on other tests
            //[TestMethod]
            //public void CorrectlyHandlesCustomizedValuesDuringSerialization()
            //{
            //    var testModel = new TestModel();
            //    testModel.IncludedCatelProperty = "BeforeSerializingMember";

            //    var xmlSerializer = ServiceLocator.Default.ResolveType<IXmlSerializer>();
            //    xmlSerializer.SerializingMember += (sender, e) =>
            //    {
            //        if (ReferenceEquals(e.SerializationContext.Model, testModel))
            //        {
            //            if (e.MemberValue.Name == "IncludedCatelProperty")
            //            {
            //                Assert.AreEqual("BeforeSerializingMember", e.MemberValue.Value);

            //                e.MemberValue.Value = "AfterSerializingMember";
            //            }
            //        }
            //    };

            //    xmlSerializer.SerializedMember += (sender, e) =>
            //    {
            //        if (ReferenceEquals(e.SerializationContext.Model, testModel))
            //        {
            //            if (e.MemberValue.Name == "IncludedCatelProperty")
            //            {
            //                Assert.AreEqual("AfterSerializingMember", e.MemberValue.Value);
            //            }
            //        }
            //    };

            //    xmlSerializer.DeserializedMember += (sender, e) =>
            //    {
            //        if (e.MemberValue.Name == "IncludedCatelProperty")
            //        {
            //            Assert.AreEqual("AfterSerializingMember", e.MemberValue.Value);

            //            e.MemberValue.Value = "AfterDeserializedMember";
            //        }
            //    };

            //    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, SerializationFactory.GetXmlSerializer());

            //    Assert.AreEqual("AfterDeserializedMember", clonedModel.IncludedCatelProperty);
            //}

            [TestMethod]
            public void CorrectlyHandlesSameInstancesInGraph()
            {
                var graph = SerializationTestHelper.CreateComplexCircularTestModelGraph();

                var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, SerializationFactory.GetXmlSerializer());

                Assert.IsNotNull(clonedGraph);
                Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel));
            }
        }

        [TestClass]
        public class TheWarmupMethod
        {
            [TestMethod]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new Type[] { typeof(CircularTestModel), typeof(TestModel) };

                var serializer = SerializationFactory.GetXmlSerializer();

                TimeMeasureHelper.MeasureAction(5, "Xml serializer warmup",
                    () => serializer.Warmup(typesToWarmup),
                    () =>
                    {
                        TypeCache.InitializeTypes(false);

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }

            [TestMethod]
            public void WarmsUpAllTypes()
            {
                var serializer = SerializationFactory.GetXmlSerializer();

                TimeMeasureHelper.MeasureAction(5, "Xml serializer warmup",
                    () => serializer.Warmup(),
                    () =>
                    {
                        TypeCache.InitializeTypes(false);

                        ConsoleHelper.Write("TypeCache contains {0} items", TypeCache.GetTypes().Count());
                        ConsoleHelper.Write("TypeCache contains {0} ModelBase items", TypeCache.GetTypes(x => typeof(ModelBase).IsAssignableFromEx(x)).Count());
                    });

                // TODO: No way to see if this is cached (otherwise we have to write this feature on DataContractSerializerFactory)
                // This unit test is written to easily test this functionality though.
            }
        }
    }
}
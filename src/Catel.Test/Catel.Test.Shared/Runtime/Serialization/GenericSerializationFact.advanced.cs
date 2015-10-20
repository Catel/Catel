// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericSerializationFacts.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Catel.Test.Runtime.Serialization
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
    using Catel.Runtime.Serialization.Binary;
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
            public abstract class AbstractBase : ModelBase
            {
                public AbstractBase()
                {

                }

#if NET
                protected AbstractBase(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
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
                    : base(info, context)
                {
                }
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
                    : base(info, context)
                {
                }
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

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(testModel, serializer);

                    Assert.AreEqual(null, clonedModel._excludedField, description);
                    Assert.AreEqual("included", clonedModel._includedField, description);

                    Assert.AreEqual(null, clonedModel.ExcludedRegularProperty, description);
                    Assert.AreEqual("included", clonedModel.IncludedRegularProperty, description);

                    Assert.AreEqual(null, clonedModel.ExcludedCatelProperty, description);
                    Assert.AreEqual("included", clonedModel.IncludedCatelProperty, description);

                    Assert.AreEqual(null, clonedModel.GetValue(TestModel.ExcludedProtectedCatelPropertyProperty.Name), description);
                });
            }

            [TestCase]
            public void CorrectlyIncludesNonSerializablePropertiesIncludedWithAttributes()
            {
                var model = new CTL550Model();
                model.Color = Colors.Red;

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedModel = SerializationTestHelper.SerializeAndDeserialize(model, serializer);

                    Assert.AreEqual(Colors.Red, clonedModel.Color, description);
                });
            }

            [TestCase]
            public void CorrectlyHandlesSerializationOfCollectionsWithAbstractClasses()
            {
                var collection = new CatelModelAdvancedSerializationFacts.ContainerClass();
                collection.Items.Add(new CatelModelAdvancedSerializationFacts.DerivedClass { Name = "item 1" });

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(collection, serializer);

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

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer);

                    Assert.IsNotNull(clonedGraph, description);
                    Assert.IsTrue(ReferenceEquals(clonedGraph, clonedGraph.CircularModel.CircularModel), description);
                }, false);
            }

            [TestCase] // CTL-550
            public void CorrectlyHandlesSameInstancesOfNonCatelObjectsInGraph()
            {
                var graph = new ReusedCollectionsModel();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedGraph = SerializationTestHelper.SerializeAndDeserialize(graph, serializer);

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
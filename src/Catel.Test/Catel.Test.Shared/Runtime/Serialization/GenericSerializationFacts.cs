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
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Media;
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

    public class GenericSerializationFacts
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        [TestFixture]
        public class CatelModelBasicSerializationFacts
        {
            [TestCase]
            public void SerializationLevel1()
            {
                var originalObject = ModelBaseTestHelper.CreateIniEntryObject();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationLevel2()
            {
                var originalObject = ModelBaseTestHelper.CreateIniFileObject();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationLevel3()
            {
                var originalObject = ModelBaseTestHelper.CreateComputerSettingsObject();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

            [TestCase]
            public void SerializationComplexGraphWithInheritance()
            {
                var originalObject = ModelBaseTestHelper.CreateHierarchicalGraphWithInheritance();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                }, false);
            }

            [TestCase]
            public void SerializationWithCustomTypes()
            {
                var originalObject = new ObjectWithCustomType();
                originalObject.FirstName = "Test";
                originalObject.Gender = Gender.Female;

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }

#if NET
            [TestCase]
            public void SerializationWithPrivateMembers()
            {
                var originalObject = new ObjectWithPrivateMembers("My private member");
                originalObject.PublicMember = "My public member";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject, clonedObject, description);
                });
            }
#endif

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexHierarchy();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer);

                    Assert.IsTrue(complexHierarchy == deserializedObject, description);
                });
            }
        }

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
                { }
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
                { }
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
                { }
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
                var collection = new ContainerClass();
                collection.Items.Add(new DerivedClass { Name = "item 1" });

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

        [TestFixture]
        public class NonCatelModelBasicSerializationFacts
        {
            [TestCase]
            public void SerializeSimpleModels()
            {
                var originalObject = new NonCatelTestModel();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void SerializeWithIFieldSerializable()
            {
                var originalObject = new NonCatelTestModelWithIFieldSerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.IsTrue(originalObject.GetViaInterface, description);
                    Assert.IsTrue(clonedObject.SetViaInterface, description);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void SerializeWithIPropertySerializable()
            {
                var originalObject = new NonCatelTestModelWithIPropertySerializable();
                originalObject.FirstName = "Test";
                originalObject.LastName = "Subject";

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.IsTrue(originalObject.GetViaInterface, description);
                    Assert.IsTrue(clonedObject.SetViaInterface, description);

                    Assert.AreEqual(originalObject.FirstName, clonedObject.FirstName, description);
                    Assert.AreEqual(originalObject.LastName, clonedObject.LastName, description);
                });
            }

            [TestCase]
            public void CanSerializeAndDeserializeComplexHierarchies()
            {
                var complexHierarchy = ComplexSerializationHierarchy.CreateComplexNonCatelHierarchy();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(complexHierarchy, serializer);

                    Assert.AreEqual(complexHierarchy.LastName, deserializedObject.LastName, description);
                    Assert.AreEqual(complexHierarchy.Persons.Count, deserializedObject.Persons.Count, description);

                    for (int i = 0; i < deserializedObject.Persons.Count; i++)
                    {
                        var expectedPerson = complexHierarchy.Persons[i];
                        var actualPerson = deserializedObject.Persons[i];

                        Assert.AreEqual(expectedPerson.Gender, actualPerson.Gender, description);
                        Assert.AreEqual(expectedPerson.FirstName, actualPerson.FirstName, description);
                        Assert.AreEqual(expectedPerson.LastName, actualPerson.LastName, description);
                    }
                });
            }
        }

        [TestFixture]
        public class NonCatelModelAdvancedSerializationFacts
        {
        }

        [TestFixture]
        public class CollectionSerializationFacts
        {
            [Serializable]
            [Table("Countries")]
            [KnownType(typeof(Country))]
            public class Country : ModelBase
            {
                public Country()
                {
                }

#if NET
                public Country(SerializationInfo info, StreamingContext context)
                    : base(info, context)
                {
                }
#endif

                [Key]
                public Guid Id
                {
                    get { return this.GetValue<Guid>(IdProperty); }
                    set { this.SetValue(IdProperty, value); }
                }

                public static readonly PropertyData IdProperty = RegisterProperty<Country, Guid>(o => o.Id, Guid.NewGuid);

                [StringLength(256)]
                public string IsoCode
                {
                    get { return this.GetValue<string>(IsoCodeProperty); }
                    set { this.SetValue(IsoCodeProperty, value); }
                }

                public static readonly PropertyData IsoCodeProperty = RegisterProperty<Country, string>(o => o.IsoCode);

                [StringLength(400)]
                public string Description
                {
                    get { return this.GetValue<string>(DescriptionProperty); }
                    set { this.SetValue(DescriptionProperty, value); }
                }

                public static readonly PropertyData DescriptionProperty = RegisterProperty<Country, string>(o => o.Description);

                public DateTime CreateDate
                {
                    get { return this.GetValue<DateTime>(CreateDateProperty); }
                    set { this.SetValue(CreateDateProperty, value); }
                }

                public static readonly PropertyData CreateDateProperty = RegisterProperty<Country, DateTime>(o => o.CreateDate);

                public Guid CreateUserId
                {
                    get { return this.GetValue<Guid>(CreateUserIdProperty); }
                    set { this.SetValue(CreateUserIdProperty, value); }
                }

                public static readonly PropertyData CreateUserIdProperty = RegisterProperty<Country, Guid>(o => o.CreateUserId);

                public DateTime? DeleteDate
                {
                    get { return this.GetValue<DateTime?>(DeleteDateProperty); }
                    set { this.SetValue(DeleteDateProperty, value); }
                }

                public static readonly PropertyData DeleteDateProperty = RegisterProperty<Country, DateTime?>(o => o.DeleteDate);

                public Guid? DeleteUserId
                {
                    get { return this.GetValue<Guid?>(DeleteUserIdProperty); }
                    set { this.SetValue(DeleteUserIdProperty, value); }
                }

                public static readonly PropertyData DeleteUserIdProperty = RegisterProperty<Country, Guid?>(o => o.DeleteUserId);

                public bool IsDeleted
                {
                    get { return this.GetValue<bool>(IsDeletedProperty); }
                    set { this.SetValue(IsDeletedProperty, value); }
                }

                public static readonly PropertyData IsDeletedProperty = RegisterProperty<Country, bool>(o => o.IsDeleted);

                public byte[] TimeStamp
                {
                    get { return this.GetValue<byte[]>(TimeStampProperty); }
                    set { this.SetValue(TimeStampProperty, value); }
                }

                public static readonly PropertyData TimeStampProperty = RegisterProperty<Country, byte[]>(o => o.TimeStamp);

                public DateTime UpdateDate
                {
                    get { return this.GetValue<DateTime>(UpdateDateProperty); }
                    set { this.SetValue(UpdateDateProperty, value); }
                }

                public static readonly PropertyData UpdateDateProperty = RegisterProperty<Country, DateTime>(o => o.UpdateDate);

                public Guid UpdateUserId
                {
                    get { return this.GetValue<Guid>(UpdateUserIdProperty); }
                    set { this.SetValue(UpdateUserIdProperty, value); }
                }

                public static readonly PropertyData UpdateUserIdProperty = RegisterProperty<Country, Guid>(o => o.UpdateUserId);

                private static IEnumerable<Type> countryTypes;

                private static IEnumerable<Type> GetKnownTypes()
                {
                    Contract.Ensures(Contract.Result<IEnumerable<Type>>() != null);

                    if (countryTypes == null)
                    {
                        countryTypes = AppDomain.CurrentDomain.GetLoadedAssemblies(true)
                                .SelectMany(a => a.GetTypes())
                                .Where(t => typeof(Country).IsAssignableFrom(t))
                                .ToList();
                    }

                    return countryTypes;
                }
            }

            [DataContract]
            [KnownType("GetKnownTypes")]
            public class DataSourceResult
            {
                #region Public Properties
                [DataMember]
                public object Aggregates { get; set; }

                [DataMember]
                public IEnumerable Data { get; set; }

                [DataMember]
                public int Total { get; set; }
                #endregion

                #region Public Methods and Operators
                public static Type[] GetKnownTypes()
                {
                    var assembly = AppDomain.CurrentDomain.GetLoadedAssemblies(false).FirstOrDefault(a => a.FullName.StartsWith("DynamicClasses", StringComparison.Ordinal));
                    var types = new List<Type>(assembly == null ? new Type[0] : assembly.GetTypes().Where(t => t.Name.StartsWith("DynamicClass", StringComparison.Ordinal)).ToArray());

                    return types.ToArray();
                }
                #endregion
            }

            [DataContract(Name = "sort")]
            public class SortDescriptor
            {
                #region Public Properties

                [DataMember(Name = "dir")]
                public string Direction { get; set; }

                [DataMember(Name = "field")]
                public string Field { get; set; }

                #endregion
            }

            [DataContract(Name = "filter")]
            public class FilterDescriptor
            {
                [DataMember(Name = "field")]
                public string Field { get; set; }

                [DataMember(Name = "filters")]
                public IEnumerable<FilterDescriptor> Filters { get; set; }

                [DataMember(Name = "logic")]
                public string LogicalOperator { get; set; }

                [DataMember(Name = "operator")]
                public string Operator { get; set; }

                [DataMember(Name = "value")]
                public object Value { get; set; }
            }

            [TestCase]
            public void CanSerializeCollection()
            {
                var countrylist = new List<Country>();
                countrylist.Add(new Country { IsoCode = "AF", Description = "Afghanistan" });
                countrylist.Add(new Country { IsoCode = "AG", Description = "Agypt" });

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(countrylist, serializer);

                    Assert.AreEqual(countrylist.Count, deserializedObject.Count, description);

                    for (int i = 0; i < deserializedObject.Count; i++)
                    {
                        var expectedItem = countrylist[i];
                        var actualItem = deserializedObject[i];

                        Assert.AreEqual(expectedItem.IsoCode, actualItem.IsoCode, description);
                        Assert.AreEqual(expectedItem.Description, actualItem.Description, description);
                    }
                });
            }

            [TestCase]
            public void CanSerializeArray()
            {
                var countrylist = new[]
                {
                    new Country {IsoCode = "AF", Description = "Afghanistan"},
                    new Country {IsoCode = "AG", Description = "Agypt"},
                };

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(countrylist, serializer);

                    Assert.AreEqual(countrylist.GetType(), deserializedObject.GetType(), description);
                    Assert.AreEqual(countrylist.Length, deserializedObject.Length, description);

                    for (var i = 0; i < deserializedObject.Length; i++)
                    {
                        var expectedItem = countrylist[i];
                        var actualItem = deserializedObject[i];

                        Assert.AreEqual(expectedItem.IsoCode, actualItem.IsoCode, description);
                        Assert.AreEqual(expectedItem.Description, actualItem.Description, description);
                    }
                });
            }

            [TestCase]
            public void CanSerializeDictionary()
            {
                var dictionary = new Dictionary<string, int>();
                dictionary.Add("skip", 1);
                dictionary.Add("take", 2);
                dictionary.Add("some other string", 3);

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(dictionary, serializer);

                    Assert.AreEqual(dictionary.Count, deserializedObject.Count, description);

                    Assert.IsTrue(deserializedObject.ContainsKey("skip"));
                    Assert.AreEqual(1, deserializedObject["skip"]);
                    Assert.IsTrue(deserializedObject.ContainsKey("take"));
                    Assert.AreEqual(2, deserializedObject["take"]);
                    Assert.IsTrue(deserializedObject.ContainsKey("some other string"));
                    Assert.AreEqual(3, deserializedObject["some other string"]);
                });
            }

            [TestCase(42)]
            [TestCase(42d)]
            [TestCase("some string")]
            [TestCase(true)]
            public void CanSerializeBasicTypes<T>(T value)
            {
                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(value, serializer);

                    Assert.AreEqual(value, deserializedObject);
                });
            }

            [TestCase]
            public void CanSerializeCustomDataObject()
            {
                var countrylist = new List<Country>();
                countrylist.Add(new Country { IsoCode = "AF", Description = "Afghanistan" });
                countrylist.Add(new Country { IsoCode = "AG", Description = "Agypt" });

                var dataSourceResult = new DataSourceResult();
                dataSourceResult.Total = 243;
                dataSourceResult.Data = countrylist;

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(dataSourceResult, serializer);

                    Assert.AreEqual(243, deserializedObject.Total, description);

                    int counter = 0;
                    foreach (var country in dataSourceResult.Data)
                    {
                        var existingCountry = countrylist[counter++];

                        Assert.AreEqual(existingCountry.IsoCode, ((Country)country).IsoCode, description);
                        Assert.AreEqual(existingCountry.Description, ((Country)country).Description, description);
                    }
                });
            }

            [TestCase]
            public void CustomizedJsonParsingWithNullValue()
            {
                var json = "{ \"skip\":0,\"take\":10,\"filter\":null,\"includeDeleted\":false}";

                CustomizedJsonParsing(json, parameters =>
                {
                    Assert.AreEqual(0, parameters[0]);
                    Assert.AreEqual(10, parameters[1]);
                    Assert.AreEqual(false, parameters[2]);
                    Assert.IsNull(parameters[3]);
                });
            }

            [TestCase]
            public void CustomizedJsonParsing()
            {
                var json = "{\"take\":10,\"skip\":0,\"page\":1,\"pageSize\":10,\"sort\":[{\"field\":\"IsoCode\",\"dir\":\"asc\"}]}";

                CustomizedJsonParsing(json, parameters =>
                {
                    Assert.AreEqual(typeof(int), parameters[0].GetType());
                    Assert.AreEqual(0, parameters[0]);

                    Assert.AreEqual(typeof(int), parameters[1].GetType());
                    Assert.AreEqual(10, parameters[1]);

                    Assert.AreEqual(typeof(bool), parameters[2].GetType());
                    Assert.AreEqual(false, parameters[2]);

                    var sort = ((List<SortDescriptor>)parameters[3])[0];
                    Assert.IsNotNull(sort);
                    Assert.AreEqual("IsoCode", sort.Field);
                    Assert.AreEqual("asc", sort.Direction);
                });
            }

            private void CustomizedJsonParsing(string json, Action<object[]> assertAction)
            {
                var parameters = new object[] { 0, 10, false, null, null };
                var parameterTypes = new[] { typeof(int), typeof(int), typeof(bool), typeof(IEnumerable<SortDescriptor>), typeof(FilterDescriptor) };
                var parameterNames = new Dictionary<string, int> { { "skip", 0 }, { "take", 1 }, { "includeDeleted", 2 }, { "sort", 3 }, { "filter", 4 } };

                var serializer = SerializationTestHelper.GetJsonSerializer();
                serializer.PreserveReferences = false;
                serializer.WriteTypeInfo = false;

                using (var memoryStream = new MemoryStream())
                {
                    var writer = new StreamWriter(memoryStream);
                    writer.Write(json);
                    writer.Flush();

                    memoryStream.Position = 0L;

                    var reader = new StreamReader(memoryStream);
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        jsonReader.Read();

                        if (jsonReader.TokenType != JsonToken.StartObject)
                        {
                            throw new InvalidOperationException("Input needs to be wrapped in an object");
                        }

                        jsonReader.Read();

                        while (jsonReader.TokenType == JsonToken.PropertyName)
                        {
                            var parameterName = jsonReader.Value as string;

                            jsonReader.Read();
                            int parameterIndex;

                            if ((parameterName != null) && parameterNames.TryGetValue(parameterName, out parameterIndex))
                            {
                                parameters[parameterIndex] = serializer.Deserialize(parameterTypes[parameterIndex], jsonReader);
                            }
                            else
                            {
                                jsonReader.Skip();
                            }

                            jsonReader.Read();
                        }
                    }
                }

                assertAction(parameters);
            }
        }

        [TestFixture, Explicit]
        public class TheWarmupMethod
        {
            [TestCase]
            public void WarmsUpSpecificTypes()
            {
                var typesToWarmup = new[] { typeof(CircularTestModel), typeof(TestModel) };

                TestSerializationOnAllSerializers((serializer, description) =>
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
                TestSerializationOnAllSerializers((serializer, description) =>
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

        [TestFixture]
        public class TheKeyValuePairSerializerModifier
        {
            [TestCase]
            public void SerializesAndDeserializesKeyValuePairs()
            {
                var originalObject = new TestModelWithKeyValuePair();

                TestSerializationOnAllSerializers((serializer, description) =>
                {
                    var clonedObject = SerializationTestHelper.SerializeAndDeserialize(originalObject, serializer);

                    Assert.AreEqual(originalObject.KeyValuePair, clonedObject.KeyValuePair, description);
                    Assert.AreEqual(originalObject.KeyValuePairAsObject, clonedObject.KeyValuePairAsObject, description);
                });
            }
        }

        private static void TestSerializationOnAllSerializers(Action<ISerializer, string> action, bool testWithoutGraphIdsAsWell = true)
        {
            var serializers = new List<ISerializer>();

            serializers.Add(SerializationTestHelper.GetXmlSerializer(XmlSerializerOptimalizationMode.Performance));
            serializers.Add(SerializationTestHelper.GetXmlSerializer(XmlSerializerOptimalizationMode.PrettyXml));
            serializers.Add(SerializationTestHelper.GetBinarySerializer());
            serializers.Add(SerializationTestHelper.GetJsonSerializer());

            if (testWithoutGraphIdsAsWell)
            {
                var basicJsonSerializer = SerializationTestHelper.GetJsonSerializer();
                basicJsonSerializer.PreserveReferences = false;
                basicJsonSerializer.WriteTypeInfo = false;
                serializers.Add(basicJsonSerializer);
            }

            foreach (var serializer in serializers)
            {
                var typeName = serializer.GetType().GetSafeFullName();

                Log.Info();
                Log.Info();
                Log.Info();
                Log.Info("=== TESTING SERIALIZER: {0} ===", typeName);
                Log.Info();
                Log.Info();
                Log.Info();

                action(serializer, typeName);
            }
        }
    }
}
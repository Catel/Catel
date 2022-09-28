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

                [Key]
                public Guid Id
                {
                    get { return GetValue<Guid>(IdProperty); }
                    set { SetValue(IdProperty, value); }
                }

                public static readonly IPropertyData IdProperty = RegisterProperty<Country, Guid>(o => o.Id, Guid.NewGuid);

                [StringLength(256)]
                public string IsoCode
                {
                    get { return GetValue<string>(IsoCodeProperty); }
                    set { SetValue(IsoCodeProperty, value); }
                }

                public static readonly IPropertyData IsoCodeProperty = RegisterProperty<Country, string>(o => o.IsoCode);

                [StringLength(400)]
                public string Description
                {
                    get { return GetValue<string>(DescriptionProperty); }
                    set { SetValue(DescriptionProperty, value); }
                }

                public static readonly IPropertyData DescriptionProperty = RegisterProperty<Country, string>(o => o.Description);

                public DateTime CreateDate
                {
                    get { return GetValue<DateTime>(CreateDateProperty); }
                    set { SetValue(CreateDateProperty, value); }
                }

                public static readonly IPropertyData CreateDateProperty = RegisterProperty<Country, DateTime>(o => o.CreateDate);

                public Guid CreateUserId
                {
                    get { return GetValue<Guid>(CreateUserIdProperty); }
                    set { SetValue(CreateUserIdProperty, value); }
                }

                public static readonly IPropertyData CreateUserIdProperty = RegisterProperty<Country, Guid>(o => o.CreateUserId);

                public DateTime? DeleteDate
                {
                    get { return GetValue<DateTime?>(DeleteDateProperty); }
                    set { SetValue(DeleteDateProperty, value); }
                }

                public static readonly IPropertyData DeleteDateProperty = RegisterProperty<Country, DateTime?>(o => o.DeleteDate);

                public Guid? DeleteUserId
                {
                    get { return GetValue<Guid?>(DeleteUserIdProperty); }
                    set { SetValue(DeleteUserIdProperty, value); }
                }

                public static readonly IPropertyData DeleteUserIdProperty = RegisterProperty<Country, Guid?>(o => o.DeleteUserId);

                public bool IsDeleted
                {
                    get { return GetValue<bool>(IsDeletedProperty); }
                    set { SetValue(IsDeletedProperty, value); }
                }

                public static readonly IPropertyData IsDeletedProperty = RegisterProperty<Country, bool>(o => o.IsDeleted);

                public byte[] TimeStamp
                {
                    get { return GetValue<byte[]>(TimeStampProperty); }
                    set { SetValue(TimeStampProperty, value); }
                }

                public static readonly IPropertyData TimeStampProperty = RegisterProperty<Country, byte[]>(o => o.TimeStamp);

                public DateTime UpdateDate
                {
                    get { return GetValue<DateTime>(UpdateDateProperty); }
                    set { SetValue(UpdateDateProperty, value); }
                }

                public static readonly IPropertyData UpdateDateProperty = RegisterProperty<Country, DateTime>(o => o.UpdateDate);

                public Guid UpdateUserId
                {
                    get { return GetValue<Guid>(UpdateUserIdProperty); }
                    set { SetValue(UpdateUserIdProperty, value); }
                }

                public static readonly IPropertyData UpdateUserIdProperty = RegisterProperty<Country, Guid>(o => o.UpdateUserId);

                private static IEnumerable<Type> CountryTypes;

                private static IEnumerable<Type> GetKnownTypes()
                {
                    Contract.Ensures(Contract.Result<IEnumerable<Type>>() is not null);

                    if (CountryTypes is null)
                    {
                        CountryTypes = AssemblyHelper.GetLoadedAssemblies(AppDomain.CurrentDomain, true)
                                .SelectMany(a => a.GetTypes())
                                .Where(t => typeof(Country).IsAssignableFrom(t))
                                .ToList();
                    }

                    return CountryTypes;
                }
            }

            [DataContract]
            [KnownType(nameof(DataSourceResult.GetKnownTypes))]
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
                    var assembly = AssemblyHelper.GetLoadedAssemblies(AppDomain.CurrentDomain, false).FirstOrDefault(a => a.FullName.StartsWith("DynamicClasses", StringComparison.Ordinal));
                    var types = new List<Type>(assembly is null ? Array.Empty<Type>() : assembly.GetTypes().Where(t => t.Name.StartsWith("DynamicClass", StringComparison.Ordinal)).ToArray());

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

            public interface IObservableCollection<T> : IList<T>, INotifyPropertyChanged, INotifyCollectionChanged
            {
                #region Public Methods and Operators

                void AddItems(IEnumerable<T> collection);

                void RemoveItems(IEnumerable<T> collection);

                IDisposable SuspendChangeNotifications();

                #endregion
            }

            [Serializable]
            [DebuggerDisplay("Count = {Count}")]
            public abstract class ModelObservableCollectionBase<T> : ChildAwareModelBase, IObservableCollection<T>, IList
                where T : class
            {
                #region Constants and Fields

                public static readonly IPropertyData ItemsProperty = RegisterProperty
                    <ModelObservableCollectionBase<T>, FastObservableCollection<T>>(
                        // ReSharper restore StaticFieldInGenericType
                        o => o.Items, () => new FastObservableCollection<T>());

                #endregion

                #region Constructors and Destructors

                protected ModelObservableCollectionBase(IEnumerable<T> enumeration = null)
                {
                    if (enumeration is not null)
                    {
                        ((ICollection<T>)Items).AddRange(enumeration);
                    }
                }

                #endregion

                #region Public Events
                public event NotifyCollectionChangedEventHandler CollectionChanged;
                #endregion

                #region Public Properties

                public int Count
                {
                    get
                    {
                        return Items.Count;
                    }
                }

                public FastObservableCollection<T> Items
                {
                    get { return GetValue<FastObservableCollection<T>>(ItemsProperty); }
                    set { SetValue(ItemsProperty, value); }
                }

                #endregion

                #region Explicit Interface Properties

                bool IList.IsFixedSize
                {
                    get
                    {
                        return ((IList)Items).IsFixedSize;
                    }
                }

                bool IList.IsReadOnly
                {
                    get
                    {
                        return ((IList)Items).IsReadOnly;
                    }
                }

                bool ICollection.IsSynchronized
                {
                    get
                    {
                        return ((ICollection)Items).IsSynchronized;
                    }
                }

                object ICollection.SyncRoot
                {
                    get
                    {
                        return ((ICollection)Items).SyncRoot;
                    }
                }

                bool ICollection<T>.IsReadOnly
                {
                    get
                    {
                        return ((ICollection<T>)Items).IsReadOnly;
                    }
                }

                #endregion

                #region Public Indexers

                public T this[int index]
                {
                    get
                    {
                        return Items[index];
                    }

                    set
                    {
                        Items[index] = value;
                    }
                }

                #endregion

                #region Explicit Interface Indexers

                object IList.this[int index]
                {
                    get
                    {
                        return ((IList)Items)[index];
                    }

                    set
                    {
                        ((IList)Items)[index] = value;
                    }
                }

                #endregion

                #region Public Methods and Operators

                public void Add(T item)
                {
                    Items.Add(item);
                }

                public void AddItems(IEnumerable<T> collection)
                {
                    Items.AddItems(collection);
                }

                public void AddItems(IEnumerable collection)
                {
                    Items.AddItems(collection);
                }

                public void Clear()
                {
                    Items.Clear();
                }

                public bool Contains(T item)
                {
                    return Items.Contains(item);
                }

                public void CopyTo(T[] array, int index)
                {
                    Items.CopyTo(array, index);
                }

                public IEnumerator<T> GetEnumerator()
                {
                    return Items.GetEnumerator();
                }

                public int IndexOf(T item)
                {
                    return Items.IndexOf(item);
                }

                public void Insert(int index, T item)
                {
                    Items.Insert(index, item);
                }

                public bool Remove(T item)
                {
                    return Items.Remove(item);
                }

                public void RemoveAt(int index)
                {
                    Items.RemoveAt(index);
                }

                public void RemoveItems(IEnumerable<T> collection)
                {
                    Items.RemoveItems(collection);
                }

                public void RemoveItems(IEnumerable collection)
                {
                    Items.RemoveItems(collection);
                }

                #endregion

                #region Explicit Interface Methods

                int IList.Add(object value)
                {
                    return ((IList)Items).Add(value);
                }

                bool IList.Contains(object value)
                {
                    return ((IList)Items).Contains(value);
                }

                void ICollection.CopyTo(Array array, int index)
                {
                    ((ICollection)Items).CopyTo(array, index);
                }

                int IList.IndexOf(object value)
                {
                    return ((IList)Items).IndexOf(value);
                }

                void IList.Insert(int index, object value)
                {
                    ((IList)Items).Insert(index, value);
                }

                void IList.Remove(object value)
                {
                    ((IList)Items).Remove(value);
                }

                void IList.RemoveAt(int index)
                {
                    RemoveAt(index);
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return Items.GetEnumerator();
                }

                #endregion

                #region Methods
                protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
                {
                    Contract.Requires(e is not null);

                    CollectionChanged?.Invoke(this, e);
                }

                protected override void OnPropertyObjectCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
                {
                    base.OnPropertyObjectCollectionChanged(sender, e);

                    if (sender == Items)
                    {
                        OnCollectionChanged(e);
                    }
                }

                protected override void OnPropertyObjectPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    base.OnPropertyObjectPropertyChanged(sender, e);

                    if (sender == Items)
                    {
                        RaisePropertyChanged(e.PropertyName);
                    }
                }

                public IDisposable SuspendChangeNotifications()
                {
                    throw new NotImplementedException();
                }

                #endregion
            }

            [Serializable]
            public class Floor : ModelBase
            {
                public Floor()
                {
                }

                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                public static readonly IPropertyData NameProperty = RegisterProperty<Floor, string>(o => o.Name);
            }

            public interface IFloorCollection : IObservableCollection<Floor>
            {

            }

            [Serializable]
            public class FloorCollection : ModelObservableCollectionBase<Floor>, IFloorCollection
            {
                public FloorCollection()
                {
                }
            }

            [Serializable]
            [SerializeAsCollection]
            public class FloorCollectionAsCollection : ModelObservableCollectionBase<Floor>, IFloorCollection
            {
                public FloorCollectionAsCollection()
                {
                }
            }

            [Serializable]
            public class Building : ModelBase
            {
                public Building()
                {
                }

                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                public static readonly IPropertyData NameProperty = RegisterProperty<Building, string>(o => o.Name);


                public IFloorCollection Floors
                {
                    get { return GetValue<IFloorCollection>(FloorsProperty); }
                    set { SetValue(FloorsProperty, value); }
                }

                public static readonly IPropertyData FloorsProperty = RegisterProperty<Building, IFloorCollection>(o => o.Floors, () => (IFloorCollection)new FloorCollection());
            }

            [Serializable]
            public class BuildingAsCollection : ModelBase
            {
                public BuildingAsCollection()
                {
                }

                public string Name
                {
                    get { return GetValue<string>(NameProperty); }
                    set { SetValue(NameProperty, value); }
                }

                public static readonly IPropertyData NameProperty = RegisterProperty<BuildingAsCollection, string>(o => o.Name);


                public IFloorCollection Floors
                {
                    get { return GetValue<IFloorCollection>(FloorsProperty); }
                    set { SetValue(FloorsProperty, value); }
                }

                public static readonly IPropertyData FloorsProperty = RegisterProperty<BuildingAsCollection, IFloorCollection>(o => o.Floors, () => (IFloorCollection)new FloorCollectionAsCollection());
            }

            [Serializable]
            public class BuildingCollection : ModelObservableCollectionBase<Building>
            {
                public BuildingCollection()
                {
                }
            }

            [Serializable]
            [SerializeAsCollection]
            public class BuildingCollectionAsCollection : ModelObservableCollectionBase<BuildingAsCollection>
            {
                public BuildingCollectionAsCollection()
                {
                }
            }

            [TestCase]
            public void CanSerializeCollection()
            {
                var countrylist = new List<Country>();
                countrylist.Add(new Country { IsoCode = "AF", Description = "Afghanistan" });
                countrylist.Add(new Country { IsoCode = "AG", Description = "Agypt" });

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(countrylist, serializer, config);

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
            public void CanSerializeModelBaseAndCollectionAsModel()
            {
                var b1 = new Building { Name = "B1" };
                b1.Floors.Add(new Floor { Name = "F1" });
                b1.Floors.Add(new Floor { Name = "F2" });
                b1.Floors.Add(new Floor { Name = "F3" });

                var b2 = new Building { Name = "B2" };
                var b3 = new Building { Name = "B3" };

                var bc = new BuildingCollection();
                bc.Add(b1);
                bc.Add(b2);
                bc.Add(b3);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(bc, serializer, config);

                    Assert.AreEqual(bc.Count, deserializedObject.Count, description);

                    Assert.AreEqual(3, deserializedObject.Count, description);
                    Assert.AreEqual("B1", deserializedObject[0].Name, description);
                    Assert.AreEqual("F1", deserializedObject[0].Floors[0].Name, description);
                    Assert.AreEqual("F2", deserializedObject[0].Floors[1].Name, description);
                    Assert.AreEqual("F3", deserializedObject[0].Floors[2].Name, description);
                    Assert.AreEqual("B2", deserializedObject[1].Name, description);
                    Assert.AreEqual("B3", deserializedObject[2].Name, description);
                });
            }

            [TestCase]
            public void CanSerializeModelBaseAndCollectionAsCollection()
            {
                var b1 = new BuildingAsCollection { Name = "B1" };
                b1.Floors.Add(new Floor { Name = "F1" });
                b1.Floors.Add(new Floor { Name = "F2" });
                b1.Floors.Add(new Floor { Name = "F3" });

                var b2 = new BuildingAsCollection { Name = "B2" };
                var b3 = new BuildingAsCollection { Name = "B3" };

                var bc = new BuildingCollectionAsCollection();
                bc.Add(b1);
                bc.Add(b2);
                bc.Add(b3);

                TestSerializationOnAllSerializers((serializer, config, description) =>
                //TestSerializationOnJsonSerializer((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(bc, serializer, config);

                    Assert.AreEqual(bc.Count, deserializedObject.Count, description);

                    Assert.AreEqual(3, deserializedObject.Count, description);
                    Assert.AreEqual("B1", deserializedObject[0].Name, description);
                    Assert.AreEqual("F1", deserializedObject[0].Floors[0].Name, description);
                    Assert.AreEqual("F2", deserializedObject[0].Floors[1].Name, description);
                    Assert.AreEqual("F3", deserializedObject[0].Floors[2].Name, description);
                    Assert.AreEqual("B2", deserializedObject[1].Name, description);
                    Assert.AreEqual("B3", deserializedObject[2].Name, description);
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

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(countrylist, serializer, config);

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

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(dictionary, serializer, config);

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
                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(value, serializer, config);

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

                TestSerializationOnAllSerializers((serializer, config, description) =>
                {
                    var deserializedObject = SerializationTestHelper.SerializeAndDeserialize(dataSourceResult, serializer, config);

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
#pragma warning disable IDISP001 // Dispose created.
                    var writer = new StreamWriter(memoryStream);
#pragma warning restore IDISP001 // Dispose created.
                    writer.Write(json);
                    writer.Flush();

                    memoryStream.Position = 0L;

#pragma warning disable IDISP001 // Dispose created.
                    var reader = new StreamReader(memoryStream);
#pragma warning restore IDISP001 // Dispose created.

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

                            if ((parameterName is not null) && parameterNames.TryGetValue(parameterName, out var parameterIndex))
                            {
                                parameters[parameterIndex] = serializer.Deserialize(parameterTypes[parameterIndex], jsonReader, null);
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
    }
}

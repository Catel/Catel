namespace Catel.Tests.Runtime.Serialization.TestModels
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using Catel.Data;

    public class InheritedFromModelBase : ModelBase
    {
        [XmlAttribute]
        public string? Name { get; set; }
    }

    public class NotInheritedFromModelBase
    {
        public string? Name { get; set; }
    }

    [Serializable]
    public class DataItem
    {
        public string? Name { get; set; }
    }

    public interface IDataItem
    {
        string? Name { get; set; }
    }

    [Serializable]
    public class DataItemV : ModelBase, IDataItem
    {
        public string? Name
        {
            get { return GetValue<string?>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);

        public IDataItem? First
        {
            get { return GetValue<IDataItem>(FirstProperty); }
            set { SetValue(FirstProperty, value); }
        }

        public static readonly IPropertyData FirstProperty = RegisterProperty("First", default(IDataItem?));

        public IDataItem? Second
        {
            get { return GetValue<IDataItem>(SecondProperty); }
            set { SetValue(SecondProperty, value); }
        }

        public static readonly IPropertyData SecondProperty = RegisterProperty("Second", default(IDataItem?));
    }

    public class DataItemRPart
    {
        public string? Name { get; set; }

        public IDataItem? Item { get; set; }
    }

    public class DataItemRPart2
    {
        public string? Name { get; set; }

        public IDataItem? Item { get; set; }
    }

    [Serializable]
    public class DataItemR : ModelBase, IDataItem
    {
        public DataItemR()
        {
            Parts = new List<DataItemRPart>();
            Parts2 = new List<DataItemRPart2>();
        }

        public string? Name
        {
            get { return GetValue<string?>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);

        public List<DataItemRPart> Parts
        {
            get { return GetValue<List<DataItemRPart>>(PartsProperty); }
            set { SetValue(PartsProperty, value); }
        }

        public static readonly IPropertyData PartsProperty = RegisterProperty("Parts", default(List<DataItemRPart>));

        public List<DataItemRPart2> Parts2
        {
            get { return GetValue<List<DataItemRPart2>>(Parts2Property); }
            set { SetValue(Parts2Property, value); }
        }

        public static readonly IPropertyData Parts2Property = RegisterProperty("Parts2", default(List<DataItemRPart2>));
    }

    [Serializable]
    public class DataItemD : ModelBase, IDataItem
    {
        public string? Name
        {
            get { return GetValue<string?>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        public static readonly IPropertyData NameProperty = RegisterProperty("Name", string.Empty);
    }

    public class ContentData
    {
        public List<IDataItem> DataItems { get; set; } = new();
        public Dictionary<string, IDataItem> Roots { get; set; } = new();
    }

    public class SerializableData
    {
        public List<DataItem> Items { get; set; } = new();
        public Dictionary<string, DataItem> Roots { get; set; } = new();
    }
}

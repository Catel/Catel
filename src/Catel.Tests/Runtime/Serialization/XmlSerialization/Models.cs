namespace Namespace_1
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

    public class SerializableData
    {
        public List<DataItem> Items { get; set; } = new();
        public Dictionary<string, DataItem> Roots { get; set; } = new();
    }
}

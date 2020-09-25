namespace Catel.Data
{
    using System;
    using System.ComponentModel;
    using Catel.Reflection;


    /// <summary>
    /// Object that contains all the property data that is used by the <see cref="ModelBase"/> class.
    /// </summary>
    public interface IPropertyData
    {
        bool IncludeInBackup { get; }
        bool IncludeInSerialization { get; }
        bool IsCalculatedProperty { get; set; }
        bool IsModelBaseProperty { get; }
        bool IsSerializable { get; }
        string Name { get; }
        Type Type { get; }

        EventHandler<PropertyChangedEventArgs> PropertyChangedEventHandler { get; }

        object GetDefaultValue();
        TValue GetDefaultValue<TValue>();
        CachedPropertyInfo GetPropertyInfo(Type containingType);
    }
}

namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to define that a specific member must be excluded from the serialization by the serialization engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ExcludeFromSerializationAttribute : Attribute
    {
    }
}

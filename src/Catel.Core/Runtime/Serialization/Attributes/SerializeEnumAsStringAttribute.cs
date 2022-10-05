namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Attribute to define that a enum member must be serialized as string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SerializeEnumAsStringAttribute : Attribute
    {
    }
}

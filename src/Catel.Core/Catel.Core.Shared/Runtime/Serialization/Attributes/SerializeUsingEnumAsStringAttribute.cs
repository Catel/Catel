using System;
using System.Collections.Generic;
using System.Text;

namespace Catel.Runtime.Serialization.Attributes
{
    /// <summary>
    /// Attribute to define that a enum member must be serialized as string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class SerializeUsingEnumAsStringAttribute : Attribute
    {
    }
}

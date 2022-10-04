namespace Catel.Runtime.Serialization
{
    using System;

    /// <summary>
    /// Parse the members decorated with this attribute using <c>Parse</c> and <c>ToString</c>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
    public class SerializeUsingParseAndToStringAttribute : Attribute
    {
    }
}

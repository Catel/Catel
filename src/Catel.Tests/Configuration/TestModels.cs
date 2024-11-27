namespace Catel.Tests.Configuration
{
    using System.Runtime.Serialization;

    [DataContract]
    public class ComplexObject
    {
        [DataMember] public string ValueA { get; set; } = string.Empty;
        [DataMember] public int ValueB { get; set; }
    }
}

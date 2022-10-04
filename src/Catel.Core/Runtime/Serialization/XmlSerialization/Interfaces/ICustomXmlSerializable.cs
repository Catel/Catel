namespace Catel.Runtime.Serialization.Xml
{
    using System.Xml.Linq;

    /// <summary>
    /// Allows a type to implement their own (de)serialization mechanism.
    /// </summary>
    public interface ICustomXmlSerializable
    {
        /// <summary>
        /// Serializes the object to the specified xml element.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        void Serialize(System.Xml.XmlWriter xmlWriter);

        /// <summary>
        /// Serializes the object from the specified xml element.
        /// <para />
        /// Note that the object is always constructed by the serialization engine, the
        /// object itself must read the values from the <paramref name="xmlReader" />.
        /// </summary>
        /// <param name="xmlReader">The XML element.</param>
        void Deserialize(System.Xml.XmlReader xmlReader);
    }
}

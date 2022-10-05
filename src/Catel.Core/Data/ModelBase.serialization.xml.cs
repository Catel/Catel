namespace Catel.Data
{
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Runtime.Serialization.Xml;
    using Runtime.Serialization;
    using Scoping;
    using System;

    public partial class ModelBase
    {
        private static readonly Func<SerializationScope> XmlSerializationScopeFactory = new Func<SerializationScope>(() => new SerializationScope(SerializationFactory.GetXmlSerializer(), null));

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema? IXmlSerializable.GetSchema()
        {
            // As requested by the documentation, we return null
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.</param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement && !reader.HasAttributes)
            {
                return;
            }

            var contextInfo = new XmlSerializationContextInfo(reader, this);

            var scopeName = SerializationContextHelper.GetSerializationScopeName();
            using (var scopeManager = ScopeManager<SerializationScope>.GetScopeManager(scopeName, XmlSerializationScopeFactory))
            {
                var serializer = scopeManager.ScopeObject.Serializer;
                serializer.Deserialize(this, contextInfo, scopeManager.ScopeObject.Configuration);
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            // Note: although this XmlWriter doesn't have settings, it has an internal writer that
            // is used (with the correct settings). For more details, see the source at:
            // https://referencesource.microsoft.com/#System.Runtime.Serialization/System/Runtime/Serialization/XmlSerializableWriter.cs

            var scopeName = SerializationContextHelper.GetSerializationScopeName();
            using (var scopeManager = ScopeManager<SerializationScope>.GetScopeManager(scopeName, XmlSerializationScopeFactory))
            {
                var serializer = scopeManager.ScopeObject.Serializer;
                serializer.Serialize(this, new XmlSerializationContextInfo(writer, this), scopeManager.ScopeObject.Configuration);
            }
        }
    }
}

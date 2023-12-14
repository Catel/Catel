namespace Catel.Runtime.Serialization.Xml
{
    using System.Xml;

    /// <summary>
    /// Serialization configuration with additional xml configuration.
    /// </summary>
    public class XmlSerializationConfiguration : SerializationConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlSerializationConfiguration"/> class.
        /// </summary>
        public XmlSerializationConfiguration()
        {
        }

        /// <summary>
        /// Gets or sets the xml writer settings.
        /// </summary>
        public XmlWriterSettings? WriterSettings { get; set; }

        /// <summary>
        /// Gets or sets the xml reader settings.
        /// </summary>
        public XmlReaderSettings? ReaderSettings { get; set; }
    }
}

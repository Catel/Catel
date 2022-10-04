namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Xml.Linq;

    /// <summary>
    /// Xml serialization context factory.
    /// </summary>
    public class XmlSerializationContextInfoFactory : ISerializationContextInfoFactory
    {
        /// <summary>
        /// Gets the serialization context information.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="model">The model.</param>
        /// <param name="data">The data.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>
        /// ISerializationContext.
        /// </returns>
        public ISerializationContextInfo GetSerializationContextInfo(ISerializer serializer, object model, object data, ISerializationConfiguration configuration)
        {
            throw new NotImplementedException();
            //return new XmlSerializationContextInfo((XElement)data, model);
        }
    }
}

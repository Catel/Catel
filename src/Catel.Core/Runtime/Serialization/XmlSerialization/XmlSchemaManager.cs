namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    /// Xml schema manager to create xml schemas for models in Catel.
    /// </summary>
    public class XmlSchemaManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether the xml schema manager should generate flat schemas.
        /// <para />
        /// This means that classes will be generated as is. This results in easier types, but the complex types
        /// cannot be re-used using WCF.
        /// </summary>
        /// <value><c>true</c> if the xml schema manager should generate flat schemas; otherwise, <c>false</c>.</value>
        public static bool GenerateFlatSchemas { get; set; }

        /// <summary>
        /// Gets the XML schema.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <returns>XmlSchemaComplexType.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="schemaSet"/> is <c>null</c>.</exception>
        /// <remarks>
        /// Do not move this method, it is used by Catel.Fody.
        /// </remarks>
        public static XmlQualifiedName GetXmlSchema(Type type, XmlSchemaSet schemaSet)
        {
            return XmlSchemaHelper.GetXmlSchema(type, schemaSet, GenerateFlatSchemas);
        }
    }
}

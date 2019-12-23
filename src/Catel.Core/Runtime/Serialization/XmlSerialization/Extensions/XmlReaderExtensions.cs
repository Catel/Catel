namespace Catel.Runtime.Serialization
{
    using System.Xml;

    /// <summary>
    /// Xml reader extension methods.
    /// </summary>
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Moves the xml reader to the next content element.
        /// </summary>
        /// <param name="xmlReader"></param>
        /// <param name="startMember"></param>
        /// <returns></returns>
        public static bool MoveToNextContentElement(this XmlReader xmlReader, string startMember)
        {
            if (xmlReader.EOF)
            {
                // Reached end of document
                return false;
            }

            if (xmlReader.NodeType == XmlNodeType.EndElement)
            {
                if (xmlReader.LocalName.Equals(startMember))
                {
                    // We've hit the end of the object
                    return false;
                }
                else
                {
                    // End of any object, move to content
                    xmlReader.Read();
                    xmlReader.MoveToContent();
                }
            }

            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                xmlReader.MoveToContent();
            }

            return xmlReader.NodeType != XmlNodeType.EndElement;
        }
    }
}

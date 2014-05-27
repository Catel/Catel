#if NET

#pragma warning disable 1591 // 1591 = missing xml

using System.Xml;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class XmlBamlNode
    {
        public virtual XmlNodeType NodeType
        {
            get { return XmlNodeType.None;}
        }
    }
}

#endif
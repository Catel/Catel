#pragma warning disable 1591 // 1591 = missing xml

using System.Collections.Generic;
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

    internal class XmlBamlNodeCollection : List<XmlBamlNode>
    {}
}

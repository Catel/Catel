#if NET

#pragma warning disable 1591 // 1591 = missing xml

using System.Xml;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class XmlBamlText : XmlBamlNode
    {
        private string _text;

        public XmlBamlText(string text)
        {
            _text = text;
        }

        public string Text
        {
            get { return _text; }
        }

        public override System.Xml.XmlNodeType NodeType
        {
            get
            {
                return XmlNodeType.Text;
            }
        }
    }
}

#endif
#if NET

#pragma warning disable 1591 // 1591 = missing xml

using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class XmlBamlElement : XmlBamlNode
    {
        private ArrayList _arguments = new ArrayList();
        private XmlNamespaceCollection _namespaces = new XmlNamespaceCollection();
        private TypeDeclaration _typeDeclaration;
        private KeysResourcesCollection _keysResources = new KeysResourcesCollection();
        private long _position;

        public XmlBamlElement()
        {
        }


        public XmlBamlElement(XmlBamlElement parent)
        {
            this.Namespaces.AddRange(parent.Namespaces);
        }

        public XmlNamespaceCollection Namespaces
        {
            get { return _namespaces; }
        }

        public TypeDeclaration TypeDeclaration
        {
            get
            {
                return this._typeDeclaration;
            }
            set
            {
                this._typeDeclaration = value;
            }
        }

        public override XmlNodeType NodeType
        {
            get
            {
                return XmlNodeType.Element;
            }
        }

        public long Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public override string ToString()
        {
            return String.Format("Element: {0}", TypeDeclaration.Name);
        }
    }

    internal class XmlBamlEndElement : XmlBamlElement
    {
        public XmlBamlEndElement(XmlBamlElement start)
        {
            this.TypeDeclaration = start.TypeDeclaration;
            this.Namespaces.AddRange(start.Namespaces);
        }

        public override XmlNodeType NodeType
        {
            get
            {
                return XmlNodeType.EndElement;
            }
        }

        public override string ToString()
        {
            return String.Format("EndElement: {0}", TypeDeclaration.Name);
        }
    }

    internal class KeysResourcesCollection : List<KeysResource>
    {
        public KeysResource Last
        {
            get
            {
                if (this.Count == 0)
                    return null;
                return this[this.Count - 1];
            }
        }

        public KeysResource First
        {
            get
            {
                if (this.Count == 0)
                    return null;
                return this[0];
            }
        }
    }

    internal class KeysResource
    {
        private KeysTable _keys = new KeysTable();
        private ArrayList _staticResources = new ArrayList();

        public KeysTable Keys
        {
            get { return _keys; }
        }

        public ArrayList StaticResources
        {
            get { return _staticResources; }
        }
    }

    internal class KeysTable
    {
        private Hashtable table = new Hashtable();

        public String this[long position]
        {
            get
            {
                return (string)this.table[position];
            }
            set
            {
                this.table[position] = value;
            }
        }

        public int Count
        {
            get { return this.table.Count; }
        }

        public void Remove(long position)
        {
            this.table.Remove(position);
        }

        public bool HasKey(long position)
        {
            return this.table.ContainsKey(position);
        }
    }
}

#endif
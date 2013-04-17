#pragma warning disable 1591 // 1591 = missing xml

using System;

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class XmlBamlPropertyElement : XmlBamlElement
    {
        private readonly PropertyType _propertyType;
        private PropertyDeclaration propertyDeclaration;


        public XmlBamlPropertyElement(PropertyType propertyType, PropertyDeclaration propertyDeclaration)
        {
            _propertyType = propertyType;
            this.propertyDeclaration = propertyDeclaration;
        }

        public XmlBamlPropertyElement(XmlBamlElement parent, PropertyType propertyType, PropertyDeclaration propertyDeclaration)
            : base(parent)
        {
            _propertyType = propertyType;
            this.propertyDeclaration = propertyDeclaration;
            this.TypeDeclaration = propertyDeclaration.DeclaringType;
        }

        public PropertyDeclaration PropertyDeclaration
        {
            get
            {
                return this.propertyDeclaration;
            }
        }

        public PropertyType PropertyType
        {
            get { return _propertyType; }
        }

        public override string ToString()
        {
            return String.Format("PropertyElement: {0}.{1}", TypeDeclaration.Name, PropertyDeclaration.Name);
        }
    }
}

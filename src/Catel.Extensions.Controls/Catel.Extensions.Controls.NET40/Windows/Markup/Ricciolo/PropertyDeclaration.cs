#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    internal class PropertyDeclaration
    {
        private TypeDeclaration declaringType;
        private string name;

        // Methods
        public PropertyDeclaration(string name)
        {
            this.name = name;
            this.declaringType = null;
        }

        public PropertyDeclaration(string name, TypeDeclaration declaringType)
        {
            this.name = name;
            this.declaringType = declaringType;
        }

        public override string ToString()
        {
            if (((this.DeclaringType != null) && (this.DeclaringType.Name == "XmlNamespace")) && ((this.DeclaringType.Namespace == null) && (this.DeclaringType.Assembly == null)))
            {
                if ((this.Name == null) || (this.Name.Length == 0))
                {
                    return "xmlns";
                }
                return ("xmlns:" + this.Name);
            }
            return this.Name;
        }

        // Properties
        public TypeDeclaration DeclaringType
        {
            get
            {
                return this.declaringType;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}

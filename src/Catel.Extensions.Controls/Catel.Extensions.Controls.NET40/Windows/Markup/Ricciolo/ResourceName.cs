#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    public class ResourceName
    {
        private string name;

        public ResourceName(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.Name;
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

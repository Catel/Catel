#if NET

#pragma warning disable 1591 // 1591 = missing xml

namespace Ricciolo.StylesExplorer.MarkupReflection
{
    public class ResourceName
    {
        private readonly string _name;

        public ResourceName(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public string Name
        {
            get { return _name; }
        }
    }
}

#endif

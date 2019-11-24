namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Memory efficient typed property bag that takes care of boxing.
    /// </summary>
    public partial class TypedPropertyBag : PropertyBagBase
    {
        private readonly HashSet<string> _names = new HashSet<string>();

        public override string[] GetAllNames()
        {
            lock (_names)
            {
                return _names.ToArray();
            }
        }

        public override bool IsAvailable(string name)
        {
            Argument.IsNotNullOrWhitespace(nameof(name), name);

            lock (_names)
            {
                return _names.Contains(name);
            }
        }

        private void AddName(string name)
        {
            lock (_names)
            {
                _names.Add(name);
            }
        }
    }
}

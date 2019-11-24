namespace Catel.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Catel.Logging;
    using Catel.Reflection;

    /// <summary>
    /// Memory efficient typed property bag that takes care of boxing.
    /// </summary>
    public partial class TypedPropertyBag : PropertyBagBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Type> _propertyTypes = new Dictionary<string, Type>();

        public override string[] GetAllNames()
        {
            lock (_propertyTypes)
            {
                return _propertyTypes.Keys.ToArray();
            }
        }

        public override bool IsAvailable(string name)
        {
            Argument.IsNotNullOrWhitespace(nameof(name), name);

            lock (_propertyTypes)
            {
                return _propertyTypes.ContainsKey(name);
            }
        }

        private void EnsureIntegrity(string name, Type type)
        {
            lock (_propertyTypes)
            {
                if (_propertyTypes.TryGetValue(name, out var existingType))
                {
                    if (existingType != type && !existingType.IsAssignableFromEx(type))
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>($"Property '{name}' is already registered as '{existingType.FullName}' which is not compatible with '{type.FullName}'");
                    }
                }
                else
                {
                    _propertyTypes[name] = type;
                }
            }
        }
    }
}

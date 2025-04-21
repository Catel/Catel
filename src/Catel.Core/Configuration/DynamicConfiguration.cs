namespace Catel.Configuration
{
    using System.Collections.Generic;
    using Catel.Data;

    /// <summary>
    /// Dynamic configuration.
    /// </summary>
    public class DynamicConfiguration : ModelBase
    {
#pragma warning disable IDE1006 // Naming Styles
        protected static readonly HashSet<string> DynamicProperties = new HashSet<string>();
#pragma warning restore IDE1006 // Naming Styles

        private readonly HashSet<string> _propertiesSetAtLeastOnce = new HashSet<string>();

        public DynamicConfiguration()
        {
        }

        protected override IPropertyBag CreatePropertyBag()
        {
            // Fix for https://github.com/Catel/Catel/issues/1517 since values
            // are read as string, but could be retrieved as bool, etc
            return new PropertyBag();
        }

        /// <summary>
        /// Registers the configuration key.
        /// </summary>
        /// <param name="name">The name.</param>
        public virtual void RegisterConfigurationKey(string name)
        {
            // Dynamic registrations
            DynamicProperties.Add(name);

            if (IsConfigurationValueSet(name))
            {
                return;
            }

            var propertyData = RegisterProperty<object>(name);

            InitializePropertyAfterConstruction(propertyData);
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual object? GetConfigurationValue(string name)
        {
            RegisterConfigurationKey(name);

            return GetValue<object>(name);
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public virtual void SetConfigurationValue(string name, object? value)
        {
            RegisterConfigurationKey(name);

            SetValue(name, value);

            MarkConfigurationValueAsSet(name);
        }

        /// <summary>
        /// Determines whether the specified property is set. If not, a default value should be returned.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the property is set; otherwise, <c>false</c>.</returns>
        public virtual bool IsConfigurationValueSet(string name)
        {
            if (!IsPropertyRegistered(GetType(), name))
            {
                return false;
            }

            lock (_propertiesSetAtLeastOnce)
            {
                return _propertiesSetAtLeastOnce.Contains(name);
            }
        }

        /// <summary>
        /// Marks the property as set at least once so it doesn't have a default value.
        /// </summary>
        /// <param name="name">The name.</param>
        public virtual void MarkConfigurationValueAsSet(string name)
        {
            lock (_propertiesSetAtLeastOnce)
            {
                _propertiesSetAtLeastOnce.Add(name);
            }
        }
    }
}

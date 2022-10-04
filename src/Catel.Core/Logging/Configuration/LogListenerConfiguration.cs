namespace Catel.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using Catel.IoC;
    using Catel.Reflection;

    /// <summary>
    /// The log listener configuration element.
    /// </summary>
    public sealed class LogListenerConfiguration : ConfigurationElement
    {
        /// <summary>
        /// The type property name.
        /// </summary>
        private const string TypePropertyName = "type";

        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The dynamic properties.
        /// </summary>
        private readonly Dictionary<string, string> _dynamicProperties = new Dictionary<string, string>(); 

        /// <summary>
        /// Initializes a new instance of the <see cref="LogListenerConfiguration"/> class.
        /// </summary>
        public LogListenerConfiguration()
        {
            
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [ConfigurationProperty(TypePropertyName, IsRequired = true)]
        public string Type
        {
            get { return (string) this[TypePropertyName]; }
            set { this[TypePropertyName] = value; }
        }

        /// <summary>
        /// Gets a value indicating whether an unknown attribute is encountered during deserialization.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>true when an unknown attribute is encountered while deserializing; otherwise, false.</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            _dynamicProperties[name] = value;

            return true;
        }

        /// <summary>
        /// Gets the log listener which this configuration represents.
        /// </summary>
        /// <param name="assembly">The assembly to load the product info from. If <c>null</c>, the entry assembly will be used.</param>
        /// <returns>The <see cref="ILogListener"/>.</returns>
        public ILogListener GetLogListener(Assembly? assembly = null)
        {
            var typeAsString = ObjectToStringHelper.ToString(Type);

            Log.Debug("Creating ILogListener based on configuration for type '{0}'", typeAsString);

            ILogListener? logListener = null;

            var type = TypeCache.GetType(Type, allowInitialization: false);
            if (type is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to retrieve type '{typeAsString}'");
            }

            var typeFactory = IoCConfiguration.DefaultTypeFactory;
#pragma warning disable HAA0101 // Array allocation for params parameter
            logListener = typeFactory.CreateInstanceWithParametersAndAutoCompletion(type, assembly) as ILogListener;
#pragma warning restore HAA0101 // Array allocation for params parameter
            if (logListener is null)
            {
                logListener = typeFactory.CreateInstance(type) as ILogListener;
            }

            if (logListener is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to instantiate type '{typeAsString}' or it does not implement ILogListener and thus cannot be used as such");
            }

            foreach (var dynamicProperty in _dynamicProperties)
            {
                if (string.Equals(dynamicProperty.Key, TypePropertyName, StringComparison.InvariantCulture))
                {
                    continue;
                }

                var propertyInfo = type.GetPropertyEx(dynamicProperty.Key);
                if (propertyInfo is null)
                {
                    Log.Warning("Property '{0}.{1}' cannot be found, make sure that it exists to load the value correctly", typeAsString, dynamicProperty.Key);
                    continue;
                }

                Log.Debug("Setting property '{0}' to value '{1}'", dynamicProperty.Key, ObjectToStringHelper.ToString(dynamicProperty.Value));

                var propertyValue = StringToObjectHelper.ToRightType(propertyInfo.PropertyType, dynamicProperty.Value);
                PropertyHelper.SetPropertyValue(logListener, dynamicProperty.Key, propertyValue);
            }

            Log.Debug("Created ILogListener based on configuration for type '{0}'", typeAsString);

            return logListener;
        }
    }
}

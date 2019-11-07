// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfiguration.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using System.Collections.Generic;
    using System.Xml;
    using Catel.IoC;
    using Catel.Runtime.Serialization.Xml;
    using Data;
    using Runtime.Serialization;

    /// <summary>
    /// Dynamic configuration.
    /// </summary>
    [SerializerModifier(typeof(DynamicConfigurationSerializerModifier))]
    public class DynamicConfiguration : ModelBase, ICustomXmlSerializable
    {
        private readonly HashSet<string> _propertiesSetAtLeastOnce = new HashSet<string>();

        #region Methods
        /// <summary>
        /// Registers the configuration key.
        /// </summary>
        /// <param name="name">The name.</param>
        public virtual void RegisterConfigurationKey(string name)
        {
            if (IsConfigurationValueSet(name))
            {
                return;
            }

            var propertyData = RegisterProperty(name, typeof(object));

            InitializePropertyAfterConstruction(propertyData);
        }

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public virtual object GetConfigurationValue(string name)
        {
            RegisterConfigurationKey(name);

            return GetValue<object>(name);
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public virtual void SetConfigurationValue(string name, object value)
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
            Argument.IsNotNull("name", name);

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
            Argument.IsNotNull("name", name);

            lock (_propertiesSetAtLeastOnce)
            {
                _propertiesSetAtLeastOnce.Add(name);
            }
        }
        #endregion

        public virtual void Serialize(XmlWriter xmlWriter)
        {
            if (xmlWriter != null)
            {
                var xmlSerializer = ServiceLocator.Default.ResolveType<IXmlSerializer>();
                xmlSerializer.Serialize(this, new XmlSerializationContextInfo(xmlWriter, this)
                {
                    AllowCustomXmlSerialization = false
                });
            }
        }

        public virtual void Deserialize(XmlReader xmlReader)
        {
            if (xmlReader != null)
            {
                if (xmlReader.ReadState == ReadState.Initial)
                {
                    xmlReader.Read();
                }

                xmlReader.MoveToContent();

                var parentNode = xmlReader.LocalName;

                xmlReader.Read();
                
                while (xmlReader.MoveToNextContentElement(parentNode))
                {
                    var elementName = xmlReader.LocalName;
                    var value = xmlReader.ReadElementContentAsString();

                    RegisterConfigurationKey(elementName);
                    MarkConfigurationValueAsSet(elementName);

                    SetValue(elementName, value);
                }
            }
        }
    }
}

﻿namespace Catel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using Catel.Data;
    using Catel.Reflection;
    using Catel.Runtime.Serialization.Xml;
    using Runtime.Serialization;

    /// <summary>
    /// Dynamic configuration.
    /// </summary>
    [SerializerModifier(typeof(DynamicConfigurationSerializerModifier))]
    public class DynamicConfiguration : ModelBase, ICustomXmlSerializable
    {
#pragma warning disable IDE1006 // Naming Styles
        protected static readonly HashSet<string> DynamicProperties = new HashSet<string>();
#pragma warning restore IDE1006 // Naming Styles

        private readonly HashSet<string> _propertiesSetAtLeastOnce = new HashSet<string>();

        private readonly IXmlSerializer _xmlSerializer;

        public DynamicConfiguration(IXmlSerializer serializer)
            : base(serializer)
        {
            _xmlSerializer = serializer;
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

        protected virtual IXmlSerializer GetXmlSerializer()
        {
            return _xmlSerializer;
        }

        public virtual void Serialize(XmlWriter xmlWriter)
        {
            if (xmlWriter is not null)
            {
                var xmlSerializer = GetXmlSerializer();
                xmlSerializer.Serialize(this, new XmlSerializationContextInfo(xmlWriter, this)
                {
                    AllowCustomXmlSerialization = false
                });
            }
        }

        public virtual void Deserialize(XmlReader xmlReader)
        {
            var propertyDataManager = PropertyDataManager.Default;
            var type = GetType();

            if (xmlReader is not null)
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
                    var valueRead = false;
                    object? value = null;

                    var elementName = xmlReader.LocalName;

                    // If simple property
                    var typeAttribute = xmlReader.GetAttribute("ctl:type");
                    if (typeAttribute is null)
                    {
                        // Fallback mechanism for older serialization formats, see https://github.com/Catel/Catel/issues/1535
                        typeAttribute = xmlReader.GetAttribute("type");
                    }

                    if (typeAttribute is not null)
                    {
                        var elementType = TypeCache.GetTypeWithoutAssembly(typeAttribute);
                        if (elementType is not null)
                        {
                            if (elementType != typeof(string) && !elementType.IsValueTypeEx())
                            {
                                var instance = Activator.CreateInstance(elementType);
                                if (instance is not null)
                                {
                                    // Complex object, use xml serializer
                                    var xmlSerializer = GetXmlSerializer();
                                    value = xmlSerializer.Deserialize(elementType, new XmlSerializationContextInfo(xmlReader, instance));
                                    valueRead = true;
                                }
                            }
                        }
                    }

                    if (!valueRead)
                    {
                        value = xmlReader.ReadElementContentAsString();
                        valueRead = true;
                    }

                    var valueSet = false;

                    if (!DynamicProperties.Contains(elementName) && propertyDataManager.IsPropertyRegistered(type, elementName))
                    {
                        // If registered property, cast & set
                        var propertyData = propertyDataManager.GetPropertyData(type, elementName);

                        if (value is string stringValue)
                        {
                            value = StringToObjectHelper.ToRightType(propertyData.Type, stringValue);
                        }
    
                        SetValue(elementName, value);

                        valueSet = true;
                    }

                    if (!valueSet)
                    {
                        // Set dynamic value as string
                        RegisterConfigurationKey(elementName);
                        MarkConfigurationValueAsSet(elementName);

                        SetValue(elementName, value);
                    }
                }
            }
        }
    }
}

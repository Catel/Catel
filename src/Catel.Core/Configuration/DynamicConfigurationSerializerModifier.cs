// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationSerializerModifier.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Configuration
{
    using Runtime.Serialization;
    using Runtime.Serialization.Xml;

    /// <summary>
    /// Dynamic configuration serializer modifier.
    /// </summary>
    public class DynamicConfigurationSerializerModifier : SerializerModifierBase
    {
        private readonly ISerializationManager _serializationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicConfigurationSerializerModifier"/> class.
        /// </summary>
        /// <param name="serializationManager">The serialization manager.</param>
        public DynamicConfigurationSerializerModifier(ISerializationManager serializationManager)
        {
            Argument.IsNotNull("serializationManager", serializationManager);

            _serializationManager = serializationManager;
        }

        /// <summary>
        /// Called when the object is about to be serialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public override void OnSerializing(ISerializationContext context, object model)
        {
            _serializationManager.Clear(model.GetType());

            base.OnSerializing(context, model);
        }

        /// <summary>
        /// Called when the object is about to be deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public override void OnDeserializing(ISerializationContext context, object model)
        {
            base.OnDeserializing(context, model);

            var dynamicConfiguration = (DynamicConfiguration)model;

            var xmlSerializationContext = ((ISerializationContext<XmlSerializationContextInfo>)context).Context;

            var reader = xmlSerializationContext.XmlReader;
            if (reader != null)
            {
                while (reader.Read())
                {
                    if (reader.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        var elementName = reader.LocalName;

                        dynamicConfiguration.RegisterConfigurationKey(elementName);
                        dynamicConfiguration.MarkConfigurationValueAsSet(elementName);
                    }
                }
            }
        }
    }
}

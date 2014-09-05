// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicConfigurationSerializerModifier.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
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
        /// <summary>
        /// Called when the object is about to be deserialized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="model">The model.</param>
        public override void OnDeserializing(ISerializationContext context, Data.IModel model)
        {
            base.OnDeserializing(context, model);

            var dynamicConfiguration = (DynamicConfiguration) model;

            var xmlSerializationContext = ((ISerializationContext<XmlSerializationContextInfo>)context).Context;
            var element = xmlSerializationContext.Element;

            if (element != null)
            {
                foreach (var childElement in element.Elements())
                {
                    var elementName = childElement.Name.LocalName;

                    dynamicConfiguration.RegisterConfigurationKey(elementName);
                }
            }
        }
    }
}
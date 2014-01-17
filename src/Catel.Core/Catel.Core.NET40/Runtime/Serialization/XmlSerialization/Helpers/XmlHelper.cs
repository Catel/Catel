// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2014 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using Catel.IoC;
    using Logging;

    /// <summary>
    /// Helper class for xml serialization.
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Converts a value to an xml element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="objectValue">The object value.</param>
        /// <returns>The created <see cref="XElement"/>.</returns>
        public static XElement ConvertToXml(string elementName, Type objectType, object objectValue)
        {
            Argument.IsNotNullOrWhitespace("elementName", elementName);
            Argument.IsNotNull("objectType", objectType);

            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var dataContractSerializerFactory = dependencyResolver.Resolve<IDataContractSerializerFactory>();
            var dataContractSerializer = dataContractSerializerFactory.GetDataContractSerializer(typeof(object), objectType, elementName);

            var document = new XDocument();

            using (var writer = document.CreateWriter())
            {
                dataContractSerializer.WriteObject(writer, objectValue);
            }

            return document.Root;
        }

        /// <summary>
        /// Converts the specified xml element to an object.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="createDefaultValue">The create default value.</param>
        /// <returns>The created object.</returns>
        public static object ConvertToObject(XElement element, Type objectType, Func<object> createDefaultValue)
        {
            Argument.IsNotNull("element", element);
            Argument.IsNotNull("objectType", objectType);

            string xmlName = element.Name.LocalName;

            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var dataContractSerializerFactory = dependencyResolver.Resolve<IDataContractSerializerFactory>();
            var dataContractSerializer = dataContractSerializerFactory.GetDataContractSerializer(typeof(object), objectType, xmlName);

            var attribute = element.Attribute(XName.Get("type", "http://catel.codeplex.com"));
            if (attribute != null)
            {
                var actualTypeToDeserialize = (from t in dataContractSerializer.KnownTypes
                                               where string.Equals(t.FullName, attribute.Value)
                                               select t).FirstOrDefault();
                if (actualTypeToDeserialize != null)
                {
                    dataContractSerializer = dataContractSerializerFactory.GetDataContractSerializer(typeof(object), actualTypeToDeserialize, xmlName);
                }
                else
                {
                    Log.Warning("Could not find type '{0}', falling back to original type '{1}'", attribute.Value, objectType.FullName);
                }
            }

            try
            {
                object value = dataContractSerializer.ReadObject(element.CreateReader(), false);
                return value;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to deserialize '{0}', falling back to default value", xmlName);

                return (createDefaultValue != null) ? createDefaultValue() : null;
            }
        }
    }
}
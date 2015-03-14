// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlSchemaHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2015 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

#if NET

namespace Catel.Runtime.Serialization.Xml
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;
    using Catel.IoC;
    using Data;
    using Logging;
    using Reflection;

    /// <summary>
    /// Helper class for xml schemas.
    /// </summary>
    public static class XmlSchemaHelper
    {
        #region Constants
        /// <summary>
        /// The log.
        /// </summary>
        public static ILog Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default xml schema.
        /// </summary>
        public const string Xmlns = "http://www.w3.org/2001/XMLSchema";
        #endregion

        #region Properties
        private static IXmlNamespaceManager XmlNamespaceManager
        {
            get { return IoCConfiguration.DefaultDependencyResolver.Resolve<IXmlNamespaceManager>(); }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the XML schema.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="generateFlatSchema">A value indicating whether the schema should be generated as flat schema.</param>
        /// <returns>The qualified name of the type.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="schemaSet"/> is <c>null</c>.</exception>
        public static XmlQualifiedName GetXmlSchema(Type type, XmlSchemaSet schemaSet, bool generateFlatSchema)
        {
            Argument.IsNotNull("schemaSet", schemaSet);

            var dependencyResolver = IoCConfiguration.DefaultDependencyResolver;
            var serializationManager = dependencyResolver.Resolve<ISerializationManager>();

            string typeNs = GetTypeNamespaceForSchema(type);
            var schema = GetOrCreateSchema(typeNs, schemaSet, serializationManager);

            // Check if it already exists
            var typeName = GetTypeNameForSchema(type);
            var existingType = (from x in schema.Items.Cast<object>()
                                where x is XmlSchemaComplexType && ((XmlSchemaComplexType)x).Name == typeName
                                select (XmlSchemaComplexType)x).FirstOrDefault();
            if (existingType != null)
            {
                return new XmlQualifiedName(existingType.Name, typeNs);
            }

            var typeSchema = CreateSchemaComplexType(type, schema, schemaSet, serializationManager, generateFlatSchema, new HashSet<string>());

            var root = new XmlSchemaElement();
            root.Name = string.Format("{0}", typeSchema.Name);
            root.SchemaTypeName = new XmlQualifiedName(typeSchema.Name, typeNs);
            root.IsNillable = true;
            schema.Items.Add(root);

            return new XmlQualifiedName(typeSchema.Name, typeNs);
        }

        /// <summary>
        /// Adds the type to schema set by reading the <see cref="XmlSchemaProviderAttribute" /> or by
        /// using the known schema sets information.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="exportedTypes">The exported types.</param>
        /// <returns>The xml qualified name.</returns>
        private static XmlQualifiedName AddTypeToSchemaSet(Type type, XmlSchemaSet schemaSet, ISerializationManager serializationManager,
            HashSet<string> exportedTypes)
        {
            var attribute = (from x in type.GetCustomAttributesEx(typeof(XmlSchemaProviderAttribute), false)
                             select x as XmlSchemaProviderAttribute).FirstOrDefault();
            if (attribute == null)
            {
                if (type.IsBasicType())
                {
                    return new XmlQualifiedName(type.Name.ToLower(), Xmlns);
                }

                return null;
            }

            var methodToInvoke = type.GetMethodEx(attribute.MethodName, BindingFlags.Public | BindingFlags.Static);
            if (methodToInvoke == null)
            {
                Log.Error("Expected method '{0}.{1}' because of the XmlSchemaProvider attribute, but it was not found", type.FullName, attribute.MethodName);
                return null;
            }

            var qualifiedName = (XmlQualifiedName)methodToInvoke.Invoke(null, new object[] { schemaSet });
            return qualifiedName;
        }

        /// <summary>
        /// Returns the sequence of properties of the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="exportedTypes">The exported types.</param>
        /// <returns>Sequence containing all properties.</returns>
        private static XmlSchemaSequence GetPropertiesSequence(Type type, XmlSchema schema, XmlSchemaSet schemaSet, ISerializationManager serializationManager, HashSet<string> exportedTypes)
        {
            Argument.IsNotNull("type", type);
            Argument.IsNotNull("schema", schema);
            Argument.IsNotNull("schemaSet", schemaSet);

            var propertiesSequence = new XmlSchemaSequence();

            if (typeof(ModelBase).IsAssignableFromEx(type))
            {
                var members = new List<MemberInfo>();
                members.AddRange(from field in serializationManager.GetFieldsToSerialize(type)
                                 select type.GetFieldEx(field));
                members.AddRange(from property in serializationManager.GetPropertiesToSerialize(type)
                                 select type.GetPropertyEx(property));

                foreach (var member in members)
                {
                    var propertySchemaElement = new XmlSchemaElement();
                    propertySchemaElement.Name = member.Name;

                    var memberType = typeof(object);
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo != null)
                    {
                        memberType = fieldInfo.FieldType;
                    }

                    var propertyInfo = member as PropertyInfo;
                    if (propertyInfo != null)
                    {
                        memberType = propertyInfo.PropertyType;
                    }

                    propertySchemaElement.IsNillable = memberType.IsNullableType();
                    propertySchemaElement.MinOccurs = 0;

                    var exporter = new XsdDataContractExporter(schemaSet);

                    var alreadyExported = IsAlreadyExported(schemaSet, memberType, exporter, exportedTypes);
                    if (!alreadyExported)
                    {
                        if (!exportedTypes.Contains(memberType.FullName))
                        {
                            exportedTypes.Add(memberType.FullName);
                        }

                        try
                        {
                            if (exporter.CanExport(memberType))
                            {
                                exporter.Export(memberType);
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore
                        }
                    }

                    propertySchemaElement.SchemaType = exporter.GetSchemaType(memberType);
                    propertySchemaElement.SchemaTypeName = exporter.GetSchemaTypeName(memberType);

                    propertiesSequence.Items.Add(propertySchemaElement);
                }
            }

            return propertiesSequence;
        }

        /// <summary>
        /// Determines whether the specified member type is already exported to the schema set.
        /// </summary>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="memberType">Type of the member.</param>
        /// <param name="exporter">The exporter.</param>
        /// <param name="exportedTypes">The exported types.</param>
        /// <returns><c>true</c> if the specified member type is already exported to the schema set; otherwise, <c>false</c>.</returns>
        private static bool IsAlreadyExported(XmlSchemaSet schemaSet, Type memberType, XsdDataContractExporter exporter, HashSet<string> exportedTypes)
        {
            if (exportedTypes.Contains(memberType.FullName))
            {
                return true;
            }

            var schemaTypeName = exporter.GetSchemaTypeName(memberType);

            foreach (XmlSchema xmlSchema in schemaSet.Schemas())
            {
                foreach (var item in xmlSchema.Items)
                {
                    var simpleType = item as XmlSchemaSimpleType;
                    if (simpleType != null)
                    {
                        if (string.Equals(simpleType.Name, memberType.Name) || string.Equals(simpleType.Name, schemaTypeName.Name))
                        {
                            return true;
                        }
                    }

                    var complexType = item as XmlSchemaComplexType;
                    if (complexType != null)
                    {
                        if (string.Equals(complexType.Name, memberType.Name) || string.Equals(complexType.Name, schemaTypeName.Name))
                        {
                            return true;
                        }
                    }

                    var xmlSchemaElement = item as XmlSchemaElement;
                    if (xmlSchemaElement != null)
                    {
                        if (string.Equals(xmlSchemaElement.Name, memberType.Name) || string.Equals(xmlSchemaElement.Name, schemaTypeName.Name))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Creates the an xml schema for a complex type. This method automatically takes care of
        /// any base classes that must be added.
        /// <para />
        /// This method already adds the <see cref="XmlSchemaComplexType" /> to the <see cref="XmlSchemaSet" />.
        /// </summary>
        /// <param name="type">The type to create the complex schema for.</param>
        /// <param name="schema">The schema.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <param name="generateFlatSchema">A value indicating whether the schema should be generated as flat schema.</param>
        /// <param name="exportedTypes">The exported types.</param>
        /// <returns>The complex schema for the specified type.</returns>
        private static XmlSchemaComplexType CreateSchemaComplexType(Type type, XmlSchema schema, XmlSchemaSet schemaSet, ISerializationManager serializationManager, bool generateFlatSchema, HashSet<string> exportedTypes)
        {
            // Determine name, which is complex in generic types
            string typeName = GetTypeNameForSchema(type);

            // First, add the type, otherwise we might get into a stackoverflow when using generic base types
            // <xs:complexType>
            var modelBaseType = new XmlSchemaComplexType();
            modelBaseType.Name = typeName;
            modelBaseType.IsMixed = false;

            schema.Items.Add(modelBaseType);

            var propertiesSequence = GetPropertiesSequence(type, schema, schemaSet, serializationManager, exportedTypes);

            // If flat, don't generate base classes, just the type itself
            if (generateFlatSchema)
            {
                modelBaseType.Particle = propertiesSequence;
                return modelBaseType;
            }

            if (type.IsGenericType)
            {
                var genericComplexType = new XmlSchemaComplexType();
                genericComplexType.Name = typeName;

                // <xs:annotation>
                var annotation = new XmlSchemaAnnotation();

                // <xs:appinfo>
                var appInfo = new XmlSchemaAppInfo();

                //<GenericType xmlns="http://schemas.microsoft.com/2003/10/Serialization/" Name="DataContractBaseOf{0}{#}" Namespace="http://schemas.datacontract.org/2004/07/STC">
                //  <GenericParameter Name="ProfileDateRange2" Namespace="http://schemas.datacontract.org/2004/07/STC.Meter.ProfileData"/>
                //</GenericType>
                var genericTypeElement = new XElement("GenericType");
                genericTypeElement.Add(new XAttribute("Name", string.Format("{0}Of{{0}}{{#}}", typeName)));
                genericTypeElement.Add(new XAttribute("Namespace", GetTypeNamespaceForSchema(type)));

                foreach (var genericArgument in type.GetGenericArgumentsEx())
                {
                    var genericArgumentQualifiedName = AddTypeToSchemaSet(genericArgument, schemaSet, serializationManager, exportedTypes);
                    var genericArgumentElement = new XElement("GenericParameter");
                    genericArgumentElement.Add(new XAttribute("Name", genericArgumentQualifiedName.Name));
                    genericArgumentElement.Add(new XAttribute("Namespace", genericArgumentQualifiedName.Namespace));
                    genericTypeElement.Add(genericArgumentElement);
                }

                var conversionDoc = new XmlDocument();
                appInfo.Markup = new XmlNode[] { conversionDoc.CreateTextNode(genericTypeElement.ToString()) };

                annotation.Items.Add(appInfo);
            }

            var baseTypeQualifiedName = AddTypeToSchemaSet(type.BaseType, schemaSet, serializationManager, exportedTypes);
            if (baseTypeQualifiedName != null)
            {
                // <xs:extensions base="address">
                var complexContentExtension = new XmlSchemaComplexContentExtension();
                complexContentExtension.BaseTypeName = baseTypeQualifiedName;
                complexContentExtension.Particle = propertiesSequence;

                // <xs:complexContent>
                var complexContent = new XmlSchemaComplexContent();
                complexContent.Content = complexContentExtension;

                modelBaseType.ContentModel = complexContent;
            }
            else
            {
                modelBaseType.Particle = propertiesSequence;
            }

            return modelBaseType;
        }

        /// <summary>
        /// Gets the or create schema from the schema set.
        /// <para />
        /// If the namespace does not yet exists, it is created and added. Otherwise the existing
        /// one is returned.
        /// </summary>
        /// <param name="xmlns">The xml namespace.</param>
        /// <param name="schemaSet">The schema set.</param>
        /// <param name="serializationManager">The serialization manager.</param>
        /// <returns>The xml schema to use.</returns>
        private static XmlSchema GetOrCreateSchema(string xmlns, XmlSchemaSet schemaSet, ISerializationManager serializationManager)
        {
            var schemas = schemaSet.Schemas(xmlns);

            foreach (XmlSchema schema in schemas)
            {
                return schema;
            }

            var newSchema = new XmlSchema();
            newSchema.TargetNamespace = xmlns;
            newSchema.ElementFormDefault = XmlSchemaForm.Qualified;

            schemaSet.Add(newSchema);

            return newSchema;
        }

        /// <summary>
        /// Gets the name of the type to be used inside an xml schema.
        /// </summary>
        /// <param name="type">The type to determine the name for.</param>
        /// <returns>The name.</returns>
        private static string GetTypeNameForSchema(Type type)
        {
            string typeName = type.Name;

            if (type.IsGenericType)
            {
                int firstPosition = typeName.IndexOf("`");
                typeName = typeName.Substring(0, firstPosition);
                typeName = string.Format("{0}Of{1}", typeName, type.GetGenericArguments()[0].Name);
            }

            return typeName;
        }

        /// <summary>
        /// Gets the type namespace to be used inside an xml schema.
        /// </summary>
        /// <param name="type">The type to determine the namespace for.</param>
        /// <returns>The namespace.</returns>
        private static string GetTypeNamespaceForSchema(Type type)
        {
            var xmlNamespaceManager = XmlNamespaceManager;
            var xmlNamespace = xmlNamespaceManager.GetNamespace(type, "ctl");

            return xmlNamespace.Uri;
        }
        #endregion
    }
}

#endif